using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class InteractionBase : MonoBehaviour, IInteraction
{
    [Header("Interaction")]
    [SerializeField] private string interactionName = "Interact";
    [SerializeField] private string interactionDescription = "";
    [SerializeField] protected int followerLimit = 5;

    [Header("Trigger")]
    [SerializeField] private bool elephantInRange;
    [SerializeField] private bool handlerInRange;

    [SerializeField] protected bool handlerOnly = false;

    public string InteractionName => interactionName;
    public string InteractionDescription => interactionDescription;
    public bool ElephantInRange => elephantInRange;
    public bool HandlerInRange => handlerInRange;

    protected Player player;

    protected virtual void Awake()
    {
        Collider interactionCollider = GetComponent<Collider>();

        if (!interactionCollider.isTrigger)
        {
            Debug.LogWarning($"{name}: Interaction collider should have Is Trigger enabled.", this);
        }

        player = FindAnyObjectByType<Player>();
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elephantInRange = true;
        }

        if(other.CompareTag("Handler"))
        {
            handlerInRange = true;
            Debug.Log("Handler enter");
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elephantInRange = false;
        }

        if (other.CompareTag("Handler"))
        {
            handlerInRange = false;
            Debug.Log("Handler exit");
        }
    }

    public abstract void Interact();
}