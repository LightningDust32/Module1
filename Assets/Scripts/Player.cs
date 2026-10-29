using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    private RandomWeatherEvent randomWeatherEvent;

    [Header("Movement")]
    [SerializeField] private float elephantMoveSpeed = 50f;
    [SerializeField] private float handlerMoveSpeed = 20f;
    [SerializeField] private float turnSpeed = 120f;
    [SerializeField] private float gravity = -9f;

    private float verticalVelocity;

    [Header("Elephant Visuals")]
    [SerializeField] private Transform elephantBody;

    [Header("Elephant Momentum")]
    [SerializeField] private float elephantAcceleration = 1.6f;
    [SerializeField] private float elephantDeceleration = 2.8f;
    [SerializeField] private float reverseSpeedFactor = 0.2f;

    [Header("Elephant Bobbing")]
    [SerializeField] private float stepLength = 2.5f;
    [SerializeField] private float bobKick = 0.08f;
    [SerializeField] private float bodySpringStiffness = 25f;
    [SerializeField] private float bodySpringDamping = 6f;

    [Header("Elephant Lean")]
    [SerializeField] private float maxForwardLean = 4f;
    [SerializeField] private float maxTurnLean = 5f;
    [SerializeField] private float leanSmoothness = 5f;

    private float elephantCurrentSpeed;
    private float previousElephantSpeed;

    private Vector3 bodyRestPosition;
    private Quaternion bodyRestRotation;

    private float bobPosition;
    private float bobVelocity;
    private float stepDistance;

    private float currentForwardLean;
    private float currentTurnLean;

    [Header("Camera")]
    [SerializeField] private float cameraDistance = 10f;
    [SerializeField] private float cameraRotationSpeed = 80f;
    [SerializeField] private float zoomSpeed = 300f;
    [SerializeField] private float minCameraVerticalAngle = 10f;
    [SerializeField] private float maxCameraVerticalAngle = 70f;

    [Header("Handler")]
    [SerializeField] GameObject handler;
    [SerializeField] GameObject mountPoint;
    [SerializeField] GameObject dismountPoint;
    [SerializeField] Collider handlerCollider;

    [SerializeField] float mountDistance;

    bool isMounted = true;

    private float cameraYaw = 0f;
    private float cameraPitch = 25f;


    [Header("Elephant Stats")]
    [SerializeField] private float elephantHunger = 100f;
    [SerializeField] private float elephantThirst = 100f;
    [SerializeField] private float elephantFatigue = 100f;
    [SerializeField] private float elephantHealth = 100f;

    [Header("Elephant Maximums")]
    [SerializeField] private float elephantMaxHunger = 100f;
    [SerializeField] private float elephantMaxThirst = 100f;
    [SerializeField] private float elephantMaxFatigue = 100f;
    [SerializeField] private float elephantMaxHealth = 100f;

    [Header("Elephant Damages")]
    [SerializeField] private float hungerDamage = 0.5f;
    [SerializeField] private float thirstDamage = 1f;
    [SerializeField] private float fatigueDamage = 0.5f;
    [SerializeField] private float temperatureDamage = 0.5f;

    [Header("Handler Stats")]
    [SerializeField] private float handlerHunger = 100f;
    [SerializeField] private float handlerThirst = 100f;
    [SerializeField] private float handlerFatigue = 100f;

    [Header("Handler Maximums")]
    [SerializeField] private float handlerMaxHunger = 75f;
    [SerializeField] private float handlerMaxThirst = 75f;
    [SerializeField] private float handlerMaxFatigue = 75f;

    private bool elephantDied;


    [Header("Time")]
    [SerializeField] private int currentHour = 6;
    [SerializeField] private int currentMinute = 0;

    [SerializeField] private int sunriseHour = 6;
    [SerializeField] private int sunsetHour = 18;

    private int currentDay = 1;

    [Header("Travel Event")]
    [SerializeField] private float distanceUntilWalkEvent = 50f;
    [SerializeField] private GameEvent walkEvent;

    [Header("Wind")]
    [SerializeField] public Vector3 windDirection;
    [SerializeField] private float windStrength = 0.3f;
    [SerializeField] bool windActive = false;

    [Header("Followers")]
    [SerializeField] private SphereCollider followerRadius;
    [SerializeField] AudioClip callSound;

    private bool followersCalled = true;
    private int followerTotal = 0;


    private float distanceTravelled;

    // Input
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 zoomInput;

    private InputSystem_Actions inputActions;

    private CharacterController elephantController;
    private CharacterController handlerController;

    private Inventory inventory;

    private float tempModifier;
    private float speedModifier;

    private IInteraction currentInteraction;

    Animator animator;

    private void Awake()
    {
        elephantController = GetComponent<CharacterController>();
        handlerController = handler.GetComponent<CharacterController>();

        handlerCollider = handler.GetComponent<Collider>();

        inputActions = new InputSystem_Actions();

        inventory = GetComponent<Inventory>();

        followerRadius = GetComponent<SphereCollider>();

        randomWeatherEvent = FindAnyObjectByType<RandomWeatherEvent>();

        animator = GetComponent<Animator>();

        // Store these for returning to normal after bounce/lean animations
        if (elephantBody != null)
        {
            bodyRestPosition = elephantBody.localPosition;
            bodyRestRotation = elephantBody.localRotation;
        }

    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;

        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLook;

        inputActions.Player.Zoom.performed += OnZoom;

        inputActions.Player.Call.performed += OnCall;

        inputActions.Player.Interact.performed += OnInteract;

        inputActions.Player.Pause.performed += OnPause;

        inputActions.Player.Switch.performed += OnSwitch;

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

        inputActions.Player.Zoom.performed -= OnZoom;

        inputActions.Player.Call.performed -= OnCall;

        inputActions.Player.Interact.performed -= OnInteract;

        inputActions.Player.Pause.performed -= OnPause;

        inputActions.Player.Switch.performed -= OnSwitch;

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
        UpdateHealth();
        RegenerateHealth();
    }

    public void SetWind(float strength)
    {
        if (strength == 1f)
        {
            windActive = false;
            return;
        }
        windActive = true;
        windStrength = strength;
        windDirection = new Vector3 ( Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f) );
        float angle = Mathf.Atan2(windDirection.x, windDirection.z) * Mathf.Rad2Deg;
    }

    private void Move()
    {
        if (Time.timeScale < 1)
        {
            return;
        }

        float deltaTime = Time.deltaTime;

        if (isMounted)
        {
            MoveElephant(deltaTime);
        }
        else
        {
            MoveHandler(deltaTime);
        }
    }

    private void MoveElephant(float deltaTime)
    {
        // Turn the elephant.
        float turn = moveInput.x;
        transform.Rotate(Vector3.up, turn * turnSpeed * deltaTime);

        // Apply fatigue to the elephant's maximum speed.
        speedModifier = Mathf.Lerp(0.3f, 1f, elephantFatigue / 100f);

        float maxElephantSpeed = elephantMoveSpeed * speedModifier;

        // Smoothly accelerate and decelerate.
        float targetSpeed = moveInput.y * maxElephantSpeed;

        // Reduce speed when reversing direction.
        if (Mathf.Sign(targetSpeed) != Mathf.Sign(elephantCurrentSpeed) && Mathf.Abs(elephantCurrentSpeed) > 0.01f)
        {
            targetSpeed *= reverseSpeedFactor;
        }

        previousElephantSpeed = elephantCurrentSpeed;

        float accelerationRate = Mathf.Abs(targetSpeed) > Mathf.Abs(elephantCurrentSpeed) && Mathf.Sign(targetSpeed) == Mathf.Sign(elephantCurrentSpeed) ? elephantAcceleration : elephantDeceleration;

        // When starting from rest, use acceleration.
        if (Mathf.Abs(elephantCurrentSpeed) < 0.01f || Mathf.Sign(targetSpeed) == Mathf.Sign(elephantCurrentSpeed))
        {
            accelerationRate = Mathf.Abs(targetSpeed) > Mathf.Abs(elephantCurrentSpeed) ? elephantAcceleration : elephantDeceleration;
        }

        elephantCurrentSpeed = Mathf.MoveTowards(elephantCurrentSpeed, targetSpeed, accelerationRate * deltaTime);

        // Gravity and movement remain on the parent controller.
        if (elephantController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * deltaTime;

        Vector3 movementDelta = transform.forward * elephantCurrentSpeed * deltaTime + Vector3.up * verticalVelocity * deltaTime;

        Vector3 previousPosition = transform.position;

        elephantController.Move(movementDelta);

        // Use actual displacement for travel events and footsteps.
        Vector3 actualMovement = transform.position - previousPosition;
        actualMovement.y = 0f;

        distanceTravelled += actualMovement.magnitude;

        CheckTravelEvent();

        // Trigger a visual dip at regular intervals while moving.
        if (actualMovement.magnitude > 0f)
        {
            stepDistance += actualMovement.magnitude;

            while (stepDistance >= stepLength)
            {
                stepDistance -= stepLength;
                bobVelocity -= bobKick;
            }
        }

        AnimateElephant(deltaTime);
    }

    private void MoveHandler(float deltaTime)
    {
        Transform mover = handler.transform;

        if (handlerController == null)
            return;

        mover.Rotate(Vector3.up, moveInput.x * turnSpeed * deltaTime);

        Vector3 horizontalMovement = mover.forward * moveInput.y * handlerMoveSpeed * deltaTime;

        if (handlerController.isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        verticalVelocity += gravity * deltaTime;

        Vector3 movementDelta = horizontalMovement + Vector3.up * verticalVelocity * deltaTime;
        handlerController.Move(movementDelta);
    }

    private void AnimateElephant(float deltaTime)
    {
        if (elephantBody == null)
            return;

        // Damped spring for the vertical body bob.
        bobVelocity += (-bodySpringStiffness * bobPosition - bodySpringDamping * bobVelocity) * deltaTime;

        bobPosition += bobVelocity * deltaTime;

        // Lean forward while accelerating and backward while slowing.
        float acceleration = (elephantCurrentSpeed - previousElephantSpeed) / deltaTime;

        float forwardLeanTarget = Mathf.Clamp(-acceleration * 1.5f, -maxForwardLean, maxForwardLean);

        // Lean sideways into turns.
        float turnLeanTarget = -moveInput.x * maxTurnLean;

        currentForwardLean = Mathf.Lerp(currentForwardLean, forwardLeanTarget, leanSmoothness * deltaTime);

        currentTurnLean = Mathf.Lerp(currentTurnLean, turnLeanTarget, leanSmoothness * deltaTime);

        // Apply animation only to the visual element, not the controller.
        elephantBody.localPosition = bodyRestPosition + Vector3.up * bobPosition;

        elephantBody.localRotation = bodyRestRotation * Quaternion.Euler(currentForwardLean, 0f, currentTurnLean);
    }


    void UpdateHealth()
    {
        float damagePerSecond = 0f;

        // Add damage if stats are below a certain threshold
        // Check hunger
        if (elephantHunger < 25f)
        {
            damagePerSecond += hungerDamage;
        }

        // Check thirst
        if (elephantThirst < 25f)
        {
            damagePerSecond += thirstDamage;
        }

        // Check fatigue
        if (elephantFatigue < 25f)
        {
            damagePerSecond += fatigueDamage;
        }



        // Check temperature
        if (randomWeatherEvent.currentTemp < -5f)
        {
            damagePerSecond += temperatureDamage;
        }

        // Update the health variable
        elephantHealth -= damagePerSecond * Time.deltaTime;


        // Clamp health between 0 and 100
        elephantHealth = Mathf.Clamp(elephantHealth, 0f, 100f);
    }

    void RegenerateHealth()
    {
        bool idealConditions = elephantHunger >= 25f && elephantThirst >= 25f && elephantFatigue >= 25f && randomWeatherEvent.currentTemp >= -5f;

        if (idealConditions)
        {
            elephantHealth += 0.5f * Time.deltaTime;

            elephantHealth = Mathf.Clamp(elephantHealth, 0f, 100f);
        }
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

        if(cameraTransform.TryGetComponent<Camera>(out Camera camera))
        {
            if(camera.orthographic)
            {
                camera.orthographicSize -= zoomInput.y * zoomSpeed;
                camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 2, 100);
            }
            else
            {
                cameraDistance -= zoomInput.y * zoomSpeed * Time.deltaTime;
            }
        }

        Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);

        Vector3 offset = rotation * new Vector3(0f, 0f, -cameraDistance);

        if(handler != null)
        {
            cameraTransform.position = handler.transform.position + offset;

            cameraTransform.LookAt(handler.transform.position);
        }
        else
        {
            cameraTransform.position = transform.position + offset;

            cameraTransform.LookAt(transform.position);
        }

        
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

    private void StopFollowingFollowers()
    {
        Follower[] followers = FindObjectsByType<Follower>();

        foreach (Follower follower in followers)
        {
            follower.StopFollowing();
        }

        followerTotal = 0;

        followersCalled = false;

        if(animator != null)
        {
            animator.SetBool("Call", followersCalled);
        }
    }

    private void CallNearbyFollowers()
    {
        // Bug found in testing where if the radius is overlapping when reactivated, it wont retrigger the follower, this is the fix (doing it manually on all followers within range)
        if (followerRadius == null)
        {
            return;
        }

        Vector3 worldCenter = followerRadius.transform.TransformPoint(followerRadius.center);

        float worldRadius = followerRadius.radius * Mathf.Max(followerRadius.transform.lossyScale.x, followerRadius.transform.lossyScale.y, followerRadius.transform.lossyScale.z);

        Collider[] nearbyColliders = Physics.OverlapSphere(worldCenter, worldRadius, Physics.AllLayers, QueryTriggerInteraction.Ignore);

        followersCalled = true;

        if (animator != null)
        {
            animator.SetBool("Call", followersCalled);
        }

        foreach (Collider nearbyCollider in nearbyColliders)
        {
            Follower follower = nearbyCollider.GetComponentInParent<Follower>();

            if (follower != null)
            {
                follower.StartFollowing(transform);
                followerTotal += 1;
            }
        }
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

    public float GetElephantHealth()
    {
        return elephantHealth;
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

    public float GetElephantMaxHealth()
    {
        return elephantMaxHealth;
    }

    public int GetFollowerCount()
    {
        return followerTotal;
    }

    public void ChangeElephantThirst(float thirst)
    {
        float previousThirst = elephantThirst;

        // If restoring thirst, only work if player has water
        if (thirst > 0)
        {
            ItemData water = inventory.FindItem("Water");

            if(water == null)
            {
                Debug.Log("No Water");
                return;
            }
            else
            {
                inventory.RemoveItem(water);
            }
        }

        elephantThirst += thirst;
        elephantThirst = Mathf.Clamp(elephantThirst, 0f, elephantMaxThirst);
    }

    public void ChangeElephantHunger(float hunger)
    {
        float previousHunger = elephantHunger;

        // If restoring hunger, only work if player has food
        if (hunger > 0)
        {
            ItemData food = inventory.FindItem("Food");

            if (food == null)
            {
                Debug.Log("No Food");
                return;
            }
            else
            {
                inventory.RemoveItem(food);
            }
        }

        elephantHunger += hunger;
        elephantHunger = Mathf.Clamp(elephantHunger, 0f, elephantMaxHunger);
    }

    public void ChangeElephantFatigue(float fatigue)
    {
        float previousFatigue = elephantFatigue;

        elephantFatigue += fatigue;
        elephantFatigue = Mathf.Clamp(elephantFatigue, 0f, elephantMaxFatigue);
    }


    public void ChangeElephantHealth(float health)
    {
        float previousHealth = elephantHealth;

        elephantHealth += health;
        elephantHealth = Mathf.Clamp(elephantHealth, 0f, elephantMaxHealth);

        if (previousHealth > 0f && elephantHealth <= 0f && !elephantDied)
        {
            elephantDied = true;

            if (GameManager.instance != null)
            {
                GameManager.instance.ElephantDied();
            }

            if (elephantHealth > 0f)
            {
                elephantDied = false;
            }
        }
    }
    

    public void SetMoveSpeed(float speedMod)
    {
        speedModifier = speedMod;
    }

    public void SetModifier(float modifier)
    {
        tempModifier = modifier;

    }

    public void SetCurrentInteraction(IInteraction interaction)
    {
        currentInteraction = interaction;
    }

    public bool GetIsMounted()
    {
        return isMounted;
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

    public int GetCurrentTime()
    {
        return currentHour;
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

    private void SetHandlerPosition(Vector3 position, Quaternion rotation, CharacterController controller)
    {
        if(controller != null)
        {
            controller.enabled = false;
        }

        handler.transform.SetLocalPositionAndRotation(position, rotation);

        if (controller != null)
        {
            controller.enabled = true;
        }
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

    public void OnZoom(InputAction.CallbackContext context)
    {
        zoomInput = context.ReadValue<Vector2>();
    }

    public void OnCall(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        followersCalled = !followersCalled;

        if (followerRadius != null)
        {
            followerRadius.enabled = followersCalled;
        }

        if (followersCalled)
        {
            CallNearbyFollowers();
            Debug.Log("Followers called. Follow the player.");
        }
        else
        {
            StopFollowingFollowers();
            Debug.Log("Followers halted.");
        }

        if(callSound != null && SoundManager.instance != null)
        {
            SoundManager.instance.PlaySound(callSound);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (currentInteraction == null)
        {
            Debug.Log("Nothing nearby to interact with.");
            return;
        }

        currentInteraction.Interact();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        UIManager.Instance.Pause();
    }

    private void OnSwitch(InputAction.CallbackContext context)
    {
        if(handler == null || mountPoint == null || dismountPoint == null)
        {
            return;
        }

        CharacterController handlerController = handler.GetComponent<CharacterController>();

        if (isMounted)
        {
            // Dismount logic
            handler.transform.SetParent(null, true);

            SetHandlerPosition(dismountPoint.transform.position, dismountPoint.transform.rotation, handlerController);

            isMounted = false;
            handlerController.enabled = true;
            elephantController.enabled = false;
            handlerCollider.enabled = true;
        }
        else
        {
            float distance = Vector3.Distance(handler.transform.position, mountPoint.transform.position);

            if(distance > mountDistance)
            {
                return;
            }

            SetHandlerPosition(mountPoint.transform.position, mountPoint.transform.rotation, handlerController);

            handler.transform.SetParent(transform, true);

            isMounted = true;
            handlerController.enabled = false;
            elephantController.enabled = true;
            handlerCollider.enabled = false;
        }
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

    public GameObject GetHandler()
    {
        return handler;
    }

    private void OnTriggerEnter(Collider other)
    {
        Follower follower = other.GetComponent<Follower>();

        InteractionBase interaction = other.GetComponent<InteractionBase>();

        if (follower != null)
        {
            follower.StartFollowing(transform);
            followerTotal += 1;
        }

        if (interaction != null)
        {
            Debug.Log("Interaction trigger: " + interaction.gameObject.name);
            SetCurrentInteraction(interaction);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractionBase interaction = other.GetComponentInParent<InteractionBase>();

        if (interaction != null)
        {
            if (currentInteraction == interaction)
            {
                SetCurrentInteraction(null);
            }
        }
    }
}
