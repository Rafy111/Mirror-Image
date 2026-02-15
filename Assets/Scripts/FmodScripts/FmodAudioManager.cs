using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class FmodAudioManager : MonoBehaviour
{
    [Header("Components")]
    public bool PlayMusicAtStartup = true;

    //Others
    public static FmodAudioManager instance { get; private set; }
    EventInstance MusicInstance;



    void Awake() { if (instance == null) instance = this; }

    void Start()
    {
        if (PlayMusicAtStartup) PlayMusic(FmodEvents.instance.Music);
    }


    // Music Related
    public void PlayMusic(EventReference Audio)
    {
        MusicInstance = RuntimeManager.CreateInstance(Audio);
        MusicInstance.start();
    }

    public void SetMusicParameter(string ParameterName, float Value) { MusicInstance.setParameterByName(ParameterName, Value); }


    // Sound Related
    public void PlayOneSound(EventReference Audio, Vector3 PlayPos) { RuntimeManager.PlayOneShot(Audio, PlayPos); }

    public EventInstance CreateSoundInstance(EventReference Audio)
    {
        EventInstance NewEvent = RuntimeManager.CreateInstance(Audio);
        return NewEvent;
    }
}
