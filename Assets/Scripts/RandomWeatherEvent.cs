using Unity.VisualScripting;
using UnityEngine;



enum Weather
{
    calm, // average temperature / no effect to speed
    lightSnowfall, // slightly lower temperature / no effect to speed
    snowShower, // decently lower temperature / slight effect to speed
    generalSnowstorm, // significantly lower temperature / decent effect to speed
    blizzard, // major lower temperature / significant effect to speed
    iceStorm, // major lower temperature / major effect to speed
    none,
}


public class RandomWeatherEvent : MonoBehaviour
{
    // chances to genereate each weather event listed in the "Weather" enum
    float[] weatherEventChances = { 50f, 20f, 10f, 8f, 7f, 5f };
    string[] events = { "calm", "lightSnowfall", "snowShower", "generalSnowstorm", "blizzard", "iceStorm" };

    float[] moveSpeeds = { 5f, 5f, 4.5f };

    [SerializeField] Player player; 



    public void pickRandomEvent()
    {
        // pick a random starting number
        float random = Random.Range(0, 100f);

        for (int i = 0; i < weatherEventChances.Length; i++)
        {
            // keep subtracting event chances until random becomes 0
            random -= weatherEventChances[i];
            if (random <= 0)
            {
                // trigger the event that crossed random over 0
                TriggerEvent(i);
                break;
            }
        }
    }

    public void TriggerEvent(int i)
    {
        Debug.Log(events[i]);
    }

    private void Update()
    {
        pickRandomEvent();
    }
}
