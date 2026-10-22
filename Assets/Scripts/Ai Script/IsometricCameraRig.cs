using UnityEngine;

/// <summary>
/// Isometric / third-person follow camera. Attach to the Camera.
/// The lazy follow makes the elephant feel like it has inertia, and
/// AddImpulse() gives a small vertical "thud" on every footstep.
/// </summary>
[RequireComponent(typeof(Camera))]
public class IsometricCameraRig : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("View")]
    public float yaw = 45f;
    public float pitch = 35f;
    public float distance = 25f;
    public bool orthographic = true;
    public float orthoSize = 7f;
    [Range(20f, 90f)] public float perspectiveFov = 35f;

    [Header("Follow")]
    [Tooltip("Higher = lazier camera = heavier feel.")]
    public float followSmoothTime = 0.35f;

    [Header("Footstep Thud")]
    public float kickStiffness = 120f;
    public float kickDamping = 9f;
    public float maxKick = 0.5f;

    Camera cam;
    Vector3 focusPoint, focusVelocity;
    float kickPos, kickVel;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = orthographic;
        if (orthographic) cam.orthographicSize = orthoSize;
        else cam.fieldOfView = perspectiveFov;

        if (target) focusPoint = target.position + targetOffset;
    }

    public void AddImpulse(float amount)
    {
        kickVel -= amount;
    }

    void LateUpdate()
    {
        if (!target) return;
        float dt = Time.deltaTime;

        focusPoint = Vector3.SmoothDamp(focusPoint, target.position + targetOffset,
                                        ref focusVelocity, followSmoothTime);

        kickVel += (-kickStiffness * kickPos - kickDamping * kickVel) * dt;
        kickPos = Mathf.Clamp(kickPos + kickVel * dt, -maxKick, maxKick);

        Quaternion rot = Quaternion.Euler(pitch, yaw, 0f);
        transform.rotation = rot;
        transform.position = focusPoint + rot * new Vector3(0f, kickPos, -distance);
    }
}
