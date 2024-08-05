using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{

    public void StartTeamGame()
    {
        SceneManager.LoadScene("TEAM_GAME");
    }

    public void StartSoloGame()
    {
        SceneManager.LoadScene("SOLO_GAME");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MENU");
    }
}