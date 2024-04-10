using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{

    [SerializeField] public static GameObject[] FieldTiles;
    [SerializeField] public static Dictionary<PieceColor, GameObject[]> StartTiles = new Dictionary<PieceColor, GameObject[]>();
    [SerializeField] public static Dictionary<PieceColor, GameObject[]> EndTiles = new Dictionary<PieceColor, GameObject[]>();
    public GameManager GameManager;


    // Start is called before the first frame update
    void Start()
    {
        FieldTiles = GameObject.FindGameObjectsWithTag("Board").OrderBy(x => x.GetComponent<GameTile>().Id).ToArray();
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        FillDictionaries();
    }

    private void FillDictionaries()
    {
        foreach (var color in (PieceColor[])Enum.GetValues(typeof(PieceColor)))
        {
            StartTiles[color] = GameObject.FindGameObjectsWithTag($"{color}Start");
            EndTiles[color] = GameObject.FindGameObjectsWithTag($"{color}End");
        }
    }

    public static void HighlightPossibleMove(Piece piece, PieceColor color, int roll)
    { 
        piece.Chosen.SetActive(true);
        var realIndex = (piece.CurrentTile + roll) % 40;
        if (piece.CurrentTile != -1)
        {
            FieldTiles[realIndex].GetComponent<Image>().color = Color.green;
            FieldTiles[realIndex].GetComponent<Button>().interactable = true;
            CheckPiecesInDanger(color, realIndex);
        }
        else if (piece.CurrentTile == -1 && roll == 6)
        {
            realIndex = 0;
            FieldTiles[realIndex].GetComponent<Image>().color = Color.green;
            FieldTiles[realIndex].GetComponent<Button>().interactable = true;
            CheckPiecesInDanger(color, realIndex);
        }
    }

    private static void CheckPiecesInDanger(PieceColor color, int tile)
    {
        if (GameManager.GamePlan[tile])
        {
            GameManager.GamePlan[tile].GetComponent<Piece>().TurnHighlightOn();
        }
    }

    public void UnselectHighlightedMoves()
    {
        var allPieces = GameManager.GetAllPieces();
        foreach (var tile in FieldTiles)
        {
            tile.GetComponent<Image>().color = Color.white;
            tile.GetComponent<Button>().interactable = false;

        }
        foreach (var piece in allPieces)
        {
            piece.GetComponent<Piece>().TurnHighlightOff();
            piece.GetComponent<Piece>().Chosen.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
