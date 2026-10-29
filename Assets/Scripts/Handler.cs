using UnityEngine;

public class Handler : MonoBehaviour
{
    Player player;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionBase interaction = other.GetComponent<InteractionBase>();

        if (interaction != null)
        {
            Debug.Log("Handler Interaction trigger: " + interaction.gameObject.name);
            player.SetCurrentInteraction(interaction);
        }
    }
}
