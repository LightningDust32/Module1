using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;



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
    float[] moveSpeeds = { 1f, 1f, 0.8f, 0.7f, 0.5f, 0.3f};
    float[] modifier = { 1f, 1f, 1.2f, 1.5f, 2f, 2.2f };
    float[] windSpeeds = { 1f, 2f, 3f, 4f, 5f, 6f };

    [SerializeField] Player player;
    public TMP_Text weatherTypeUI;



    [SerializeField] float startTemp = 10f;
    [SerializeField] float currentTemp;
    [SerializeField] float minTemp = -20f;
    [SerializeField] float maxTemp = 20f;
    public TMP_Text temperatureUI;




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
        weatherTypeUI.text = "Weather: " + events[i];
        player.SetMoveSpeed(moveSpeeds[i]);
        player.SetModifier(modifier[i]);
        player.SetWind(windSpeeds[i]);
    }

    private void Update()
    {
        
    }

    private void Start()
    {
        currentTemp = startTemp;
        SetTemperatureUI(currentTemp);
        pickRandomEvent();
        StartCoroutine(Wait10Seconds());
    }

    IEnumerator Wait10Seconds()
    {
        yield return new WaitForSeconds(10);
        pickRandomEvent();
        StartCoroutine(Wait10Seconds());
    }

    private void SetTemperatureUI(float temperature)
    {
        temperatureUI.text = currentTemp + "°c";
    }
}
