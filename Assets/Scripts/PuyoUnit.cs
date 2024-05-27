
using System.Collections;
using UnityEngine;

[System.Serializable]
public class PuyoCorners
{
    public Sprite puyoHorizontalSide;

    public Sprite puyoVerticalSide;
}

public enum PuyoSide
{
    Left,
    Top,
    Right,
    Bot
}

[System.Serializable]
public class SpriteSides
{
    public PuyoSide side;
    public SpriteRenderer spriteRenderer;
    public Animator animator;
}


public class PuyoUnit : MonoBehaviour
{
    public bool ActivelyFalling { get; private set; }  
    public int ColorIdx { get; private set; }

    [SerializeField] private float timeToDropNextStep = .05f;
    [SerializeField] private Sprite[] puyoSpriteArray;
    [SerializeField] private PuyoCorners[] puyoCornersArray;
    [SerializeField] private SpriteSides[] puyoSidesSpriteRenderers;
    [SerializeField] private Animator animator;

    private bool forcedDownwards = false;
    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void SetColorIdx(int index)
    {
        ColorIdx = index;
        ChangeToOtherColor(ColorIdx);
        animator.SetTrigger(ColorIdx.ToString());
    }

    public int GetColorIdx()
    {
        return ColorIdx;
    }

    public IEnumerator DropToFloor()
    {
        Vector3 currentPos = RoundVector(gameObject.transform.position);

        for (int row = (int)currentPos.y - 1; row >= 0; row--)
        {
            int currentX = (int)currentPos.x;
            if (GameBoard.IsEmpty(currentX, row))
            {
                forcedDownwards = true;
                ActivelyFalling = true;
                GameBoard.Clear(currentX, row + 1);
                GameBoard.Add(currentX, row, gameObject.transform);
                gameObject.transform.position += Vector3.down;
                yield return new WaitForSecondsRealtime(timeToDropNextStep);
            }
            else
            {
                ActivelyFalling = false;
                forcedDownwards = false;
                break;
            }
        }
        forcedDownwards = false;
        ActivelyFalling = false;
    }

    public void DropToFloorExternal()
    {
        StartCoroutine(DropToFloor());
    }

    public void ChangeToOtherColor(int index)
    {
        ColorIdx = index;
        GetComponent<SpriteRenderer>().sprite = puyoSpriteArray[ColorIdx];
        foreach (SpriteSides spriteSide in puyoSidesSpriteRenderers)
        {
            if (spriteSide.side == PuyoSide.Left || spriteSide.side == PuyoSide.Right)
            {
                spriteSide.spriteRenderer.sprite = puyoCornersArray[ColorIdx].puyoHorizontalSide;

            }
            else if (spriteSide.side == PuyoSide.Top || spriteSide.side == PuyoSide.Bot)
            {
                spriteSide.spriteRenderer.sprite = puyoCornersArray[ColorIdx].puyoVerticalSide;

            }
        }
        animator.SetTrigger(ColorIdx.ToString());
    }

    public Vector3 RoundVector(Vector3 vect)
    {
        return new Vector2(Mathf.Round(vect.x), Mathf.Round(vect.y));
    }

    public Sprite GetRightCornerSprite()
    {
        return puyoCornersArray[ColorIdx].puyoHorizontalSide;
    }

    public void SetCorner(PuyoSide side, bool what)
    {
        foreach (SpriteSides spriteSide in puyoSidesSpriteRenderers)
        {
            if (side == spriteSide.side)
            {
                spriteSide.spriteRenderer.enabled = what;
            }
        }
    }

    public void StopFalling()
    {
        ActivelyFalling = false;
    }

    public bool IsForcedDownwards()
    {
        return forcedDownwards;
    }
}
