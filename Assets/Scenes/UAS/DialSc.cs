using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialSc : MonoBehaviour
{
    public List<string> AllDial;
    public TMP_Text TextSayer;
    public TMP_Text TextDialogue;

    public void StartDial()
    {
        if (AllDial.Count > 0)
        {
            string[] NewCommand = AllDial[0].Split(" - ");
            AllDial.RemoveAt(0);

            TextSayer.text = NewCommand[0];
            TextDialogue.text = NewCommand[1];
        }
    }

    public void NextLine()
    {
        StartDial();
    }
}
