using UnityEngine;
using UnityEngine.SceneManagement;

public class PhoneTabMainMenu : MonoBehaviour
{
    public GameObject Tab_Current;

    public void ChangeTab(GameObject Tab)
    {
        Tab.SetActive(true);
        Tab_Current.SetActive(false);
        Tab_Current = Tab;
    }

    public void QuitApp()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("TheGame");
    }
}
