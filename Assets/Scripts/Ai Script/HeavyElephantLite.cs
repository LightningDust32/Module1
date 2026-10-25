using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class HeavyElephantLite : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Child object holding the model. Required.")]
    public Transform body;
    public Transform cameraTransform;          // defaults to Camera.main

    [Header("Movement")]
    public float maxSpeed = 3f;
    public float acceleration = 1.6f;
    public float deceleration = 2.8f;
    public float turnSpeedStanding = 80f;      // deg/s
    public float turnSpeedMoving = 40f;
    [Range(0f, 1f)] public float turnAroundSpeedFactor = 0.2f;
    public float gravity = -25f;

    [Header("Weight Feel")]
    [Tooltip("Metres between footfalls. Bigger = slower, heavier steps.")]
    public float stepLength = 2.5f;
    public float dipKick = 0.6f;               // downward impulse per step
    public float rollKick = 14f;               // sideways rock per step (deg/s impulse)
    public float springStiffness = 90f;
    public float springDamping = 8f;
    public float maxLean = 8f;
    public float leanPerAcceleration = 3f;
    public float cameraKick = 1.2f;

    CharacterController controller;
    float speed, previousSpeed, verticalVelocity;
    float stepPhase;
    int lastStep, side = 1;
    float bobPosition, bobVelocity, rollPosition, rollVelocity, leanPosition, leanVelocity;
    Vector3 restPosition, restScale;
    Quaternion restRotation;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
        if (body)
        {
            restPosition = body.localPosition;
            restRotation = body.localRotation;
            restScale = body.localScale;
        }
    }

    void Update()
    {
        float deltaTime = Time.deltaTime;
        if (deltaTime <= 0f) return;

        Vector2 input = Vector2.zero;
        Vector3 desired = Vector3.zero;

        if (input.sqrMagnitude > 0.01f && cameraTransform)
        {
            Vector3 forward = cameraTransform.forward; forward.y = 0f; forward.Normalize();
            Vector3 rotation = cameraTransform.right;   rotation.y = 0f; rotation.Normalize();
            desired = (forward * input.y + rotation * input.x).normalized;
        }

        // --- slow turning ---
        float speedNorm = Mathf.Clamp01(speed / maxSpeed);
        float facing = 1f;

        if (desired != Vector3.zero)
        {
            float rate = Mathf.Lerp(turnSpeedStanding, turnSpeedMoving, speedNorm);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(desired), rate * deltaTime);
            facing = Vector3.Dot(transform.forward, desired);
        }

        // --- momentum ---
        float target = desired == Vector3.zero ? 0f : maxSpeed * Mathf.Clamp01(input.magnitude) * Mathf.Lerp(turnAroundSpeedFactor, 1f, Mathf.Clamp01(facing));

        previousSpeed = speed;

        speed = Mathf.MoveTowards(speed, target, (target > speed ? acceleration : deceleration) * deltaTime);

        // --- move ---
        if (controller.isGrounded && verticalVelocity < 0f) verticalVelocity = -2f;
        verticalVelocity += gravity * deltaTime;
        controller.Move((transform.forward * speed + Vector3.up * verticalVelocity) * deltaTime);

        // --- footsteps: one every stepLength metres, alternating sides ---
        speedNorm = Mathf.Clamp01(speed / maxSpeed);
        stepPhase += speed * deltaTime / Mathf.Max(0.01f, stepLength);
        int step = Mathf.FloorToInt(stepPhase);
        if (step != lastStep)
        {
            lastStep = step;
            side = -side;
            Footfall(Mathf.Max(0.25f, speedNorm));
        }

        Animate(deltaTime, speedNorm);
    }

    void Animate(float deltaTime, float speedNorm)
    {
        // damped springs: dip, side rock, and acceleration lean
        bobVelocity  += (-springStiffness * bobPosition  - springDamping * bobVelocity)  * deltaTime;
        bobPosition  += bobVelocity * deltaTime;

        rollVelocity += (-springStiffness * rollPosition - springDamping * rollVelocity) * deltaTime;
        rollPosition += rollVelocity * deltaTime;

        float accel = (speed - previousSpeed) / deltaTime;
        float leanTarget = Mathf.Clamp(accel * leanPerAcceleration, -maxLean, maxLean);
        leanVelocity += (40f * (leanTarget - leanPosition) - 8f * leanVelocity) * deltaTime;
        leanPosition += leanVelocity * deltaTime;

        if (!body) return;
        body.localPosition = restPosition + Vector3.up * bobPosition;
        body.localRotation = restRotation * Quaternion.Euler(leanPosition, 0f, rollPosition);

        float breath = 1f + Mathf.Sin(Time.time * 0.9f) * 0.012f * (1f - speedNorm);
        body.localScale = new Vector3(restScale.x, restScale.y * breath, restScale.z);
    }

    void Footfall(float step)
    {
        bobVelocity  -= dipKick * step;
        rollVelocity += side * rollKick * step;

        if (footDust)
        {
            footDust.transform.position = transform.position + transform.right * side * 0.6f + transform.forward * 0.8f;
            footDust.Emit(Mathf.RoundToInt(dustPerStep * step));
        }

        if (audioSource && footstepClips != null && footstepClips.Length > 0)
        {
            audioSource.pitch = Random.Range(0.7f, 0.85f);
            audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)], Mathf.Lerp(0.5f, 1f, step));
        }
    }
}
