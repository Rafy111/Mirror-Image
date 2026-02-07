using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivitySc : MonoBehaviour
{
    [Header("Properties")]
    public int RoomId;
    public int ObjectId;
    public float Duration;
    public float TimeNeeded;

    [Header("Requirements")]
    public int EnergyNeeded;
    public int StressLimit;

    [Header("UI")]
    public Image ProgressBar;
    public Image Icon;

    [Header("Complete")]
    public bool HasCompletion = true;
    public string StatAdd;
    public int Ammount;
    public bool Sleep = false;

    [Header("References")]
    //public ActivitySc ActivityScript;
    //public ActivitySc RundownScript;
    public TMP_Text RundownTime;
    public int RundownId;
    public Button RundownButton;

    //Other
    CommonData CommonData;
    ScheduleActivity ScheduleActivity;
    float ActivityDurationPassed = 0;

    void Start()
    {
        GameObject CommonDataObj = GameObject.FindGameObjectWithTag("CommonData");
        CommonData = CommonDataObj.GetComponent<CommonData>();
        ScheduleActivity = CommonDataObj.GetComponent<ScheduleActivity>();

        //if (gameObject.TryGetComponent<Button>(out Button ThisButton)) RundownButton = ThisButton;
    }

    public void UpdateBar(float CurrentDuration)
    {
        ProgressBar.fillAmount = Duration - CurrentDuration <= 0 ? 0 : 1 - (CurrentDuration / Duration);
    }

    public void CheckCompletion(float CurrentDuration)
    {
        ActivityDurationPassed += CurrentDuration;
        if (ActivityDurationPassed >= TimeNeeded)
        {
            HasCompletion = false;
            Icon.sprite = ScheduleActivity.IconDone;
            CommonData.AddStatValue(StatAdd, Ammount);

            ScheduleActivity.CurrentRundownSc.Icon.sprite = ScheduleActivity.IconDone;

            if (Sleep)
            {
                CommonData.StopAllCoroutines();
                ScheduleActivity.StopAllCoroutines();
                CommonData.AddStatValue("Energy", 100);
                CommonData.NextDay();
            }
        }
    }

    public void ActivityFailed()
    {
        HasCompletion = false;
        Icon.sprite = ScheduleActivity.IconFailed;

        ScheduleActivity.CurrentRundownSc.Icon.sprite = ScheduleActivity.IconFailed;
    }
}
