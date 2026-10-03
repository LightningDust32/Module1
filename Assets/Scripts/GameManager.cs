using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Loss States")]
    [SerializeField] bool activateLossStates = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void ElephantHungerReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Elephant hunger reached zero.");
    }

    public void ElephantThirstReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Elephant thirst reached zero.");
    }

    public void ElephantFatigueReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Elephant fatigue reached zero.");
    }

    public void ElephantTemperatureReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Elephant temperature reached zero.");
    }

    public void HandlerHungerReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Handler hunger reached zero.");
    }

    public void HandlerThirstReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Handler thirst reached zero.");
    }

    public void HandlerFatigueReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Handler fatigue reached zero.");
    }
}
