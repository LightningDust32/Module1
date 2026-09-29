using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private GameEvent gameEvent;

    private bool playerInside;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;

        Debug.Log($"Player entered event: {gameEvent.eventName}");

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

        Debug.Log($"Player left event: {gameEvent.eventName}");

        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideEventButton(this);
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

        if (EventManager.instance != null)
        {
            EventManager.instance.ApplyEvent(gameEvent);
        }
    }

    public GameEvent GetGameEvent()
    {
        return gameEvent;
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }

}
