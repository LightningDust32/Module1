using UnityEngine;

public class LiftObject : InteractionBase
{
    [SerializeField] GameObject reward;

    public override void Interact()
    {
        if (!ElephantInRange)
        {
            return;
        }

        if(reward != null)
        {
            GameObject soldier = Instantiate(reward, transform.position, transform.rotation);

            soldier.transform.SetParent(null);
        }

        Destroy(gameObject);

        Debug.Log($"Interacted with {InteractionName}.");
    }
}
