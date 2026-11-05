using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Follower follower = other.GetComponent<Follower>();
        Player player = other.GetComponent<Player>();

        if(follower != null)
        {
            follower.StopFollowing();
            Destroy(other.gameObject);
        }

        if(player != null)
        {
            GameManager.instance.Restart();
        }
    }
}
