using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ComputerPlayer : MonoBehaviour
{
    private bool canMove;

    public bool computerRoundFinished;
    public bool isWaitingForPlayerDecision;
    public bool playerFinished;
    public bool isSoloGame;

    private int currentStart;
    private int rollLimit;

    public GameObject AfterPlayerWin;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private DiceControl diceControl;
    [SerializeField] private Button rollButton;
    [SerializeField] private PlayerCount playerCount;

    private Piece bestOption;

    void Awake()
    {
        Time.timeScale = 1;
        playerCount = FindObjectOfType<PlayerCount>();
    }

    public IEnumerator PlayComputerRound(PieceColor currentPlayer)
    {
        CheckForHumanFinish();
        gameManager.TurnOffRollWarning();
        rollButton.interactable = false;
        computerRoundFinished = false;
        yield return new WaitForSeconds(0.5f);
        gameManager.CheckForNewFinisher();
        CheckForGameEnd();
        CheckForComputerRoundEnd(currentPlayer);
        if (gameManager.GetNumberOfFinished(currentPlayer) == 4)
        {
            computerRoundFinished = true;
        }
        gameManager.CurrentPlayerImage.color = tileManager.EndTiles[currentPlayer][0].GetComponent<Image>().color;
        currentStart = (int)currentPlayer * 10;
        rollLimit = gameManager.ActivePiecesCount[currentPlayer] == 0 ? 3 : 1;
        while (!computerRoundFinished)
        {
            yield return PerformComputerAction(currentPlayer);
        }
    }

    IEnumerator PerformComputerAction(PieceColor currentPlayer)
    {
        canMove = false;
        if (gameManager.ActivePiecesCount[currentPlayer] != 0)
        {
            rollLimit = 1;
        }
        yield return new WaitForSeconds(1f);
        gameManager.CurrentRoll = Random.Range(1, 7);
        yield return diceControl.RollAnimation(true);
        if (gameManager.CanMove(currentPlayer, true))
        {
            canMove = true;
            yield return new WaitForSeconds(0.5f);
            var movedPiece = FindBestMove(currentPlayer);
            var newPosition = GetNewPosition(movedPiece);
            ComputerMove(movedPiece, newPosition);
        }
        rollLimit -= 1;
        if (rollLimit == 0)
        {
            if (gameManager.CurrentRoll == 6 && canMove && gameManager.GetNumberOfFinished(currentPlayer) != 4)
            {
                rollLimit = 1;
            }
            else
            {
                computerRoundFinished = true;
                yield return gameManager.EndRoundCoroutine(currentPlayer);
            }
        }
    }

    public void CheckForHumanFinish()
    {
        if (gameManager.GetNumberOfFinished(PieceColor.Blue) == 4 && !playerFinished &&
            !gameManager.AllPiecesFinished())
        {
            if (isSoloGame || (!isSoloGame && playerCount.Count == 2 &&
                               gameManager.GetNumberOfFinished(PieceColor.Red) == 4)
                           || (!isSoloGame && playerCount.Count == 3 &&
                               gameManager.GetNumberOfFinished(PieceColor.Red) == 4) &&
                           gameManager.GetNumberOfFinished(PieceColor.Green) == 4)
            {
                isWaitingForPlayerDecision = true;
                AfterPlayerWin.SetActive(true);
                Time.timeScale = 0f;
                playerFinished = true;
                soundManager.PlayFanfareSound();
            }
        }
    }

    public void CheckForGameEnd()
    {
        if (gameManager.AllPiecesFinished())
        {
            gameManager.gameFinished = true;
            computerRoundFinished = true;
        }
    }

    public void CheckForComputerRoundEnd(PieceColor currentPlayer)
    {
        if (gameManager.GetNumberOfFinished(currentPlayer) == 4)
        {
            computerRoundFinished = true;
        }
    }

    public Piece FindBestMove(PieceColor color)
    {
        bestOption = null;
        foreach (var piece in gameManager.Pieces[color])
        {
            Piece pieceComp = piece.GetComponent<Piece>();
            if (bestOption == null)
            {
                bestOption = pieceComp;
            }
            //finishing
            CheckFinish(pieceComp);
            //attacking
            CheckAttack(pieceComp);
            //starting
            CheckStart(pieceComp);
            //moving
            CheckMove(pieceComp);
            if (!CanFinish(bestOption) && !CanAttack(bestOption) && !CanStart(bestOption) &&
                     !CanOnlyMove(bestOption))
            {
                bestOption = pieceComp;
            }
        }
        return bestOption;
    }

    public void CheckFinish(Piece piece)
    {
        if (CanFinish(piece) && (!CanStart(bestOption)) || (CanFinish(piece) && EnemyInProximity(piece)))
        {
            bestOption = piece;
        }
    }

    public void CheckAttack(Piece piece)
    {
        if (CanAttack(piece))
        {
            //comparing attack value
            if (CanAttack(bestOption))
            {
                bestOption = CompareAttacks(piece, bestOption);
            }
            // there is no more valuable move now
            else if (!CanFinish(bestOption) || CanStart(piece))
            {
                bestOption = piece;
            }
        }
    }

    public void CheckStart(Piece piece)
    {
        if (CanStart(piece))
        {
            //it doesn't matter which piece is summoned to the board
            if (CanStart(bestOption))
            {
                bestOption = piece;
            }
            else if (!CanFinish(bestOption) && !CanAttack(bestOption))
            {
                if (CanOnlyMove(bestOption))
                {
                    bestOption = CompareStartAndMove(bestOption, piece);
                }
                else
                {
                    bestOption = piece;
                }
            }
            else if (CanFinish(bestOption) && !EnemyInProximity(bestOption))
            {
                bestOption = piece;
            }
        }
    }

    public void CheckMove(Piece piece)
    {
        if (CanOnlyMove(piece))
        {
            if (!CanFinish(bestOption) && !CanAttack(bestOption))
            {
                if (CanOnlyMove(bestOption))
                {
                    bestOption = CompareMoves(piece, bestOption);
                }
                else if (CanStart(bestOption))
                {
                    bestOption = CompareStartAndMove(piece, bestOption);
                }
                else
                {
                    bestOption = piece;
                }
            }
        }
    }

    public bool CanFinish(Piece piece)
    {
        return piece.TilesGone <= TileManager.FieldSize && piece.TilesGone + gameManager.CurrentRoll > TileManager.FieldSize
                                                        && piece.TilesGone + gameManager.CurrentRoll < 45 &&
                                                        tileManager.EndFields[piece.Color][(piece.TilesGone + gameManager.CurrentRoll) % 5 - 1] == null;
    }

    public bool CanAttack(Piece piece)
    {
        // attack from start
        if (piece.CurrentTile == -1 && gameManager.CurrentRoll == 6 && (gameManager.GamePlan[currentStart] != null
                                                             && gameManager.GamePlan[currentStart].GetComponent<Piece>().Color != piece.Color))
        {
            return true;
        }
        // attack from field
        if (piece.CurrentTile != -1 && (!piece.hasFinished || piece.TilesGone + gameManager.CurrentRoll <= TileManager.FieldSize) 
                                    && gameManager.GamePlan[(piece.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize] != null
                                    && gameManager.GamePlan[(piece.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize].GetComponent<Piece>().Color != piece.Color)
        {
            return true;
        }
        return false;
    }

    public bool CanStart(Piece piece)
    {
        return gameManager.CurrentRoll == 6 && piece.CurrentTile == -1 && 
               (gameManager.GamePlan[currentStart] == null || gameManager.GamePlan[currentStart].GetComponent<Piece>().Color != piece.Color);
    }

    public bool CanOnlyMove(Piece piece)
    {
        return piece.CurrentTile != -1 && piece.TilesGone + gameManager.CurrentRoll <= TileManager.FieldSize
                                       && gameManager.GamePlan[
                                           (piece.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize] == null;
    }

    public Piece CompareAttacks(Piece piece1, Piece piece2)
    {
        // both can attack from start
        if (piece1.CurrentTile == -1 && piece2.CurrentTile == -1)
        {
            return piece1;
        }

        // piece1 is at the start and piece2 is in the field
        if (piece1.CurrentTile == -1)
        {
            var target1 = gameManager.GamePlan[currentStart].GetComponent<Piece>();
            var target2 = gameManager.GamePlan[(piece2.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }

        // piece2 is at the start and piece1 is in the field
        if (piece2.CurrentTile == -1)
        {
            if (gameManager.GamePlan[currentStart] == null)
            {
                Debug.Log("target1 is missing");
            }

            if (gameManager.GamePlan[(piece1.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize] == null)
            {
                Debug.Log("target2 is missing");
            }
            var target1 = gameManager.GamePlan[currentStart].GetComponent<Piece>();
            var target2 = gameManager.GamePlan[(piece1.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }
        //both pieces are in the field
        else
        {
            var target1 = gameManager.GamePlan[(piece1.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize].GetComponent<Piece>();
            var target2 = gameManager.GamePlan[(piece2.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }
    }

    public Piece CompareMoves(Piece piece1, Piece piece2)
    {
        return piece1.TilesGone + gameManager.CurrentRoll >= piece2.TilesGone + gameManager.CurrentRoll ? piece1 : piece2;
    }

    public Piece CompareStartAndMove(Piece mover, Piece starter)
    {
        return mover.TilesGone + gameManager.CurrentRoll >= 33 ? starter : mover;
    }


    public void ComputerMove(Piece piece, int newPosition)
    {
        soundManager.PlayMoveSound();
        if ((newPosition != currentStart || piece.CurrentTile != -1) || (gameManager.CurrentRoll == 6))
        {
            UpdateInfoBeforeComputerMove(piece);
            if (piece.TilesGone > TileManager.FieldSize)
            {
                gameManager.MoveToEnd(piece, (piece.TilesGone % 5) - 1);
            }
            else
            {
                gameManager.MoveOnBoard(piece, newPosition);
            }
        }
    }

    public int GetNewPosition(Piece piece)
    {
        return piece.CurrentTile == -1 ? currentStart : (piece.CurrentTile + gameManager.CurrentRoll) % TileManager.FieldSize;
    }

    public bool EnemyInProximity(Piece piece)
    {
        for (int i = 1; i < 6; i++)
        {
            var positionBehind = piece.CurrentTile < 5 ? 40 - i : piece.CurrentTile - i;
            if (gameManager.GamePlan[positionBehind] != null && gameManager.GamePlan[positionBehind].GetComponent<Piece>().Color != piece.Color)
            {
                Debug.Log("enemy close behind");
                return true;
            }
        }
        Debug.Log("no enemy close behind");
        return false;
    }

    public void UpdateInfoBeforeComputerMove(Piece piece)
    {
        if (piece.CurrentTile == -1)
        {
            gameManager.ActivePiecesCount[piece.Color] += 1;
            piece.TilesGone = 1;
        }
        else if (piece.TilesGone > TileManager.FieldSize)
        {
            tileManager.EndFields[piece.Color][piece.TilesGone % 5 - 1] = null;
            piece.TilesGone += gameManager.CurrentRoll;
        }
        else
        {
            gameManager.GamePlan[piece.CurrentTile] = null;
            piece.TilesGone += gameManager.CurrentRoll;
        }
    }

    public void ContinueWatching()
    {
        Time.timeScale = 1;
        isWaitingForPlayerDecision = false;
        AfterPlayerWin.SetActive(false);
    }
}