using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSpeed = 120f;

    [Header("Camera")]
    [SerializeField] private float cameraDistance = 10f;
    [SerializeField] private float cameraRotationSpeed = 80f;
    [SerializeField] private float minCameraVerticalAngle = 10f;
    [SerializeField] private float maxCameraVerticalAngle = 70f;

    private float cameraYaw = 0f;
    private float cameraPitch = 25f;

    [Header("Elephant Stats")]
    [SerializeField] private float elephantHunger = 100f;
    [SerializeField] private float elephantThirst = 100f;
    [SerializeField] private float elephantFatigue = 100f;
    [SerializeField] private float elephantTemperature = 100f;

    [Header("Elephant Maximums")]
    [SerializeField] private float elephantMaxHunger = 100f;
    [SerializeField] private float elephantMaxThirst = 100f;
    [SerializeField] private float elephantMaxFatigue = 100f;
    [SerializeField] private float elephantMaxTemperature = 100f;

    [Header("Handler Stats")]
    [SerializeField] private float handlerHunger = 100f;
    [SerializeField] private float handlerThirst = 100f;
    [SerializeField] private float handlerFatigue = 100f;

    [Header("Handler Maximums")]
    [SerializeField] private float handlerMaxHunger = 75f;
    [SerializeField] private float handlerMaxThirst = 75f;
    [SerializeField] private float handlerMaxFatigue = 75f;

    [Header("Time")]
    [SerializeField] private int currentHour = 6;
    [SerializeField] private int currentMinute = 0;

    [SerializeField] private int sunriseHour = 6;
    [SerializeField] private int sunsetHour = 18;

    [Header("Travel Event")]
    [SerializeField] private float distanceUntilWalkEvent = 50f;
    [SerializeField] private GameEvent walkEvent;

    private float distanceTravelled;

    // Input
    private Vector2 moveInput;
    private Vector2 lookInput;

    private InputSystem_Actions inputActions;

    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;

        inputActions.Player.Interact.performed += OnInteract;
        inputActions.Player.Interact.canceled += OnInteract;

        inputActions.Player.Crouch.performed += OnCrouch;
        inputActions.Player.Crouch.canceled += OnCrouch;

        inputActions.Player.Jump.performed += OnJump;
        inputActions.Player.Jump.canceled += OnJump;

        inputActions.Player.Sprint.performed += OnSprint;
        inputActions.Player.Sprint.canceled += OnSprint;

    }

    private void OnDisable()
    {
        inputActions.Disable();

        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLook;

        inputActions.Player.Interact.performed -= OnInteract;
        inputActions.Player.Interact.canceled -= OnInteract;

        inputActions.Player.Crouch.performed -= OnCrouch;
        inputActions.Player.Crouch.canceled -= OnCrouch;

        inputActions.Player.Jump.performed -= OnJump;
        inputActions.Player.Jump.canceled -= OnJump;

        inputActions.Player.Sprint.performed -= OnSprint;
        inputActions.Player.Sprint.canceled -= OnSprint;

    }

    private void OnDestroy()
    {
        inputActions.Dispose();
    }

    private void Update()
    {
        Move();
        UpdateCamera();
    }

    private void Move()
    {
        // A / D turn the elephant
        float turn = moveInput.x;

        transform.Rotate(Vector3.up, turn * turnSpeed * Time.deltaTime);

        // W / S move the elephant forward/backward
        float forward = moveInput.y;

        Vector3 movement = transform.forward * forward;

        movement = Vector3.ClampMagnitude(movement, 1f);

        Vector3 movementDelta = movement * moveSpeed * Time.deltaTime;

        controller.Move(movement * moveSpeed * Time.deltaTime);

        Vector3 horizontalMovement = new Vector3(movementDelta.x, 0f, movementDelta.z);

        distanceTravelled += horizontalMovement.magnitude;

        CheckTravelEvent();
    }

    private void CheckTravelEvent()
    {
        if (distanceTravelled < distanceUntilWalkEvent)
        {
            return;
        }

        TriggerWalkEvent();

        distanceTravelled = 0f;
    }

    private void TriggerWalkEvent()
    {
        if (EventManager.instance == null)
        {
            Debug.LogWarning("Player: EventManager not found.");
            return;
        }

        if (walkEvent == null)
        {
            Debug.LogWarning("Player: Walk event has not been assigned.");
            return;
        }

        Debug.Log("Travel distance reached. Triggering Walk event.");

        EventManager.instance.ApplyEvent(walkEvent);
    }

    private void UpdateCamera()
    {
        if (cameraTransform == null)
        {
            return;
        }

        cameraYaw += lookInput.x * cameraRotationSpeed * Time.deltaTime;

        cameraPitch -= lookInput.y * cameraRotationSpeed * Time.deltaTime;

        cameraPitch = Mathf.Clamp(cameraPitch, minCameraVerticalAngle, maxCameraVerticalAngle);

        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);

        Vector3 offset = rotation * new Vector3(0f, 0f, -cameraDistance);

        cameraTransform.position = transform.position + offset;

        cameraTransform.LookAt(transform.position);
    }

    // Time
    public void AdvanceTime(int minutes)
    {
        currentMinute += minutes;

        while (currentMinute >= 60)
        {
            currentMinute -= 60;
            currentHour++;
        }
    }

    public string GetTimeString()
    {
        return $"{currentHour:00}:{currentMinute:00}";
    }

    public bool IsDaylight()
    {
        return currentHour >= sunriseHour && currentHour < sunsetHour;
    }

    public float GetElephantThirst()
    {
        return elephantThirst;
    }

    public float GetElephantHunger()
    {
        return elephantHunger;
    }

    public float GetElephantFatigue()
    {
        return elephantFatigue;
    }

    public float GetElephantTemp()
    {
        return elephantTemperature;
    }

    public float GetElephantMaxHunger()
    {
        return elephantMaxHunger;
    }

    public float GetElephantMaxThirst()
    {
        return elephantMaxThirst;
    }

    public float GetElephantMaxFatigue()
    {
        return elephantMaxFatigue;
    }

    public float GetElephantMaxTemperature()
    {
        return elephantMaxTemperature;
    }

    public void ChangeElephantThirst(float thirst)
    {
        elephantThirst += thirst;
        elephantThirst = Mathf.Clamp(elephantThirst, 0f, elephantMaxThirst);
    }

    public void ChangeElephantHunger(float hunger)
    {
        elephantHunger += hunger;
        elephantHunger = Mathf.Clamp(elephantHunger, 0f, elephantMaxHunger);
    }

    public void ChangeElephantFatigue(float fatigue)
    {
        elephantFatigue += fatigue;
        elephantFatigue = Mathf.Clamp(elephantFatigue, 0f, elephantMaxFatigue);
    }

    public void ChangeElephantTemp(float temp)
    {
        elephantTemperature += temp;
        elephantTemperature = Mathf.Clamp(elephantTemperature, 0f, elephantMaxTemperature);
    }


    // Handler Stats
    public float GetHandlerHunger()
    {
        return handlerHunger;
    }

    public float GetHandlerThirst()
    {
        return handlerThirst;
    }

    public float GetHandlerFatigue()
    {
        return handlerFatigue;
    }

    public void ChangeHandlerHunger(float hunger)
    {
        handlerHunger += hunger;
        handlerHunger = Mathf.Clamp(handlerHunger, 0f, handlerMaxHunger);
    }

    public void ChangeHandlerThirst(float thirst)
    {
        handlerThirst += thirst;
        handlerThirst = Mathf.Clamp(handlerThirst, 0f, handlerMaxThirst);
    }

    public void ChangeHandlerFatigue(float fatigue)
    {
        handlerFatigue += fatigue;
        handlerFatigue = Mathf.Clamp(handlerFatigue, 0f, handlerMaxFatigue);
    }


    //Input Stubs

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Stub
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // Stub
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Stub
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        // Stub
    }
}
