using System.Collections;
using UnityEngine;

[System.Serializable]
public enum PuyoState
{
    Up,
    UpsideDown,
    Left,
    Right
}


public class Puyo : MonoBehaviour
{
    public GameObject[] unitArray = new GameObject[2];

    public float fallSpeed {private get; set; }
    
    public bool CanBeMoved { get; private set; }
    [SerializeField] private GameObject puyoUnitPrefab;
     [SerializeField] private float moveDownDefaultDelay = .03f;
    [SerializeField] private float dropDownStepDelay = 0.01f;
    private Vector3 left = Vector3.left;
    private Vector3 right = Vector3.right;
    private Vector3 down = Vector3.down;
    private Vector3 up = Vector3.up;
    private float interval;
    private bool puyoUnitDropsFinished = false;

    private bool canBeMovedDown = true;
   
    private float moveDownDelay;


    void Awake()
    {
        unitArray[0] = Instantiate(puyoUnitPrefab, transform.position, Quaternion.identity);
        unitArray[1] = Instantiate(puyoUnitPrefab, new Vector2(transform.position.x + 1, transform.position.y), Quaternion.identity);
        unitArray[0].transform.parent = gameObject.transform;
        unitArray[1].transform.parent = gameObject.transform;
        UpdateGameBoard();
        moveDownDelay = moveDownDefaultDelay;
CanBeMoved = true;
    }
    public void SetPuyosColors(int color1,int color2)
    {
        unitArray[0].GetComponent<PuyoUnit>().SetColorIdx( color1);
        unitArray[1].GetComponent<PuyoUnit>().SetColorIdx( color2);
    }
    void FixedUpdate()
    {
        AutoDrop();
        if (moveDownDelay > 0)
        {
            canBeMovedDown = false;
            moveDownDelay -= Time.deltaTime;
        }
        else
        {
            canBeMovedDown = true;
        }
        // GameBoard.DebugBoard();           
    }

   
    void AutoDrop()
    {
        if (interval > fallSpeed)
        {
            MoveDown();
            interval = 0;
        }
        else
        {
            interval += Time.deltaTime;
        }
    }

    //////////////
    // Movement //
    //////////////

    public bool MoveLeft()
    {
        if (ValidMove(left))
        {
            Move(left, transform);
            return true;
        }
        else return false;
    }

    public bool MoveRight()
    {
        if (ValidMove(right))
        {
            Move(right, transform);
            return true;
        }
        else return false;
    }

    public void MoveDown()
    {
        if (ValidMove(down) && canBeMovedDown)
        {
            moveDownDelay = moveDownDefaultDelay;
            Move(down, transform);
        }
        else if (!ValidMove(down))
        {
            DisableSelf();
            SoundManager.Instance.PlayFalling();
        }
    }

