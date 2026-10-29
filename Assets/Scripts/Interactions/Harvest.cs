using UnityEngine;

public class Harvest : InteractionBase
{
    [SerializeField] GameObject tree;

    public override void Interact()
    {
        if (!ElephantInRange)
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

        // Once spawned, move a follower to it
    }
}
