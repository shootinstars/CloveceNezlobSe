using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneManagement : MonoBehaviour
{

    [SerializeField] private TMP_Text menuText;

    void Start()
    {
        if (menuText != null)
        {
            var languageManager = FindObjectOfType<LanguageManager>();
            if (languageManager == null)
            {
                Debug.Log("WHY");
            }
            if (languageManager.LanguageId == 0)
            {
                menuText.text = languageManager.menuTextCzech;
            }
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("GAME");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MENU");
    }
}