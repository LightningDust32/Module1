using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Follower follower = other.GetComponent<Follower>();

        if(follower != null)
        {
            follower.StopFollowing();
            Debug.Log("Follower is Kill");
            Destroy(other.gameObject);
        }
    }
}