    public IEnumerator DropDown()
    {
        while (ValidMove(down))
        {
            if (canBeMovedDown)
            {
                moveDownDelay = dropDownStepDelay;
                Move(down, transform);
            }
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public void RotateLeft()
    {
        Vector3 vect = GetClockwiseRotationVector();
        if (ValidRotate(vect))
        {
            Move(vect, unitArray[1].transform);
        }
        //else
        //{
        //    vect = GetCounterClockwiseRotationVector();
        //    Move(left, transform);
        //    Move(vect, unitArray[1].transform);
        //}
    }

    public void RotateRight()
    {
        Vector3 vect = GetCounterClockwiseRotationVector();
        if (ValidRotate(vect))
        {
            Move(vect, unitArray[1].transform);
        }
        else if (Mathf.Round(unitArray[1].transform.position.y) < GameBoard.Height-1) // этот ужас переделать потом какнибудь 
        {
            if (GetTetrominoState() == PuyoState.Up && (MoveLeft()))
            {
                RotateRight();
            }
            else if (MoveRight())
            {
                {
                    RotateRight();
                }
            }
            else if (!MoveLeft() && !MoveRight())
            {
                SwapUnits();
            }
        }
        else if(canBeMovedDown)
        {
            MoveDown();
            RotateRight();
        }
        SoundManager.Instance.PlayPerevertysh();
    }

    void Move(Vector3 vector, Transform target)
    {
        ClearCurrentGameboardPosition();
        target.position += vector;
        UpdateGameBoard();
    }

    void ClearCurrentGameboardPosition()
    {
        foreach (Transform puyoUnit in transform)
        {
            GameBoard.Clear(puyoUnit.transform.position.x, puyoUnit.transform.position.y);
        }
    }

    void UpdateGameBoard()
    {
        foreach (Transform puyoUnit in transform)
        {
            GameBoard.Add(puyoUnit.position.x, puyoUnit.position.y, puyoUnit);
        }
    }

    private void SwapUnits()
    {
        PuyoUnit var0 = unitArray[0].GetComponent<PuyoUnit>();
        PuyoUnit var1 = unitArray[1].GetComponent<PuyoUnit>();
        int var0Index = var0.ColorIdx;
        var0.ChangeToOtherColor(var1.ColorIdx);
        var1.ChangeToOtherColor(var0Index);

    }

    Vector3 GetClockwiseRotationVector()
    {
        Vector3 puyoUnitPos = RoundVector(unitArray[1].transform.position);

        if (Vector3.Distance(puyoUnitPos + left, transform.position) == 0)
        {
            return new Vector3(-1, -1);
        }
        else if (Vector3.Distance(puyoUnitPos + up, transform.position) == 0)
        {
            return new Vector3(-1, +1);
        }
        else if (Vector3.Distance(puyoUnitPos + right, transform.position) == 0)
        {
            return new Vector3(+1, +1);
        }
        else if (Vector3.Distance(puyoUnitPos + down, transform.position) == 0)
        {
            return new Vector3(+1, -1);
        }

        return new Vector3(0, 0);
    }

    Vector3 GetCounterClockwiseRotationVector()
    {
        try
        {
            Vector3 puyoUnitPos = RoundVector(unitArray[1].transform.position);
            if (Vector3.Distance(puyoUnitPos + left, transform.position) == 0)
            {
                return new Vector3(-1, +1);
            }
            else if (Vector3.Distance(puyoUnitPos + up, transform.position) == 0)
            {
                return new Vector3(+1, +1);
            }
            else if (Vector3.Distance(puyoUnitPos + right, transform.position) == 0)
            {
                return new Vector3(+1, -1);
            }
            else if (Vector3.Distance(puyoUnitPos + down, transform.position) == 0)
            {
                return new Vector3(-1, -1);
            }

        }
        catch
        {
            Debug.LogError("bug here");
        }


        return new Vector3(0, 0);
    }

    private PuyoState GetTetrominoState()    //если делать разворот в обу стороны то сделать енам вместо булки
    {
        if (Vector3.Distance(RoundVector(unitArray[1].transform.position) + up, transform.position) == 0)
            return PuyoState.Up;
        else if ((Vector3.Distance(RoundVector(unitArray[1].transform.position) + down, transform.position) == 0))
            return PuyoState.UpsideDown;
        else if ((Vector3.Distance(RoundVector(unitArray[1].transform.position) + left, transform.position) == 0))
            return PuyoState.Right;
        else
            return PuyoState.Left;
    }



    bool ActivelyFalling()
    {
        return unitArray[0].GetComponent<PuyoUnit>().ActivelyFalling ||
            unitArray[1].GetComponent<PuyoUnit>().ActivelyFalling;
    }

    ///////////////////////////
    // Movement Constraints //
    /////////////////////////

    bool ValidMove(Vector3 direction)
    {
        foreach (Transform puyo in transform)
        {
            Vector3 newPosition = new Vector3(puyo.position.x + direction.x, puyo.position.y + direction.y, 0);

            if (!GameBoard.FreeSpace(newPosition, transform))
            {
                return false;
            }
        }
        return true;
    }

    bool ValidRotate(Vector3 direction)
    {
        Vector3 puyoPos = unitArray[1].transform.position;
        Vector3 newPosition = new Vector3(puyoPos.x + direction.x, puyoPos.y + direction.y);
        return GameBoard.FreeSpace(newPosition, transform);
    }

    ////////////////
    // PuyoUnits //
    ///////////////

    private void DropPuyoUnits()
    {
        foreach (Transform puyoUnit in transform)
        {
            StartCoroutine(puyoUnit.gameObject.GetComponent<PuyoUnit>().DropToFloor());
        }
    }

    ////////////////
    // Utilities //
    ///////////////

    public Vector3 RoundVector(Vector3 vect)
    {
        return new Vector2(Mathf.Round(vect.x), Mathf.Round(vect.y));
    }

    void DisableSelf()
    {
        gameObject.GetComponent<PlayerController>().enabled = false;
        DropPuyoUnits();
        //enabled = false;
        CanBeMoved = false;
        StartCoroutine(SpawnNextBlock());

    }

    IEnumerator SpawnNextBlock()
    {
        yield return new WaitUntil(() => !ActivelyFalling());

        FindObjectOfType<PuyoSpawner>().SpawnPuyo();
        //GameObject.Find("PuyoSpawner").GetComponent<PuyoSpawner>().SpawnPuyo();// эту гадость вынести в гей контролер
        Destroy(this);
    }
}
