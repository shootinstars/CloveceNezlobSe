using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum PieceColor
{
    Blue,
    Red,
    Green,
    Yellow
}

public class Piece : MonoBehaviour
{

    [SerializeField] private PieceColor _color;
    [SerializeField] private GameObject _startTile;
    [SerializeField] private GameObject _occupiedHighlight;
    private int _currentTile = -1;

    // Start is called before the first frame update
    void Start()
    {
        TurnHighlightOff();
        if (gameObject.name == "BluePiece0")
        {
            _currentTile = 0;
        } else if (gameObject.name == "RedPiece0")
        {
            _currentTile = 6;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentTile()
    {
        return _currentTile;
    }

    public void ReturnHome()
    {
        gameObject.transform.position = _startTile.transform.position;
        _currentTile = -1;
    }

    public void TurnHighlightOn()
    {
        _occupiedHighlight.SetActive(true);
    }

    public void TurnHighlightOff()
    {
        _occupiedHighlight.SetActive(false);
    }

    public PieceColor GetColor()
    {
        return _color;
    }
}
