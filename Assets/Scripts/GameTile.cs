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
        if (gameManager.TileTutorial.activeSelf)
        {
            gameManager.tutorialPartFinished = true;
            gameManager.TutorialChosenPiece.transform.position = gameObject.transform.position;
            return;
        }
        if (gameManager.Tutorial.activeSelf)
        {
            return;
        }
        gameManager.Move(Id);
    }
}
