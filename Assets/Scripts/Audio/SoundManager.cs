using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        // Sound Volume
        if (!PlayerPrefs.HasKey("soundVolume"))
        {
            PlayerPrefs.SetFloat("soundVolume", 1);
            LoadSoundVolume();
        }
        else
        {
            LoadSoundVolume();
        }
    }

    // Sound Volume
    public void ChangeSoundVolume()
    {
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<AudioSource>().volume = volumeSlider.value * PlayerPrefs.GetFloat("masterVolume");
        SaveSoundVolume();
    }

    private void SaveSoundVolume()
    {
        PlayerPrefs.SetFloat("soundVolume", volumeSlider.value);
    }

    private void LoadSoundVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
    }
}