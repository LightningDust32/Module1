using UnityEngine;


[CreateAssetMenu(menuName = "Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int maxStack;
    public int weight;


    public Sprite icon;

    public bool isDrink;
    public bool isFood;
    public bool isMaterial;

    public int regenAmount;

    public bool isRaw;
}
