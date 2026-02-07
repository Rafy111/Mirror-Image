using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeUi : MonoBehaviour
{
    public GameObject CurrentUi;
    public GameObject ToggleableUi;

    public void SwitchUi(GameObject Ui)
    {
        CurrentUi.SetActive(false);
        CurrentUi = Ui;
        CurrentUi.SetActive(true);
    }

    public void ToggleUi(bool State)
    {
        ToggleableUi.SetActive(State);
    }
}
