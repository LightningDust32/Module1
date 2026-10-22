using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Legless version: a heavy, momentum-based elephant whose weight comes entirely
/// from body motion. Every footstep dips the body and rocks it left/right.
/// Works with IsometricCameraRig (optional).
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class HeavyElephantLite : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Child object holding the model. Required.")]
    public Transform visual;
    public Transform cameraTransform;          // defaults to Camera.main
    public IsometricCameraRig cameraRig;       // optional

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

    [Header("Feedback (optional)")]
    public ParticleSystem footDust;
    public int dustPerStep = 6;
    public AudioSource audioSource;
    public AudioClip[] footstepClips;

    CharacterController controller;
    float speed, prevSpeed, vertVel;
    float stepPhase;
    int lastStep, side = 1;
    float bobPos, bobVel, rollPos, rollVel, leanPos, leanVel;
    Vector3 restPos, restScale;
    Quaternion restRot;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        if (!cameraTransform && Camera.main) cameraTransform = Camera.main.transform;
        if (visual)
        {
            restPos = visual.localPosition;
            restRot = visual.localRotation;
            restScale = visual.localScale;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        // --- input, relative to camera ---
        Vector2 input = ReadInput();
        Vector3 desired = Vector3.zero;
        if (input.sqrMagnitude > 0.01f && cameraTransform)
        {
            Vector3 f = cameraTransform.forward; f.y = 0f; f.Normalize();
            Vector3 r = cameraTransform.right;   r.y = 0f; r.Normalize();
            desired = (f * input.y + r * input.x).normalized;
        }

        // --- slow turning ---
        float speedNorm = Mathf.Clamp01(speed / maxSpeed);
        float facing = 1f;
        if (desired != Vector3.zero)
        {
            float rate = Mathf.Lerp(turnSpeedStanding, turnSpeedMoving, speedNorm);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, Quaternion.LookRotation(desired), rate * dt);
            facing = Vector3.Dot(transform.forward, desired);
        }

        // --- momentum ---
        float target = desired == Vector3.zero ? 0f :
            maxSpeed * Mathf.Clamp01(input.magnitude) *
            Mathf.Lerp(turnAroundSpeedFactor, 1f, Mathf.Clamp01(facing));
        prevSpeed = speed;
        speed = Mathf.MoveTowards(speed, target, (target > speed ? acceleration : deceleration) * dt);

        // --- move ---
        if (controller.isGrounded && vertVel < 0f) vertVel = -2f;
        vertVel += gravity * dt;
        controller.Move((transform.forward * speed + Vector3.up * vertVel) * dt);

        // --- footsteps: one every stepLength metres, alternating sides ---
        speedNorm = Mathf.Clamp01(speed / maxSpeed);
        stepPhase += speed * dt / Mathf.Max(0.01f, stepLength);
        int step = Mathf.FloorToInt(stepPhase);
        if (step != lastStep)
        {
            lastStep = step;
            side = -side;
            Footfall(Mathf.Max(0.25f, speedNorm));
        }

        Animate(dt, speedNorm);
    }

    void Animate(float dt, float speedNorm)
    {
        // damped springs: dip, side rock, and acceleration lean
        bobVel  += (-springStiffness * bobPos  - springDamping * bobVel)  * dt;
        bobPos  += bobVel * dt;
        rollVel += (-springStiffness * rollPos - springDamping * rollVel) * dt;
        rollPos += rollVel * dt;

        float accel = (speed - prevSpeed) / dt;
        float leanTarget = Mathf.Clamp(accel * leanPerAcceleration, -maxLean, maxLean);
        leanVel += (40f * (leanTarget - leanPos) - 8f * leanVel) * dt;
        leanPos += leanVel * dt;

        if (!visual) return;
        visual.localPosition = restPos + Vector3.up * bobPos;
        visual.localRotation = restRot * Quaternion.Euler(leanPos, 0f, rollPos);

        float breath = 1f + Mathf.Sin(Time.time * 0.9f) * 0.012f * (1f - speedNorm);
        visual.localScale = new Vector3(restScale.x, restScale.y * breath, restScale.z);
    }

    void Footfall(float s)
    {
        bobVel  -= dipKick * s;
        rollVel += side * rollKick * s;
        if (cameraRig) cameraRig.AddImpulse(cameraKick * s);

        if (footDust)
        {
            footDust.transform.position = transform.position +
                transform.right * side * 0.6f + transform.forward * 0.8f;
            footDust.Emit(Mathf.RoundToInt(dustPerStep * s));
        }

        if (audioSource && footstepClips != null && footstepClips.Length > 0)
        {
            audioSource.pitch = Random.Range(0.7f, 0.85f);
            audioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length)],
                                    Mathf.Lerp(0.5f, 1f, s));
        }
    }

    Vector2 ReadInput()
    {
#if ENABLE_INPUT_SYSTEM
        Vector2 v = Vector2.zero;
        var k = Keyboard.current;
        if (k != null)
        {
            if (k.wKey.isPressed || k.upArrowKey.isPressed)    v.y += 1f;
            if (k.sKey.isPressed || k.downArrowKey.isPressed)  v.y -= 1f;
            if (k.dKey.isPressed || k.rightArrowKey.isPressed) v.x += 1f;
            if (k.aKey.isPressed || k.leftArrowKey.isPressed)  v.x -= 1f;
        }
        var g = Gamepad.current;
        if (g != null) v += g.leftStick.ReadValue();
        return Vector2.ClampMagnitude(v, 1f);
#else
        return Vector2.ClampMagnitude(
            new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
#endif
    }
}
