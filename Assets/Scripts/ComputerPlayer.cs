using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using JetBrains.Annotations;
using UnityEngine;
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
            if ((!gameManager.CanMove(currentColor) || (gameManager.ActivePiecesCount[currentColor] == 0 && computerRoll != 6 && rollLimit < 1)))
            {
                gameManager.EndRound();
            }
            else if (gameManager.CanMove(currentColor) && (gameManager.ActivePiecesCount[currentColor] != 0 || computerRoll == 6))
            {
                yield return new WaitForSeconds(0.5f);
                var movedPiece = FindBestMove(currentColor);
                var newPosition = GetNewPosition(movedPiece);
                Debug.Log("New position: " + newPosition);
                ComputerMove(movedPiece, newPosition);
                if (computerRoll != 6)
                {
                    gameManager.EndRound();
                }
            }
            
            rollLimit -= 1;
            if (rollLimit == 0)
            {
                if (computerRoll == 6)
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
            else if (CanFinish(pieceComp))
            {
                bestOption = pieceComp;
            }
            else if (CanAttack(pieceComp))
            {
                if (CanAttack(bestOption))
                {
                    bestOption = CompareAttacks(pieceComp, bestOption);
                }
            }
            else if (CanStart(pieceComp))
            {
                if (CanOnlyMove(bestOption))
                {
                    bestOption = CompareStartAndMove(bestOption, pieceComp);
                }
                else if (CanStart(bestOption))
                {
                    bestOption = pieceComp;
                }
            }
            else
            {
                if (CanOnlyMove(bestOption))
                {
                    bestOption = CompareMoves(pieceComp, bestOption);
                }
                else
                {
                    bestOption = pieceComp;
                }
            }
        }
        return bestOption;
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
        if (gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize] != null 
            && gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>().Color != piece.Color)
        {
            return true;
        }
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
            var target1 = gameManager.GamePlan[currentStart].GetComponent<Piece>();
            var target2 = gameManager.GamePlan[(piece1.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }
        else
        {
            var target1 = gameManager.GamePlan[(piece1.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>();
            var target2 = gameManager.GamePlan[(piece2.CurrentTile + computerRoll) % TileManager.FieldSize].GetComponent<Piece>();
            return target1.TilesGone + (gameManager.GetNumberOfFinished(target1.Color) * 100) >= target2.TilesGone + (gameManager.GetNumberOfFinished(target2.Color) * 100) ? piece1 : piece2;
        }

        //both pieces are in the field
    }

    public bool CanFinish(Piece piece)
    {
        return piece.TilesGone + computerRoll > TileManager.FieldSize && tileManager.EndFields[piece.Color] == null;
    }

    public bool CanStart(Piece piece)
    {
        return computerRoll == 6 && piece.CurrentTile == -1 && gameManager.GamePlan[currentStart] == null;
    }

    public bool CanOnlyMove(Piece piece)
    {
        return piece.CurrentTile != 1 && gameManager.GamePlan[(piece.CurrentTile + computerRoll) % TileManager.FieldSize] == null;
    }

    public Piece CompareMoves(Piece piece1, Piece piece2)
    {
        return piece1.CurrentTile + computerRoll >= piece2.CurrentTile + computerRoll ? piece1 : piece2;
    }

    public Piece CompareStartAndMove(Piece mover, Piece starter)
    {
        return mover.TilesGone + computerRoll >= 30 ? mover : starter;
    }

    public void ComputerMove(Piece piece, int newPosition)
    {
        if (newPosition != currentStart || computerRoll == 6)
        {
            gameManager.UpdateInfoBeforeMove(piece);
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

    public int getComputerRoll()
    {
        return computerRoll;
    }
}