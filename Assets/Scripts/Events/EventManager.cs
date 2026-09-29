using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;

    public static EventManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void ApplyEvent(GameEvent gameEvent)
    {
        if (gameEvent == null)
        {
            Debug.LogWarning("EventManager: No event was provided.");
            return;
        }

        if (player == null)
        {
            Debug.LogWarning("EventManager: Player reference is missing.");
            return;
        }

        // Advance time
        player.AdvanceTime(gameEvent.timeMinutes);

        // Elephant
        player.ChangeElephantHunger(gameEvent.elephantHunger);
        player.ChangeElephantThirst(gameEvent.elephantThirst);
        player.ChangeElephantFatigue(gameEvent.elephantFatigue);
        player.ChangeElephantTemp(gameEvent.elephantTemperature);

        // Handler
        player.ChangeHandlerHunger(gameEvent.handlerHunger);
        player.ChangeHandlerThirst(gameEvent.handlerThirst);
        player.ChangeHandlerFatigue(gameEvent.handlerFatigue);

        UIManager.Instance.UpdateStats();
        UIManager.Instance.ShowEvent(gameEvent);
    }
}
