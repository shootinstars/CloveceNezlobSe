using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.UI;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ComputerPlayer : MonoBehaviour
{
    public bool computerRoundFinished;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private SoundManager soundManager;

    [SerializeField] private DiceControl diceControl;
    [SerializeField] private Button rollButton;
    [SerializeField] private GameObject afterPlayerWin;
    private int computerRoll;
    private int currentStart;
    private bool canMove;
    public bool isWaitingForPlayerDecision;
    public bool playerFinished;


    public IEnumerator PlayComputerRound(PieceColor currentColor)
    {
        if (gameManager.GetNumberOfFinished(PieceColor.Blue) == 4 && !playerFinished && !gameManager.AllPiecesFinished())
        {
            isWaitingForPlayerDecision = true;
            afterPlayerWin.SetActive(true);
            Time.timeScale = 0f;
            playerFinished = true;
        } 
        gameManager.TurnOffRollWarning();
        rollButton.interactable = false;
        computerRoundFinished = false;
        yield return new WaitForSeconds(0.5f);
        gameManager.CheckForNewFinisher();
        if (gameManager.AllPiecesFinished())
        {
            gameManager.gameFinished = true;
            computerRoundFinished = true;
        }

        if (gameManager.GetNumberOfFinished(currentColor) == 4)
        {
            computerRoundFinished = true;
        }
        gameManager.CurrentPlayerImage.color = tileManager.EndTiles[currentColor][0].GetComponent<Image>().color;
        currentStart = (int)currentColor * 10;
        var rollLimit = gameManager.ActivePiecesCount[currentColor] == 0 ? 3 : 1;
        while (!computerRoundFinished)
        {
            canMove = false;
            if (gameManager.ActivePiecesCount[currentColor] != 0)
            {
                rollLimit = 1;
            }
            yield return new WaitForSeconds(1f);
            computerRoll = Random.Range(1, 7);
            yield return diceControl.RollAnimation(true);
            if (gameManager.CanMove(currentColor, true) && (gameManager.ActivePiecesCount[currentColor] != 0 || computerRoll == 6))
            {
                canMove = true;
                yield return new WaitForSeconds(0.5f);
                var movedPiece = FindBestMove(currentColor);
                var newPosition = GetNewPosition(movedPiece);
                ComputerMove(movedPiece, newPosition);
            }
            rollLimit -= 1;
            if (rollLimit == 0)
            {
                if (computerRoll == 6 && canMove && gameManager.GetNumberOfFinished(currentColor) != 4)
                {
                    rollLimit = 1;
                }
                else
                {
                    computerRoundFinished = true;
                    yield return gameManager.EndRoundCoroutine(currentColor);
                }
            }
        }
    }

    [CanBeNull]
    public Piece FindBestMove(PieceColor color)
    {
        Piece bestOption = null;
        foreach (var piece in gameManager.Pieces[color])
        {
            Piece pieceComp = piece.GetComponent<Piece>();
            if (bestOption == null)
            {
                bestOption = pieceComp;
            }
            //finishing
            else if (CanFinish(pieceComp) && (!CanStart(bestOption)) || (CanFinish(pieceComp) && EnemyInProximity(pieceComp)))
            {
                bestOption = pieceComp;
            }
            //attacking
            else if (CanAttack(pieceComp))
            {
                //comparing attack value
                if (CanAttack(bestOption))
                {
                    bestOption = CompareAttacks(pieceComp, bestOption);
                }
                // there is no more valuable move now
                else if (!CanFinish(bestOption) || CanStart(pieceComp))
                {
                    bestOption = pieceComp;
                }
            }
            //starting
            else if (CanStart(pieceComp))
            {
                //it doesn't matter which piece is summoned to the board
                if (CanStart(bestOption))
                {
                    bestOption = pieceComp;
                }
                else if (!CanFinish(bestOption) && !CanAttack(bestOption))
                {
                    if (CanOnlyMove(bestOption))
                    {
                        bestOption = CompareStartAndMove(bestOption, pieceComp);
                    }
                    else
                    {
                        bestOption = pieceComp;
                    }
                }
                else if (CanFinish(bestOption) && !EnemyInProximity(bestOption))
                {
                    bestOption = pieceComp;
                }
            }
            else if (CanOnlyMove(pieceComp))
            {
                if (!CanFinish(bestOption) && !CanAttack(bestOption))
                {
                    if (CanOnlyMove(bestOption))
                    {
                        bestOption = CompareMoves(pieceComp, bestOption);
                    }
                    else if (CanStart(bestOption))
                    {
                        bestOption = CompareStartAndMove(pieceComp, bestOption);
                    }
                    else
                    {
                        bestOption = pieceComp;
                    }
                }
            }
            else if (!CanFinish(bestOption) && !CanAttack(bestOption) && !CanStart(bestOption) &&
                     !CanOnlyMove(bestOption))
            {
                bestOption = pieceComp;
            }
        }
        return bestOption;
    }

    public bool CanFinish(Piece piece)
    {
        return piece.TilesGone <= TileManager.FieldSize && piece.TilesGone + computerRoll > TileManager.FieldSize
                                                        && piece.TilesGone + computerRoll < 45 &&
                                                        tileManager.EndFields[piece.Color][(piece.TilesGone + computerRoll) % 5 - 1] == null;
    }

    public bool CanAttack(Piece piece)
    {
        // attack from start
        if (piece.CurrentTile == -1 && computerRoll == 6 && (gameManager.GamePlan[currentStart] != null
                                                             && gameManager.GamePlan[currentStart].GetComponent<Piece>().Color != piece.Color))
        {
            return true;
        }
        // attack from field
        if (piece.CurrentTile != -1 && (!piece.hasFinished || piece.TilesGone + computerRoll <= TileManager.FieldSize) 
                                    && gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize] != null
                                    && gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>().Color != piece.Color)
        {
            return true;
        }
        return false;
    }

    public bool CanStart(Piece piece)
    {
        return computerRoll == 6 && piece.CurrentTile == -1 && 
               (gameManager.GamePlan[currentStart] == null || gameManager.GamePlan[currentStart].GetComponent<Piece>().Color != piece.Color);
    }

    public bool CanOnlyMove(Piece piece)
    {
        return piece.CurrentTile != -1 && piece.TilesGone + computerRoll <= TileManager.FieldSize
                                       && gameManager.GamePlan[
                                           (piece.CurrentTile + computerRoll) % TileManager.FieldSize] == null;
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
            var target2 = gameManager.GamePlan[(piece2.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }

        // piece2 is at the start and piece1 is in the field
        if (piece2.CurrentTile == -1)
        {
            if (gameManager.GamePlan[currentStart] == null)
            {
                Debug.Log("target1 is missing");
            }

            if (gameManager.GamePlan[(piece1.CurrentTile + computerRoll) % TileManager.FieldSize] == null)
            {
                Debug.Log("target2 is missing");
            }
            var target1 = gameManager.GamePlan[currentStart].GetComponent<Piece>();
            var target2 = gameManager.GamePlan[(piece1.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }
        //both pieces are in the field
        else
        {
            var target1 = gameManager.GamePlan[(piece1.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>();
            var target2 = gameManager.GamePlan[(piece2.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }
    }

    public Piece CompareMoves(Piece piece1, Piece piece2)
    {
        return piece1.TilesGone + computerRoll >= piece2.TilesGone + computerRoll ? piece1 : piece2;
    }

    public Piece CompareStartAndMove(Piece mover, Piece starter)
    {
        return mover.TilesGone + computerRoll >= 33 ? starter : mover;
    }


    public void ComputerMove(Piece piece, int newPosition)
    {
        soundManager.PlayMoveSound();
        if ((newPosition != currentStart || piece.CurrentTile != -1) || (computerRoll == 6))
        {
            UpdateInfoBeforeComputerMove(piece);
            if (piece.TilesGone > TileManager.FieldSize)
            {
                MoveToEnd(piece, (piece.TilesGone % 5) - 1);
            }
            else
            {
                MoveOnBoard(piece, newPosition);
            }
        }
    }

    public void MoveOnBoard(Piece piece, int newPosition) 
    {
        if (piece.CurrentTile != -1 || computerRoll == 6)
        {
            piece.CurrentTile = newPosition;
            if (gameManager.GamePlan[newPosition] != null)
            {
                soundManager.PlayScreamSound();
                gameManager.GamePlan[newPosition].GetComponent<Piece>().ReturnHome();
            }
            piece.gameObject.transform.position = tileManager.getFieldTiles()[newPosition].transform.position;
            gameManager.GamePlan[newPosition] = piece.gameObject;
        }
    }

    public void MoveToEnd(Piece piece, int endPosition)
    {
        Debug.Log(endPosition);
        piece.hasFinished = true;
        piece.CurrentTile = piece.TilesGone + endPosition;
        tileManager.EndFields[piece.Color][endPosition] = piece.gameObject;
        piece.gameObject.transform.position = tileManager.EndTiles[piece.Color][endPosition].transform.position;
    }

    public int GetNewPosition(Piece piece)
    {
        return piece.CurrentTile == -1 ? currentStart : (piece.CurrentTile + computerRoll) % TileManager.FieldSize;
    }

    public bool EnemyInProximity(Piece piece)
    {
        for (int i = 1; i < 6; i++)
        {
            var positionBehind = piece.CurrentTile - i;
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
            piece.TilesGone += computerRoll;
        }
        else
        {
            gameManager.GamePlan[piece.CurrentTile] = null;
            piece.TilesGone += computerRoll;
        }
    }

    public int getComputerRoll()
    {
        return computerRoll;
    }

    public void ContinueWatching()
    {
        Time.timeScale = 1;
        isWaitingForPlayerDecision = false;
        afterPlayerWin.SetActive(false);
    }
}