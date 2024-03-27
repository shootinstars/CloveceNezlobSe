using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] Color _color;
    [SerializeField] private Bubble _currBubble;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
