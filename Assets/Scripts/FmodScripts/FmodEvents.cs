using FMODUnity;
using UnityEngine;

public class FmodEvents : MonoBehaviour
{
    [Header("Music Events")]
    public EventReference Music;

    [Header("Sound Events")]
    public EventReference Sfx_Notification;
    public EventReference Sfx_SpeedUp;
    public EventReference Sfx_SpeedDown;

    //Others
    public static FmodEvents instance { get; private set; }


    void Awake() { if (instance == null) instance = this; }
}
