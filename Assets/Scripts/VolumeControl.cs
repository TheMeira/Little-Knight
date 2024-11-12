using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeControl : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource audioSource;
    public Image iconImage;
    public Sprite iconOn;
    public Sprite iconOff;

    public AudioMixer audioMixer;
    public string musicVolumeParam = "Music Value";
    public string sfxVolumeParam = "Sfx Value";

    private const string VolumePrefsKey = "VolumeLevel";

    void Start()
    {
        // Load volume data from PlayerPrefs
        LoadVolumeData();

        // Set initial volume and icon
        volumeSlider.value = audioSource.volume;
        UpdateIcon();
    }

    public void OnVolumeChanged()
    {
        // Update audio source volume
        audioSource.volume = volumeSlider.value;

        // Check if volume is zero and update icon
        if (volumeSlider.value <= 0)
        {
            audioSource.volume = 0; // Ensure volume is exactly 0
            UpdateIcon(true);
        }
        else
        {
            UpdateIcon(false);
        }

        // Set audio mixer parameters
        SetAudioMixerParameters();

        // Save volume data to PlayerPrefs
        SaveVolumeData();
    }

    void UpdateIcon(bool isMuted = false)
    {
        // Update icon based on mute status
        iconImage.sprite = isMuted ? iconOff : iconOn;
    }

    void SaveVolumeData()
    {
        PlayerPrefs.SetFloat(VolumePrefsKey, volumeSlider.value);
    }

    void LoadVolumeData()
    {
        if (PlayerPrefs.HasKey(VolumePrefsKey))
        {
            float volume = PlayerPrefs.GetFloat(VolumePrefsKey);
            volumeSlider.value = volume;
            audioSource.volume = volume;
            UpdateIcon(volume <= 0);
            SetAudioMixerParameters();
        }
    }

    void SetAudioMixerParameters()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat(musicVolumeParam, GetAdjustedVolume(volumeSlider.value));
            audioMixer.SetFloat(sfxVolumeParam, GetAdjustedVolume(volumeSlider.value));
        }
    }

    float GetAdjustedVolume(float volume)
    {
        // Adjust volume to fit the range of Audio Mixer parameters (-80 to 0)
        return Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
    }
}
