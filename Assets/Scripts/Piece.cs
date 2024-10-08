using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


[Serializable]
public enum PieceColor
{
    Blue = 0,
    Red = 1,
    Green = 2,
    Yellow = 3,
}

public class Piece : MonoBehaviour
{

    public PieceColor Color;
    [SerializeField] private GameObject startTile;
    [SerializeField] private GameObject OccupiedHighlight;
    public GameObject Chosen;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private TutorialScript tutorial;

    public int CurrentTile = -1;
    public int TilesGone;
    public bool hasFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        TurnHighlightOff();
    }



    public void ReturnHome()
    {
        gameObject.transform.position = startTile.transform.position;
        gameManager.DecreaseActivePieces(Color);
        CurrentTile = -1;
        TilesGone = 0;
    }

    public void TurnHighlightOn()
    {
        OccupiedHighlight.SetActive(true);
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void TurnHighlightOff()
    {
        OccupiedHighlight.SetActive(false);
    }

    public void GetKickedOut()
    {
        if (OccupiedHighlight.activeSelf)
        {
            gameManager.Move(CurrentTile);
        }
    }

    public void AttemptMove()
    {
        if (tutorial.PieceTutorial.activeSelf)
        {
            tutorial.TutorialChosenPiece = gameObject;
            tutorial.tutorialPartFinished = true;
            return;
        }

        if (tutorial.TutorialScreen.activeSelf)
        {
            return;
        }
        if (!OccupiedHighlight.activeSelf)
        {
            gameManager.TurnOffRollWarning();
            gameManager.SetChosenPiece(gameObject);
            tileManager.UnselectHighlightedMoves();
            tileManager.HighlightPossibleMove(this, Color, gameManager.CurrentRoll);
        }
    }

    public void ResetPiece()
    {
        CurrentTile = -1;
        TilesGone = 0;
        hasFinished = false;
        gameObject.transform.position = startTile.transform.position;
    }
}
