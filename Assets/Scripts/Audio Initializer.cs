using UnityEngine;

public class AudioInitializer : MonoBehaviour
{
    [SerializeField] private AudioSource mySource; // Reference to the AudioSource component

    private const string VolumeKey = "MusicVolume"; // Key for saving volume in PlayerPrefs

    private void Start()
    {
        // Load saved volume from PlayerPrefs
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);

        // Apply the saved volume to the AudioSource
        if (mySource != null)
        {
            mySource.volume = savedVolume;
        }
        else
        {
            Debug.LogError("AudioSource is not assigned in the Inspector!");
        }
    }
}
