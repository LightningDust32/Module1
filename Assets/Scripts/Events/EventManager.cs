using UnityEngine;

public class EventManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] GameEvent testEvent;

    private void Awake()
    {
        ApplyEvent(testEvent);
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

        Debug.Log(
            $"EVENT: {gameEvent.eventName}\n" +
            $"Time: {player.GetTimeString()}\n" +
            $"Elephant - Hunger: {player.GetElephantHunger():0.0}, " +
            $"Thirst: {player.GetElephantThirst():0.0}, " +
            $"Fatigue: {player.GetElephantFatigue():0.0}, " +
            $"Temperature: {player.GetElephantTemp():0.0}\n" +
            $"Handler - Hunger: {player.GetHandlerHunger():0.0}, " +
            $"Thirst: {player.GetHandlerThirst():0.0}, " +
            $"Fatigue: {player.GetHandlerFatigue():0.0}"
        );
    }
}
