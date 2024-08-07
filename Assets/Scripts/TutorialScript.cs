using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScript : MonoBehaviour
{

    public GameObject TutorialScreen;
    public GameObject PieceTutorial;
    public GameObject TileTutorial;
    public GameObject TutorialChosenPiece;
    public GameObject DiceTutorial;

    public bool tutorialPartFinished;
    public bool tutorialComplete;

    [SerializeField] private GameObject diceTutorialBackground;
    [SerializeField] private GameObject pieceTutorialBackground;
    [SerializeField] private GameObject tileTutorialBackground;
    [SerializeField] private GameObject tileTutorialGreyBackground;
    [SerializeField] private GameObject skipTutorialButton;


    [SerializeField] private GameManager gameManager;
    [SerializeField] private DiceControl diceControl;
    [SerializeField] private TileManager tileManager;

    public IEnumerator StartDiceTutorial()
    {
        tutorialPartFinished = false;
        DiceTutorial.SetActive(true);
        diceTutorialBackground.SetActive(true);
        yield return new WaitUntil(() => tutorialPartFinished);
    }

    public IEnumerator StartPieceTutorial()
    {
        gameManager.RollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[5];
        gameManager.ActivateBluePieces();
        DiceTutorial.SetActive(false);
        tutorialPartFinished = false;
        PieceTutorial.SetActive(true);
        diceTutorialBackground.SetActive(false);
        pieceTutorialBackground.SetActive(true);
        yield return new WaitUntil(() => tutorialPartFinished);
    }

    public IEnumerator StartTileTutorial()
    {
        tileTutorialGreyBackground.SetActive(true);
        tileManager.getFieldTiles()[0].GetComponent<Button>().interactable = true;
        gameManager.DeactivateBluePieces();
        PieceTutorial.SetActive(false);
        tutorialPartFinished = false;
        TileTutorial.SetActive(true);
        pieceTutorialBackground.SetActive(false);
        tileTutorialBackground.SetActive(true);
        yield return new WaitUntil(() => tutorialPartFinished);
    }

    public void EndTutorial()
    {
        tileManager.getFieldTiles()[0].GetComponent<Button>().interactable = false;
        tileTutorialGreyBackground.SetActive(false);
        foreach (var piece in gameManager.Pieces[PieceColor.Blue])
        {
            piece.GetComponent<Piece>().ResetPiece();
        }
        gameManager.ChangeToDefaultButton();
        HideTutorial();
    }

    public void HideTutorial()
    {
        DiceTutorial.SetActive(false);
        PieceTutorial.SetActive(false);
        TileTutorial.SetActive(false);
        TutorialScreen.SetActive(false);
        diceTutorialBackground.SetActive(false);
        tileTutorialBackground.SetActive(false);
        pieceTutorialBackground.SetActive(false);
        skipTutorialButton.SetActive(false);
    }
}
