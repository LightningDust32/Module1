using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private GameEvent[] gameEvent;
    [SerializeField] private GameEvent restEvent;

    private GameEvent currentEvent;

    private bool playerInside;
    private Player player;

    [SerializeField] bool buttonEvent = true;
    [SerializeField] bool movementEvent = false;

    [SerializeField] int triggerCount = 3;
    int timesTriggered = 0;

    private void Start()
    {
        currentEvent = gameEvent[Random.Range(0, gameEvent.Length)];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;
        player = other.GetComponent<Player>();

        if (UIManager.Instance != null && buttonEvent)
        {
            UIManager.Instance.ShowEventButton(this);
        }
        else if (movementEvent == true)
        {
            timesTriggered++;
            Debug.Log(timesTriggered);

            if (timesTriggered >= triggerCount)
            {
                ActivateEvent();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = false;

        if (UIManager.Instance != null && buttonEvent)
        {
            UIManager.Instance.HideEventButton(this);
            currentEvent = gameEvent[Random.Range(0, gameEvent.Length)];
        }
    }

    public void ActivateEvent()
    {
        if (!playerInside)
        {
            Debug.Log("Player is no longer inside this event trigger.");
            return;
        }

        if (gameEvent == null)
        {
            Debug.LogWarning("EventTrigger has no GameEvent assigned.");
            return;
        }

        // After sunset, only the rest event can be activated.
        if (player.IsNight())
        {
            player.RestUntilMorning(restEvent);
        }

        if (gameEvent == null)
        {
            Debug.LogWarning("EventTrigger: No game event assigned.");
            return;
        }

        EventManager.instance.ApplyEvent(currentEvent);

        if (UIManager.Instance != null && buttonEvent)
        {  
            UIManager.Instance.HideEventButton(this);
        }

        if (currentEvent.singleUse == true)
        {
            Destroy(gameObject);
        }

    }

    public GameEvent GetGameEvent()
    {
        return currentEvent;
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }

}
