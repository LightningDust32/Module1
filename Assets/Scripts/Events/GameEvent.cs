using UnityEngine;

[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/GameEvent")]
public class GameEvent : ScriptableObject
{
    [Header("Event Information")]
    public string eventName;
    public string eventDescription;

    [Header("Time")]
    public int timeMinutes;

    [Header("Elephant Changes")]
    public float elephantHunger;
    public float elephantThirst;
    public float elephantFatigue;
    public float elephantTemperature;

    [Header("Handler Changes")]
    public float handlerHunger;
    public float handlerThirst;
    public float handlerFatigue;
}
