using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{

    [SerializeField] private GameObject[] _fieldTiles;
    [SerializeField] private Dictionary<Color, GameObject[]> _startTiles = new Dictionary<Color, GameObject[]>();
    [SerializeField] private Dictionary<Color, GameObject[]> _endTiles = new Dictionary<Color, GameObject[]>();


    // Start is called before the first frame update
    void Start()
    {
        FillDictionaries();
        _fieldTiles = GameObject.FindGameObjectsWithTag("Board");
    }

    private void FillDictionaries()
    {
        foreach (Color color in (Color[])Enum.GetValues(typeof(Color)))
        {
            _startTiles[color] = GameObject.FindGameObjectsWithTag($"{color}Start");
            _endTiles[color] = GameObject.FindGameObjectsWithTag($"{color}End");
            Debug.Log(_startTiles[color].Length + _endTiles[color].Length);
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

    public GameObject[] GetStartTilesByColor(Color color)
    {
        return _startTiles[color];
    }

    public GameObject[] GetEndTilesByColor(Color color)
    {
        return _endTiles[color];
    }
}
