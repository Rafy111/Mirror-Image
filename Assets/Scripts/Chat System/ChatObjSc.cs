using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatObjSc : MonoBehaviour
{
    [Header("Day Indicator")]
    public TMP_Text DayNo;

    [Header("Normal Chat")]
    public TMP_Text Chat;

    [Header("Choices")]
    public TMP_Text Choice1;
    public TMP_Text Choice2;
    public int SetLine1;
    public int SetLine2;

    [Header("Input Type")]
    public TMP_InputField InputBox;
    public string InputType;

    public void Input_Send()
    {
        switch (InputType)
        {
            case "Name":
                GameObject.FindGameObjectWithTag("CommonData").GetComponent<ChatSystem>().SetName(InputBox.text);
                break;
        }
    }

    public void ChoiceSelect(bool Left)
    {
        GameObject.FindGameObjectWithTag("CommonData").GetComponent<ChatSystem>().ChoiceSelect(Left ? Choice1.text : Choice2.text, Left ? SetLine1 : SetLine2);
    }
}