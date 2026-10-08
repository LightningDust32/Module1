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

    [Header("Inventory")]
    [SerializeField] private GameObject inventoryScreen;
    [SerializeField] private InventorySlotUI[] slots;

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
            player = FindAnyObjectByType<Player>();
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
        dayText.text = "Day: " + player.GetDay();
    }

    public void ShowEventButton(EventTrigger eventTrigger)
    {
        if (eventTrigger == null)
        {
            return;
        }

        if (eventText == null)
        {
            return;
        }

        currentEventTrigger = eventTrigger;

        GameEvent gameEvent = eventTrigger.GetGameEvent();

        if (gameEvent == null)
        {
            eventText.text = "";
            return;
        }

        eventText.text = gameEvent.eventDescription;

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

        if (eventText != null)
        {
            eventText.text = "";
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

    public void ToggleInventory(Inventory inventory)
    {
        inventoryScreen.SetActive(!inventoryScreen.activeSelf);

        DisplayInventory(inventory);
    }

    public void DisplayInventory(Inventory inventory)
    {
        foreach (InventorySlotUI slot in slots)
        {
            slot.ClearSlot();
        }

        for (int i = 0; i < inventory.items.Count && i < slots.Length; i++)
        {
            slots[i].SetItem(inventory.items[i]);
        }
    }
}
