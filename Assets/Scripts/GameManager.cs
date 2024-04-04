using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private bool _gameFinished = false;
    private bool _roundFinished = false;
    private TileManager _tileManager;
    private Dictionary<PieceColor, GameObject[]> _pieces = new Dictionary<PieceColor, GameObject[]>();
    private GameObject[] _allPieces;
    private PieceColor _currentPlayer;

    // Start is called before the first frame update
    void Start()
    {
        _tileManager = gameObject.GetComponent<TileManager>();
        InitializePiecesDict();
        _allPieces = _pieces.Values.SelectMany(array => array).ToArray();
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
        _tileManager.HighlightPossibleMoves(_pieces[_currentPlayer], _currentPlayer, roll);
        if (roll == 6)
        {
            _roundFinished = true;
        }
    }

    private void InitializePiecesDict()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            _pieces[color] = GameObject.FindGameObjectsWithTag($"{color}Piece");
            Debug.Log(_pieces[color].Length);
        }
    }

    IEnumerator PlayGame()
    {
        while (!_gameFinished)
        {
            yield return PlayRound(PieceColor.Blue);
            yield return PlayRound(PieceColor.Red);
            yield return PlayRound(PieceColor.Green);
            yield return PlayRound(PieceColor.Yellow);
        }
    }

    IEnumerator PlayRound(PieceColor color)
    {
        _roundFinished = false;
        _currentPlayer = color;
        Debug.Log(color + " player is currently on turn");

        yield return new WaitUntil(() => _roundFinished);
    }

    public GameObject[] GetAllPieces()
    {
        return _allPieces;
    }

}