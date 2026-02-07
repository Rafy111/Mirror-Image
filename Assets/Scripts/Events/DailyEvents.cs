using System.Collections.Generic;
using UnityEngine;

public class DailyEvents : MonoBehaviour
{
    [Header("Components")]
    public int IgnoreTolerantHours = 2;
    public bool EventAnswered = false;

    [Header("Other Dialogues")]
    [TextArea(0, 2)] public List<string> IgnoredEvent;

    [Header("Daily Events")]
    [TextArea(0, 2)] public List<string> D_Event_1;
    [TextArea(0, 2)] public List<string> D_Event_2;
    [TextArea(0, 2)] public List<string> D_Event_3;
    [TextArea(0, 2)] public List<string> D_Event_4;
    [TextArea(0, 2)] public List<string> D_Event_5;

    //Other
    ChatSystem ChatSystem;
    public List<List<string>> AllDailyEvents;


    void Start()
    {
        ChatSystem = GetComponent<ChatSystem>();
        AllDailyEvents = new List<List<string>> { D_Event_1, D_Event_2, D_Event_3, D_Event_4, D_Event_5 };
    }

    public void StartRandomEvent()
    {
        EventAnswered = false;
        ChatSystem.StartNewDialogue(AllDailyEvents[Random.Range(0, AllDailyEvents.Count - 1)]);
    }

    public void RandomEventIgnored()
    {
        if (EventAnswered) return;
        if (ChatSystem.CurrentPlayerInput != null) Destroy(ChatSystem.CurrentPlayerInput);
        ChatSystem.StartNewDialogue(IgnoredEvent);
    }
}
