using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicKeepPlay : MonoBehaviour
{
    public static MusicKeepPlay instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
