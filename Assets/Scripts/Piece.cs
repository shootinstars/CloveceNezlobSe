using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum PieceColor
{
    Blue = 0,
    Red = 1,
    Green = 2,
    Yellow = 3,
}

public class Piece : MonoBehaviour
{

    [SerializeField] public PieceColor Color;
    [SerializeField] public GameObject StartTile;
    [SerializeField] public GameObject OccupiedHighlight;
    [SerializeField] public GameObject Chosen;
    public GameManager GameManager;
    public TileManager TileManager;


    public int CurrentTile = -1;
    public int TilesGone = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        TileManager = GameObject.Find("GameManager").GetComponent<TileManager>();
        TurnHighlightOff();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ReturnHome()
    {
        gameObject.transform.position = StartTile.transform.position;
        GameManager.ActivePiecesCount[Color] -= 1;
        CurrentTile = -1;
    }

    public void TurnHighlightOn()
    {
        OccupiedHighlight.SetActive(true);
        gameObject.GetComponent<Button>().interactable = true;
    }

    public void TurnHighlightOff()
    {
        OccupiedHighlight.SetActive(false);
    }

    public void GetKickedOut()
    {
        if (OccupiedHighlight.activeSelf)
        {
            GameManager.Move(CurrentTile);
        }
    }

    public void AttemptMove()
    {
        if (!OccupiedHighlight.activeSelf)
        {
            Debug.Log(CurrentTile);
            GameManager.ChosenPiece = gameObject;
            TileManager.UnselectHighlightedMoves();
            TileManager.HighlightPossibleMove(this, Color, GameManager.CurrentRoll);
        }
    }
}
