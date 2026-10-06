using UnityEngine;


[CreateAssetMenu(menuName = "Items")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int maxStack;
    public int weight;


    public Sprite icon;
}
