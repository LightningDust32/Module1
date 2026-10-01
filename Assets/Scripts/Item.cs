using System.Runtime.CompilerServices;
using UnityEngine;


public enum ItemType
{
    Food,
    Water,
    Wood,
    None
}



public class Item : MonoBehaviour
{
    private Inventory inventory;
    [SerializeField] public ItemType itemType;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = FindAnyObjectByType<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        inventory.AddItem(this);
        Destroy(gameObject);
    }
}
