﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard
{
    public static Transform[,] gameBoard = new Transform[6, 14];
    public static int width = 6;
    public static int height = 14;
    public static int totalColors = 7;

    public static bool WithinBorders(Vector3 target)
    {
        return target.x > -1 &&
            target.x < width &&
            target.y > -1 &&
            target.y < height;
    }

    public static bool FreeSpace(Vector3 target, Transform parentTransform)
    {
        if (WithinBorders(target))
        {
            return gameBoard[(int)target.x, (int)target.y] == null ||
                gameBoard[(int)target.x, (int)target.y].parent == parentTransform;
        }
        return false;
    }

    public static List<PuyoUnit> GetPuyoUnits()
    {
        List<PuyoUnit> units = new List<PuyoUnit>();
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (gameBoard[col, row] != null)
                {
                    units.Add(gameBoard[col, row].GetComponent<PuyoUnit>());
                }
            }
        }
        return units;
    }

    public static List<PuyoUnit> GetPuyoUnitsWithoutFlyingOne()
    {
        List<PuyoUnit> units = new List<PuyoUnit>();
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (gameBoard[col, row] != null && !gameBoard[col, row].GetComponentInParent<Puyo>())
                {
                    units.Add(gameBoard[col, row].GetComponent<PuyoUnit>());
                }
            }
        }
        return units;
    }

    public static bool IsEmpty(int col, int row)
    {
        if (WithinBorders(new Vector3(col, row, 0)))
        {
            return gameBoard[col, row] == null;
        }
        return false;
    }

    public static bool ColorMatches(int col, int row, Transform puyoUnit)
    {
        if (WithinBorders(new Vector3(col, row, 0)))
        {
            return gameBoard[col, row].GetComponent<PuyoUnit>().colorIdx == puyoUnit.GetComponent<PuyoUnit>().colorIdx;
        }
        return false;
    }

    public static bool HasMatchingNeighbor(Vector2 pos, Vector3 direction, Transform puyoUnitTransform)
    {
        Vector2 newPos = new Vector2(pos.x + direction.x, pos.y + direction.y);
        return !IsEmpty((int)newPos.x, (int)newPos.y) && ColorMatches((int)newPos.x, (int)newPos.y, puyoUnitTransform);
    }

    public static void ClearBoard()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                Clear(col, row);
            }
        }
    }

    public static void Clear(float col, float row)
    { 
        gameBoard[(int)col, (int)row] = null;
    }

    public static void Add(float col, float row, Transform obj)
    {
        gameBoard[(int)col, (int)row] = obj;
    }

    public static void Delete(Transform puyo)
    {
        try
        {
            Vector2 pos = new Vector2(Mathf.Round(puyo.position.x), Mathf.Round(puyo.position.y));
            gameBoard[(int)pos.x, (int)pos.y] = null;
            Object.Destroy(puyo.gameObject);
        }
        catch
        {
            Debug.Log("Error deleting puyo");
        }
    }

    public static bool WhatToDelete()
    {
        List<Transform> groupToDelete = new List<Transform>();
        List<Transform> groupToPoof = new List<Transform>();

        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                List<Transform> currentGroup = new List<Transform>();
                if (gameBoard[col, row] != null)
                {

                    Transform current = gameBoard[col, row];
                    if (groupToDelete.IndexOf(current) == -1)
                    {
                        AddNeighbors(current, currentGroup);
                    }
                }
                if (currentGroup.Count >= 4)
                {
                    GameUIController.instance.UpdateCombo(1);
                    GameUIController.instance.UpdateScore(currentGroup.Count);
                    foreach (Transform puyo in currentGroup)
                    {
                        groupToDelete.Add(puyo);
                    }
                }
            }
        }
        if (groupToDelete.Count != 0)
        {
            PuyoSpawner.ComboedThisRound(true);
            DeleteUnits(groupToDelete);
            SoundManager.Instance.PlayRazbitie();
            return true;
        }
        else
        {
            return false;
        }
    }

    public static void DropAllColumns()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (gameBoard[col, row] != null)
                {
                    Transform puyoUnit = gameBoard[col, row];
                    puyoUnit.gameObject.GetComponent<PuyoUnit>().DropToFloorExternal();

                }
            }
        }
    }

    static void AddNeighbors(Transform currentUnit, List<Transform> currentGroup)
    {
        PuyoUnit puyoUnit = currentUnit.GetComponent<PuyoUnit>();
        DisableSides(puyoUnit);
        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.right, Vector3.left };
        if (currentGroup.IndexOf(currentUnit) == -1)
        {
            currentGroup.Add(currentUnit);
        }
        else
        {
            return;
        }

        foreach (Vector3 direction in directions)
        {
            int nextX = (int)(Mathf.Round(currentUnit.position.x) + Mathf.Round(direction.x));
            int nextY = (int)(Mathf.Round(currentUnit.position.y) + Mathf.Round(direction.y));

            if (!IsEmpty(nextX, nextY) && ColorMatches(nextX, nextY, currentUnit))
            {
                Transform nextUnit = gameBoard[nextX, nextY];
                AddNeighbors(nextUnit, currentGroup);

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
    }

    static void DeleteUnits(List<Transform> unitsToDelete)
    {
        foreach (Transform unit in unitsToDelete)
        {
            Delete(unit);
        }
    }

    public static void DeleteAllPuyos()
    {
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                    Delete(gameBoard[col, row]);
            }
        }
    }

    public static bool AnyFallingBlocks()
    {
        for (int row = height - 1; row >= 0; row--)
        {
            for (int col = 0; col < width; col++)
            {
                if (gameBoard[col, row] != null)
                {
                    PuyoUnit puyoUnit = gameBoard[col, row].gameObject.GetComponent<PuyoUnit>();
                    if (puyoUnit.IsForcedDownwards())
                    {
                        return true;
                    }
                    else if (puyoUnit.ActivelyFalling)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private static void DisableSides(PuyoUnit puyoUnit)
    {
        puyoUnit.SetCorner(PuyoSide.Top, false);
        puyoUnit.SetCorner(PuyoSide.Left, false);
        puyoUnit.SetCorner(PuyoSide.Right, false);
        puyoUnit.SetCorner(PuyoSide.Bot, false);
    }
}
