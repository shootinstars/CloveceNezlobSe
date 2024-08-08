using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public bool ShouldRoll;
    public bool gameFinished;
    private bool roundFinished;
    private bool hasToMove;
    private bool isPaused;
    

    private GameObject chosenPiece;
    [SerializeField] private GameObject rollAgain;
    [SerializeField] private GameObject rollWarning;

    [SerializeField] private GameObject pauseScreen;

    [SerializeField] private GameObject resultScreen;
    [SerializeField] private Image firstIcon;
    [SerializeField] private Image secondIcon;
    [SerializeField] private Image thirdIcon;
    [SerializeField] private Image fourthIcon;


    public GameObject RollButton;

    public Image CurrentPlayerImage;

    public PieceColor CurrentPlayer;

    [SerializeField] private TileManager tileManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private DiceControl diceControl;
    [SerializeField] private ComputerPlayer computerPlayer;
    [SerializeField] private TutorialScript tutorial;

    private int rollCount;
    public int CurrentRoll;
    private int lastFinished;

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
        foreach (var piece in Pieces[CurrentPlayer])
        {

            piece.GetComponent<Button>().interactable = !piece.GetComponent<Piece>().hasFinished;
        }
    }

    private void TurnPiecesOff()
    {
        foreach (var piece in Pieces[CurrentPlayer])
        {
            piece.GetComponent<Button>().interactable = false;
            piece.GetComponent<Piece>().Chosen.SetActive(false);
        }
    }

    public IEnumerator EndRoundCoroutine(PieceColor color)
    {
        RollButton.GetComponent<Button>().interactable = false;
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
        if (tutorial.TutorialScreen.activeSelf)
        {
            tutorial.tutorialPartFinished = true;
            tutorial.tutorialComplete = true;
            return;
        }
        StartCoroutine(EndRoundCoroutine(CurrentPlayer));
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

    public void DecreaseActivePieces(PieceColor color)
    {
        ActivePiecesCount[color] -= 1;
    }

    IEnumerator PlayGame()
    {

        var humanPlayerCount = FindObjectOfType<PlayerCount>().Count;
        while (!tutorial.tutorialComplete)
        {
            yield return tutorial.StartDiceTutorial();
            if (!tutorial.tutorialComplete)
            {
                yield return tutorial.StartPieceTutorial();
            }
            if (!tutorial.tutorialComplete)
            {
                yield return tutorial.StartTileTutorial();
            }
            tutorial.tutorialComplete = true;
        }
        tutorial.EndTutorial();
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
            RollButton.GetComponent<Button>().interactable = true;
            CurrentPlayer = color;
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
        RollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[roll - 1];
    }

    public void ChangeToDefaultButton()
    {
        RollButton.GetComponent<Button>().image.sprite = diceControl.DiceSprites[6];
    }

    public GameObject[] GetAllPieces()
    {
        return Pieces.Values.SelectMany(array => array).ToArray();
    }

    public void HandleRollButton()
    {
        var limit = ActivePiecesCount[CurrentPlayer] == 0 ? 3 : 1;
        if (CanMove(CurrentPlayer, false) && ((CurrentRoll != 6 && rollCount >= limit && limit != 3) || CurrentRoll == 6))
        {
            hasToMove = true;
        }
    }

    public void Roll()
    {
        if (tutorial.DiceTutorial.activeSelf)
        {
            tutorial.tutorialPartFinished = true;
            return;
        }
        if (tutorial.TutorialScreen.activeSelf) return;
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
            var limit = ActivePiecesCount[CurrentPlayer] == 0 ? 3 : 1;
            CurrentRoll = roll;
            if (roll != 6 && rollCount >= limit)
            {
                if (limit == 3)
                {
                    EndRound();
                }
                else if (!CanMove(CurrentPlayer, false) && !HasNotStarted(CurrentPlayer))
                {
                    EndRound();
                }
            }

            if (roll == 6 && !CanMove(CurrentPlayer, false))
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
        var roll = CurrentRoll;
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
        }
        return false;
    }

    public bool HasNotStarted(PieceColor color)
    {
        return ActivePiecesCount[color] == 0;
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
            piece.TilesGone += CurrentRoll;
        }
        else
        {
            GamePlan[piece.CurrentTile] = null;
            piece.TilesGone += CurrentRoll;
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
        if ((CurrentRoll != 6 && (ActivePiecesCount[pieceComp.Color] != 0)) || GetNumberOfFinished(CurrentPlayer) == 4)
        {
            EndRound();
        } else
        {
            RollButton.GetComponent<Button>().interactable = true;
            rollAgain.SetActive(true);
        }
    }

    public void MoveOnBoard(Piece piece, int newPosition)
    {
        if (piece.CurrentTile != -1 || CurrentRoll == 6)
        {
            piece.CurrentTile = newPosition;
            if (GamePlan[newPosition] != null)
            {
                soundManager.PlayScreamSound();
                GamePlan[newPosition].GetComponent<Piece>().ReturnHome();
            }
            piece.gameObject.transform.position = tileManager.getFieldTiles()[newPosition].transform.position;
            GamePlan[newPosition] = piece.gameObject;
        }
    }

    public void MoveToEnd(Piece piece, int endPosition)
    {
        if (endPosition >= 0 && endPosition <= 3)
        {
            piece.hasFinished = true;
            piece.CurrentTile = piece.TilesGone + endPosition;
            tileManager.EndFields[piece.Color][endPosition] = chosenPiece;
            piece.gameObject.transform.position = tileManager.EndTiles[piece.Color][endPosition].transform.position;
        }
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
}