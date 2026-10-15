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
    blizzardExtremeAvalancheRisk, // major lower temperature / major effect to speed
    none,
}


public class RandomWeatherEvent : MonoBehaviour
{
    // chances to genereate each weather event listed in the "Weather" enum
    float[] weatherEventChancesEarlyMorning = { 60f, 30f, 5f, 2.5f, 1.5f, 1f };
    float[] weatherEventChancesAfternoon = { 80f, 10f, 5f, 2.5f, 1.5f, 1f };
    float[] weatherEventChancesEvening = { 20f, 25f, 40f, 10f, 3f, 2f };
    float[] weatherEventChancesMidnight = { 5f, 15f, 50f, 20f, 6f, 4f };


    string[] events = { "calm", "lightSnowfall", "snowShower", "generalSnowstorm", "blizzard", "blizzard + Extreme avalanche risk" };
    float[] moveSpeeds = { 1f, 1f, 0.8f, 0.7f, 0.5f, 0.3f};
    float[] modifier = { 1f, 1f, 1.2f, 1.5f, 2f, 2.2f };
    float[] windSpeeds = { 1f, 2f, 3f, 4f, 5f, 6f };
    int[] temperatures = { 5, 1, -5, -8, -15, -16 };
    float[] waitUntilNextWeatherChange = { 30, 30, 30, 25, 20, 10 };
    float waitUntilChange;
    float[] avalancheRisk = { 1f, 3f, 5f, 20f, 30f, 50f };


    public float avalancheRiskPercent;

    [SerializeField] Player player;
    public TMP_Text weatherTypeUI;



    [SerializeField] float startTemp = 10f;
    [SerializeField] public float currentTemp;
    [SerializeField] float minTemp = -20f;
    [SerializeField] float maxTemp = 20f;
    public TMP_Text temperatureUI;

    private bool openToElements = true;




    public void pickRandomEvent()
    {
        // pick a random starting number
        float random = Random.Range(0, 100f);

        for (int i = 0; i < weatherEventChancesEarlyMorning.Length; i++)
        {
            // keep subtracting event chances until random becomes 0
            int currentTime = player.GetCurrentTime();

            if (currentTime >= 1 && currentTime <= 6)
            {
                random -= weatherEventChancesEarlyMorning[i];
            }
            if (currentTime >= 7 && currentTime <= 12)
            {
                random -= weatherEventChancesAfternoon[i];
            }
            if (currentTime >= 13 && currentTime <= 18)
            {
                random -= weatherEventChancesEvening[i];
            }
            if (currentTime >= 19 && currentTime <= 24)
            {
                random -= weatherEventChancesMidnight[i];
            }

            if (random <= 0)
            {
                // trigger the event that crossed random over 0
                TriggerEvent(i);
                SetFog(i);
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
        waitUntilChange = waitUntilNextWeatherChange[i];
        avalancheRiskPercent = avalancheRisk[i];

        if(temperatureUI == null)
        {
            return;
        }

        if (openToElements)
        {
            int tempVariation = Random.Range(-3, 3);
            int newTemp = temperatures[i] + tempVariation;
            currentTemp = newTemp;  
            temperatureUI.text = newTemp + "°c";
        }
    }

    public void SetFog(int i)
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = Color.white;
        switch(i)
        {
            case 0:
                RenderSettings.fogDensity = 0.001f;
                break;
            case 1:
                RenderSettings.fogDensity = 0.002f;
                break;
            case 2:
                RenderSettings.fogDensity = 0.003f;
                break;
            case 3:
                RenderSettings.fogDensity = 0.004f;
                break;
            case 4:
                RenderSettings.fogDensity = 0.005f;
                break;
            case 5:
                RenderSettings.fogDensity = 0.006f;
                break;
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
        yield return new WaitForSeconds(waitUntilChange);
        //yield return new WaitForSeconds(1);
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
