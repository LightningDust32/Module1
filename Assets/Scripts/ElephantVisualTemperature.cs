using UnityEngine;

public class ElephantVisualTemperature : MonoBehaviour
{


    public RandomWeatherEvent randomWeather;

    [Header("Weather Colours")]
    public Color calmColour = new Color32(133, 133, 133, 255);
    public Color lightSnowfallColour = new Color32(115, 140, 165, 255);
    public Color snowShowerColour = new Color32(95, 135, 190, 255);
    public Color generalSnowstormColour = new Color32(65, 105, 176, 255);
    public Color blizzardColour = new Color32(50, 70, 145, 255);
    public Color extremeBlizzardColour = new Color32(120, 81, 169, 255);
    public Color noneColour = new Color32(133, 133, 133, 255);

    public float transitionSpeed = 2f;

    private Renderer[] elephantRenderers;

    void Start()
    {
        // find all the renderers of the different elephant parts
        elephantRenderers = GetComponentsInChildren<Renderer>();
    }

    void Update()
    {
        Color targetColour = GetNewColour(randomWeather.currentWeather);

        foreach (Renderer rend in elephantRenderers)
        {
            foreach (Material mat in rend.materials)
            {
                mat.color = Color.Lerp(mat.color, targetColour, Time.deltaTime * transitionSpeed);
            }
        }
    }




    Color GetNewColour(string currentWeather)
    {
        switch (currentWeather)
        {
            case "calm":
               // Debug.Log("Changing Color");
                return calmColour;
            case "lightSnowfall":
                return lightSnowfallColour;
            case "snowShower":
                return snowShowerColour;
            case "generalSnowstorm":
                return generalSnowstormColour;
            case "blizzard":
                return blizzardColour;
            case "blizzard + Extreme avalanche risk":
                return extremeBlizzardColour;
            default:
                return noneColour;
        }
    }
}
