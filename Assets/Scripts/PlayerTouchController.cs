using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerTouchController : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private float defaultTimeToStartFalling = .8f;
    private float timeToStartFalling;
    private Puyo puyo;
    private Vector2 direction;
    private Vector2 position;
    private bool isDragging;
    private bool isMoving;// это для детекта что я веду в сторону и чтоб не срабатывал переворот после 

    private void OnEnable()
    {
        PuyoSpawner.NewPuyo += UpdatePuyo;
        timeToStartFalling = defaultTimeToStartFalling;
    }

    private void OnDisable()
    {
        PuyoSpawner.NewPuyo -= UpdatePuyo;
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Home) || Input.GetKeyUp(KeyCode.Menu))
        {
            GameUIController.instance.PauseGame();
        }
    }
    private void OnApplicationPause(bool pause)
    {
        //if(pause) 
        //    GameUIController.instance.PauseGame(pause);
    }
    private void MoveDown()
    {
        if (direction == Vector2.zero && puyo != null && puyo.CanBeMoved)
        {
            puyo.MoveDown();
        }
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //if(puyo!=null)
        if(!isDragging&& puyo != null && puyo.CanBeMoved && !isMoving)
            puyo.RotateRight();
            
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        isMoving = true;
        isDragging = true;
        SwipeDetected(eventData.delta.x, eventData.delta.y);
        position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (puyo != null && puyo.CanBeMoved)
        {
            if (Mathf.Abs(position.x - eventData.position.x) > 120 && (direction != Vector2.down && direction != Vector2.up)) // ��� ���� ������� �� 
            {
                if ((position.x - eventData.position.x) < 0)
                    puyo.MoveRight();
                else
                    puyo.MoveLeft();
                position.x = eventData.position.x;
            }

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
        Debug.Log("POSITION Y = " + (position.y - eventData.position.y));
        if(puyo!=null && puyo.CanBeMoved)
        {
            if (direction == Vector2.down && Mathf.Abs(position.y - eventData.position.y) > 120)
            {
                StartCoroutine(puyo.DropDown());
            }
            else if (direction == Vector2.up&& Mathf.Abs(position.y - eventData.position.y) > 150)
            {       
                puyo.RotateRight();
            }
                  
                isDragging = false;
        }
       
        direction = Vector2.zero;
        isMoving = false;
    }

    private void UpdatePuyo(Puyo NewPuyo )
    {
        isDragging = false;
        puyo = NewPuyo;
    }
}
