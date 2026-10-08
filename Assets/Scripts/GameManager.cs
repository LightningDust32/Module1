using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    [Header("Loss States")]
    [SerializeField] bool activateLossStates = false;

    private bool gameOver = false;

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

        gameOver = true;

        Debug.Log("LOSS STATE: Gisgo starved.");
    }

    public void ElephantThirstReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        Debug.Log("LOSS STATE: Gisgo thirst reached zero.");
    }

    public void ElephantFatigueReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        gameOver = true;

        Debug.Log("Gisgo Collapsed From Exhaustion.");
    }

    public void ElephantTempReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        gameOver = true;

        Debug.Log("LOSS STATE: Elephant temperature reached zero.");
    }

    public void ElephantHealthReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        gameOver = true;

        Debug.Log("LOSS STATE: Elephant health reached zero.");
    }

    public void HandlerHungerReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        gameOver = true;

        Debug.Log("LOSS STATE: Handler hunger reached zero.");
    }

    public void HandlerThirstReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        gameOver = true;

        Debug.Log("LOSS STATE: Handler thirst reached zero.");
    }

    public void HandlerFatigueReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        gameOver = true;

        Debug.Log("LOSS STATE: Handler fatigue reached zero.");
    }
}
