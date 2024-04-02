using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    private bool _gameFinished = false;
    private bool _roundFinished = false;
    private TileManager _tileManager;
    private Dictionary<Color, GameObject[]> _pieces = new Dictionary<Color, GameObject[]>();

    // Start is called before the first frame update
    void Start()
    {
        
        _tileManager = gameObject.GetComponent<TileManager>();
        InitializePiecesDict();
        StartCoroutine(PlayGame());
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Roll()
    {
        var roll = Random.Range(1, 7);
        Debug.Log(roll);
        if (roll == 6)
        {
            _roundFinished = true;
        }
    }

    private void InitializePiecesDict()
    {
        foreach (Color color in (Color[]) Enum.GetValues(typeof(Color)))
        {
            _pieces[color] = GameObject.FindGameObjectsWithTag($"{color}Piece");
            Debug.Log(_pieces[color].Length);
        }
    }

    IEnumerator PlayGame()
    {
        while (!_gameFinished)
        {
            yield return PlayRound(Color.Blue);
            yield return PlayRound(Color.Red);
            yield return PlayRound(Color.Green);
            yield return PlayRound(Color.Yellow);
        }
    }

    IEnumerator PlayRound(Color color)
    {
        _roundFinished = false;
        Debug.Log(color + " player is currently on turn");
        yield return new WaitUntil(() => _roundFinished);
    }

}