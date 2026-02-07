using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScheduleActivity : MonoBehaviour
{
    [Header("Main")]
    public Transform ActivitySlot;
    public Transform ActivityPutPlace;
    public Transform RundownSlot;
    public Transform RundownPutPlace;
    public Sprite IconDone;
    public Sprite IconFailed;

    [Header("Walk Component")]
    public Transform CharaObject;
    public Animator CharaAnimator;
    public float WalkingSpeed;

    [Header("Moods")]
    public int Mood = 4;
    public TMP_Text EmotText;
    public Image EmotIcon;
    public List<string> EmotList;
    public List<Sprite> EmotIconList;

    [Header("Rooms")]
    public List<Transform> RoomPoints;

    [Header("Objects")]
    public List<Transform> ObjectsBedroom;
    public List<Transform> ObjectsLivingRoom;
    public List<Transform> ObjectsKitchen;
    public List<Transform> ObjectsBathroom;

    [Header("Schedules")]
    public List<GameObject> SchedulesWeekday;
    public List<GameObject> RundownWeekday;

    [Header("Lists")]
    public List<ActivitySc> ActivityObj;
    public List<ActivitySc> RundownObj;
    public List<List<Transform>> AllObjectPoints;

    [Header("Others")]
    public Color Cl_Selected;
    public Color Cl_Disabled;
    public Color Cl_Holder;
    public ActivitySc CurrentRundownSc;

    //Others
    CommonData CommonData;
    ActivitySc CurrentActivitySc;
    float ActivityDuration;
    float CurrentDuration;
    Transform TargetRoom;
    Transform TargetObject;
    Coroutine CoroWalking;
    int CurrentRoom = 1;
    int RoomToGo;
    bool ReachedDestination;
    ActivitySc CurrentSelectedActivityRundown;
    int RundownActivityIdSelected;
    bool RundownActivitySelected = false;


    void Start()
    {
        CommonData = GetComponent<CommonData>();
        AllObjectPoints = new List<List<Transform>>{ObjectsBedroom, ObjectsLivingRoom, ObjectsKitchen, ObjectsBathroom};
        SetMood(4);
        //BuildActivityList();
    }

    public void BuildActivityList()
    {
        ActivityObj.Clear();
        ActivitySc[] TodayActivities = ActivitySlot.GetComponentsInChildren<ActivitySc>();
        foreach (ActivitySc Child in TodayActivities) ActivityObj.Add(Child);

        RundownObj.Clear();
        ActivitySc[] TodayRundown = RundownSlot.GetComponentsInChildren<ActivitySc>();
        foreach (ActivitySc Child in TodayRundown) RundownObj.Add(Child);

        int NumHold = 0;
        foreach (var Child in RundownObj)
        {
            Child.RundownId = NumHold;
            Child.RundownButton.onClick.AddListener(delegate {SelectActivityOnRundown(Child);});
            NumHold++;
        }
    }

    public void SelectActivityOnRundown(ActivitySc Script)
    {
        if (CurrentSelectedActivityRundown != null) CurrentSelectedActivityRundown.ProgressBar.color = Cl_Holder;
        CurrentSelectedActivityRundown = Script;
        Cl_Holder = CurrentSelectedActivityRundown.ProgressBar.color;
        CurrentSelectedActivityRundown.ProgressBar.color = Cl_Selected;
        RundownActivityIdSelected = CurrentSelectedActivityRundown.RundownId;
        RundownActivitySelected = true;
    }

    public void ResetSelectedRundownActivity()
    {
        RundownActivitySelected = false;
        if (CurrentSelectedActivityRundown != null) CurrentSelectedActivityRundown.ProgressBar.color = Cl_Holder;
    }

    public void ChangeActivityRundown(ActivitySc Script)
    {
        if (RundownActivitySelected)
        {
            ActivitySc CurrRunSc = RundownObj[RundownActivityIdSelected];
            CurrRunSc.ProgressBar.color = Script.ProgressBar.color;
            CurrRunSc.Icon.sprite = Script.Icon.sprite;
            Cl_Holder = Script.ProgressBar.color;

            ActivitySc CurrAcSc = ActivityObj[RundownActivityIdSelected];
            CurrAcSc.ProgressBar.color = Script.ProgressBar.color;
            CurrAcSc.Icon.sprite = Script.Icon.sprite;

            CurrAcSc.RoomId = Script.RoomId;
            CurrAcSc.ObjectId = Script.ObjectId;
            CurrAcSc.TimeNeeded = Script.TimeNeeded;

            CurrAcSc.StressLimit = Script.StressLimit;
            CurrAcSc.StatAdd = Script.StatAdd;
            CurrAcSc.Ammount = Script.Ammount;

            CurrentSelectedActivityRundown = null;
            RundownActivitySelected = false;
        }
    }

    public void ActivityStartup()
    {
        if (ActivitySlot != null) Destroy(ActivitySlot.gameObject);
        if (RundownSlot != null) Destroy(RundownSlot.gameObject);

        GameObject CurrentSchedule = Instantiate(SchedulesWeekday[Mood], ActivityPutPlace);
        CurrentSchedule.SetActive(true);
        ActivitySlot = CurrentSchedule.transform;

        GameObject CurrentRundown = Instantiate(RundownWeekday[Mood], RundownPutPlace);
        CurrentRundown.SetActive(true);
        RundownSlot = CurrentRundown.transform;

        BuildActivityList();

        if (ActivityObj.Count <= 0) return;

        CurrentRundownSc = RundownObj[0];
        CurrentActivitySc = ActivityObj[0];
        StartActivity();
    }

    public void ActivityDrain(int MinutesPerSecond)
    {
        if (ActivityObj.Count <= 0) return;

        CurrentDuration += MinutesPerSecond;
        if (CurrentDuration < ActivityDuration)
        {
            if (ReachedDestination && CurrentActivitySc.HasCompletion) CurrentActivitySc.CheckCompletion(MinutesPerSecond);
            CurrentActivitySc.UpdateBar(CurrentDuration);
        }
        else
        {
            //CommonData.TimeProgress = false;
            GameObject ToDestroy = ActivityObj[0].gameObject;
            ActivityObj.RemoveAt(0);
            Destroy(ToDestroy);

            RundownObj.RemoveAt(0);
            foreach (var Child in RundownObj) Child.RundownId--;

            if (ActivityObj.Count > 0)
            {
                CurrentActivitySc = ActivityObj[0];
                CurrentRundownSc = RundownObj[0];
                StartActivity();
            }
        }
    }

    public void StartActivity()
    {
        if (CoroWalking != null)
        {
            StopCoroutine(CoroWalking);
        }
        ReachedDestination = false;
        CurrentDuration = 0;
        GetActivityData();
    }

    public void GetActivityData()
    {
        if (ActivityObj.Count <= 0) return;

        ActivityDuration = CurrentActivitySc.Duration;
        RoomToGo = CurrentActivitySc.RoomId;
        TargetObject = AllObjectPoints[CurrentActivitySc.RoomId - 1][CurrentActivitySc.ObjectId - 1];

        RundownObj[0].ProgressBar.color = Cl_Disabled;
        RundownObj[0].RundownButton.enabled = false;
        CurrentRundownSc.RundownTime.text = "Sudah<br>Lewat";

        //if (!CommonData.TimeProgress) CommonData.StartMidTimer();
        CoroWalking = StartCoroutine(CharaWalk());
    }

    public IEnumerator CharaWalk()
    {
        Vector2 CharaPos = CharaObject.position;
        Vector2 ObjPos = TargetObject.position;

        if (CharaPos != ObjPos)
        {
            CharaAnimator.SetTrigger("isWalking");

            TargetRoom = RoomPoints[CurrentRoom - 1];

            if (CharaObject.position.x > TargetRoom.position.x) CharaObject.localScale = new Vector3(-1f, CharaObject.localScale.y, CharaObject.localScale.z);
            else CharaObject.localScale = new Vector3(1f, CharaObject.localScale.y, CharaObject.localScale.z);

            while (CharaObject.position.x != TargetRoom.position.x && CharaObject.position.y != TargetRoom.position.y)
            {
                CharaObject.transform.position = Vector2.MoveTowards(CharaObject.position, TargetRoom.position, WalkingSpeed * Time.deltaTime);
                yield return 0;
            }

            while (CurrentRoom != RoomToGo)
            {
                if (RoomToGo > CurrentRoom) CurrentRoom++;
                else if (RoomToGo < CurrentRoom) CurrentRoom--;

                TargetRoom = RoomPoints[CurrentRoom - 1];

                if (CharaObject.position.x > TargetRoom.position.x) CharaObject.localScale = new Vector3(-1f, CharaObject.localScale.y, CharaObject.localScale.z);
                else CharaObject.localScale = new Vector3(1f, CharaObject.localScale.y, CharaObject.localScale.z);

                while (CharaObject.position.x != TargetRoom.position.x && CharaObject.position.y != TargetRoom.position.y)
                {
                    CharaObject.transform.position = Vector2.MoveTowards(CharaObject.position, TargetRoom.position, WalkingSpeed * Time.deltaTime);
                    yield return 0;
                }
            }

            TargetRoom = TargetObject;

            if (CharaObject.position.x > TargetRoom.position.x) CharaObject.localScale = new Vector3(-1f, CharaObject.localScale.y, CharaObject.localScale.z);
            else CharaObject.localScale = new Vector3(1f, CharaObject.localScale.y, CharaObject.localScale.z);

            while (CharaObject.position.x != TargetRoom.position.x && CharaObject.position.y != TargetRoom.position.y)
            {
                CharaObject.transform.position = Vector2.MoveTowards(CharaObject.position, TargetRoom.position, WalkingSpeed * Time.deltaTime);
                yield return 0;
            }

            CharaAnimator.SetTrigger("isIdle");
        }

        if (CurrentActivitySc.EnergyNeeded <= CommonData.Energy && CurrentActivitySc.StressLimit >= CommonData.Stress)
        {
            ReachedDestination = true;
            CommonData.AddStatValue("Energy", -CurrentActivitySc.EnergyNeeded);
        }
        else CurrentActivitySc.ActivityFailed();
    }

    public void SetMood(int Id)
    {
        Mood = Id;
        EmotText.text = EmotList[Id];
        EmotIcon.sprite = EmotIconList[Id];
    }
}
