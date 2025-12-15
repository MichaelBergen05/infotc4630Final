using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject Info;

    public void StartGame()
    {
        SceneManager.LoadScene("_Level_Mode");
    }

    public void Quit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void InfoToggle()
    {
        Info.SetActive(!Info.activeSelf);
    }


}
