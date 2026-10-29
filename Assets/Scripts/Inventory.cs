
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEditor.Rendering;

public enum InventoryUseMode
{
    consume,
    campfire,
}

public class Inventory : MonoBehaviour
{
    public List<InventorySlot> items = new List<InventorySlot>();

    public event Action OnInventoryChanged;
    private InputSystem_Actions inputActions;

    [SerializeField] private GameEvent eatEvent;
    [SerializeField] private GameEvent drinkEvent;

    public InventoryUseMode inventoryUseMethod;

    public GameObject localCampfire;
    Campfire campfire;



    private void Awake()
    {
        inputActions = new InputSystem_Actions();
        inventoryUseMethod = InventoryUseMode.consume;
    }

    public void AddItem(ItemData item)
    {
        if (item == null) return;

        Debug.Log("Added " + item.itemName);

        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item &&
                items[i].quantity < item.maxStack)
            {
                items[i].quantity++;
                OnInventoryChanged?.Invoke();
                return;
            }
        }

        InventorySlot slot = new InventorySlot();
        slot.item = item;
        slot.quantity = 1;
        items.Add(slot);

        OnInventoryChanged?.Invoke();
    }

    public void RemoveItem(ItemData item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].item == item)
            {
                items[i].quantity--;

                if (items[i].quantity <= 0)
                {
                    items.RemoveAt(i);
                }

                OnInventoryChanged?.Invoke();
                Debug.Log("Removed " + item.itemName);
                return;
            }
        }
    }

    public ItemData FindItem(string name)
    {
        foreach (InventorySlot slot in items)
        {
            if (slot.item != null &&
                slot.item.name == name)
            {
                Debug.Log("Item Found");
                return slot.item;
            }
        }

        return null;
    }
    private void OnEnable()
    {
        inputActions.Player.InventorySlot1.performed += OnSlot1;
        inputActions.Player.InventorySlot2.performed += OnSlot2;
        inputActions.Player.InventorySlot3.performed += OnSlot3;
        inputActions.Player.InventorySlot4.performed += OnSlot4;
        inputActions.Player.InventorySlot5.performed += OnSlot5;
        inputActions.Player.InventorySlot6.performed += OnSlot6;

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.InventorySlot1.performed -= OnSlot1;
        inputActions.Player.InventorySlot2.performed -= OnSlot2;
        inputActions.Player.InventorySlot3.performed -= OnSlot3;
        inputActions.Player.InventorySlot4.performed -= OnSlot4;
        inputActions.Player.InventorySlot5.performed -= OnSlot5;
        inputActions.Player.InventorySlot6.performed -= OnSlot6;

        inputActions.Disable();
    }


    private void UseSlot(int slotIndex)
    {
        if (items.Count <= slotIndex)
            return;

        if (items[slotIndex].item == null)
            return;

        string itemName = items[slotIndex].item.itemName;

        if (inventoryUseMethod == InventoryUseMode.consume)
        {
            if (itemName == "Water")
            {
                EventManager.instance.ApplyEvent(drinkEvent);
            }
            else if (itemName == "Food")
            {
                EventManager.instance.ApplyEvent(eatEvent);
            }
        }
        else if (inventoryUseMethod == InventoryUseMode.campfire)
        {
            if (itemName == "Wood")
            {
                Campfire campfire = localCampfire.GetComponent<Campfire>();
                campfire.ToggleCampfire(true);
                ItemData wood = FindItem("Wood");
                RemoveItem(wood);
            }
        }





    }

    private void OnSlot1(InputAction.CallbackContext context)
    {
        UseSlot(0);
    }

    private void OnSlot2(InputAction.CallbackContext context)
    {
        UseSlot(1);
    }

    private void OnSlot3(InputAction.CallbackContext context)
    {
        UseSlot(2);
    }

    private void OnSlot4(InputAction.CallbackContext context)
    {
        UseSlot(3);
    }

    private void OnSlot5(InputAction.CallbackContext context)
    {
        UseSlot(4);
    }

    private void OnSlot6(InputAction.CallbackContext context)
    {
        UseSlot(5);
    }
}
