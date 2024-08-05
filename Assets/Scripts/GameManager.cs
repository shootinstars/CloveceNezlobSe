using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public bool ShouldRoll;
    public bool tutorialPartFinished;
    private bool tutorialComplete;
    public bool gameFinished;
    private bool roundFinished;
    private bool hasToMove;
    private bool isPaused;
    

    private GameObject chosenPiece;
    [SerializeField] private GameObject rollAgain;
    [SerializeField] private GameObject rollButton;
    [SerializeField] private GameObject skipTutorialButton;
    [SerializeField] private GameObject rollWarning;

    [SerializeField] private GameObject diceTutorial;

    [SerializeField] private GameObject diceTutorialBackground;
    [SerializeField] private GameObject pieceTutorialBackground;
    [SerializeField] private GameObject tileTutorialBackground;
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

    public Image CurrentPlayerImage;
    

    [SerializeField] private PieceColor currentPlayer;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private DiceControl diceControl;
    [SerializeField] private ComputerPlayer computerPlayer;

    private int rollCount;
    private int currentRoll;
    private int lastFinished = 0;

    public Dictionary<PieceColor, GameObject[]> Pieces { get; } = new();
    public Dictionary<PieceColor, int> ActivePiecesCount = new();
    private PieceColor[] finishingOrder = new PieceColor[4];
    private HashSet<PieceColor> finishedColors = new HashSet<PieceColor>();
    public GameObject[] GamePlan { get; } = new GameObject[TileManager.FieldSize];

    // Start is called before the first frame update
    void Start()
    {
        rollAgain.SetActive(false);
        InitializePiecesDict();
        InitializeActiveCount();
        computerPlayer = FindObjectOfType<ComputerPlayer>();
        StartCoroutine(PlayGame());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !computerPlayer.AfterPlayerWin.activeSelf && !resultScreen.activeSelf)
        {
            if (!isPaused)
            {
                Time.timeScale = 0f;
                pauseScreen.SetActive(true);
                isPaused = true;
            }
            else
            {
                Time.timeScale = 1f;
                pauseScreen.SetActive(false);
                isPaused = false;
            }
        }
    }

    IEnumerator StartDiceTutorial()
    {
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

    public void EndTutorial()
    {
        tileManager.getFieldTiles()[0].GetComponent<Button>().interactable = false;
        tileTutorialGreyBackground.SetActive(false);
        foreach (var piece in Pieces[PieceColor.Blue])
        {
            piece.GetComponent<Piece>().ResetPiece();
        }
        ChangeToDefaultButton();
        HideTutorial();
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
        skipTutorialButton.SetActive(false);
    }

    public void ActivateBluePieces()
    {
        foreach (var piece in Pieces[PieceColor.Blue])
        {
            piece.GetComponent<Button>().interactable = true;
        }
    }

    public void DeactivateBluePieces()
    {
        foreach (var piece in Pieces[PieceColor.Blue])
        {
            piece.GetComponent<Button>().interactable = false;
        }
    }

    private void InitializePiecesDict()
    {
        foreach (var color in (PieceColor[]) Enum.GetValues(typeof(PieceColor)))
        {
            Pieces[color] = GameObject.FindGameObjectsWithTag($"{color}Piece");
        }
    }

    public void TurnPiecesOn()
    {
        foreach (var piece in Pieces[currentPlayer])
        {

            piece.GetComponent<Button>().interactable = !piece.GetComponent<Piece>().hasFinished;
        }
    }

    private void TurnPiecesOff()
    {
        foreach (var piece in Pieces[currentPlayer])
        {
            piece.GetComponent<Button>().interactable = false;
            piece.GetComponent<Piece>().Chosen.SetActive(false);
        }
    }

    public IEnumerator EndRoundCoroutine(PieceColor color)
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
        computerPlayer.computerRoundFinished = true;
        roundFinished = true;
    }

    public void EndRound()
    {
        if (Tutorial.activeSelf)
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
            ActivePiecesCount.Add(color, 0);
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
        ActivePiecesCount[color] -= 1;
    }

    IEnumerator PlayGame()
    {

        var humanPlayerCount = FindObjectOfType<PlayerCount>().Count;
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
            tutorialComplete = true;
        }
        EndTutorial();
        if (!computerPlayer.isSoloGame)
        {
            while (!gameFinished)
            {
                switch (humanPlayerCount)
                {
                    case 2:
                        yield return PlayRound(PieceColor.Blue);
                        yield return PlayRound(PieceColor.Red);
                        yield return computerPlayer.PlayComputerRound(PieceColor.Green);
                        yield return computerPlayer.PlayComputerRound(PieceColor.Yellow);
                        break;
                    case 3:
                        yield return PlayRound(PieceColor.Blue);
                        yield return PlayRound(PieceColor.Red);
                        yield return PlayRound(PieceColor.Green);
                        yield return computerPlayer.PlayComputerRound(PieceColor.Yellow);
                        break;
                    default:
                        yield return PlayRound(PieceColor.Blue);
                        yield return PlayRound(PieceColor.Red);
                        yield return PlayRound(PieceColor.Green);
                        yield return PlayRound(PieceColor.Yellow);
                        break;
                }
            }
        }
        else
        {
            while (!gameFinished)
            {
                yield return PlayRound(PieceColor.Blue);
                yield return computerPlayer.PlayComputerRound(PieceColor.Red);
                yield return computerPlayer.PlayComputerRound(PieceColor.Green);
                yield return computerPlayer.PlayComputerRound(PieceColor.Yellow);
            }
        }
        EndGame();
    }

    IEnumerator PlayRound(PieceColor color)
    {
        CheckForNewFinisher();
        roundFinished = false;
        if (AllPiecesFinished())
        {
            gameFinished = true;
            roundFinished = true;
        }
        if (GetNumberOfFinished(color) == 4)
        {
            TurnPiecesOff();
            roundFinished = true;
        }

        if (!roundFinished)
        {
            CurrentPlayerImage.color = tileManager.EndTiles[color][0].GetComponent<Image>().color;
            rollWarning.SetActive(false);
            ShouldRoll = true;
            rollAgain.SetActive(false);
            rollButton.GetComponent<Button>().interactable = true;
            currentPlayer = color;
            rollCount = 0;
        }
        yield return new WaitUntil(() => roundFinished);
    }

    public bool AllPiecesFinished()
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

    public int GetNumberOfFinished(PieceColor color)
    {
        var finished = 0;
        foreach (var piece in Pieces[color])
        {
            if (piece.GetComponent<Piece>().hasFinished)
            {
                finished++;
            }
        }

        return finished;
    }

    public void ChangeButton(int roll)
    {
        rollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[roll - 1];
    }

    public void ChangeToDefaultButton()
    {
        rollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[6];
    }

    public GameObject[] GetAllPieces()
    {
        return Pieces.Values.SelectMany(array => array).ToArray();
    }

    public void HandleRollButton()
    {
        var limit = ActivePiecesCount[currentPlayer] == 0 ? 3 : 1;
        if (CanMove(currentPlayer, false) && ((currentRoll != 6 && rollCount >= limit && limit != 3) || currentRoll == 6))
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
            StartCoroutine(diceControl.RollAnimation(false));
            ShouldRoll = false;
            var roll = Random.Range(1, 7);
            rollAgain.SetActive(false);
            rollCount++;
            var limit = ActivePiecesCount[currentPlayer] == 0 ? 3 : 1;
            currentRoll = roll;
            if (roll != 6 && rollCount >= limit)
            {
                if (limit == 3)
                {
                    EndRound();
                }
                else if (!CanMove(currentPlayer, false))
                {
                    EndRound();
                }
            }

            if (roll == 6 && !CanMove(currentPlayer, false))
            {
                EndRound(); 

            }

            if (limit == 3 && rollCount < 3 && roll != 6)
            {
                rollAgain.SetActive(true);
            }
        }
    }

    private Boolean HasColorFinished(PieceColor color)
    {
        foreach (var piece in Pieces[color])
        {
            if (!piece.GetComponent<Piece>().hasFinished)
            {
                return false;
            }
        }
        return true;
    }

    public void CheckForNewFinisher()
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

    public Boolean CanMove(PieceColor color, bool isComputer)
    {
        var roll = isComputer ? computerPlayer.getComputerRoll() : currentRoll;
        foreach (var pieceObject in Pieces[color])
        {
            Piece piece = pieceObject.GetComponent<Piece>();
            if (piece.CurrentTile != -1 && piece.TilesGone + roll <= 44)
            {
                if (piece.TilesGone + roll <= TileManager.FieldSize && (GamePlan[(piece.CurrentTile + roll) % TileManager.FieldSize] == null
                                                           || GamePlan[(piece.CurrentTile + roll) % TileManager.FieldSize].GetComponent<Piece>().Color != color))
                {
                    return true;
                }

                if (piece.TilesGone + roll > TileManager.FieldSize &&
                    tileManager.EndFields[color][(piece.TilesGone + roll) % 5 - 1] == null && !piece.hasFinished)
                {
                    return true;
                }
            }

            if (piece.CurrentTile == -1
                && roll == 6
                && (GamePlan[(int)color * 10] == null || GamePlan[(int)color * 10].GetComponent<Piece>().Color != color))
            {
                return true;
            }

            if (ActivePiecesCount[color] == 0)
            {
                return true;
            }
        }
        return false;
    }

    public void UpdateInfoBeforeMove(Piece piece)
    {
        if (piece.CurrentTile == -1)
        {
            ActivePiecesCount[piece.Color] += 1;
            piece.TilesGone = 1;
        }
        else if (piece.TilesGone > TileManager.FieldSize)
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

    public void Move(int newPosition)
    {
        hasToMove = false;
        rollWarning.SetActive(false);
        var pieceComp = chosenPiece.GetComponent<Piece>();
        TurnPiecesOff();
        UpdateInfoBeforeMove(pieceComp);
        soundManager.PlayMoveSound();
        if (pieceComp.TilesGone > TileManager.FieldSize)
        {
            MoveToEnd(pieceComp, newPosition);
        }
        else
        {
            MoveOnBoard(pieceComp, newPosition);
        }
        tileManager.UnselectHighlightedMoves();
        if (currentRoll != 6 && (ActivePiecesCount[pieceComp.Color] != 0))
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
        piece.gameObject.transform.position = tileManager.getFieldTiles()[newPosition].transform.position;
        GamePlan[newPosition] = chosenPiece;
    }

    public void MoveToEnd(Piece piece, int endPosition)
    {
        piece.hasFinished = true;
        piece.CurrentTile = piece.TilesGone + endPosition;
        tileManager.EndFields[piece.Color][endPosition] = chosenPiece;
        piece.gameObject.transform.position = tileManager.EndTiles[piece.Color][endPosition].transform.position;
    }

    public void EndGame()
    {
        soundManager.PlayFanfareSound();
        resultScreen.SetActive(true);
        computerPlayer.isWaitingForPlayerDecision = true;
        firstIcon.sprite = Pieces[finishingOrder[0]][0].GetComponent<Image>().sprite;
        firstIcon.color = Pieces[finishingOrder[0]][0].GetComponent<Image>().color;
        secondIcon.color = Pieces[finishingOrder[1]][0].GetComponent<Image>().color;
        secondIcon.sprite = Pieces[finishingOrder[1]][0].GetComponent<Image>().sprite;
        thirdIcon.sprite = Pieces[finishingOrder[2]][0].GetComponent<Image>().sprite;
        thirdIcon.color = Pieces[finishingOrder[2]][0].GetComponent<Image>().color;
        fourthIcon.sprite = Pieces[finishingOrder[3]][0].GetComponent<Image>().sprite;
        fourthIcon.color = Pieces[finishingOrder[3]][0].GetComponent<Image>().color;
    }

    public void HideWarning()
    {
        rollWarning.SetActive(false);
    }

    public PieceColor getCurrentPlayer()
    {
        return currentPlayer;
    }
}