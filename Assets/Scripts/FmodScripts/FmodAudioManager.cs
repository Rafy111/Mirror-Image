using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FmodAudioManager : MonoBehaviour
{
    public static FmodAudioManager instance { get; private set; }
    EventInstance MusicInstance;


    void Awake() { if (instance == null) instance = this; }

    void Start()
    {
        PlayMusic(FmodEvents.instance.Music);
    }

    void PlayMusic(EventReference Audio)
    {
        MusicInstance = RuntimeManager.CreateInstance(Audio);
        MusicInstance.start();
    }

    public void PlayOneSound(EventReference Audio, Vector3 PlayPos) { RuntimeManager.PlayOneShot(Audio, PlayPos); }

    public void SetMusicParameter(string ParameterName, float Value) { MusicInstance.setParameterByName(ParameterName, Value); }
}
