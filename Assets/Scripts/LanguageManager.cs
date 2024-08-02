using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public int LanguageId;

    public string menuTextCzech = "HR�T";
    public string skipTutorialCzech = "P�esko�it v�uku";
    public string rollAgainCzech = "H�zej znovu!";
    public string endTurnCzech = "Ukon�it tah";
    public string rollWarningCzech = "Nejprve prove� sv�j tah!";
    public string currentTurnCzech = "Aktu�ln� tah:";

    public string diceTutorialCzech = "Klikni na kostku";
    public string pieceTutorialCzech = "Klikni na figurku";
    public string tileTutorialCzech = "Klikni na pol��ko";
    public string endTurnTutorialCzech = "Jestli�e nem��e� hnout s ��dnou ze sv�ch figurek, ukon�i tah";

    public string resultHeadlineCzech = "V�sledky";
    public string playAgainCzech = "Hr�t znovu";
    public string toMenuCzech = "Zp�tky do menu";
    public string pauseCzech = "PAUZA";

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetCzechLanguage()
    {
        LanguageId = 0;
    }

    public void SetEnglishLanguage()
    {
        LanguageId = 1;
    }

}
