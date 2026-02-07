using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeManager : MonoBehaviour
{
    [SerializeField] Slider masterSlider;

    void Start()
    {
        // Master Volume
        if (!PlayerPrefs.HasKey("masterVolume"))
        {
            PlayerPrefs.SetFloat("masterVolume", 1);
            LoadMasterVolume();
        }
        else
        {
            LoadMasterVolume();
        }
    }

    // Master Volume
    public void ChangeMasterVolume()
    {
        GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVolume") * masterSlider.value;
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("soundVolume") * masterSlider.value;
        SaveMasterVolume();
    }

    private void SaveMasterVolume()
    {
        PlayerPrefs.SetFloat("masterVolume", masterSlider.value);
    }

    private void LoadMasterVolume()
    {
        masterSlider.value = PlayerPrefs.GetFloat("masterVolume");
    }
}
