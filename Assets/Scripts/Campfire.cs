using Unity.VisualScripting;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    Player player;
    Inventory inventory;
    public GameObject fireParticles;
    [SerializeField] bool lit;




    private void Awake()
    {

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();
        inventory = FindAnyObjectByType<Inventory>();
        if (lit)
        {
            fireParticles.SetActive(true);
        }
        else
        {
            fireParticles.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Player" && player.GetIsMounted() == true) || other.gameObject.tag == "Handler")
        {
            inventory.inventoryUseMethod = InventoryUseMode.campfire;
            inventory.localCampfire = this.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Player" && player.GetIsMounted() == true) || other.gameObject.tag == "Handler")
        {
            inventory.inventoryUseMethod = InventoryUseMode.consume;
        }
    }

    public void ToggleCampfire(bool toggle)
    {
        if (toggle)
        {
            fireParticles.SetActive(true);
        }
        else
        {
            fireParticles.SetActive(false);
        }
    }


}
