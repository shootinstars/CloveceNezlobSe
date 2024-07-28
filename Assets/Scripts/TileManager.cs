using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private GameObject[] fieldTiles;

    public Dictionary<PieceColor, GameObject[]> EndTiles { get; } = new Dictionary<PieceColor, GameObject[]>();
    public Dictionary<PieceColor, GameObject[]> EndFields { get; } = new Dictionary<PieceColor, GameObject[]>();

    private Dictionary<PieceColor, Color> highlightColors = new Dictionary<PieceColor, Color>();
    private Color highlightRed = new Color(253/255f, 116/255f, 116/255f);
    private Color highlightGreen = new Color(186/255f, 1, 97/255f);
    private Color highlightBlue = new Color(78/255f, 202/255f, 1);
    private Color highlightYellow = new Color(1, 1, 90 / 255f);
    private Color highlight = Color.gray;
    private Color highlightBrown = new Color(195 / 255f, 116 / 255f, 14 / 255f);


    // Start is called before the first frame update
    void Awake()
    {
        fieldTiles = GameObject.FindGameObjectsWithTag("Board").OrderBy(x => x.GetComponent<GameTile>().Id).ToArray();
        FillDictionaries();
    }

    private void FillDictionaries()
    {
        foreach (var color in (PieceColor[])Enum.GetValues(typeof(PieceColor)))
        {
            EndTiles[color] = GameObject.FindGameObjectsWithTag($"{color}End").OrderBy(x => x.GetComponent<GameTile>().Id).ToArray();
            EndFields[color] = new GameObject[4];
        }
        highlightColors.Add(PieceColor.Blue, highlightBlue);
        highlightColors.Add(PieceColor.Green, highlightGreen);
        highlightColors.Add(PieceColor.Yellow, highlightYellow);
        highlightColors.Add(PieceColor.Red, highlightRed);
    }

    public void HighlightPossibleMove(Piece piece, PieceColor color, int roll)
    { 
        piece.Chosen.SetActive(true);
        var realIndex = (piece.CurrentTile + roll) % 40;
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
                Debug.Log(endIndex % 5 - 1);
                // EndTiles[piece.Color][endIndex % 5 - 1].GetComponent<Image>().color = highlightColors[piece.Color];
                EndTiles[piece.Color][endIndex % 5 - 1].GetComponent<Image>().color = highlightBrown;
                EndTiles[piece.Color][endIndex % 5 - 1].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void HighlightTile(int index, PieceColor color)
    {
        //fieldTiles[index].GetComponent<Image>().color = highlightColors[color];
        fieldTiles[index].GetComponent<Image>().color = highlightBrown;
        fieldTiles[index].GetComponent<Button>().interactable = true;
        CheckPiecesInDanger(color, index);
    }

    public void HighlightStart(PieceColor color)
    {
        var realIndex = 0 + (int) color * 10;
        //fieldTiles[realIndex].GetComponent<Image>().color = highlightColors[color];
        fieldTiles[realIndex].GetComponent<Image>().color = highlightBrown;
        fieldTiles[realIndex].GetComponent<Button>().interactable = true;
        CheckPiecesInDanger(color, realIndex);
    }

    private void CheckPiecesInDanger(PieceColor color, int tile)
    {
        if (gameManager.GamePlan[tile] != null && gameManager.GamePlan[tile].GetComponent<Piece>().Color != color)
        {
            gameManager.GamePlan[tile].GetComponent<Piece>().TurnHighlightOn();
        }
    }

    public void UnselectHighlightedMoves()
    {
        var allPieces = gameManager.GetAllPieces();
        foreach (var tile in fieldTiles)
        {
            tile.GetComponent<Image>().color = tile.GetComponent<GameTile>().BaseColor;
            tile.GetComponent<Button>().interactable = false;

        }
        foreach (var piece in allPieces)
        {
            piece.GetComponent<Piece>().TurnHighlightOff();
            piece.GetComponent<Piece>().Chosen.SetActive(false);
        }

        foreach (var color in EndTiles.Keys)
        {
            foreach (var endTile in EndTiles[color])
            {
                endTile.GetComponent<Image>().color = endTile.GetComponent<GameTile>().BaseColor;
                endTile.GetComponent<Button>().interactable = false;
            }
        }
    }

    public GameObject[] getFieldTiles()
    {
        return fieldTiles;
    }

}
