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
            Instantiate(soldiers);
        }

        Destroy(gameObject);

        Debug.Log($"Interacted with {InteractionName}.");
    }
}
