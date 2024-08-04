using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{

    [SerializeField] private TMP_Text playTeamText;
    [SerializeField] private TMP_Text playSoloText;

    void Start()
    {
        if (playSoloText != null)
        {
            var languageManager = FindObjectOfType<LanguageManager>();
            if (languageManager.LanguageId == 0)
            {
                playTeamText.text = languageManager.playTeamTextCzech;
                playSoloText.text = languageManager.playSoloTextCzech;
            }
        }
    }

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