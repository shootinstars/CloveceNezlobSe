using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public bool GameFinished = false;
    public bool RoundFinished = false;

    public TileManager TileManager;
    public GameObject[] AllPieces;
    public PieceColor CurrentPlayer;
    public Dictionary<PieceColor, GameObject[]> Pieces = new();
    public static int CurrentRoll;
    public static GameObject[] GamePlan = new GameObject[40];


    // Start is called before the first frame update
    void Start()
    {
        TileManager = gameObject.GetComponent<TileManager>();
        InitializePiecesDict();
        AllPieces = Pieces.Values.SelectMany(array => array).ToArray();
        StartCoroutine(PlayGame());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Roll()
    {
        var roll = Random.Range(1, 7);
        Debug.Log("rolled: " + roll);
        CurrentRoll = roll;
        if (roll == 6)
        {
            RoundFinished = true;
        }
    }

    private void InitializePiecesDict()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            Pieces[color] = GameObject.FindGameObjectsWithTag($"{color}Piece");
            Debug.Log(Pieces[color].Length);
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
        RoundFinished = false;
        CurrentPlayer = color;
        Debug.Log(color + " player is currently on turn");

        yield return new WaitUntil(() => RoundFinished);
    }

    public GameObject[] GetAllPieces()
    {
        return AllPieces;
    }

    public static void Move(GameObject piece, int newPosition)
    {
        piece.GetComponent<Piece>().CurrentTile = newPosition;
        if (GamePlan[newPosition] != null)
        {
            GamePlan[newPosition].GetComponent<Piece>().ReturnHome();
        }
        piece.transform.position = TileManager.FieldTiles[newPosition].transform.position;
        TileManager.FieldTiles[newPosition] = piece;
    }

}