using UnityEngine;

public class Avalanche : MonoBehaviour
{
    RandomWeatherEvent randomWeatherEvent;
    bool slide = false;
    float slideSpeed = 20;
    GameObject avalancheObject;
    private void Start()
    {
        avalancheObject = transform.parent.gameObject;
        randomWeatherEvent = FindAnyObjectByType<RandomWeatherEvent>();   
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
          //  Debug.Log("Entered Avalanche Zone");
            float avalancheChance = Random.Range(0, 1000);

            if (avalancheChance < randomWeatherEvent.avalancheRiskPercent)
            {
                slide = true;
            }
        }
    }

    private void Update()
    {
        if (slide)
        {
            avalancheObject.transform.position += -transform.forward * slideSpeed * Time.deltaTime;
        }
    }




}
