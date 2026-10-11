using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractionBase : MonoBehaviour, IInteraction
{
    [Header("Interaction")]
    [SerializeField] private string interactionName = "Interact";
    [SerializeField] private string interactionDescription = "";

    [Header("Trigger")]
    [SerializeField] private bool playerInRange;

    public string InteractionName => interactionName;
    public string InteractionDescription => interactionDescription;
    public bool PlayerInRange => playerInRange;

    protected virtual void Awake()
    {
        Collider interactionCollider = GetComponent<Collider>();

        if (!interactionCollider.isTrigger)
        {
            Debug.LogWarning($"{name}: Interaction collider should have Is Trigger enabled.", this);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = true;
        Debug.Log($"Player can interact with {interactionName}.");
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        playerInRange = false;
        Debug.Log($"Player left {interactionName}.");
    }

    public abstract void Interact();
}