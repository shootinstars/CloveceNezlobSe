using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{

    [SerializeField] private GameObject[] _fieldTiles;
    [SerializeField] private Dictionary<PieceColor, GameObject[]> _startTiles = new Dictionary<PieceColor, GameObject[]>();
    [SerializeField] private Dictionary<PieceColor, GameObject[]> _endTiles = new Dictionary<PieceColor, GameObject[]>();
    private GameManager _gameManager;


    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        FillDictionaries();
    }

    private void FillDictionaries()
    {
        foreach (var color in (PieceColor[])Enum.GetValues(typeof(PieceColor)))
        {
            _startTiles[color] = GameObject.FindGameObjectsWithTag($"{color}Start");
            _endTiles[color] = GameObject.FindGameObjectsWithTag($"{color}End");
        }
    }

    public void HighlightPossibleMoves(GameObject[] pieces, PieceColor color, int roll)
    {
        foreach (var piece in pieces)
        {
            var currentPiece = piece.GetComponent<Piece>();
            if (currentPiece.GetCurrentTile() == -1) continue;
            var realIndex = (currentPiece.GetCurrentTile() + roll) % 40;
            _fieldTiles[realIndex].GetComponent<Image>().color = Color.green;
            _fieldTiles[realIndex].GetComponent<Button>().interactable = true;
            CheckPiecesInDanger(color, realIndex);
        }
    }

    private void CheckPiecesInDanger(PieceColor color, int tile)
    {
        var allPieces = _gameManager.GetAllPieces();
        foreach (var piece in allPieces)
        {
            var currentPiece = piece.GetComponent<Piece>();
            if (currentPiece.GetColor() != color && currentPiece.GetCurrentTile() == tile)
            {
                currentPiece.TurnHighlightOn();
            }
        }
    }

    public void UnselectHighlightedMoves()
    {
        var allPieces = _gameManager.GetAllPieces();
        foreach (var tile in _fieldTiles)
        {
            tile.GetComponent<Image>().color = Color.white;
            tile.GetComponent<Button>().interactable = false;

        }
        foreach (var piece in allPieces)
        {
            piece.GetComponent<Piece>().TurnHighlightOff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject[] GetFieldTiles()
    {
        return _fieldTiles;
    }

    public GameObject[] GetStartTilesByColor(PieceColor color)
    {
        return _startTiles[color];
    }

    public GameObject[] GetEndTilesByColor(PieceColor color)
    {
        return _endTiles[color];
    }
}
