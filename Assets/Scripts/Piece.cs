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

    [SerializeField] public PieceColor Color;
    [SerializeField] public GameObject StartTile;
    [SerializeField] public GameObject OccupiedHighlight;
    [SerializeField] public GameObject Chosen;


    public int CurrentTile = -1;

    // Start is called before the first frame update
    void Start()
    {
        TurnHighlightOff();
        if (gameObject.name == "BluePiece0")
        {
            GameManager.GamePlan[0] = gameObject;
            CurrentTile = 0;
        } else if (gameObject.name == "RedPiece0")
        {
            CurrentTile = 6;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int GetCurrentTile()
    {
        return CurrentTile;
    }

    public void ReturnHome()
    {
        gameObject.transform.position = StartTile.transform.position;
        CurrentTile = -1;
    }

    public void TurnHighlightOn()
    {
        OccupiedHighlight.SetActive(true);
    }

    public void TurnHighlightOff()
    {
        OccupiedHighlight.SetActive(false);
    }

    public PieceColor GetColor()
    {
        return Color;
    }

    public void AttemptMove()
    {
        TileManager.HighlightPossibleMove(this, Color, GameManager.CurrentRoll);
    }
}
