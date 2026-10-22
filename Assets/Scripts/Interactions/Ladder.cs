using UnityEngine;

public class Ladder : InteractionBase
{
    [SerializeField] GameObject ladderBase;
    [SerializeField] GameObject ladderTop;

    public override void Interact()
    {
        if (!PlayerInRange)
        {
            return;
        }

        if(ladderBase == null ||  ladderTop == null)
        {
            return;
        }

        float baseDistance = Vector3.Distance(player.transform.position, ladderBase.transform.position);
        float topDistance = Vector3.Distance(player.transform.position, ladderTop.transform.position);
        CharacterController controller = player.GetComponent<CharacterController>();

        if(baseDistance < topDistance)
        {
            controller.enabled = false;
            player.transform.position = ladderTop.transform.position;
            Debug.Log("climb up");
            controller.enabled = true;
        }
        else
        {
            controller.enabled = false;
            player.transform.position = ladderBase.transform.position;
            Debug.Log("climb down");
            controller.enabled = true;
        }
    }
}
