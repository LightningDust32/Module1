using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenge : InteractionBase
{
    [Header("Scavenge")]
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private float scoutRadius = 8f;
    [SerializeField] private float scoutDuration = 5f;
    [SerializeField] private float spawnRadius = 10f;

    private bool isScavenging;

    public override void Interact()
    {
        if (isScavenging)
            return;

        if (!ElephantInRange)
            return;

        List<Follower> followers = player.GetFollowers();

        if (followers == null || followers.Count < followerLimit)
            return;

        if (itemPrefab == null)
            return;

        StartCoroutine(ScavengeRoutine(followers));
    }

    private IEnumerator ScavengeRoutine(List<Follower> followers)
    {
        isScavenging = true;

        // Limit the number of items to the follower limit.
        int itemCount = Mathf.Min(followers.Count, followerLimit);

        List<Follower> assignedFollowers = new List<Follower>();

        for (int i = 0; i < itemCount; i++)
        {
            Follower follower = followers[i];

            if (follower == null)
                continue;

            // Spawn an item at a random nearby position.
            Vector3 spawnPosition = transform.position + new Vector3(Random.Range(-spawnRadius, spawnRadius), 0f, Random.Range(-spawnRadius, spawnRadius));

            GameObject item = Instantiate(itemPrefab, spawnPosition, Quaternion.identity);

            // Give this follower its own item to investigate.
            follower.StartFollowing(item.transform);

            assignedFollowers.Add(follower);
        }

        // Allow the followers time to investigate their items.
        yield return new WaitForSeconds(scoutDuration);

        // Recall all assigned followers.
        foreach (Follower follower in assignedFollowers)
        {
            if (follower != null)
            {
                follower.StartFollowing(player.transform);
            }
        }

        isScavenging = false;
    }
}