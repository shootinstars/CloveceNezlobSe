using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public int LanguageId;

    public string menuTextCzech = "HRÁT";
    public string skipTutorialCzech = "Pøeskoèit výuku";
    public string rollAgainCzech = "Házej znovu!";
    public string endTurnCzech = "Ukonèit tah";
    public string rollWarningCzech = "Nejprve proveï svùj tah!";
    public string currentTurnCzech = "Aktuální tah:";

    public string diceTutorialCzech = "Klikni na kostku";
    public string pieceTutorialCzech = "Klikni na figurku";
    public string tileTutorialCzech = "Klikni na políèko";
    public string endTurnTutorialCzech = "Jestliže nemùžeš hnout s žádnou ze svých figurek, ukonèi tah";

    public string resultHeadlineCzech = "Výsledky";
    public string playAgainCzech = "Hrát znovu";
    public string toMenuCzech = "Zpátky do menu";
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
