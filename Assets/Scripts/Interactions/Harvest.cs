using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Harvest : InteractionBase
{
    [SerializeField] GameObject tree;
    [SerializeField] private float scoutRadius = 8f;
    [SerializeField] private float scoutDuration = 5f;
    [SerializeField] private float treeSpawnRadius = 10f;

    private bool isHarvesting;

    public override void Interact()
    {
        if (isHarvesting)
            return;

        if (!ElephantInRange)
            return;

        List<Follower> followers = player.GetFollowers();

        if (followers == null || followers.Count < followerLimit)
            return;

        if (tree == null)
            return;

        StartCoroutine(HarvestRoutine(followers));
    }

    private IEnumerator HarvestRoutine(List<Follower> followers)
    {
        isHarvesting = true;

        Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-treeSpawnRadius, treeSpawnRadius), 0f, Random.Range(-treeSpawnRadius, treeSpawnRadius));

        GameObject harvestTree = Instantiate(tree, spawnPosition, Quaternion.identity);

        // Choose one follower to harvest the tree.
        int workerIndex = Random.Range(0, followers.Count);
        Follower worker = followers[workerIndex];

        // Send all other followers to scout nearby positions.
        foreach (Follower follower in followers)
        {
            if (follower == null || follower == worker)
                continue;

            GameObject scoutTarget = new GameObject("Scout Target");
            scoutTarget.transform.position = transform.position + new Vector3(Random.Range(-scoutRadius, scoutRadius), 0f, Random.Range(-scoutRadius, scoutRadius));

            follower.StartFollowing(scoutTarget.transform);

            Destroy(scoutTarget, scoutDuration + 10f);
        }

        // Send the chosen worker to the tree.
        worker.StartFollowing(harvestTree.transform);

        // Allow the scouts time to explore.
        yield return new WaitForSeconds(scoutDuration);

        // Recall scouts to the player. Leave the worker at the tree.
        foreach (Follower follower in followers)
        {
            if (follower == null || follower == worker)
                continue;

            follower.StartFollowing(player.transform);
        }

        isHarvesting = false;
    }
}