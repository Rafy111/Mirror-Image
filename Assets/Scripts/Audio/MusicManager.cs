using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;

    void Start()
    {
        // Music Volume
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1);
            LoadMusicVolume();
        }
        else
        {
            LoadMusicVolume();
        }
    }


    // Music Volume
    public void ChangeMusicVolume()
    {
        GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>().volume = volumeSlider.value * PlayerPrefs.GetFloat("masterVolume");
        SaveMusicVolume();
    }

    private void SaveMusicVolume()
    {
        PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
    }

    private void LoadMusicVolume()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
    }
}
