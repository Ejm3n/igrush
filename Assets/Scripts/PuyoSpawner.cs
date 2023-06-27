using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoSpawner : MonoBehaviour
{
    [SerializeField] private float timeToSpeedUpdate = 60;
    [SerializeField] private float PuyoSpeed = 1;
    [SerializeField] private float PuyoMinSpeed = 1;
    [SerializeField] private float PuyoMaxSpeed = 2;
    [SerializeField] private float PuyoSpeedStep = 0.25f;
    private Puyo activePuyo;
  
    // private Canvas gameOver; 

    void Start()
    {
        StartCoroutine(UpdatePuyoSpeed());
        SpawnPuyo();
    }

    public void SpawnPuyo(){
        if(GameBoard.WhatToDelete()){
            StartCoroutine(DelayDelete());
        }

        StartCoroutine(DelaySpawn());
    }

    private bool GameIsOver(){
        return 
            GameBoard.gameBoard[(int)transform.position.x, (int)transform.position.y] != null ||
            GameBoard.gameBoard[(int)transform.position.x + 1, (int)transform.position.y] != null;
    }

    IEnumerator DelayDelete(){
        GameBoard.DropAllColumns();
        yield return new WaitUntil(() => !GameBoard.AnyFallingBlocks());
        if(GameBoard.WhatToDelete()){
            StartCoroutine(DelayDelete());
        };

    }

    IEnumerator DelaySpawn(){
        yield return new WaitUntil(() => !GameBoard.AnyFallingBlocks() && !GameBoard.WhatToDelete());
        if(GameIsOver()){
            //GameObject.Find("GameOverCanvas").GetComponent<CanvasGroup>().alpha = 1;
            GameUIController.instance.SetEndCanvas(true);
            enabled = false; 
        } else {
            GameUIController.instance.ClearCombo();
            activePuyo = Instantiate((GameObject)Resources.Load("Puyo"), transform.position, Quaternion.identity).GetComponent<Puyo>();
            activePuyo.fallSpeed = PuyoSpeed;
        }
    }

    private IEnumerator UpdatePuyoSpeed()
    {
        bool downscaling = false;
        while(!GameIsOver())
        {
            yield return new WaitForSeconds(timeToSpeedUpdate);
            if (downscaling)
            {
                PuyoSpeed += PuyoSpeedStep;
                if(PuyoSpeed >= PuyoMaxSpeed)
                    downscaling = false;
            }
            else if(!downscaling)
            {
                PuyoSpeed -= PuyoSpeedStep;
                if (PuyoSpeed <= PuyoMinSpeed)
                    downscaling = true;
            }
            Debug.Log("puyo speed = " + PuyoSpeed);
            Debug.Log("downscaling? " + downscaling);

        }
        

    }

}