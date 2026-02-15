using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSystem : MonoBehaviour
{
    [Header("Main Components")]
    public Transform ChatHolder;
    public float WaitEachText = 1.5f;
    public bool PlayerInChatRoom = false;
    public bool ChatStopped = false;
    public int LastChatDay = 0;
    public bool PrologueDone = false;

    [Header("Text Prefabs")]
    public GameObject Chat_DayIndicator;
    public GameObject Chat_Unread;
    public GameObject Chat_Player;
    public GameObject Chat_OtherEnd;
    public GameObject Chat_Choices;
    public GameObject Chat_ChoicesSolo;
    public GameObject Chat_Input;

    [Header("Notification")]
    public GameObject NotifIcon;
    public GameObject NotifFromPhone;
    public AudioClip Sfx_Notif;

    [Header("FadeBlocker")]
    public Animator Anim_FadeBlocker;
    public Animator Anim_UiMain;

    [Header("Dialogues")]
    [TextArea(0, 3)] public List<string> Dial_Prologue;

    [Header("Components Used")]
    public int ChatLine = 0;
    public List<string> DialUsed;
    public GameObject ObjectToSpawn;
    public GameObject CurrentPlayerInput;
    public GameObject CurrentUnreadLabel;

    //Other
    CommonData CommonData;
    DailyEvents DailyEvents;
    AudioSource SoundManager;


    void Start()
    {
        CommonData = GetComponent<CommonData>();
        DailyEvents = GetComponent<DailyEvents>();
        //SoundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<AudioSource>();
    }

    public void StartNewDialogue(List<string> Dialogue)
    {
        DialUsed = new List<string>(Dialogue);
        ChatLine = 0;
        StartChatting();
    }

    public void StartChatting()
    {
        StartCoroutine(Chatting());
    }

    public IEnumerator Chatting()
    {
        while (ChatLine < DialUsed.Count)
        {
            if (DialUsed[ChatLine].Contains(" - "))
            {
                string[] CurrentLine = DialUsed[ChatLine].Split(" - ");
                switch (CurrentLine[0])
                {
                    case "Chat":
                        switch (CurrentLine[1])
                        {
                            case "Other": ObjectToSpawn = Chat_OtherEnd; break;
                            case "Player": ObjectToSpawn = Chat_Player; break;
                        }
                        CheckIfPlayerInChatRoom();
                        GameObject NewChat = Instantiate(ObjectToSpawn, ChatHolder);
                        if (CurrentLine[2].Contains("[Player]")) CurrentLine[2] = CurrentLine[2].Replace("[Player]", CommonData.Name);
                        NewChat.GetComponent<ChatObjSc>().Chat.text = CurrentLine[2];
                        if (!PlayerInChatRoom)// && !PrologueDone)
                        {
                            ChatStopped = true;
                            yield break;
                        }
                        yield return new WaitForSecondsRealtime(WaitEachText);
                        break;

                    case "Wait":
                        yield return new WaitForSecondsRealtime(float.Parse(CurrentLine[1]));
                        break;

                    case "Choice":
                        switch (CurrentLine[1])
                        {
                            case "1":
                                CurrentPlayerInput = Instantiate(Chat_ChoicesSolo, ChatHolder);
                                ChatObjSc CurrentChatScriptSolo = CurrentPlayerInput.GetComponent<ChatObjSc>();
                                CurrentChatScriptSolo.Choice1.text = CurrentLine[2];
                                CurrentChatScriptSolo.SetLine1 = CurrentLine[3] == "Next" ? ChatLine + 1 : int.Parse(CurrentLine[3]);
                                break;

                            case "2":
                                CurrentPlayerInput = Instantiate(Chat_Choices, ChatHolder);

                                ChatObjSc CurrentChatScript = CurrentPlayerInput.GetComponent<ChatObjSc>();
                                string[] TempButtonNames = CurrentLine[2].Split(" / ");
                                string[] TempButtonLines = CurrentLine[3].Split("/");

                                CurrentChatScript.Choice1.text = TempButtonNames[0];
                                CurrentChatScript.Choice2.text = TempButtonNames[1];
                                CurrentChatScript.SetLine1 = int.Parse(TempButtonLines[0]);
                                CurrentChatScript.SetLine2 = int.Parse(TempButtonLines[1]);
                                break;
                        }
                        ChatLine++;
                        yield break;

                    case "Jump":
                        ChatLine = int.Parse(CurrentLine[1]) - 1;
                        break;

                    case "StatAdd":
                        CommonData.AddStatValue(CurrentLine[1], int.Parse(CurrentLine[2]));
                        break;

                    case "Input":
                        switch (CurrentLine[1])
                        {
                            case "Name":
                                CurrentPlayerInput = Instantiate(Chat_Input, ChatHolder);
                                CurrentPlayerInput.GetComponent<ChatObjSc>().InputType = "Name";
                                break;
                        }
                        ChatLine++;
                        yield break;

                    case "CheckName":
                        string[] AfterCheckSkipLine = CurrentLine[2].Split("/");
                        ChatLine = int.Parse(CurrentLine[1] == CommonData.Name ? AfterCheckSkipLine[0] : AfterCheckSkipLine[1]) - 1;
                        break;

                    case "Anim":
                        switch (CurrentLine[1])
                        {
                            case "FadeBlocker": Anim_FadeBlocker.SetTrigger(CurrentLine[2]); break;
                            case "Main": Anim_UiMain.SetTrigger(CurrentLine[2]); break;
                            case "Phone": CommonData.Anim_Phone.SetTrigger(CurrentLine[2]); break;
                        }
                        ChatLine++;
                        yield break;
                }
            }
            else if (DialUsed[ChatLine] == "Check") CheckLastChatDay();
            else if (DialUsed[ChatLine] == "EventDone") DailyEvents.EventAnswered = true;
            ChatLine++;
        }
        ChatLine = 0;
        DialUsed.Clear();
        yield return null;
    }

    public void CheckLastChatDay()
    {
        if (LastChatDay < CommonData.Days)
        {
            LastChatDay = CommonData.Days;
            ObjectToSpawn = Instantiate(Chat_DayIndicator, ChatHolder);
            ObjectToSpawn.GetComponent<ChatObjSc>().DayNo.text = "- Day " + LastChatDay.ToString() + " -";
        }
    }

    public void CheckIfPlayerInChatRoom()
    {
        if (!PlayerInChatRoom)
        {
            //SoundManager.PlayOneShot(Sfx_Notif);
            FmodAudioManager.instance.PlayOneSound(FmodEvents.instance.Sfx_Notification, transform.position);
            NotifIcon.SetActive(true);
            NotifFromPhone.SetActive(true);
            if (CurrentUnreadLabel == null) CurrentUnreadLabel = Instantiate(Chat_Unread, ChatHolder);
        }
    }

    public void UnreadLabelDestroy()
    {
        if (CurrentUnreadLabel != null)
        {
            Destroy(CurrentUnreadLabel);
            CurrentUnreadLabel = null;
        }
    }

    public void SetName(string Name)
    {
        CommonData.Name = Name;
        Destroy(CurrentPlayerInput);
        StartCoroutine(PlayerTalk(Name));
    }

    public void ChoiceSelect(string ChoiceText, int LineSkip)
    {
        Destroy(CurrentPlayerInput);
        ChatLine = LineSkip;
        StartCoroutine(PlayerTalk(ChoiceText));
    }

    public IEnumerator PlayerTalk(string DialPl)
    {
        GameObject NewChat = Instantiate(Chat_Player, ChatHolder);
        if (DialPl.Contains("[Player]")) DialPl = DialPl.Replace("[Player]", CommonData.Name);
        NewChat.GetComponent<ChatObjSc>().Chat.text = DialPl;
        yield return new WaitForSecondsRealtime(WaitEachText);
        StartChatting();
    }

    public void StartChatContinue()
    {
        StartCoroutine(ChatContinue());
    }

    public IEnumerator ChatContinue()
    {
        ChatStopped = false;
        yield return new WaitForSecondsRealtime(WaitEachText);
        ChatLine++;
        StartChatting();
    }
}
