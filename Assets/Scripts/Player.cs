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
    [SerializeField] private float gravity = -9f;

    private float verticalVelocity;

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

    private bool elephantStarved;
    private bool elephantThirstZeroTriggered;
    private bool elephantFatigueZeroTriggered;
    private bool elephantFroze;

    private bool handlerHungerZeroTriggered;
    private bool handlerThirstZeroTriggered;
    private bool handlerFatigueZeroTriggered;

    [Header("Time")]
    [SerializeField] private int currentHour = 6;
    [SerializeField] private int currentMinute = 0;

    [SerializeField] private int sunriseHour = 6;
    [SerializeField] private int sunsetHour = 18;

    private int currentDay = 1;

    [Header("Travel Event")]
    [SerializeField] private float distanceUntilWalkEvent = 50f;
    [SerializeField] private GameEvent walkEvent;

    private float distanceTravelled;

    // Input
    private Vector2 moveInput;
    private Vector2 lookInput;

    private InputSystem_Actions inputActions;

    private CharacterController controller;

    private Inventory inventory;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        inputActions = new InputSystem_Actions();

        inventory = GetComponent<Inventory>();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;

        inputActions.Player.Inventory.performed += ToggleInventory;
        inputActions.Player.Inventory.canceled -= ToggleInventory;

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

        inputActions.Player.Inventory.performed -= ToggleInventory;
        inputActions.Player.Inventory.canceled -= ToggleInventory;

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

        Vector3 horizontalMovement = movement * moveSpeed * Time.deltaTime;

        // Keep the player attached to the terrain.
        if (controller.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        // Apply gravity.
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 movementDelta = horizontalMovement + Vector3.up * verticalVelocity * Time.deltaTime;

        Vector3 previousPosition = transform.position;

        controller.Move(movementDelta);

        // Measure actual movement after collision resolution.
        Vector3 actualMovement = transform.position - previousPosition;
        actualMovement.y = 0f;

        distanceTravelled += actualMovement.magnitude;

        distanceTravelled += horizontalMovement.magnitude;

        CheckTravelEvent();
    }

    private void CheckTravelEvent()
    {
        // Trigger time passing after walking
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

        while (currentHour >= 24)
        {
            currentHour -= 24;
            currentDay++;
        }
    }

    public void RestUntilMorning(GameEvent restEvent)
    {
        if (restEvent == null)
        {
            Debug.LogWarning("Player: Rest event has not been assigned.");
            return;
        }

        // If we're already in daylight, there is nothing to rest until.
        if (IsDaylight())
        {
            return;
        }

        if (elephantFatigue < elephantMaxFatigue)
        {
            elephantFatigue = elephantMaxFatigue;
        }

        if (handlerFatigue < handlerMaxFatigue)
        {
            handlerFatigue = handlerMaxFatigue;
        }

        // Advance to the next sunrise.
        currentDay++;

        currentHour = sunriseHour;
        currentMinute = 0;
    }

    public string GetTimeString()
    {
        return $"{currentHour:00}:{currentMinute:00}";
    }

    public string GetDayString()
    {
        return "Day: " + currentDay;
    }

    public int GetDay()
    {
        return currentDay;
    }

    public bool IsDaylight()
    {
        return currentHour >= sunriseHour && currentHour < sunsetHour;
    }

    public bool IsNight()
    {
        return !IsDaylight();
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
        float previousThirst = elephantThirst;

        elephantThirst += thirst;
        elephantThirst = Mathf.Clamp(elephantThirst, 0f, elephantMaxThirst);

        if (previousThirst > 0f && elephantThirst <= 0f && !elephantThirstZeroTriggered)
        {
            elephantThirstZeroTriggered = true;

            if (GameManager.instance != null)
            {
                GameManager.instance.ElephantThirstReachedZero();
            }

            if (elephantThirst > 0f)
            {
                elephantThirstZeroTriggered = false;
            }
        }
    }

    public void ChangeElephantHunger(float hunger)
    {
        float previousHunger = elephantHunger;

        elephantHunger += hunger;
        elephantHunger = Mathf.Clamp(elephantHunger, 0f, elephantMaxHunger);

        if(previousHunger > 0f && elephantHunger <= 0f && !elephantStarved)
        {
            elephantStarved = true;

            if(GameManager.instance != null)
            {
                GameManager.instance.ElephantHungerReachedZero();
            }

            if(elephantHunger > 0f)
            {
                elephantStarved = false;
            }
        }
    }

    public void ChangeElephantFatigue(float fatigue)
    {
        float previousFatigue = elephantFatigue;

        elephantFatigue += fatigue;
        elephantFatigue = Mathf.Clamp(elephantFatigue, 0f, elephantMaxFatigue);

        if(previousFatigue > 0f && elephantFatigue <= 0f && !elephantFatigueZeroTriggered)
        {
            elephantFatigueZeroTriggered = true;

            if(GameManager.instance != null)
            {
                GameManager.instance.ElephantFatigueReachedZero();
            }

            if(elephantFatigue > 0f)
            {
                elephantFatigueZeroTriggered = false;
            }
        }
    }

    public void ChangeElephantTemp(float temp)
    {
        float previousTemp = elephantTemperature;

        elephantTemperature += temp;
        elephantTemperature = Mathf.Clamp(elephantTemperature, 0f, elephantMaxTemperature);

        if( previousTemp > 0f && elephantTemperature <= 0f && !elephantFroze)
        {
            elephantFroze = true;

            if(GameManager.instance != null)
            {
                GameManager.instance.ElephantTempReachedZero();
            }

            if( elephantTemperature > 0f)
            {
                elephantFroze = false;
            }
        }
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

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        UIManager.Instance.ToggleInventory(inventory);
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
