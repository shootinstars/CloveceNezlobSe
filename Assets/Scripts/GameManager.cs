using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public bool GameFinished = false;
    public bool RoundFinished = false;

    public TMP_Text CurrentPlayerInfo;
    public TMP_Text RollInfo;

    public GameObject RollAgain;
    public GameObject ChosenPiece;
    public GameObject RollButton;
    

    public PieceColor CurrentPlayer;
    public TileManager TileManager;

    public int RollCount;
    public int CurrentRoll;

    public GameObject[] AllPieces;
    public Dictionary<PieceColor, GameObject[]> Pieces = new();
    public Dictionary<PieceColor, int> ActivePiecesCount = new();
    public GameObject[] GamePlan = new GameObject[40];


    // Start is called before the first frame update
    void Start()
    {
        TileManager = GetComponent<TileManager>();
        CurrentPlayerInfo = GameObject.Find("CurrentPlayerInfo").GetComponent<TMP_Text>();
        RollInfo = GameObject.Find("RollInfo").GetComponent<TMP_Text>();
        RollAgain = GameObject.Find("RollAgain");
        RollButton = GameObject.Find("RollButton");
        RollAgain.SetActive(false);
        InitializePiecesDict();
        AllPieces = Pieces.Values.SelectMany(array => array).ToArray();
        InitializeActiveCount();
        StartCoroutine(PlayGame());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Roll()
    {
        TurnPiecesOn(CurrentPlayer);
        var roll = Random.Range(1, 7);
        RollInfo.text = "Rolled:" + roll;
        RollCount++;
        var limit = ActivePiecesCount[CurrentPlayer] == 0 ? 3 : 1;
        Debug.Log(limit + " is the limit");
        if (roll != 6 && RollCount >= limit)
        {
            RollButton.GetComponent<Button>().interactable = false;
            if (limit == 3)
            {
                StartCoroutine(EndRoundCoroutine());
            }
        }

        if (roll == 6)
        {
            RollButton.GetComponent<Button>().interactable = false;
        }

        if (limit == 3 && roll != 6)
        {
            RollAgain.SetActive(true);
        }

        CurrentRoll = roll;
    }

    public void EndRound(PieceColor color)
    {
        TurnPiecesOff(color);
        RoundFinished = true;
    }

    public void InitializeActiveCount()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            ActivePiecesCount.Add(color, 0);
        }
    }

    IEnumerator EndRoundCoroutine()
    {
        yield return new WaitForSeconds(0.5f);
        EndRound(CurrentPlayer);
    }

    private void InitializePiecesDict()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            Pieces[color] = GameObject.FindGameObjectsWithTag($"{color}Piece");
        }
    }

    private void TurnPiecesOn(PieceColor color)
    {
        foreach (var piece in Pieces[color])
        {
            piece.GetComponent<Button>().interactable = true;
        }
    }

    private void TurnPiecesOff(PieceColor color)
    {
        foreach (var piece in Pieces[color])
        {
            piece.GetComponent<Button>().interactable = false;
            piece.GetComponent<Piece>().Chosen.SetActive(false);
        }
    }

    IEnumerator PlayGame()
    {
        while (!GameFinished)
        {
            yield return PlayRound(PieceColor.Blue);
            yield return PlayRound(PieceColor.Red);
            yield return PlayRound(PieceColor.Green);
            yield return PlayRound(PieceColor.Yellow);
        }
    }

    IEnumerator PlayRound(PieceColor color)
    {
        RollAgain.SetActive(false);
        RollInfo.text = "Roll: ";
        RollButton.GetComponent<Button>().interactable = true;
        RoundFinished = false;
        CurrentPlayer = color;
        RollCount = 0;
        CurrentPlayerInfo.text = "Current turn: " + color;
        yield return new WaitUntil(() => RoundFinished);
    }

    public GameObject[] GetAllPieces()
    {
        return AllPieces;
    }

    public void UpdateInfoBeforeMove(Piece piece)
    {
        if (piece.CurrentTile == -1)
        {
            ActivePiecesCount[piece.Color] += 1;
            piece.TilesGone = 1;
        }
        else
        {
            GamePlan[piece.CurrentTile] = null;
            piece.TilesGone += CurrentRoll;
        }
    }

    public void Move(int newPosition)
    {
        Debug.Log("blue Pieces on board:" + ActivePiecesCount[PieceColor.Blue]);
        Debug.Log(CurrentRoll);
        var pieceComp = ChosenPiece.GetComponent<Piece>();
        UpdateInfoBeforeMove(pieceComp);
        if (pieceComp.TilesGone > 40)
        {
            MoveToEnd(pieceComp, newPosition);
        }
        else
        {
            MoveOnBoard(pieceComp, newPosition);
        }
        TileManager.UnselectHighlightedMoves();
        if (CurrentRoll != 6 && (ActivePiecesCount[pieceComp.Color] != 0))
        {

            StartCoroutine(EndRoundCoroutine());
        } else
        {
            RollButton.GetComponent<Button>().interactable = true;
            RollAgain.SetActive(true);
        }
        Debug.Log("Moved piece has done " + pieceComp.TilesGone +  " moves");
    }

    public void MoveOnBoard(Piece piece, int newPosition)
    {
        piece.CurrentTile = newPosition;
        if (GamePlan[newPosition] != null)
        {
            GamePlan[newPosition].GetComponent<Piece>().ReturnHome();
        }
        ChosenPiece.transform.position = TileManager.FieldTiles[newPosition].transform.position;
        GamePlan[newPosition] = ChosenPiece;
    }

    public void MoveToEnd(Piece piece, int endPosition)
    {
        piece.CurrentTile = 40 + endPosition;
        TileManager.EndFields[piece.Color][endPosition] = ChosenPiece;
        ChosenPiece.transform.position = TileManager.EndTiles[piece.Color][endPosition].transform.position;
    }

}