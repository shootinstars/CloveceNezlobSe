using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public int Id;
    [SerializeField] private GameManager gameManager;
    public Color BaseColor = Color.white;

    public void ConfirmMove()
    {
    gameManager.Move(Id);
    }
}
