using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimSetMusic : MonoBehaviour
{
    public float MusicMultiplier;

    void Update()
    {
        GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>().volume = MusicMultiplier * (PlayerPrefs.GetFloat("musicVolume") * PlayerPrefs.GetFloat("masterVolume"));
    }
}
