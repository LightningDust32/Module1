using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] private GameEvent gameEvent;

    private Player player;

    private bool playerInside;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = true;

        player.CurrentEventTrigger(this);

        Debug.Log($"Player can interact with: {gameEvent.eventName}");
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInside = false;

        player.CurrentEventTrigger(null);

        Debug.Log($"Player left: {gameEvent.eventName}");
    }

    public bool IsPlayerInside()
    {
        return playerInside;
    }

    public GameEvent GetGameEvent()
    {
        return gameEvent;
    }

}
