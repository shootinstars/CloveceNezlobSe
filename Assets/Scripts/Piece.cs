using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum Color
{
    Red,
    Green,
    Blue,
    Yellow
}

public class Piece : MonoBehaviour
{

    [SerializeField] private Color _color;
    [SerializeField] private GameObject _currentTile;
    [SerializeField] private GameObject _startTile;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Move(GameObject destination)
    {
        gameObject.transform.position = destination.transform.position;
        _currentTile = destination;
    }
}
