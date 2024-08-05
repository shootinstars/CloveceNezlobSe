using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageManager : MonoBehaviour
{
    public int LanguageId;

    public string mainTitleCzech = "�LOV��E NEZLOB SE";
    public string playerCounterTitleCzech = "PO�ET HR���";
    public string playTeamTextCzech = "HR�T SPOLE�N�";
    public string playSoloTextCzech = "HR�T S�M";
    public string skipTutorialCzech = "P�esko�it v�uku";
    public string rollAgainCzech = "H�zej znovu!";
    public string rollWarningCzech = "Nejprve prove� sv�j tah!";
    public string currentTurnCzech = "Aktu�ln� tah:";

    public string diceTutorialCzech = "Klikni na kostku";
    public string pieceTutorialCzech = "Klikni na figurku";
    public string tileTutorialCzech = "Klikni na pol��ko";

    public string resultHeadlineCzech = "V�sledky";
    public string playAgainCzech = "Hr�t znovu";
    public string toMenuCzech = "Zp�tky do menu";
    public string pauseCzech = "PAUZA";

    public string congratulationsTextCzech = "GRATULACE!!!\n\nDO�EL JSI NA KONEC CESTY!!!";
    public string continueTextCzech = "Pokra�ovat ve sledov�n�";

    void Start()
    {
        congratulationsTextCzech = "GRATULACE!!!\n\nDO�EL JSI NA KONEC CESTY!!!";
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
