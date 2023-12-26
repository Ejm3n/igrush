using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoSpawner : MonoBehaviour
{
    public static Action<Puyo> NewPuyo;
    [SerializeField] private float timeToSpeedUpdate = 60;
    [SerializeField] private float PuyoSpeed = 1;
    [SerializeField] private float PuyoMinSpeed = 1;
    [SerializeField] private float PuyoMaxSpeed = 2;
    [SerializeField] private float PuyoSpeedStep = 0.25f;
    [SerializeField] private float delaySpawn = 0.3f;
    private Puyo activePuyo;
    private bool downscaling = false;
    private float timer;
   
    // private Canvas gameOver; 

    void Start()
    {
        SpawnPuyo();
        timer = timeToSpeedUpdate;
    }

    private void Update()
    {
        if (!GameIsOver())
        {
            UpdatePuyoSpeed();
        }

    }

    public void SpawnPuyo()
    {
        if (GameBoard.WhatToDelete())
        {
            StartCoroutine(DelayDelete());
        }

        StartCoroutine(DelaySpawn());
    }

    private bool GameIsOver()
    {
        return
            GameBoard.gameBoard[(int)transform.position.x, (int)transform.position.y] != null ||
            GameBoard.gameBoard[(int)transform.position.x + 1, (int)transform.position.y] != null;
    }


    IEnumerator DelayDelete()
    {
        GameBoard.DropAllColumns();
        yield return new WaitUntil(() => !GameBoard.AnyFallingBlocks());
        if (GameBoard.WhatToDelete())
        {
            StartCoroutine(DelayDelete());
        };

    }

    IEnumerator DelaySpawn()
    {
        yield return new WaitUntil(() => !GameBoard.AnyFallingBlocks() && !GameBoard.WhatToDelete() && PoofsFinished());
        yield return new WaitForSeconds(delaySpawn);
        if (GameIsOver())
        {
            //GameObject.Find("GameOverCanvas").GetComponent<CanvasGroup>().alpha = 1;
            SoundManager.Instance.ChangeMusicToEnd();
            GameUIController.instance.SetEndCanvas(true);
            enabled = false;
        }
        else
        {
            StartCoroutine(GameUIController.instance.ClearCombo());
            activePuyo = Instantiate((GameObject)Resources.Load("Puyo"), transform.position, Quaternion.identity).GetComponent<Puyo>();
            activePuyo.fallSpeed = PuyoSpeed;
            NewPuyo?.Invoke(activePuyo);
        }
    }
    private bool PoofsFinished()
    {
        List< PuyoUnit> puyoUnits = GameBoard.GetPuyoUnits();
        foreach( PuyoUnit unit in puyoUnits )
                {
            if(unit.Poofing)
                return false;
        }
        return true;
    }
    private void UpdatePuyoSpeed()
    {
        if (timer <= 0)
        {
            if (downscaling)
            {
                PuyoSpeed += PuyoSpeedStep;
                if (PuyoSpeed >= PuyoMaxSpeed)
                    downscaling = false;
            }
            else if (!downscaling)
            {
                PuyoSpeed -= PuyoSpeedStep;
                if (PuyoSpeed <= PuyoMinSpeed)
                    downscaling = true;
            }
            GameUIController.instance.ChangeBGSprites();
            timer = timeToSpeedUpdate;
        }
        //yield return new WaitForSeconds(timeToSpeedUpdate);
        else
        {
            timer -= Time.deltaTime;
        }
    }
  

    /// <summary>
    /// старый метод почемуто в один момент переставал работать тут возникает вопрос какова хуя
    /// </summary>
    /// <returns></returns>
    //private IEnumerator UpdatePuyoSpeed()
    //{
    //    bool downscaling = false;
    //    while(!GameIsOver())
    //    {
    //        yield return new WaitForSeconds(timeToSpeedUpdate);
    //        if (downscaling)
    //        {
    //            PuyoSpeed += PuyoSpeedStep;
    //            if(PuyoSpeed >= PuyoMaxSpeed)
    //                downscaling = false;
    //        }
    //        else if(!downscaling)
    //        {
    //            PuyoSpeed -= PuyoSpeedStep;
    //            if (PuyoSpeed <= PuyoMinSpeed)
    //                downscaling = true;
    //        }
    //        GameUIController.instance.ChangeBGSprites();
    //        Debug.Log("puyo speed = " + PuyoSpeed);
    //        Debug.Log("downscaling? " + downscaling);
    //    }
    //}
    //private IEnumerator PoofPuyos(float spawnTansform, float target, float yPos, Sprite sprite)
    //{
    //    bool HalfTarget = true;
    //    poofsFinished = false;
    //    if (target == MathF.Round(target))
    //        HalfTarget = false;
    //    if (spawnTansform - target < 0)//тогда влево запускать
    //    {

    //    }
    //    else
    //    {
    //        for (int i = (int)spawnTansform; i < (int)MathF.Round(target); i++)
    //        {
    //            HandChecker handChecker = Instantiate((GameObject)Resources.Load("HandChecker"), new Vector2(i, yPos), Quaternion.identity).GetComponent<HandChecker>();
    //            handChecker.SetHandChecker(sprite, HandCheckerState.FullRight);
    //            yield return new WaitForSeconds(handChecker.ReturnAnimLength());
    //        }
    //        if (HalfTarget)
    //        {
    //            HandChecker handChecker = Instantiate((GameObject)Resources.Load("HandChecker"), new Vector2(target - .5f, yPos), Quaternion.identity).GetComponent<HandChecker>();
    //            handChecker.SetHandChecker(sprite, HandCheckerState.HalfRight);
    //            yield return new WaitForSeconds(handChecker.ReturnAnimLength());
    //        }
    //    }
    //    poofsFinished = true;

    //}
}