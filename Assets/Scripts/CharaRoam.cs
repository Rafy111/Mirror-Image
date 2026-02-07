using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharaRoam : MonoBehaviour
{
    [Header("Compponents")]
    public float WaitDurMin;
    public float WaitDurMax;

    [Header("Walk Component")]
    public Transform CharaObject;
    public Animator CharaAnimator;
    public float WalkingSpeed;

    [Header("Stats")]
    public int Mood;
    public string Action;
    public int V_Action;
    public int X_Action;

    [Header("List")]
    public List<string> ObjectiveList;
    public List<Transform> StandPointList;
    public List<string> EmotList;
    public List<Sprite> EmotIconList;

    [Header("UI")]
    public TMP_Text StatsText;
    public TMP_Text LastActionInfo;
    public Image EmotIcon;

    [Header("Other")]
    public float WaitDur;
    public int NextObjective;
    public string NextObjectiveName;
    public bool ActionSuccess;
    public string LastActionCurrentInfo;
    public int IncreaseMood;
    public Transform GoTo;


    void Start()
    {
        UpdateUi();
        StartCoroutine(FirstWait());
    }

    public IEnumerator CharaWalk()
    {
        Action = "Moving";
        UpdateUi();

        if (CharaObject.position.x > GoTo.position.x) CharaObject.localScale = new Vector3 (-1f, CharaObject.localScale.y, CharaObject.localScale.z);
        else CharaObject.localScale = new Vector3(1f, CharaObject.localScale.y, CharaObject.localScale.z);

        CharaAnimator.SetTrigger("isWalking");

        while (CharaObject.position.x != GoTo.position.x && CharaObject.position.y != GoTo.position.y)
        {
            CharaObject.transform.position = Vector2.MoveTowards(CharaObject.position, GoTo.position, WalkingSpeed * Time.deltaTime);
            yield return 0;
        }

        CharaAnimator.SetTrigger("isIdle");
        StartCoroutine(CharaActivity());
    }

    public IEnumerator CharaActivity()
    {
        switch (NextObjectiveName)
        {
            case "Bed":
                LastActionCurrentInfo = "Go to sleep";
                ActionSuccess = Mood < 2;
                if (ActionSuccess) IncreaseMood = 1; else IncreaseMood = 0;
                break;

            case "Wardrobe":
                LastActionCurrentInfo = "Looking at clothes";
                ActionSuccess = Mood > 2;
                if (ActionSuccess) IncreaseMood = 1; else IncreaseMood = 0;
                break;

            case "Bookshelf":
                LastActionCurrentInfo = "Reading a book";
                ActionSuccess = Mood > 1;
                if (ActionSuccess) IncreaseMood = 1; else IncreaseMood = 0;
                break;

            case "Computer":
                LastActionCurrentInfo = "Playing a game";
                ActionSuccess = Mood > 1;
                if (ActionSuccess) IncreaseMood = 1; else IncreaseMood = -1;
                break;
        }

        if (ActionSuccess)
        {
            LastActionInfo.text = "Last Action Info:\n" + LastActionCurrentInfo;
            Action = LastActionCurrentInfo;
            V_Action++;
        }
        else
        {
            LastActionInfo.text = "Last Action Info:\n" + "Too " + EmotList[Mood] + " to " + LastActionCurrentInfo;
            Action = "Idle";
            X_Action++;
        }
        UpdateUi();

        WaitDur = Random.Range(WaitDurMin, WaitDurMax);
        yield return new WaitForSeconds(WaitDur);

        Mood += IncreaseMood;
        if (Mood < 0) Mood = 0; else if (Mood > EmotList.Count-1) Mood = EmotList.Count-1;
        UpdateUi();

        CharaMoveset();
    }

    public IEnumerator FirstWait()
    {
        WaitDur = Random.Range(WaitDurMin, WaitDurMax);
        yield return new WaitForSeconds(WaitDur);
        CharaMoveset();
    }

    public void CharaMoveset()
    {
        NextObjective = Random.Range(0, ObjectiveList.Count-1);
        UpdateObjective();
        StartCoroutine(CharaWalk());
    }

    public void UpdateObjective()
    {
        GoTo = StandPointList[NextObjective];
        NextObjectiveName = ObjectiveList[NextObjective];
    }

    public void UpdateUi()
    {
        EmotIcon.sprite = EmotIconList[Mood];

        StatsText.text = "Mood:      " + EmotList[Mood] + "\n" +
                         "Action:    " + Action         + "\n" +
                         "V Action: " + V_Action       + "\n" +
                         "X Action: " + X_Action;
    }
}
