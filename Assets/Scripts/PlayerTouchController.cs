using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTouchController : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
{
    [SerializeField] private Puyo puyo;
    private Vector2 direction;
    private Vector2 position;
    


    private void OnEnable()
    {
        PuyoSpawner.NewPuyo += UpdatePuyo;

    }

    private void OnDisable()
    {
        PuyoSpawner.NewPuyo -= UpdatePuyo;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        SwipeDetected(eventData.delta.x, eventData.delta.y);
        position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        
        if(Mathf.Abs(position.x - eventData.position.x)>130 && (direction!=Vector2.down && direction!=Vector2.up)) // рср еярэ уюпдйнд дю 
        {
            if ((position.x - eventData.position.x) < 0)
                puyo.MoveRight();
            else
                puyo.MoveLeft();
            position.x = eventData.position.x;
        }
    }

    private void SwipeDetected(float deltaX, float deltaY)
    {
        if (Mathf.Abs(deltaX) > Mathf.Abs(deltaY))
            direction = deltaX > 0 ? Vector2.right : Vector2.left;
        else
            direction = deltaY > 0 ? Vector2.up : Vector2.down;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (direction == Vector2.down)
        {
            puyo.MoveDown();
        }
        else if (direction == Vector2.up)
        {
            puyo.RotateRight();
        }
        direction = Vector2.zero;
    }

    private void UpdatePuyo(Puyo NewPuyo )
    {
        puyo = NewPuyo;
    }
}
