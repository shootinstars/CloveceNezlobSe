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
        if (gameManager.PieceTutorial.activeSelf)
        {
            gameManager.TutorialChosenPiece = gameObject;
            gameManager.tutorialPartFinished = true;
            return;
        }

        if (gameManager.Tutorial.activeSelf)
        {
            return;
        }
        if (!OccupiedHighlight.activeSelf)
        {
            gameManager.TurnOffRollWarning();
            gameManager.SetChosenPiece(gameObject);
            tileManager.UnselectHighlightedMoves();
            tileManager.HighlightPossibleMove(this, Color, gameManager.getCurrentRoll());
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
