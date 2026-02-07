using UnityEngine;

public class SplashDaysSc : MonoBehaviour
{
    CommonData Sc_CommonData;

    private void Start()
    {
        FindCommonData();
    }

    public void FindCommonData()
    {
        Sc_CommonData = GameObject.FindGameObjectWithTag("CommonData").GetComponent<CommonData>();
    }

    public void StartDayTimer()
    {
        if (Sc_CommonData == null) FindCommonData();
        Sc_CommonData.StartNewDay();
    }
}