using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    private GameManager GameManager;

    [SerializeField] public GameObject[] FieldTiles;
    [SerializeField] public Dictionary<PieceColor, GameObject[]> StartTiles = new Dictionary<PieceColor, GameObject[]>();
    [SerializeField] public Dictionary<PieceColor, GameObject[]> EndTiles = new Dictionary<PieceColor, GameObject[]>();
    [SerializeField] public Dictionary<PieceColor, GameObject[]> EndFields = new Dictionary<PieceColor, GameObject[]>();

    public Boolean MovePossible;


    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        FieldTiles = GameObject.FindGameObjectsWithTag("Board").OrderBy(x => x.GetComponent<GameTile>().Id).ToArray();
        FillDictionaries();
    }

    private void FillDictionaries()
    {
        foreach (var color in (PieceColor[])Enum.GetValues(typeof(PieceColor)))
        {
            StartTiles[color] = GameObject.FindGameObjectsWithTag($"{color}Start");
            EndTiles[color] = GameObject.FindGameObjectsWithTag($"{color}End");
            EndFields[color] = new GameObject[4];
        }
    }

    public void HighlightPossibleMove(Piece piece, PieceColor color, int roll)
    { 
        piece.Chosen.SetActive(true);
        var realIndex = (piece.CurrentTile + roll) % 40;
        Debug.Log("roll is " + roll);
        if (piece.TilesGone + roll > 40)
        {
            HighlightEnd(piece, roll);
        }
        else if (piece.CurrentTile != -1)
        {
            HighlightTile(realIndex, color);
        }
        else if (piece.CurrentTile == -1 && roll == 6)
        {
            HighlightStart(color);
        }
    }

    public void HighlightEnd(Piece piece, int roll)
    {
        var endIndex = piece.TilesGone + roll;
        if (endIndex < 45)
        {
            if (EndFields[piece.Color][endIndex % 5 - 1] == null)
            {
                EndTiles[piece.Color][endIndex % 5].GetComponent<Image>().color = Color.green;
                EndTiles[piece.Color][endIndex % 5].GetComponent<Button>().interactable = true;
                MovePossible = true;
            }
        }
    }

    public void HighlightTile(int index, PieceColor color)
    {
        FieldTiles[index].GetComponent<Image>().color = Color.green;
        FieldTiles[index].GetComponent<Button>().interactable = true;
        CheckPiecesInDanger(color, index);
    }

    public void HighlightStart(PieceColor color)
    {
        var realIndex = 0 + (int) color * 10;
        FieldTiles[realIndex].GetComponent<Image>().color = Color.green;
        FieldTiles[realIndex].GetComponent<Button>().interactable = true;
        CheckPiecesInDanger(color, realIndex);
    }

    private void CheckPiecesInDanger(PieceColor color, int tile)
    {
        if (GameManager.GamePlan[tile] != null && GameManager.GamePlan[tile].GetComponent<Piece>().Color != color)
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
