using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public int Id;
    public GameManager GameManager;
    public Color BaseColor = Color.white;

    // Start is called before the first frame update
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void ConfirmMove()
    {
    GameManager.Move(Id);
    }
}
