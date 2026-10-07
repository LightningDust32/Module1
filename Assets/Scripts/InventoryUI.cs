using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;

    private InputSystem_Actions inputActions;

    public InventorySlotUI[] slots;

    public Inventory inventory;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Inventory.performed += ToggleInventory;
    }

    private void OnDisable()
    {
        inputActions.Player.Inventory.performed -= ToggleInventory;
        inputActions.Disable();
    }

    private void ToggleInventory(InputAction.CallbackContext context)
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);

        DisplayInventory();
    }

    private void DisplayInventory()
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
