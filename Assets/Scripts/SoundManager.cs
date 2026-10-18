using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [SerializeField] private AudioSource source;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void PlaySound(AudioClip sound)
    {
        source.PlayOneShot(sound);
    }

}
