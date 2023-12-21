using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard
{
    public static Transform[,] gameBoard = new Transform[6,12];
    private static int width = 6;
    private static int height = 12;

    public static bool WithinBorders(Vector3 target){
        return target.x > -1 &&
            target.x < width &&
            target.y > -1 && 
            target.y < height;
    }
    public static bool FreeSpace(Vector3 target, Transform parentTransform){
        if(WithinBorders(target)){
            return gameBoard[(int)target.x, (int)target.y] == null ||
                gameBoard[(int)target.x, (int)target.y].parent == parentTransform;
        }
        return false;
    }

    public static bool IsEmpty(int col, int row){
        if(WithinBorders(new Vector3(col, row, 0))){
            return gameBoard[col, row] == null;
        }
        return false;
    }

    public static bool ColorMatches(int col, int row, Transform puyoUnit){
        if(WithinBorders(new Vector3(col, row, 0))){
            return gameBoard[col, row].GetComponent<PuyoUnit>().colorIdx == puyoUnit.GetComponent<PuyoUnit>().colorIdx;
        }
        return false;
    }

    public static bool HasMatchingNeighbor(Vector2 pos, Vector3 direction, Transform puyoUnitTransform){
        Vector2 newPos = new Vector2(pos.x + direction.x, pos.y + direction.y);
        return !IsEmpty((int)newPos.x, (int)newPos.y) && ColorMatches((int)newPos.x, (int)newPos.y, puyoUnitTransform);
    }

    public static void Clear(float col, float row){
        gameBoard[(int)col, (int)row] = null;
    }

    public static void Add(float col, float row, Transform obj){
        gameBoard[(int)col, (int)row] = obj;
    }

    public static void Delete(Transform puyo){
        Vector2 pos = new Vector2(Mathf.Round(puyo.position.x), Mathf.Round(puyo.position.y));
        gameBoard[(int)pos.x, (int)pos.y] = null;
        UnityEngine.Object.Destroy(puyo.gameObject);
    }
    
    public static bool WhatToDelete(){
        List<Transform> groupToDelete = new List<Transform>();
        List<Transform> groupToPoof = new List<Transform>();
        
        for (int row = 0; row < height; row++){
            for(int col = 0; col < width; col++ ){
                List<Transform> currentGroup = new List<Transform>();

                if(gameBoard[col, row] != null){
                    
                    Transform current = gameBoard[col, row];       
                    if (groupToDelete.IndexOf(current) == -1){
                        AddNeighbors(current, currentGroup);
                    }
                }

                if(currentGroup.Count >= 4){
                    GameUIController.instance.UpdateCombo(1);
                    GameUIController.instance.UpdateScore(currentGroup.Count);
                    foreach(Transform puyo in currentGroup){
                        groupToDelete.Add(puyo);
                       // groupToPoof.AddRange( AddNeighbors(puyo));
                    }
                }
            }
        }
        
        if(groupToDelete.Count != 0){
            DeleteUnits(groupToDelete);
            SoundManager.Instance.PlayRazbitie();
            //PoofUnits(groupToPoof);
            return true;
        } else {
            return false;
        }
    }

    private static void PoofUnits(List<Transform> puyosToPoof)
    {
        foreach (Transform puyo in puyosToPoof)
        {
            if(puyo != null)
            {
                puyo.GetComponent<PuyoUnit>().EnlargeHands();
            }
        }
    }

    public static bool AnyPoofAnimationsLeft()
    {
        return false;
    }

    public static void DropAllColumns(){
        for(int row = 0; row < height; row++){
            for(int col = 0; col < width; col++){
                if(gameBoard[col, row] != null){
                    Transform puyoUnit = gameBoard[col,row];
                    puyoUnit.gameObject.GetComponent<PuyoUnit>().DropToFloorExternal();
                    
                }
            }
        }
    }

    static void AddNeighbors(Transform currentUnit, List<Transform> currentGroup ){
        PuyoUnit puyoUnit = currentUnit.GetComponent<PuyoUnit>();
        DisableSides(puyoUnit);
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
        if(currentGroup.IndexOf(currentUnit) == -1){
            currentGroup.Add(currentUnit);
        } else { 
            return;
        }

        foreach(Vector3 direction in directions){
            int nextX = (int)(Mathf.Round(currentUnit.position.x) + Mathf.Round(direction.x));
            int nextY = (int)(Mathf.Round(currentUnit.position.y) + Mathf.Round(direction.y));

            if(!IsEmpty(nextX, nextY) && ColorMatches(nextX, nextY, currentUnit)){
                Transform nextUnit = gameBoard[nextX, nextY];
                AddNeighbors(nextUnit, currentGroup);

                //добавленный бред про сайды
                if(direction == Vector3.up)
                {
                    puyoUnit.SetCorner(PuyoSide.Top, true);
                }
                else if (direction == Vector3.right)
                {
                    puyoUnit.SetCorner(PuyoSide.Right, true);
                }
                else if (direction == Vector3.left)
                {
                    puyoUnit.SetCorner(PuyoSide.Left, true);
                }
                else if (direction == Vector3.down)
                {
                    puyoUnit.SetCorner(PuyoSide.Bot, true);
                }
            }
        }
    }

    static List<Transform> AddNeighbors(Transform currentUnit)
    {
       
        PuyoUnit puyoUnit = currentUnit.GetComponent<PuyoUnit>();
        List<Transform> neighbors = new List<Transform>();
        DisableSides(puyoUnit);
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
        if (currentUnit == null)
            return neighbors;

        foreach (Vector3 direction in directions)
        {
            int nextX = (int)(Mathf.Round(currentUnit.position.x) + Mathf.Round(direction.x));
            int nextY = (int)(Mathf.Round(currentUnit.position.y) + Mathf.Round(direction.y));

            if(IsEmpty(nextX, nextY))
            {
                Transform nextUnit = gameBoard[nextX, nextY];
                neighbors.Add(nextUnit);

                //добавленный бред про сайды
                if (direction == Vector3.up)
                {
                    puyoUnit.SetCorner(PuyoSide.Top, true);
                }
                else if (direction == Vector3.right)
                {
                    puyoUnit.SetCorner(PuyoSide.Right, true);
                }
                else if (direction == Vector3.left)
                {
                    puyoUnit.SetCorner(PuyoSide.Left, true);
                }
                else if (direction == Vector3.down)
                {
                    puyoUnit.SetCorner(PuyoSide.Bot, true);
                }
            }
        }
        return neighbors;
    }

    static void DeleteUnits(List<Transform> unitsToDelete){
        foreach(Transform unit in unitsToDelete){
            Delete(unit);
        }
    }

    public static bool AnyFallingBlocks(){
        for(int row = 11; row >= 0; row--){
            for(int col = 0; col < 6; col++ ){     
                if(gameBoard[col, row] != null){
                    PuyoUnit puyoUnit = gameBoard[col, row].gameObject.GetComponent<PuyoUnit>();
                    if (puyoUnit.forcedDownwards){
                        return true;
                    } else if(puyoUnit.activelyFalling)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static void DebugBoard(){
        Text text = GameObject.Find("Text").GetComponent<Text>();
        string boardContents = "";

        for(int row = 11; row >= 0; row--){
            boardContents += $"{row} :";
            for(int col = 0; col < 6; col++ ){                
                if(gameBoard[col, row] == null){
                    boardContents += "o ";
                } else {
                    int idx = gameBoard[col, row].gameObject.GetComponent<PuyoUnit>().colorIdx;
                    string[] colorArray = { "B", "G", "R", "C" };
                    boardContents += $"{colorArray[idx]} ";
                }
            }            
            boardContents += "\n";
        }
        text.text = boardContents;
    }

    private static void DisableSides(PuyoUnit puyoUnit)
    {
        puyoUnit.SetCorner(PuyoSide.Top, false);
        puyoUnit.SetCorner(PuyoSide.Left, false);
        puyoUnit.SetCorner(PuyoSide.Right, false);
        puyoUnit.SetCorner(PuyoSide.Bot, false);
    }
}
