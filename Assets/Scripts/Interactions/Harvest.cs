using UnityEngine;

public class Harvest : InteractionBase
{
    [SerializeField] GameObject tree;

    public override void Interact()
    {
        if (!PlayerInRange)
        {
            return;
        }

        if(player.GetFollowerCount() < followerLimit)
        {
            return;
        }

        if (tree != null)
        {
            GameObject harvestTree = Instantiate(tree, transform.position + new Vector3(Random.Range(10, 100), -2, Random.Range(10, 100)), transform.rotation);

            harvestTree.transform.SetParent(null);
        }
    }
}
