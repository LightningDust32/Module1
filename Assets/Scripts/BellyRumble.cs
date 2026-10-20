using UnityEngine;

public class BellyRumble : MonoBehaviour
{


    Player player;


    public float minRumbleSpeed = 1f;
    public float maxRumbleSpeed = 8f;

    public float minWobbleAmount = 0.005f;
    public float maxWobbleAmount = 0.05f;

    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Renderer sphereRenderer;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindAnyObjectByType<Player>();

        originalScale = transform.localScale;
        originalPosition = transform.localPosition;

        sphereRenderer = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float hunger = player.GetElephantHunger();

        float hungerLevel = Mathf.InverseLerp(75f, 0f, hunger);

        float rumbleSpeed = Mathf.Lerp(minRumbleSpeed, maxRumbleSpeed, hungerLevel);

        float wobbleAmount = Mathf.Lerp(0f, maxWobbleAmount, hungerLevel);

        if (hunger < 75f)
        {
            float wobbleX = Mathf.Sin(Time.time * rumbleSpeed * 2f) * wobbleAmount;

            float wobbleY = Mathf.Sin(Time.time * rumbleSpeed * 3f) * wobbleAmount * 0.5f;

            transform.localPosition = originalPosition + new Vector3(wobbleX, wobbleY, 0f);
        }
        else
        {
            transform.localPosition = originalPosition;
        }
    }
}
