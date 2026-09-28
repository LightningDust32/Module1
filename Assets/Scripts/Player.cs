using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

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

    // Input
    private Vector2 moveInput;
    private Vector2 lookInput;

    private InputSystem_Actions inputActions;

    private CharacterController controller;

    private EventTrigger currentTrigger;

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
    }

    private void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        // Prevent diagonal movement from being faster
        move = Vector3.ClampMagnitude(move, 1f);

        controller.Move(moveSpeed * Time.deltaTime * move);
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

    public void CurrentEventTrigger(EventTrigger trigger)
    {
        currentTrigger = trigger;
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
        if (!context.performed)
        {
            return;
        }

        EventTrigger eventTrigger = currentTrigger;

        if (eventTrigger == null)
        {
            return;
        }

        GameEvent gameEvent = eventTrigger.GetGameEvent();

        if (gameEvent == null)
        {
            return;
        }

        Debug.Log($"Activating event: {gameEvent.eventName}");
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
