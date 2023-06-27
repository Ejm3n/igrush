using System.Collections;
using System.Collections.Generic;
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
        if(Input.GetKeyDown(KeyCode.A)){
            puyo.MoveLeft();
        } else if(Input.GetKeyDown(KeyCode.D)){
            puyo.MoveRight();
        } else if(Input.GetKey(KeyCode.S)){
            puyo.MoveDown();
        //} else if(Input.GetKeyDown(KeyCode.Q)){
        //    puyo.RotateLeft();
        } else if(Input.GetKeyDown(KeyCode.W)){
            puyo.RotateRight();
        }
    }
}
