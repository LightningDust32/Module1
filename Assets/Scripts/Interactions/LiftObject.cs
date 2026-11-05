using UnityEngine;

public class LiftObject : InteractionBase
{
    [SerializeField] GameObject reward;

    public override void Interact()
    {
        if(!handlerOnly)
        {
            if (!ElephantInRange)
            {
                return;
            }
        }
        else
        {
            if (!HandlerInRange)
            {
                return;
            }
        }
        

        if (reward != null)
        {
            GameObject rewardSpawn = Instantiate(reward, transform.position, transform.rotation);

            rewardSpawn.transform.SetParent(null);
        }

        Destroy(gameObject);

        Debug.Log($"Interacted with {InteractionName}.");
    }
}
