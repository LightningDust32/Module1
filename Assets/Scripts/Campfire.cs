using UnityEngine;

public class Campfire : MonoBehaviour
{

    public GameObject fireParticles;
    [SerializeField] bool lit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (lit)
        {
            fireParticles.SetActive(true);
        }
        else
        {
            fireParticles.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
