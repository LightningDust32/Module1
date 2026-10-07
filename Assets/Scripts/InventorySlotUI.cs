using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;
    public TMP_Text amountText;

    public void SetItem(InventorySlot inventorySlot)
    {
        icon.sprite = inventorySlot.item.icon;
        amountText.text = inventorySlot.quantity.ToString();
    }

    public void ClearSlot()
    {
        icon.sprite = null;
        amountText.text = "";
    }
}
