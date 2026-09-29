using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Player player;
    [SerializeField] private EventManager eventManager;

    [Header("Elephant Stats")]
    [SerializeField] private Image hungerFill;
    [SerializeField] private Image thirstFill;
    [SerializeField] private Image temperatureFill;
    [SerializeField] private Image fatigueFill;

    [Header("Event Display")]
    [SerializeField] private TMP_Text eventText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text dayText;

    [Header("Event Button")]
    [SerializeField] private Button eventButton;
    [SerializeField] private TMP_Text eventButtonText;

    private EventTrigger currentEventTrigger;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
        }

        UpdateStats();
    }

    private void Update()
    {
        UpdateStats();
    }

    public void UpdateStats()
    {
        if (player == null)
        {
            return;
        }

        hungerFill.fillAmount = player.GetElephantHunger() / player.GetElephantMaxHunger();

        thirstFill.fillAmount = player.GetElephantThirst() / player.GetElephantMaxThirst();

        temperatureFill.fillAmount = player.GetElephantTemp() / player.GetElephantMaxTemperature();

        fatigueFill.fillAmount = player.GetElephantFatigue() / player.GetElephantMaxFatigue();

        timeText.text = player.GetTimeString();
        dayText.text = player.GetDayString();
    }

    public void ShowEvent(GameEvent gameEvent)
    {
        if (eventText == null)
        {
            return;
        }

        if (gameEvent == null)
        {
            eventText.text = "";
            return;
        }

        eventText.text = gameEvent.eventDescription;
    }

    public void ClearEvent()
    {
        if (eventText != null)
        {
            eventText.text = "";
        }
    }

    public void ShowEventButton(EventTrigger eventTrigger)
    {
        if (eventTrigger == null)
        {
            return;
        }

        currentEventTrigger = eventTrigger;

        GameEvent gameEvent = eventTrigger.GetGameEvent();

        if (gameEvent == null)
        {
            return;
        }

        eventButtonText.text = gameEvent.eventName;

        eventButton.gameObject.SetActive(true);

        eventButton.onClick.RemoveAllListeners();
        eventButton.onClick.AddListener(OnEventButtonClicked);
    }

    public void HideEventButton(EventTrigger eventTrigger)
    {
        if (currentEventTrigger != eventTrigger)
        {
            return;
        }

        currentEventTrigger = null;

        eventButton.onClick.RemoveAllListeners();
        eventButton.gameObject.SetActive(false);
    }

    private void OnEventButtonClicked()
    {
        if (currentEventTrigger == null)
        {
            return;
        }

        EventTrigger eventToActivate = currentEventTrigger;

        eventToActivate.ActivateEvent();

        // Remove the active event after using it.
        currentEventTrigger = null;

        eventButton.onClick.RemoveAllListeners();
        eventButton.gameObject.SetActive(false);
    }
}
