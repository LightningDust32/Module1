using UnityEngine;
using UnityEngine.SceneManagement;

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

        // Logic for part of health calculation goes here
    }

    public void ElephantThirstReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        // Logic for part of health calculation goes here
    }

    public void ElephantFatigueReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        // Logic for part of health calculation goes here
    }


    public void ElephantHealthReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        GameOver();
    }

    public void HandlerHungerReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        GameOver();
    }

    public void HandlerThirstReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        GameOver();
    }

    public void HandlerFatigueReachedZero()
    {
        if (!activateLossStates)
        {
            return;
        }

        GameOver();
    }

    public void GameOver()
    {
        if (gameOver)
        {
            return;
        }

        gameOver = true;

        Debug.Log("Game Over");

        Restart();
    }

    public void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void Win()
    {
        // Add victory logic to trigger when player reaches end
    }
}
