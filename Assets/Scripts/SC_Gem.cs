using BoardLogic;
using Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_Gem : PoolObject
{
    [HideInInspector]
    public Vector2Int posIndex;
    

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private bool mousePressed;
    private float swipeAngle = 0;
    private SC_Gem otherGem;

    public GlobalEnums.GemType type;
    public bool isMatch = false;
    private Vector2Int previousPos;
    public GameObject destroyEffect;
    public int scoreValue = 10;

    public int blastSize = 1;
    private SC_GameLogic scGameLogic;
    private bool isSet;
    private GemMover gemMover;

    private void Awake()
    {
        isSet = false;
    }


    void Update()
    {
        if (!isSet)
            return;

        //if (Vector2.Distance(tr.position, posIndex) > 0.01f)
        //    tr.position = Vector2.Lerp(tr.position, posIndex, SC_GameVariables.Instance.gemSpeed * Time.deltaTime);
        //else
        //{
        //    tr.position = new Vector3(posIndex.x, posIndex.y, 0);
        //    scGameLogic.SetGem(posIndex.x, posIndex.y, this);
        //}
        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            if (scGameLogic.CurrentState == GlobalEnums.GameState.move)
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }
    }

    public void SetupGem(SC_GameLogic _ScGameLogic,Vector2Int _Position, GemMover gemMover)
    {
        this.gemMover = gemMover;
        posIndex = _Position;
        scGameLogic = _ScGameLogic;
        isSet = true;
    }

    private void OnMouseDown()
    {
        if (scGameLogic.CurrentState == GlobalEnums.GameState.move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
        }
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
            MovePieces();
    }

    private void MovePieces()
    {
        previousPos = posIndex;

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < SC_GameVariables.Instance.rowsSize - 1)
        {
            otherGem = scGameLogic.GetGem(posIndex.x + 1, posIndex.y);
            otherGem.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < SC_GameVariables.Instance.colsSize - 1)
        {
            otherGem = scGameLogic.GetGem(posIndex.x, posIndex.y + 1);
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
        {
            otherGem = scGameLogic.GetGem(posIndex.x, posIndex.y - 1);
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngle > 135 || swipeAngle < -135 && posIndex.x > 0)
        {
            otherGem = scGameLogic.GetGem(posIndex.x - 1, posIndex.y);
            otherGem.posIndex.x++;
            posIndex.x--;
        }

        gemMover.EnqueueMove(new List<PoolObject> { this, otherGem }, new List<Vector2Int> { posIndex, otherGem.posIndex }, OnMoveGemMoveFinished);
        //gemMover.EnqueueMove(this, posIndex, OnMoveGemMoveFinished);
        //gemMover.EnqueueMove(otherGem, otherGem.posIndex, OnMoveGemMoveFinished);

        //scGameLogic.SetGem(posIndex.x,posIndex.y, this);
        //scGameLogic.SetGem(otherGem.posIndex.x, otherGem.posIndex.y, otherGem);

        StartCoroutine(CheckMoveCo());
    }

    private void OnMoveGemMoveFinished(GemMover.MoveRequest moveRequest)
    {
        for (int i = 0; i < moveRequest.target.Count; ++i)
        {
            scGameLogic.SetGem(moveRequest.endPos[i].x, moveRequest.endPos[i].y, (SC_Gem)moveRequest.target[i]);
        }
    }

    public IEnumerator CheckMoveCo()
    {
        scGameLogic.SetState(GlobalEnums.GameState.wait);

        yield return new WaitForSeconds(.5f);
        scGameLogic.FindAllMatches();

        if (otherGem != null)
        {
            if (isMatch == false && otherGem.isMatch == false)
            {
                otherGem.posIndex = posIndex;
                posIndex = previousPos;

                //scGameLogic.SetGem(posIndex.x, posIndex.y, this);
                //scGameLogic.SetGem(otherGem.posIndex.x, otherGem.posIndex.y, otherGem);

                gemMover.EnqueueMove(new List<PoolObject> { this, otherGem }, new List<Vector2Int> { posIndex, otherGem.posIndex }, OnMoveGemMoveFinished);

                //yield return new WaitForSeconds(.5f);
                yield return gemMover.IdleWait;
                scGameLogic.SetState(GlobalEnums.GameState.move);
            }
            else
            {
                scGameLogic.DestroyMatches();
            }
        }
    }


}
