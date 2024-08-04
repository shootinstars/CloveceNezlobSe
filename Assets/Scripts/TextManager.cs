    using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextManager : MonoBehaviour
{
    [SerializeField] private TMP_Text endTurnButtonText;
    [SerializeField] private TMP_Text skipTutorialText;
    [SerializeField] private TMP_Text currentTurnText;
    [SerializeField] private TMP_Text diceTutorialText;
    [SerializeField] private TMP_Text rollWarningText;
    [SerializeField] private TMP_Text pieceTutorialText;
    [SerializeField] private TMP_Text tileTutorialText;
    [SerializeField] private TMP_Text rollAgainText;
    [SerializeField] private TMP_Text endTurnTutorialText;
    [SerializeField] private TMP_Text resultHeadlineText;
    [SerializeField] private TMP_Text playAgainText;
    [SerializeField] private TMP_Text toMenuText;
    [SerializeField] private TMP_Text pauseText;
    [SerializeField] private TMP_Text pausedBackToMenuText;
    [SerializeField] private TMP_Text congratulationsText;
    [SerializeField] private TMP_Text continueText;

    // Start is called before the first frame update
    void Start()
    {
        var languageManager = FindObjectOfType<LanguageManager>();
        if (languageManager.LanguageId == 0)
        {
            rollAgainText.text = languageManager.rollAgainCzech;
            skipTutorialText.text = languageManager.skipTutorialCzech;
            rollWarningText.text = languageManager.rollWarningCzech;
            currentTurnText.text = languageManager.currentTurnCzech;
            diceTutorialText.text = languageManager.diceTutorialCzech;
            pieceTutorialText.text = languageManager.pieceTutorialCzech;
            tileTutorialText.text = languageManager.tileTutorialCzech;
            resultHeadlineText.text = languageManager.resultHeadlineCzech;
            playAgainText.text = languageManager.playAgainCzech;
            toMenuText.text = languageManager.toMenuCzech;
            pauseText.text = languageManager.pauseCzech;
            pausedBackToMenuText.text = languageManager.toMenuCzech.ToUpper();
            if (congratulationsText != null)
            {
                congratulationsText.text = languageManager.congratulationsTextCzech;
                continueText.text = languageManager.continueTextCzech;
            }
        }
    }

    
}
