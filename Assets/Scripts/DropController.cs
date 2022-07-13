using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropController : MonoBehaviour
{
    public enum DropType
    {
        Red,
        Blue,
        Green,
        Yellow
    }
    public DropType dropType;

    private BoardController board;
    public int dropColumn;
    public int dropRow;
    private int previousColumn;
    private int previousRow;

    private int targetPositionX;
    private int targetPositionY;

    public float swipeAngle = 0;
    private float swipeResist = 1f;
    public bool isMatched = false;

    private GameObject targetDrop;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;

    private DropController leftDropController;
    private DropController rightDropController;
    private DropController upDropController;
    private DropController downDropController;
    private MatchFinder matchFinder;


    private void Start() 
    {
        board = FindObjectOfType<BoardController>();
        matchFinder = FindObjectOfType<MatchFinder>();

        targetPositionX = (int) transform.position.x;
        targetPositionY = (int) transform.position.y;
        dropRow = targetPositionY;
        dropColumn = targetPositionX;
    }

    private void FixedUpdate() 
    {
        if(isMatched)
        {
            LeanTween.scale(this.gameObject, Vector2.one * 1.4f, 0.05f);
        }


        targetPositionX = dropColumn;
        targetPositionY = dropRow;
        if(Mathf.Abs(targetPositionX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetPositionX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .2f);
            if(board.allDrops[dropColumn, dropRow] != this.gameObject)
            {
                board.allDrops[dropColumn, dropRow] = this.gameObject;
            }
            matchFinder.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(targetPositionX, transform.position.y);
            transform.position = tempPosition;
            board.allDrops[dropColumn, dropRow] = this.gameObject;
        }

        if(Mathf.Abs(targetPositionY - transform.position.y) > 0.1f)
        {
            tempPosition = new Vector2(transform.position.x, targetPositionY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .2f);
            if(board.allDrops[dropColumn, dropRow] != this.gameObject)
            {
                board.allDrops[dropColumn, dropRow] = this.gameObject;
            }
            matchFinder.FindAllMatches();
        }
        else
        {
            tempPosition = new Vector2(transform.position.x, targetPositionY);
            transform.position = tempPosition;
        }
    }

    private void OnMouseDown() 
    {
        if(board.currentState == BoardController.GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    private void OnMouseUp() 
    {
        if(board.currentState == BoardController.GameState.move)
        {
            finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {   
            swipeAngle = Mathf.Atan2((finalTouchPosition.y - firstTouchPosition.y), (finalTouchPosition.x - firstTouchPosition.x)) * Mathf.Rad2Deg;
            MoveDrops();
            board.currentState = BoardController.GameState.wait;
        }
        else
        {
            board.currentState = BoardController.GameState.move;
        }
    }

    private void MoveDrops()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && dropColumn < board.boardWidth -1) //right swipe
        {
            if(board.allDrops[dropColumn + 1, dropRow]!= null)
            {
                targetDrop = board.allDrops[dropColumn + 1, dropRow];
                previousColumn = dropColumn;
                previousRow = dropRow;
                targetDrop.GetComponent<DropController>().dropColumn -= 1;
                dropColumn += 1;
            }
        } 
        else if(swipeAngle > 45 && swipeAngle <= 135 && dropColumn < board.boardHeight -1) //up swipe
        {
            if(board.allDrops[dropColumn, dropRow + 1] != null)
            {
                targetDrop = board.allDrops[dropColumn, dropRow + 1];
                previousColumn = dropColumn;
                previousRow = dropRow;
                targetDrop.GetComponent<DropController>().dropRow -= 1;
                dropRow += 1;
            }
        } 
        else if(swipeAngle > 135 || swipeAngle <= -135 && dropRow > 0) //left swipe
        {
            if(board.allDrops[dropColumn - 1, dropRow] != null)
            {
                targetDrop = board.allDrops[dropColumn - 1, dropRow];
                previousColumn = dropColumn;
                previousRow = dropRow;
                targetDrop.GetComponent<DropController>().dropColumn += 1;
                dropColumn -= 1;
            }
        }
        else if(swipeAngle < -45 && swipeAngle >= -135 && dropRow > 0) //down swipe
        {
            if(board.allDrops[dropColumn, dropRow -1] != null)
            {
                targetDrop = board.allDrops[dropColumn, dropRow -1];
                previousColumn = dropColumn;
                previousRow = dropRow;
                targetDrop.GetComponent<DropController>().dropRow += 1;
                dropRow -= 1;
            }
        }
        StartCoroutine(CheckMoveCoroutine());
    }

    private IEnumerator CheckMoveCoroutine()
    {
        yield return new WaitForSeconds(.25f);
        if(targetDrop != null)
        {
            if(!isMatched && !targetDrop.GetComponent<DropController>().isMatched)
            {
                targetDrop.GetComponent<DropController>().dropRow = dropRow;
                targetDrop.GetComponent<DropController>().dropColumn = dropColumn;
                dropRow = previousRow;
                dropColumn = previousColumn;
                yield return new WaitForSeconds(.25f);
                board.currentState = BoardController.GameState.move;
            }
            else if(isMatched || targetDrop.GetComponent<DropController>().isMatched)
            {
                board.ClearMatches();
            }
            targetDrop = null;
        }
    }
}
