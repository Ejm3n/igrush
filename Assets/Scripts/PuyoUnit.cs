
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
    public bool Poofing;
    public bool activelyFalling = true;
    public bool forcedDownwards = false;
    public bool handsAnim = false;
    public int colorIdx;

    [SerializeField] private float TimeToDropNextStep = .1f;
    [SerializeField] private Sprite[] puyoSpriteArray;
    [SerializeField] private PuyoCorners[] puyoCornersArray;
    [SerializeField] private SpriteSides[] puyoSidesSpriteRenderers;

    void Awake()
    {
        SetColorIdx(Random.Range(0, puyoSpriteArray.Length));       
    }

    public void SetColorIdx(int index)
    {
        colorIdx = index;
        ChangeToOtherColor(colorIdx);
    }

    public int GetColorIdx()
    {
        return colorIdx;
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
                GameBoard.Clear(currentX, row + 1);
                GameBoard.Add(currentX, row, gameObject.transform);
                gameObject.transform.position += Vector3.down;
                yield return new WaitForSeconds(TimeToDropNextStep);
            }
            else
            {
                activelyFalling = false;
                forcedDownwards = false;
                break;
            }
        }
        forcedDownwards = false;
        activelyFalling = false;
    }

    public void DropToFloorExternal()
    {
        StartCoroutine(DropToFloor());
    }

    public void ChangeToOtherColor(int index)
    {
        colorIdx = index;
        GetComponent<SpriteRenderer>().sprite = puyoSpriteArray[colorIdx];
        foreach (SpriteSides spriteSide in puyoSidesSpriteRenderers)
        {
            if (spriteSide.side == PuyoSide.Left || spriteSide.side == PuyoSide.Right)
            {
                spriteSide.spriteRenderer.sprite = puyoCornersArray[colorIdx].puyoHorizontalSide;
            }
            else if (spriteSide.side == PuyoSide.Top || spriteSide.side == PuyoSide.Bot)
            {
                spriteSide.spriteRenderer.sprite = puyoCornersArray[colorIdx].puyoVerticalSide;

            }
        }

    }
    public Vector3 RoundVector(Vector3 vect)
    {
        return new Vector2(Mathf.Round(vect.x), Mathf.Round(vect.y));
    }

    public void EnlargeHands(float blocksRange)
    {
        Poofing = true;
        PuyoSide side = PuyoSide.Right;
        if (blocksRange < 0)
            side = PuyoSide.Left;
        foreach (SpriteSides spriteSide in puyoSidesSpriteRenderers)
        {
            if (side == spriteSide.side)
            {
                spriteSide.spriteRenderer.enabled = true;
                spriteSide.animator.SetTrigger(blocksRange.ToString() + "Block");
                StartCoroutine(WaitForPoofAnimation(spriteSide.animator.runtimeAnimatorController.animationClips[0].length));
            }
        }
    }
    private IEnumerator WaitForPoofAnimation(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        Poofing = false;
    }
    public Sprite GetRightCornerSprite()
    {
        return puyoCornersArray[colorIdx].puyoHorizontalSide;
    }
    public void EnlargeHands()
    {
        foreach (SpriteSides spriteSide in puyoSidesSpriteRenderers)
        {
            spriteSide.animator.SetTrigger("1Block");
        }
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

}
