using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CommonData : MonoBehaviour
{
    [Header("TMP")]
    public TMP_Text Text_IntroDays;
    public TMP_Text Text_Days;
    public TMP_Text Text_Time;
    public TMP_Text Text_PhoneTime;
    public TMP_Text Text_GameSpeed;

    [Header("Animator")]
    public Animator Anim_SplashDays;
    public Animator Anim_Gui;

    [Header("Properties")]
    public bool TimeProgress;
    public float SecondsPerFiveMins;

    [Header("Stats")]
    public bool GameStarted = false;
    public string Name;
    public int Minutes = 0;
    public int Hours = 6;
    public int Days = 1;
    public int TempDays;

    [Header("Siska Stats")]
    public Slider Bar_Completion;
    public Slider Bar_Energy;
    public Slider Bar_Stress;
    public Slider Bar_Trust;
    public TMP_Text Text_Completion;
    public TMP_Text Text_Energy;
    public TMP_Text Text_Stress;
    public TMP_Text Text_Trust;
    public int Completion = 0;
    public int Energy = 100;
    public int Stress = 50;
    public int Trust = 50;

    [Header("Stats Tab")]
    public bool ShowStatsTab;
    public GameObject StatsTab;

    [Header("Game Speed")]
    public float GameSpeed = 1;
    public Color OtherSpeedColor;
    public Color SelectedSpeedColor;
    public Image CurrentSpeedButton;
    public List<Image> SpeedButtonsImage;

    [Header("Animator")]
    public Animator Anim_Phone;

    [Header("Phone Menu")]
    public string CurrenTabApp;
    public GameObject PhoneMenu;
    public Image PhoneBg;
    public Sprite PhoneBgMain;
    public Sprite PhoneBgChat;
    public Sprite PhoneBgCalendar;
    public Sprite PhoneBgOptions;
    public GameObject PhoneTab_Main;
    public GameObject PhoneTab_Chat;
    public GameObject PhoneTab_Calendar;
    public GameObject PhoneTab_Options;
    public GameObject PhoneTab_Rundown;

    [Header("Other Phone Realted")]
    public GameObject MainTab_PhoneLayout;
    public GameObject MainTab_InputName;
    public TMP_InputField NameInputField;

    [Header("Music")]
    public AudioClip Mus_Daily;

    [Header("Sound Effects")]
    public AudioClip Sfx_SpdIncrease;
    public AudioClip Sfx_SpdDecrease;

    //Other
    ChatSystem ChatSystem;
    ScheduleActivity ScheduleActivity;
    DailyEvents DailyEvents;
    AudioSource MusicManager;
    AudioSource SoundManager;


    void Start()
    {
        ChatSystem = GetComponent<ChatSystem>();
        ScheduleActivity = GetComponent<ScheduleActivity>();
        DailyEvents = GetComponent<DailyEvents>();
        //MusicManager = GameObject.FindGameObjectWithTag("MusicManager").GetComponent<AudioSource>();
        //SoundManager = GameObject.FindGameObjectWithTag("SoundManager").GetComponent<AudioSource>();

        TempDays = Days;
        SetDayText();
        SetTimeText();

        PhoneMenu.SetActive(true);
        MainTab_InputName.SetActive(true);
        MainTab_PhoneLayout.SetActive(false);
        PhoneTab_Main.SetActive(true);
        PhoneTab_Chat.SetActive(false);

        UpdateBar("All");
        Anim_Phone.SetTrigger("Startup");
    }


    // Start game --------------------------------------------------------//
    public void StartGame()
    {
        ChatSystem.PrologueDone = true;
        GameStarted = true;
        Anim_Phone.enabled = false;
        Anim_Gui.enabled = true;
        Anim_SplashDays.SetTrigger("NextDay");
    }


    // Pause & Unpause
    public void GamePause()
    {
        if (GameSpeed > 0) SoundManager.PlayOneShot(Sfx_SpdDecrease);
        Time.timeScale = 0;

        Text_PhoneTime.text = Text_Time.text;

        SpeedButtonsImage[(int)GameSpeed].color = OtherSpeedColor;
        SpeedButtonsImage[0].color = SelectedSpeedColor;

        if (CurrenTabApp == "Chat") ChatSystem.PlayerInChatRoom = true;
        if (ChatSystem.ChatStopped) ChatSystem.StartChatContinue();

        Anim_Gui.SetTrigger("Pause");
    }

    public void GameUnpause()
    {
        if (GameSpeed > 0) SoundManager.PlayOneShot(Sfx_SpdIncrease);
        Time.timeScale = GameSpeed;
        SpeedButtonsImage[0].color = OtherSpeedColor;
        SpeedButtonsImage[(int)GameSpeed].color = SelectedSpeedColor;

        ChatSystem.PlayerInChatRoom = false;
    }


    // Time System -------------------------------------------------------//
    public void StartNewDay()
    {
        int ForMood = (ScheduleActivity.Mood - 2) * -4;
        if (ForMood < 0) ForMood += 2;
        AddStatValue("Completion", ForMood);

        TempDays = Days;

        Hours = 6;
        Minutes = 0;
        SetTimeText();

        if (Days > 1) DailyEvents.StartRandomEvent();
        StartTimer();
    }

    public void StartTimer()
    {
        RestartMusicDaily();

        TimeProgress = true;
        ScheduleActivity.ActivityStartup();
        StartCoroutine(TimeStart());
    }

    public void StartMidTimer()
    {
        TimeProgress = true;
        StartCoroutine(TimeStart());
    }

    public IEnumerator TimeStart()
    {
        while (TimeProgress)
        {
            yield return new WaitForSeconds(SecondsPerFiveMins);
            if (!TimeProgress) yield break;

            Minutes += 5;
            ScheduleActivity.ActivityDrain(5);
            if (Minutes > 55)
            {
                Minutes = 0;
                Hours++;
                if (Hours > 23)
                {
                    Hours = 0;
                    TempDays++;
                    SetDayText();
                }
                else if (Hours == 6)
                {
                    NextDay();
                }
                else if (Hours == 6 + DailyEvents.IgnoreTolerantHours && Days > 1)
                {
                    DailyEvents.RandomEventIgnored();
                }
            }
            SetTimeText();
        }
    }

    public void SetTimeText()
    {
        string TempHours = (Hours < 10 ? "0" : null) + Hours.ToString();
        string TempMinutes = (Minutes < 10 ? "0" : null) + Minutes.ToString();
        Text_Time.text = TempHours + ":" + TempMinutes;
    }

    public void SetDayText()
    {
        Text_IntroDays.text = "Day " + Days.ToString();
        Text_Days.text = TempDays.ToString() + " Feb 20xx";
    }

    public void NextDay()
    {
        TimeProgress = false;
        
        Days++;
        TempDays = Days;
        SetDayText();

        Anim_SplashDays.SetTrigger("NextDay");
    }

    // Game Speed System -------------------------------------------------------//
    public void GameSpeed_Change(int SpeedInt)
    {
        CurrentSpeedButton.color = OtherSpeedColor;
        SoundManager.PlayOneShot(SpeedInt < GameSpeed ? Sfx_SpdDecrease : Sfx_SpdIncrease);
        GameSpeed = SpeedInt;
        Time.timeScale = GameSpeed;
        CurrentSpeedButton = SpeedButtonsImage[SpeedInt];
        CurrentSpeedButton.color = SelectedSpeedColor;
    }


    // Phone Menu --------------------------------------------------------------//
    public void ChangePhoneTab(string TabName)
    {
        CurrenTabApp = TabName;

        PhoneTab_Main.SetActive(TabName == "Main");
        PhoneTab_Chat.SetActive(TabName == "Chat");
        PhoneTab_Calendar.SetActive(TabName == "Calendar");
        PhoneTab_Options.SetActive(TabName == "Options");
        PhoneTab_Rundown.SetActive(TabName == "Rundown");

        switch (TabName)
        {
            case "Main":
                PhoneBg.sprite = PhoneBgMain;
                ChatSystem.UnreadLabelDestroy();
                ChatSystem.PlayerInChatRoom = false;
                break;

            case "Chat":
                if (!GameStarted)
                {
                    GameStarted = true;
                    //MusicManager.Play();
                }

                PhoneBg.sprite = PhoneBgChat;

                ChatSystem.PlayerInChatRoom = true;
                ChatSystem.NotifIcon.SetActive(false);
                ChatSystem.NotifFromPhone.SetActive(false);

                if (ChatSystem.ChatStopped) ChatSystem.StartChatContinue();
                break;

            case "Calendar":
                PhoneBg.sprite = PhoneBgCalendar;
                break;

            case "Options":
                PhoneBg.sprite = PhoneBgOptions;
                break;

            case "Rundown":
                PhoneBg.sprite = PhoneBgMain;
                ScheduleActivity.ResetSelectedRundownActivity();
                break;
        }
    }


    // Tabs --------------------------------------------------------------------//
    public void ToggleStatsTab()
    {
        ShowStatsTab = !ShowStatsTab;
        StatsTab.SetActive(ShowStatsTab);
    }

    public void SetName(int SkipLineTo)
    {
        Name = NameInputField.text;

        MainTab_InputName.SetActive(false);
        MainTab_PhoneLayout.SetActive(true);

        ChatSystem.DialUsed = new List<string>(ChatSystem.Dial_Prologue);
        if (SkipLineTo > 0) ChatSystem.ChatLine = SkipLineTo;
        ChatSystem.StartChatting();
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }


    // Stats -------------------------------------------------------------------//
    public void AddStatValue(string Stat, int Value)
    {
        switch (Stat)
        {
            case "Completion":
                Completion += Value;
                if (Completion < 0) Completion = 0;
                else if (Completion > 100) Completion = 100;
                break;

            case "Energy":
                Energy += Value;
                if (Energy < 0)
                {
                    Energy = 0;
                    UpdateBar(Stat);

                    StopAllCoroutines();
                    ScheduleActivity.StopAllCoroutines();
                    AddStatValue("Energy", 50);

                    NextDay();
                }
                else if (Energy > 100) Energy = 100;
                break;

            case "Stress":
                Stress += Value;
                if (Stress < 0) Stress = 0;
                else if (Stress > 100) Stress = 100;
                SetMood();
                break;

            case "Trust":
                Trust += Value;
                if (Trust < 0) Trust = 0;
                else if (Trust > 100) Trust = 100;
                break;
        }
        UpdateBar(Stat);
    }    

    public void UpdateBar(string Type)
    {
        switch (Type)
        {
            case "Completion":
                Text_Completion.text = Completion.ToString() + "%";
                Bar_Completion.value = Completion;
                break;

            case "Energy":
                Text_Energy.text = Energy.ToString() + "%";
                Bar_Energy.value = Energy;
                break;

            case "Stress":
                Text_Stress.text = Stress.ToString() + "%";
                Bar_Stress.value = Stress;
                break;

            case "Trust":
                Text_Trust.text = Trust.ToString() + "%";
                Bar_Trust.value = Trust;
                break;

            case "All":
                Text_Completion.text = Completion.ToString() + "%";
                Text_Energy.text = Energy.ToString() + "%";
                Text_Stress.text = Stress.ToString() + "%";
                Text_Trust.text = Trust.ToString() + "%";
                Bar_Completion.value = Completion;
                Bar_Energy.value = Energy;
                Bar_Stress.value = Stress;
                Bar_Trust.value = Trust;
                break;
        }
    }

    public void SetMood()
    {
        int MoodCounter = 0;

        while (20 + (20 * MoodCounter) < Stress)
        {
            MoodCounter++;
        }

        if (MoodCounter > 4) MoodCounter = 4;

        ScheduleActivity.SetMood(MoodCounter);
    }


    // Audio -------------------------------------------------------------------//
    public void RestartMusicDaily()
    {
        MusicManager.Stop();
        MusicManager.volume = PlayerPrefs.GetFloat("musicVolume") * PlayerPrefs.GetFloat("masterVolume");
        if (MusicManager.clip != Mus_Daily) MusicManager.clip = Mus_Daily;
        MusicManager.Play();
    }
}