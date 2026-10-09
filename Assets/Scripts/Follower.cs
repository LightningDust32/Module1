using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Follower : MonoBehaviour
{
    [Header("Following")]
    [SerializeField] private float followDistance = 3f;
    [SerializeField] private float followSpeed = 4f;
    [SerializeField] private float stoppingDistance = 1f;

    [Header("Gravity")]
    [SerializeField] private float gravity = -9f;

    private CharacterController controller;

    private Transform player;

    private float verticalVelocity;

    private bool isFollowing;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if(!isFollowing && player == null)
        {
            ApplyGravity();
            return;
        }

        FollowPlayer();
    }

    public void StartFollowing(Transform playerTransform)
    {
        if(playerTransform == null)
        {
            return;
        }

        player = playerTransform;
        isFollowing = true;
    }

    public void StopFollowing()
    {
        isFollowing = false;

        transform.SetParent(null, true);

        Debug.Log($"{gameObject.name} has stopped following.");
    }

    private void FollowPlayer()
    {
        if(!isFollowing)
        {
            return;
        }

        Vector3 targetPosition = player.position - player.forward * followDistance;

        Vector3 direction = targetPosition - transform.position;

        // Ignores vertical distance for player following
        direction.y = 0f;

        float distance = direction.magnitude;

        if(distance >  stoppingDistance)
        {
            direction.Normalize();

            Vector3 movement = direction * followSpeed * Time.deltaTime;

            controller.Move(movement);
             // Face towards movement direction
            transform.forward = direction;
        }

        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Apply gravity.
        verticalVelocity += gravity * Time.deltaTime;

        controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);
    }
}
