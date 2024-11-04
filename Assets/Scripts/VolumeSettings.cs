using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioSource mySource;  // Reference to the AudioSource component
    [SerializeField] private Slider musicSlider;    // Reference to the UI Slider

    private const string VolumeKey = "MusicVolume"; // Key for saving volume in PlayerPrefs

    private void Start()
    {
        // Load saved volume from PlayerPrefs, default to 0.5 if none saved
        float savedVolume = PlayerPrefs.GetFloat(VolumeKey, 0.5f);
        musicSlider.value = savedVolume;

        // Apply the saved volume to the AudioSource
        if (mySource != null)
        {
            mySource.volume = savedVolume;
        }

        // Add listener to the slider to update volume when changed
        musicSlider.onValueChanged.AddListener(OnVolumeChange);
    }

    private void OnVolumeChange(float volume)
    {
        // Set the volume of the AudioSource directly
        if (mySource != null)
        {
            mySource.volume = volume;
        }

        // Save the volume setting for future use
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }

    private void OnDestroy()
    {
        // Remove the listener to avoid memory leaks
        musicSlider.onValueChanged.RemoveListener(OnVolumeChange);
    }
}