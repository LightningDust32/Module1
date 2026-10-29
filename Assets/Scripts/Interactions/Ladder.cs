using UnityEngine;

public class Ladder : InteractionBase
{
    [SerializeField] GameObject ladderBase;
    [SerializeField] GameObject ladderTop;

    public override void Interact()
    {
        if (!HandlerInRange)
        {
            Debug.Log("No handler");
            return;
        }

        if(ladderBase == null ||  ladderTop == null)
        {
            Debug.Log("Missing ladder points");
            return;
        }

        GameObject handler = player.GetHandler();

        float baseDistance = Vector3.Distance(handler.transform.position, ladderBase.transform.position);
        float topDistance = Vector3.Distance(handler.transform.position, ladderTop.transform.position);
        CharacterController controller = handler.GetComponent<CharacterController>();

        if(baseDistance < topDistance)
        {
            controller.enabled = false;
            handler.transform.position = ladderTop.transform.position;
            Debug.Log("climb up");
            controller.enabled = true;
        }
        else
        {
            controller.enabled = false;
            handler.transform.position = ladderBase.transform.position;
            Debug.Log("climb down");
            controller.enabled = true;
        }
    }
}
