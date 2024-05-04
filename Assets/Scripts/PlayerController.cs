using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Puyo puyo;

    void Start()
    {
        puyo = GetComponent<Puyo>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            puyo.MoveLeft();
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            puyo.MoveRight();
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
           StartCoroutine( puyo.DropDown());
            //} else if(Input.GetKeyDown(KeyCode.Q)){
            //    puyo.RotateLeft();
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            puyo.RotateRight();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameUIController.instance.PauseGame();
        }
    }
}
