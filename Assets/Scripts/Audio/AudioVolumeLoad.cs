using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioVolumeLoad : MonoBehaviour
{
    void Start()
    {
        LoadAudio();
    }

    public void LoadAudio()
    {
        if (!PlayerPrefs.HasKey("masterVolume"))
            PlayerPrefs.SetFloat("masterVolume", 1);

        if (!PlayerPrefs.HasKey("musicVolume"))
            PlayerPrefs.SetFloat("musicVolume", 1);

        if (!PlayerPrefs.HasKey("soundVolume"))
            PlayerPrefs.SetFloat("soundVolume", 1);

        GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("musicVolume") * PlayerPrefs.GetFloat("masterVolume");
        GameObject.FindGameObjectWithTag("SoundManager").GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("soundVolume") * PlayerPrefs.GetFloat("masterVolume");
    }
}
