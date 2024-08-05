using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private GameObject playerCounterScreen;
    [SerializeField] private GameObject gameChooser;

    [SerializeField] private TMP_Text playTeamText;
    [SerializeField] private TMP_Text playSoloText;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text playerCounterTitleText;
    [SerializeField] private TMP_Text playerCounterText;


    void Start()
    {
        var languageManager = FindObjectOfType<LanguageManager>();
        if (languageManager.LanguageId == 0)
        {
            playTeamText.text = languageManager.playTeamTextCzech;
            playSoloText.text = languageManager.playSoloTextCzech;
            playerCounterTitleText.text = languageManager.playerCounterTitleCzech;
            titleText.text = languageManager.mainTitleCzech;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && playerCounterScreen.activeSelf)
        {
            TurnOffPlayerCounterScreen();
        }
    }

    public void TurnOnPlayerCounterScreen()
    {
        gameChooser.SetActive(false);
        playerCounterScreen.SetActive(true);
        FindObjectOfType<PlayerCount>().ShowCounterText();
    }

    public void TurnOffPlayerCounterScreen()
    {
        playerCounterScreen.SetActive(false);
        gameChooser.SetActive(true);
    }

    public void IncreaseCount()
    {
        FindObjectOfType<PlayerCount>().IncreaseCount();
    }

    public void DecreaseCount()
    {
        FindObjectOfType<PlayerCount>().DecreaseCount();
    }
}
