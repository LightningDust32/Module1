using UnityEngine;
using UnityEngine.InputSystem.XR;

public class Follower : MonoBehaviour
{
    private Player player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -9f;

    private float verticalVelocity;
    private bool isGrounded ;


    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void Move()
    {
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Apply gravity.
        verticalVelocity += gravity * Time.deltaTime;
    }
}
