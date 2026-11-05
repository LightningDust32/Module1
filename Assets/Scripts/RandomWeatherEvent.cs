using UnityEngine;
using System.Collections;

public enum Weather
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
    [SerializeField] ParticleSystem largeSnow;
    [SerializeField] ParticleSystem smallSnow;
    [SerializeField] GameObject snowfall;

    // chances to genereate each weather event listed in the "Weather" enum
    float[] weatherEventChancesEarlyMorning = { 60f, 30f, 5f, 2.5f, 1.5f, 1f };
    float[] weatherEventChancesAfternoon = { 80f, 10f, 5f, 2.5f, 1.5f, 1f };
    float[] weatherEventChancesEvening = { 20f, 25f, 40f, 10f, 3f, 2f };
    float[] weatherEventChancesMidnight = { 5f, 15f, 50f, 20f, 6f, 4f };


    string[] events = { "calm", "lightSnowfall", "snowShower", "generalSnowstorm", "blizzard", "blizzard + Extreme avalanche risk" };
    float[] moveSpeeds = { 1f, 1f, 1f, 1f, 1f, 1f};
    float[] modifier = { 1f, 1f, 1.1f, 1.2f, 1.3f, 1.4f };
    float[] windSpeeds = { 1f, 1.1f, 1.2f, 1.4f ,1.5f, 1.6f };
    int[] temperatures = { 5, 1, -5, -8, -15, -16 };
    float[] waitUntilNextWeatherChange = { 30, 30, 30, 25, 20, 10 };
    float waitUntilChange;
    float[] avalancheRisk = { 1f, 3f, 5f, 20f, 30f, 50f };

    float[] fogIntensity = { 0f, 0.003f, 0.008f, 0.005f, 0.015f, 0.035f };

    bool[] isSnowing = { false, true, true, true, true, true };
    float[] snowIntensity = { 0f, 10f, 20f, 50f, 100f, 150f };


    public float avalancheRiskPercent;

    [SerializeField] Player player;
    [SerializeField] ElephantVisualTemperature visualTemp;

    public string currentWeather;

    [SerializeField] float startTemp = 10f;
    [SerializeField] public float currentTemp;
    [SerializeField] float minTemp = -20f;
    [SerializeField] float maxTemp = 20f;

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
                SetSnow(i);
                break;
            }
        }
    }

    public void TriggerEvent(int i)
    {
        currentWeather = events[i];
        player.SetMoveSpeed(moveSpeeds[i]);
        player.SetModifier(modifier[i]);
        player.SetWind(windSpeeds[i]);
        waitUntilChange = waitUntilNextWeatherChange[i];
        avalancheRiskPercent = avalancheRisk[i];

        if (openToElements)
        {
            int tempVariation = Random.Range(-3, 3);
            int newTemp = temperatures[i] + tempVariation;
            currentTemp = newTemp;  
        }
    }

    public void SetFog(int intensity)
    {
        RenderSettings.fogColor = Color.white;
       
        switch (intensity)
        {
            case 0:
                RenderSettings.fogDensity = fogIntensity[intensity];
                break;
            case 1:
                RenderSettings.fogDensity = fogIntensity[intensity];
                break;
            case 2:
                RenderSettings.fogDensity = fogIntensity[intensity];
                break;
            case 3:
                RenderSettings.fogDensity = fogIntensity[intensity];
                break;
            case 4:
                RenderSettings.fogDensity = fogIntensity[intensity];
                break;
            case 5:
                RenderSettings.fogDensity = fogIntensity[intensity];
                break;
        }
    }

    public void SetSnow(int i)
    {
        if(snowfall == null)
        {
            return;
        }

        snowfall.SetActive(isSnowing[i]);

        ParticleSystem.EmissionModule largeEmission = largeSnow.emission;
        ParticleSystem.EmissionModule smallEmission = smallSnow.emission;
        largeEmission.rateOverTime = snowIntensity[i];
        smallEmission.rateOverTime = snowIntensity[i];
    }

    private void Start()
    {
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;

        RenderSettings.fogColor = Color.white;
        currentTemp = startTemp;
        SetTemperatureUI(currentTemp);
        pickRandomEvent();
        StartCoroutine(WaitForTime());
    }

    IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(waitUntilChange);
        pickRandomEvent();
        StartCoroutine(WaitForTime());
    }

    private void SetTemperatureUI(float temperature)
    {
     //   if(temperatureUI  == null)
     //   {
      //      return;
     //   }
      //  temperatureUI.text = currentTemp + "°c";
    }
}
