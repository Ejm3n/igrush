using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoard
{
    public static Transform[,] GAMEBoard = new Transform[6, 14];
    public static int Width = 6;
    public static int Height = 14;
    public static int TotalColors = 7;

    public static bool WithinBorders(Vector3 target)
    {
        return target.x > -1 &&
            target.x < Width &&
            target.y > -1 &&
            target.y < Height;
    }

    public static bool FreeSpace(Vector3 target, Transform parentTransform)
    {
        if (WithinBorders(target))
        {
            return GAMEBoard[(int)target.x, (int)target.y] == null ||
                GAMEBoard[(int)target.x, (int)target.y].parent == parentTransform;
        }
        return false;
    }

    public static List<PuyoUnit> GetPuyoUnits()
    {
        List<PuyoUnit> units = new List<PuyoUnit>();
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                if (GAMEBoard[col, row] != null)
                {
                    units.Add(GAMEBoard[col, row].GetComponent<PuyoUnit>());
                }
            }
        }
        return units;
    }

    public static List<PuyoUnit> GetPuyoUnitsWithoutFlyingOne()
    {
        List<PuyoUnit> units = new List<PuyoUnit>();
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                if (GAMEBoard[col, row] != null && !GAMEBoard[col, row].GetComponentInParent<Puyo>())
                {
                    units.Add(GAMEBoard[col, row].GetComponent<PuyoUnit>());
                }
            }
        }
        return units;
    }

    public static bool IsEmpty(int col, int row)
    {
        if (WithinBorders(new Vector3(col, row, 0)))
        {
            return GAMEBoard[col, row] == null;
        }
        return false;
    }

    public static bool ColorMatches(int col, int row, Transform puyoUnit)
    {
        if (WithinBorders(new Vector3(col, row, 0)))
        {
            return GAMEBoard[col, row].GetComponent<PuyoUnit>().ColorIdx == puyoUnit.GetComponent<PuyoUnit>().ColorIdx;
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
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                Clear(col, row);
            }
        }
    }

    public static void Clear(float col, float row)
    { 
        GAMEBoard[(int)col, (int)row] = null;
    }

    public static void Add(float col, float row, Transform obj)
    {
        GAMEBoard[(int)col, (int)row] = obj;
    }

    public static void Delete(Transform puyo)
    {
        try
        {
            Vector2 pos = new Vector2(Mathf.Round(puyo.position.x), Mathf.Round(puyo.position.y));
            GAMEBoard[(int)pos.x, (int)pos.y] = null;
            if(puyo != null)
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
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                List<Transform> currentGroup = new List<Transform>();
                if (GAMEBoard[col, row] != null)
                {

                    Transform current = GAMEBoard[col, row];
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
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                if (GAMEBoard[col, row] != null)
                {
                    Transform puyoUnit = GAMEBoard[col, row];
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
                Transform nextUnit = GAMEBoard[nextX, nextY];
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
        for (int row = 0; row < Height; row++)
        {
            for (int col = 0; col < Width; col++)
            {
                    Delete(GAMEBoard[col, row]);
            }
        }
    }

    public static bool AnyFallingBlocks()
    {
        for (int row = Height - 1; row >= 0; row--)
        {
            for (int col = 0; col < Width; col++)
            {
                if (GAMEBoard[col, row] != null)
                {
                    PuyoUnit puyoUnit = GAMEBoard[col, row].gameObject.GetComponent<PuyoUnit>();
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
