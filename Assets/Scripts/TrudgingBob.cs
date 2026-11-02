using UnityEngine;
// The movement animation from the elephant further extracted and simplified for human movements
public class TrudgingBob : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visualRoot;

    [Header("Trudging")]
    [SerializeField] private float stepLength = 1.2f;
    [SerializeField] private float bobAmount = 0.035f;

    [Header("Spring")]
    [SerializeField] private float springStiffness = 30f;
    [SerializeField] private float springDamping = 7f;

    private Vector3 restPosition;

    private float stepDistance;
    private float bobPosition;
    private float bobVelocity;

    private void Awake()
    {
        if (visualRoot != null)
        {
            restPosition = visualRoot.localPosition;
        }
    }

    public void AddMovement(float distance)
    {
        if (visualRoot == null || distance <= 0f)
            return;

        stepDistance += distance;

        while (stepDistance >= stepLength)
        {
            stepDistance -= stepLength;

            // A light downward dip with each step.
            bobVelocity -= bobAmount;
        }
    }

    private void Update()
    {
        if (visualRoot == null)
            return;

        float deltaTime = Time.deltaTime;

        if (deltaTime <= 0f)
            return;

        // Spring motion smoothly returns the body to its resting position.
        bobVelocity += (-springStiffness * bobPosition - springDamping * bobVelocity) * deltaTime;

        bobPosition += bobVelocity * deltaTime;

        visualRoot.localPosition = restPosition + Vector3.up * bobPosition;
    }
}