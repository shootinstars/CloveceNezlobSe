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
    public static bool GameFinished = false;
    public static bool RoundFinished = false;

    public static TMP_Text CurrentPlayerInfo;
    public static TMP_Text RollInfo;

    public static GameObject RollAgain;
    public static GameObject ChosenPiece;
    public static GameObject RollButton;
    

    public static PieceColor CurrentPlayer;

    public static int RollCount;
    public static int CurrentRoll;

    public static GameObject[] AllPieces;
    public static Dictionary<PieceColor, GameObject[]> Pieces = new();
    public static Dictionary<PieceColor, int> ActivePiecesCount = new();
    public static GameObject[] GamePlan = new GameObject[40];


    // Start is called before the first frame update
    void Start()
    {
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

    public static void EndRound(PieceColor color)
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
        yield return new WaitForSeconds(2);
        EndRound(CurrentPlayer);
    }

    private void InitializePiecesDict()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            Pieces[color] = GameObject.FindGameObjectsWithTag($"{color}Piece");
        }
    }

    private static void TurnPiecesOn(PieceColor color)
    {
        foreach (var piece in Pieces[color])
        {
            piece.GetComponent<Button>().interactable = true;
        }
    }

    private static void TurnPiecesOff(PieceColor color)
    {
        foreach (var piece in Pieces[color])
        {
            piece.GetComponent<Button>().interactable = false;
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
        TurnPiecesOn(color);
        CurrentPlayerInfo.text = "Current turn: " + color;
        yield return new WaitUntil(() => RoundFinished);
    }

    public static GameObject[] GetAllPieces()
    {
        return AllPieces;
    }

    public static void Move(int newPosition)
    {
        Debug.Log("blue Pieces on board:" + ActivePiecesCount[PieceColor.Blue]);
        Debug.Log(CurrentRoll);
        var pieceComp = ChosenPiece.GetComponent<Piece>();
        if (pieceComp.CurrentTile == -1)
        {
            ActivePiecesCount[pieceComp.Color] += 1;
        }
        else
        {
            GamePlan[pieceComp.CurrentTile] = null;
        }
        ChosenPiece.GetComponent<Piece>().CurrentTile = newPosition;
        if (GamePlan[newPosition] != null)
        {
            GamePlan[newPosition].GetComponent<Piece>().ReturnHome();
        }
        ChosenPiece.transform.position = TileManager.FieldTiles[newPosition].transform.position;
        GamePlan[newPosition] = ChosenPiece;
        TileManager.UnselectHighlightedMoves();
        if (CurrentRoll != 6 && (ActivePiecesCount[pieceComp.Color] != 0))
        {
            EndRound(pieceComp.Color);
        } else
        {
            RollButton.GetComponent<Button>().interactable = true;
            RollAgain.SetActive(true);
        }

        Debug.Log("Move done, new position is: " + newPosition);
    }

}