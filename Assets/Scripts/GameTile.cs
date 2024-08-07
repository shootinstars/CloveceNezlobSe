using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTile : MonoBehaviour
{
    public int Id;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TutorialScript tutorial;
    public Color BaseColor = Color.white;

    public void ConfirmMove()
    {
        if (tutorial.TileTutorial.activeSelf)
        {
            tutorial.tutorialPartFinished = true;
            tutorial.TutorialChosenPiece.transform.position = gameObject.transform.position;
            return;
        }
        if (tutorial.TutorialScreen.activeSelf)
        {
            return;
        }
        gameManager.Move(Id);
    }
}
