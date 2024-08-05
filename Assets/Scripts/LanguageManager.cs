using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public int LanguageId;

    public string mainTitleCzech = "ÈLOVÌÈE NEZLOB SE";
    public string playerCounterTitleCzech = "POÈET HRÁÈÙ";
    public string playTeamTextCzech = "HRÁT SPOLEÈNÌ";
    public string playSoloTextCzech = "HRÁT SÁM";
    public string skipTutorialCzech = "Pøeskoèit výuku";
    public string rollAgainCzech = "Házej znovu!";
    public string rollWarningCzech = "Nejprve proveï svùj tah!";
    public string currentTurnCzech = "Aktuální tah:";

    public string diceTutorialCzech = "Klikni na kostku";
    public string pieceTutorialCzech = "Klikni na figurku";
    public string tileTutorialCzech = "Klikni na políèko";

    public string resultHeadlineCzech = "Výsledky";
    public string playAgainCzech = "Hrát znovu";
    public string toMenuCzech = "Zpátky do menu";
    public string pauseCzech = "PAUZA";

    public string congratulationsTextCzech = "GRATULACE!!!\n\nDOŠEL JSI NA KONEC CESTY!!!";
    public string continueTextCzech = "Pokraèovat ve sledování";

    void Start()
    {
        congratulationsTextCzech = "GRATULACE!!!\n\nDOŠEL JSI NA KONEC CESTY!!!";
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
