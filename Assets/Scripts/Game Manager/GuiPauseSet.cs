using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiPauseSet : MonoBehaviour
{
    CommonData CommonData;

    void Start()
    {
        CommonData = GameObject.FindGameObjectWithTag("CommonData").GetComponent<CommonData>();
    }

    public void UnpauseGame()
    {
        CommonData.GameUnpause();
    }
}
