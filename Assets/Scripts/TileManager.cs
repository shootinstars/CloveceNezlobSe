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

    public const int FieldSize = 40;

    public Dictionary<PieceColor, GameObject[]> EndTiles { get; } = new Dictionary<PieceColor, GameObject[]>();
    public Dictionary<PieceColor, GameObject[]> EndFields { get; } = new Dictionary<PieceColor, GameObject[]>();

    private Color highlightBrown = new Color(164 / 255f, 87 / 255f, 0);


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
    }

    public void HighlightPossibleMove(Piece piece, PieceColor color, int roll)
    {
        UnselectHighlightedMoves();
        piece.Chosen.SetActive(true);
        var realIndex = (piece.CurrentTile + roll) % FieldSize;
        if (piece.TilesGone + roll > FieldSize)
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
                EndTiles[piece.Color][endIndex % 5 - 1].GetComponent<Image>().color = highlightBrown;
                EndTiles[piece.Color][endIndex % 5 - 1].GetComponent<Button>().interactable = true;
            }
        }
    }

    public void HighlightTile(int index, PieceColor color)
    {
        fieldTiles[index].GetComponent<Image>().color = highlightBrown;
        fieldTiles[index].GetComponent<Button>().interactable = true;
        CheckPiecesInDanger(color, index);
    }

    public void HighlightStart(PieceColor color)
    {
        var realIndex = 0 + (int) color * 10;
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
            if (piece.GetComponent<Piece>().Color != gameManager.getCurrentPlayer())
            {
                piece.GetComponent<Button>().interactable = false;
            }
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
