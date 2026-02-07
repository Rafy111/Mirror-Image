using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatNextLine : MonoBehaviour
{
    ChatSystem ChatSystem;

    void Start()
    {
        ChatSystem = GameObject.FindGameObjectWithTag("CommonData").GetComponent<ChatSystem>();
    }

    public void NextLine()
    {
        ChatSystem.StartChatting();
    }

    public void StartDay()
    {
        GameObject.FindGameObjectWithTag("CommonData").GetComponent<CommonData>().StartGame();
    }

    public void HideTab()
    {
        this.gameObject.SetActive(false);
    }
}
