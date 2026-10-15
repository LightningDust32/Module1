using UnityEngine;

public class Compass : MonoBehaviour
{
    public RectTransform compassBar;

    public RectTransform northMarker;
    public RectTransform eastMarker;
    public RectTransform southMarker;
    public RectTransform westMarker;

  //  public RectTransform objectiveMarker;


    public Transform cameraObjectTransform;
    public Transform objectiveTransform;

    private void Update()
    {
   //     SetMarkerPosition(objectiveMarker, objectiveTransform.position);
        SetMarkerPosition(northMarker, Vector3.forward * 1000000);
        SetMarkerPosition(eastMarker, Vector3.right * 1000000);
        SetMarkerPosition(southMarker, Vector3.back * 1000000);
        SetMarkerPosition(westMarker, Vector3.left * 1000000);
    }

    private void SetMarkerPosition(RectTransform markerPosition, Vector3 worldPos)
    {
        Vector3 directionToTarget = worldPos - cameraObjectTransform.position;
        float angle = Vector3.SignedAngle(new Vector3(cameraObjectTransform.forward.x, 0, cameraObjectTransform.forward.z), new Vector3(directionToTarget.x, 0, directionToTarget.z), Vector3.up);
        float compassPos = angle / Camera.main.fieldOfView;
       // compassPos = Mathf.Clamp(compassPos, -0.5f, 0.5f);


        // dont render when off to side
        if (compassPos >= -0.5f && compassPos <= 0.5f)
        {
            markerPosition.gameObject.SetActive(true);
        }
        else
        {
            markerPosition.gameObject.SetActive(false);
        }

        markerPosition.anchoredPosition = new Vector2((compassBar.rect.width * compassPos) + 95f, -10f);

    }
}
