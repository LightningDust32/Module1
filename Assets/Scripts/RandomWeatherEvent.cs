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
    float[] weatherEventChances = { 80f, 10f, 5f, 2.5f, 1.5f, 1f };
    string[] events = { "calm", "lightSnowfall", "snowShower", "generalSnowstorm", "blizzard", "blizzard + Extreme avalanche risk" };
    float[] moveSpeeds = { 1f, 1f, 0.8f, 0.7f, 0.5f, 0.3f};
    float[] modifier = { 1f, 1f, 1.2f, 1.5f, 2f, 2.2f };
    float[] windSpeeds = { 1f, 2f, 3f, 4f, 5f, 6f };
    int[] temperatures = { 5, 1, -5, -8, -15, -16 };

    [SerializeField] Player player;
    public TMP_Text weatherTypeUI;



    [SerializeField] float startTemp = 10f;
    [SerializeField] float currentTemp;
    [SerializeField] float minTemp = -20f;
    [SerializeField] float maxTemp = 20f;
    public TMP_Text temperatureUI;

    private bool openToElements = true;




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

        if (openToElements)
        {
            int tempVariation = Random.Range(-3, 3);
            int newTemp = temperatures[i] + tempVariation;
            temperatureUI.text = newTemp + "°c";
        }
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
        if(temperatureUI  == null)
        {
            return;
        }
        temperatureUI.text = currentTemp + "°c";
    }
}
