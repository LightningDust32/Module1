using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private GameEvent[] gameEvent;
    [SerializeField] private GameEvent restEvent;

    private GameEvent currentEvent;

    private bool playerInside;
    private Player player;

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

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowEventButton(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = false;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideEventButton(this);
        }

        currentEvent = gameEvent[Random.Range(0, gameEvent.Length)];
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
