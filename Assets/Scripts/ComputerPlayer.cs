using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ComputerPlayer : MonoBehaviour
{
    public bool computerRoundFinished;

    [SerializeField] private GameManager gameManager;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private DiceControl diceControl;
    private int computerRoll;
    private int currentStart;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator PlayComputerRound(PieceColor currentColor)
    {
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
            if (gameManager.ActivePiecesCount[currentColor] != 0)
            {
                rollLimit = 1;
            }
            yield return new WaitForSeconds(1f);
            computerRoll = Random.Range(1, 7);
            yield return diceControl.RollAnimation(true);
            if (gameManager.CanMove(currentColor, true) && (gameManager.ActivePiecesCount[currentColor] != 0 || computerRoll == 6))
            {
                Debug.Log("Mùžu se hýbat");
                yield return new WaitForSeconds(0.5f);
                var movedPiece = FindBestMove(currentColor);
                Debug.Log(movedPiece.name + " is moving");
                var newPosition = GetNewPosition(movedPiece);
                Debug.Log("New position: " + newPosition);
                ComputerMove(movedPiece, newPosition);
            }
            rollLimit -= 1;
            if (rollLimit == 0)
            {
                if (computerRoll == 6 && gameManager.CanMove(currentColor, true))
                {
                    rollLimit = 1;
                }
                else
                {
                    computerRoundFinished = true;
                    Debug.Log("A nebo tady?");
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
            else if (CanFinish(pieceComp))
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
                else if (!CanFinish(bestOption))
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
        if (piece.TilesGone <= TileManager.FieldSize && piece.TilesGone + computerRoll > TileManager.FieldSize &&
            tileManager.EndFields[piece.Color][(piece.TilesGone + computerRoll) % 5 - 1] == null)
        {
            Debug.Log(piece.name + " can finish, " + " roll is " + computerRoll + " current tile is " + piece.CurrentTile);
            return true;
        }
        Debug.Log(piece.name + " cannot finish, " + " roll is " + computerRoll + " current tile is " + piece.CurrentTile);
        return false;
    }

    public bool CanAttack(Piece piece)
    {
        // attack from start
        if (piece.CurrentTile == -1 && computerRoll == 6 && (gameManager.GamePlan[currentStart] != null
                                                             && gameManager.GamePlan[currentStart].GetComponent<Piece>().Color != piece.Color))
        {
            Debug.Log(piece.name + " can attack from start, " + " roll is " + computerRoll);
            return true;
        }
        // attack from field
        if (piece.CurrentTile != -1 && (!piece.hasFinished || piece.TilesGone + computerRoll <= TileManager.FieldSize) 
                                    && gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize] != null
                                    && gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>().Color != piece.Color)
        {
            Debug.Log(piece.name + " can attack from field, " + " roll is " + computerRoll);
            return true;
        }
        Debug.Log(piece.name + " cannot attack, " + " roll is " + computerRoll);
        return false;
    }

    public bool CanStart(Piece piece)
    {
        if (computerRoll == 6 && piece.CurrentTile == -1 && gameManager.GamePlan[currentStart] == null)
        {
            Debug.Log(piece.name + " can start, " + " roll is " + computerRoll);
            return true;
        }
        Debug.Log(piece.name + " cannot start, " + " roll is " + computerRoll);
        return false;
    }

    public bool CanOnlyMove(Piece piece)
    {
        if (piece.CurrentTile != -1 && piece.TilesGone + computerRoll <= TileManager.FieldSize
                                   && gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize] == null)
        {
            Debug.Log(piece.name + " can move, " + " roll is " + computerRoll + " current tile is " + piece.CurrentTile);
            return true;
        }
        Debug.Log(piece.name + " cannot move, " + " roll is " + computerRoll);
        return false;
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
        return mover.TilesGone + computerRoll >= 30 ? starter : mover;
    }


    public void ComputerMove(Piece piece, int newPosition)
    {
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
}