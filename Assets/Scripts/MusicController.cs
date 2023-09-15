using UnityEngine;

public class MusicController : MonoBehaviour
{
    public static MusicController instance = null;
    private AudioSource audioSource;

    private void Awake()
    {
        Debug.Log("MusicController Awake");

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource component found on the GameObject!");
        }
    }

    public void ToggleMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }
    }

    public bool IsPlaying()
    {
        return audioSource.isPlaying;
    }

    private void OnDestroy()
    {
        Debug.Log("MusicController Destroyed");
    }
}
