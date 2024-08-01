using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public bool ShouldRoll;
    private bool tutorialComplete = false;
    public bool tutorialPartFinished;
    private bool gameFinished;
    private bool roundFinished;
    private bool hasToMove;
    private bool isPaused;
    

    private GameObject chosenPiece;
    [SerializeField] private GameObject rollAgain;
    [SerializeField] private GameObject rollButton;
    [SerializeField] private GameObject endTurnButton;
    [SerializeField] private GameObject skipTutorialButton;
    [SerializeField] private GameObject rollWarning;

    [SerializeField] private GameObject diceTutorial;
    [SerializeField] private GameObject endTurnTutorial;

    [SerializeField] private GameObject diceTutorialBackground;
    [SerializeField] private GameObject pieceTutorialBackground;
    [SerializeField] private GameObject tileTutorialBackground;
    [SerializeField] private GameObject endTurnTutorialBackground;
    [SerializeField] private GameObject tileTutorialGreyBackground;
    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private GameObject resultScreen;
    [SerializeField] private Image firstIcon;
    [SerializeField] private Image secondIcon;
    [SerializeField] private Image thirdIcon;
    [SerializeField] private Image fourthIcon;


    public GameObject Tutorial;
    public GameObject PieceTutorial;
    public GameObject TileTutorial;
    public GameObject TutorialChosenPiece;

    [SerializeField] private Image currentPlayerImage;
    

    [SerializeField] private PieceColor currentPlayer;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private DiceControl diceControl;

    private int rollCount;
    private int currentRoll;
    private int lastFinished = 0;

    [SerializeField] private readonly Dictionary<PieceColor, GameObject[]> pieces = new();
    private Dictionary<PieceColor, int> activePiecesCount = new();
    private PieceColor[] finishingOrder = new PieceColor[4];
    private HashSet<PieceColor> finishedColors = new HashSet<PieceColor>();
    public GameObject[] GamePlan { get; } = new GameObject[40];

    // Start is called before the first frame update
    void Start()
    {
        rollAgain.SetActive(false);
        InitializePiecesDict();
        InitializeActiveCount();
        StartCoroutine(PlayGame());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
            pauseScreen.SetActive(isPaused);
        }
    }

    IEnumerator StartDiceTutorial()
    {
        endTurnButton.SetActive(false);
        tutorialPartFinished = false;
        diceTutorial.SetActive(true);
        diceTutorialBackground.SetActive(true);
        yield return new WaitUntil(() => tutorialPartFinished);
    }

    IEnumerator StartPieceTutorial()
    {
        rollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[5];
        ActivateBluePieces();
        diceTutorial.SetActive(false);
        tutorialPartFinished = false;
        PieceTutorial.SetActive(true);
        diceTutorialBackground.SetActive(false);
        pieceTutorialBackground.SetActive(true);
        yield return new WaitUntil(() => tutorialPartFinished);
    }

    IEnumerator StartTileTutorial()
    {
        tileTutorialGreyBackground.SetActive(true);
        tileManager.getFieldTiles()[0].GetComponent<Button>().interactable = true;
        DeactivateBluePieces();
        PieceTutorial.SetActive(false);
        tutorialPartFinished = false;
        TileTutorial.SetActive(true);
        pieceTutorialBackground.SetActive(false);
        tileTutorialBackground.SetActive(true);
        yield return new WaitUntil(() => tutorialPartFinished);
    }

    IEnumerator StartEndTurnTutorial()
    {
        endTurnButton.SetActive(true);
        tileTutorialGreyBackground.SetActive(false);
        tileManager.getFieldTiles()[0].GetComponent<Button>().interactable = false;
        TileTutorial.SetActive(false);
        tutorialPartFinished = false;
        endTurnTutorial.SetActive(true);
        tileTutorialBackground.SetActive(false);
        endTurnTutorialBackground.SetActive(true);
        yield return new WaitUntil(() => tutorialPartFinished);
    }

    public void EndTutorial()
    {
        endTurnTutorialBackground.SetActive(false);
        endTurnTutorial.SetActive(false);
        foreach (var piece in pieces[PieceColor.Blue])
        {
            piece.GetComponent<Piece>().ResetPiece();
        }
        ChangeToDefaultButton();
        HideTutorial();
        endTurnButton.SetActive(true);
    }

    public void HideTutorial()
    {
        diceTutorial.SetActive(false);
        PieceTutorial.SetActive(false);
        TileTutorial.SetActive(false);
        Tutorial.SetActive(false);
        diceTutorialBackground.SetActive(false);
        tileTutorialBackground.SetActive(false);
        pieceTutorialBackground.SetActive(false);
        endTurnTutorialBackground.SetActive(false);
        skipTutorialButton.SetActive(false);
    }

    public void ActivateBluePieces()
    {
        foreach (var piece in pieces[PieceColor.Blue])
        {
            piece.GetComponent<Button>().interactable = true;
        }
    }

    public void DeactivateBluePieces()
    {
        foreach (var piece in pieces[PieceColor.Blue])
        {
            piece.GetComponent<Button>().interactable = false;
        }
    }

    private void InitializePiecesDict()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            pieces[color] = GameObject.FindGameObjectsWithTag($"{color}Piece");
        }
    }

    private Boolean PlayerFinished(PieceColor color)
    {
        foreach (var pieceObject in pieces[color])
        {
            if (!pieceObject.GetComponent<Piece>().hasFinished)
            {
                return false;
            }
        }
        return true;
    }

    public void TurnPiecesOn()
    {
        foreach (var piece in pieces[currentPlayer])
        {
            piece.GetComponent<Button>().interactable = true;
        }
    }

    private void TurnPiecesOff()
    {
        foreach (var piece in pieces[currentPlayer])
        {
            piece.GetComponent<Button>().interactable = false;
            piece.GetComponent<Piece>().Chosen.SetActive(false);
        }
    }

    IEnumerator EndRoundCoroutine(PieceColor color)
    {
        rollButton.GetComponent<Button>().interactable = false;
        hasToMove = false;
        if (ShouldRoll)
        {
            yield return new WaitForSeconds(1f);
        }
        else
        {
            yield return new WaitForSeconds(1.5f);
        }
        TurnPiecesOff();
        ChangeToDefaultButton();
        roundFinished = true;
    }

    public void EndRound()
    {
        if (endTurnTutorial.activeSelf || Tutorial.activeSelf)
        {
            tutorialPartFinished = true;
            tutorialComplete = true;
            return;
        }
        StartCoroutine(EndRoundCoroutine(currentPlayer));
    }

    public void InitializeActiveCount()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            activePiecesCount.Add(color, 0);
        }
    }

    public void SetChosenPiece(GameObject piece)
    {
        chosenPiece = piece;
    }

    public void TurnOffRollWarning()
    {
        rollWarning.SetActive(false);
    }

    public int getCurrentRoll()
    {
        return currentRoll;
    }

    public void DecreaseActivePieces(PieceColor color)
    {
        activePiecesCount[color] -= 1;
    }

    IEnumerator PlayGame()
    {
        while (!tutorialComplete)
        {
            yield return StartDiceTutorial();
            if (!tutorialComplete)
            {
                yield return StartPieceTutorial();
            }
            if (!tutorialComplete)
            {
                yield return StartTileTutorial();
            }
            if (!tutorialComplete)
            {
                yield return StartEndTurnTutorial();
            }
            tutorialComplete = true;
        }
        EndTutorial();
        while (!gameFinished)
        {
            yield return PlayRound(PieceColor.Blue);
            yield return PlayRound(PieceColor.Red);
            yield return PlayRound(PieceColor.Green);
            yield return PlayRound(PieceColor.Yellow);
        }
        EndGame();
    }

    IEnumerator PlayRound(PieceColor color)
    {
        CheckForNewFinisher();
        roundFinished = false;
        if (allPiecesFinished())
        {
            gameFinished = true;
            roundFinished = true;
        }
        currentPlayerImage.color = tileManager.EndTiles[color][0].GetComponent<Image>().color;
        rollWarning.SetActive(false);
        ShouldRoll = true;
        rollAgain.SetActive(false);
        rollButton.GetComponent<Button>().interactable = true;
        endTurnButton.GetComponent<Button>().interactable = true;
        currentPlayer = color;
        rollCount = 0;
        if (PlayerFinished(color))
        {
            TurnPiecesOff();
            roundFinished = true;
        }
        yield return new WaitUntil(() => roundFinished);
    }

    public bool allPiecesFinished()
    {
        foreach (var piece in GetAllPieces())
        {
            if (!piece.GetComponent<Piece>().hasFinished)
            {
                return false;
            }
        }
        return true;
    }

    public void ChangeButton()
    {
        rollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[currentRoll - 1];
    }

    public void ChangeToDefaultButton()
    {
        rollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[6];
    }

    public GameObject[] GetAllPieces()
    {
        return pieces.Values.SelectMany(array => array).ToArray();
    }

    public void UpdateInfoBeforeMove(Piece piece)
    {
        if (piece.CurrentTile == -1)
        {
            activePiecesCount[piece.Color] += 1;
            piece.TilesGone = 1;
        }
        else if (piece.TilesGone > 40)
        {
            tileManager.EndFields[piece.Color][piece.TilesGone % 5 - 1] = null;
            piece.TilesGone += currentRoll;
        }
        else
        {
            GamePlan[piece.CurrentTile] = null;
            piece.TilesGone += currentRoll;
        }
    }

    public void HandleRollButton()
    {
        var limit = activePiecesCount[currentPlayer] == 0 ? 3 : 1;
        if ((currentRoll != 6 && rollCount >= limit && limit != 3) || currentRoll == 6)
        {
            hasToMove = true;
        }
    }

    public void Roll()
    {
        if (diceTutorial.activeSelf)
        {
            tutorialPartFinished = true;
            return;
        }
        if (Tutorial.activeSelf) return;
        if (hasToMove)
        {
            rollWarning.SetActive(true);
            return;
        }
        if (ShouldRoll)
        {
            StartCoroutine(diceControl.RollAnimation());
            ShouldRoll = false;
            var roll = Random.Range(1, 7);
            rollAgain.SetActive(false);
            rollCount++;
            endTurnButton.GetComponent<Button>().interactable = false;
            var limit = activePiecesCount[currentPlayer] == 0 ? 3 : 1;
            currentRoll = roll;
            if (roll != 6 && rollCount >= limit)
            {
                if (limit == 3)
                {
                    EndRound();
                }
                else
                {
                    endTurnButton.GetComponent<Button>().interactable = !CanMove(currentPlayer);
                }
            }

            if (roll == 6)
            {
                endTurnButton.GetComponent<Button>().interactable = !CanMove(currentPlayer);
            }

            if (limit == 3 && rollCount < 3 && roll != 6)
            {
                endTurnButton.GetComponent<Button>().interactable = !CanMove(currentPlayer);
                rollAgain.SetActive(true);
            }
        }
    }

    private Boolean HasColorFinished(PieceColor color)
    {
        foreach (var piece in pieces[color])
        {
            if (!piece.GetComponent<Piece>().hasFinished)
            {
                return false;
            }
        }
        return true;
    }

    private void CheckForNewFinisher()
    {
        foreach (var color in (PieceColor[])Enum.GetValues(typeof(PieceColor)))
        {
            if (!finishedColors.Contains(color) && HasColorFinished(color))
            {
                finishedColors.Add(color);
                finishingOrder[lastFinished] = color;
                lastFinished++;
            }
        }
    }

    private Boolean CanMove(PieceColor color)
    {
        foreach (var pieceObject in pieces[color])
        {
            Piece piece = pieceObject.GetComponent<Piece>();
            if (piece.CurrentTile != -1 && piece.TilesGone + currentRoll <= 44)
            {
                if (piece.TilesGone + currentRoll < 41 && (GamePlan[(piece.CurrentTile + currentRoll) % 40] == null
                                                           || GamePlan[(piece.CurrentTile + currentRoll) % 40].GetComponent<Piece>().Color != color))
                {
                    return true;
                }

                if (piece.TilesGone + currentRoll > 40 &&
                    tileManager.EndFields[color][(piece.TilesGone + currentRoll) % 5 - 1] == null)
                {
                    return true;
                }
            }

            if (piece.CurrentTile == -1
                && currentRoll == 6
                && (GamePlan[(int)color * 10] == null || GamePlan[(int)color * 10].GetComponent<Piece>().Color != color))
            {
                return true;
            }

            if (activePiecesCount[color] == 0)
            {
                return true;
            }
        }
        return false;
    }

    public void Move(int newPosition)
    {
        hasToMove = false;
        rollWarning.SetActive(false);
        var pieceComp = chosenPiece.GetComponent<Piece>();
        TurnPiecesOff();
        UpdateInfoBeforeMove(pieceComp);
        soundManager.PlayMoveSound();
        if (pieceComp.TilesGone > 40)
        {
            MoveToEnd(pieceComp, newPosition);
        }
        else
        {
            MoveOnBoard(pieceComp, newPosition);
        }
        tileManager.UnselectHighlightedMoves();
        if (currentRoll != 6 && (activePiecesCount[pieceComp.Color] != 0))
        {
            EndRound();
        } else
        {
            rollButton.GetComponent<Button>().interactable = true;
            rollAgain.SetActive(true);
        }
    }

    public void MoveOnBoard(Piece piece, int newPosition)
    {
        piece.CurrentTile = newPosition;
        if (GamePlan[newPosition] != null)
        {
            soundManager.PlayScreamSound();
            GamePlan[newPosition].GetComponent<Piece>().ReturnHome();
        }
        chosenPiece.transform.position = tileManager.getFieldTiles()[newPosition].transform.position;
        GamePlan[newPosition] = chosenPiece;
    }

    public void MoveToEnd(Piece piece, int endPosition)
    {
        piece.hasFinished = true;
        piece.CurrentTile = piece.TilesGone + endPosition;
        tileManager.EndFields[piece.Color][endPosition] = chosenPiece;
        chosenPiece.transform.position = tileManager.EndTiles[piece.Color][endPosition].transform.position;
    }

    public void EndGame()
    {
        resultScreen.SetActive(true);
        firstIcon.sprite = pieces[finishingOrder[0]][0].GetComponent<Image>().sprite;
        firstIcon.color = pieces[finishingOrder[0]][0].GetComponent<Image>().color;
        secondIcon.color = pieces[finishingOrder[1]][0].GetComponent<Image>().color;
        secondIcon.sprite = pieces[finishingOrder[1]][0].GetComponent<Image>().sprite;
        thirdIcon.sprite = pieces[finishingOrder[2]][0].GetComponent<Image>().sprite;
        thirdIcon.color = pieces[finishingOrder[2]][0].GetComponent<Image>().color;
        fourthIcon.sprite = pieces[finishingOrder[3]][0].GetComponent<Image>().sprite;
        fourthIcon.color = pieces[finishingOrder[3]][0].GetComponent<Image>().color;
    }
}