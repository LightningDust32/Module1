using UnityEngine;

public class Carriage : InteractionBase
{
    [SerializeField] GameObject soldiers;

    public override void Interact()
    {
        if (!PlayerInRange)
        {
            return;
        }

        if(soldiers != null)
        {
            GameObject soldier = Instantiate(soldiers, transform.position, transform.rotation);

            soldier.transform.SetParent(null);
        }

        Destroy(gameObject);

        Debug.Log($"Interacted with {InteractionName}.");
    }
}
