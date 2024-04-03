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


    // Start is called before the first frame update
    void Start()
    {
        FillDictionaries();
        _fieldTiles = GameObject.FindGameObjectsWithTag("Board");
    }

    private void FillDictionaries()
    {
        foreach (var color in (PieceColor[])Enum.GetValues(typeof(PieceColor)))
        {
            _startTiles[color] = GameObject.FindGameObjectsWithTag($"{color}Start");
            _endTiles[color] = GameObject.FindGameObjectsWithTag($"{color}End");
            Debug.Log(_startTiles[color].Length + _endTiles[color].Length);
        }
    }

    public void HighlightPossibleMoves(Piece[] pieces, PieceColor color, int roll)
    {
        foreach (var piece in pieces)
        {
            if (piece.GetCurrentTile() == -1) continue; 
            for (int i = piece.GetCurrentTile(); i < piece.GetCurrentTile() + roll + 1; i++)
            {
                var realIndex = ((int) color * 10 + i) % 40;
                _fieldTiles[realIndex].GetComponent<Image>().color = Color.green;
            }
        }
    }

    public void UnselectHighlightedMoves()
    {
        foreach (var tile in _fieldTiles)
        {
            tile.GetComponent<Image>().color = Color.white;
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
