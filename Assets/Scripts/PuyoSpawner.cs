using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuyoSpawner : MonoBehaviour
{
    public static Action<Puyo> NewPuyo;
    public static bool Ongoing = false;
    public int Difficulty = 0;
    [SerializeField] private float timeToSpeedUpdate = 60;
    [SerializeField] private float PuyoSpeed = 1;
    [SerializeField] private float PuyoMinSpeed = 1;
    [SerializeField] private float PuyoMaxSpeed = 2;
    [SerializeField] private float PuyoSpeedStep = 0.25f;
    [SerializeField] private float delaySpawn = 0.3f;
    private Puyo activePuyo;
    private int[] nextPuyoColors = new int[2] { 0, 0 };
    private bool downscaling = false;
    private float timer;
    private static bool comboThisRound;
    // private Canvas gameOver; 

    void OnEnable()
    {
        SetNextPuyos();
        SpawnPuyo();
        timer = timeToSpeedUpdate;
        Ongoing = true;
    }
    private void OnDisable()
    {
        Ongoing = false ;
    }

    private void Update()
    {
        UpdatePuyoSpeed();
    }

    public void SpawnPuyo()
    {
        if (GameBoard.WhatToDelete())
        {
            StartCoroutine(DelayDelete());
        }

        StartCoroutine(DelaySpawn());
    }
    public static void ComboedThisRound(bool comboed)
    {
        if (!comboThisRound)
            comboThisRound = true;
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
        yield return new WaitForSecondsRealtime(delaySpawn);
        if (GameIsOver())
        {
            Debug.Log("Game is over delay spawn");
            //GameObject.Find("GameOverCanvas").GetComponent<CanvasGroup>().alpha = 1;

            SoundManager.Instance.ChangeMusicToEnd();
            GameUIController.instance.SetEndCanvas(true);
            // GameBoard.DeleteAllPuyos();
            enabled = false;
        }
        else
        {
            //if combo than keep combo           
            if (!comboThisRound)
                StartCoroutine(GameUIController.instance.ClearCombo());

            GameUIController.instance.SetLastRoundComboed(comboThisRound);
            comboThisRound = false;
            activePuyo = Instantiate((GameObject)Resources.Load("Puyo"), transform.position, Quaternion.identity).GetComponent<Puyo>();
            activePuyo.SetPuyosColors(nextPuyoColors[0], nextPuyoColors[1]);
            SetNextPuyos();
            activePuyo.fallSpeed = PuyoSpeed;
            NewPuyo?.Invoke(activePuyo);
        }
    }
    private bool PoofsFinished()
    {
        List<PuyoUnit> puyoUnits = GameBoard.GetPuyoUnits();
        foreach (PuyoUnit unit in puyoUnits)
        {
            if (unit.Poofing)
                return false;
        }
        return true;
    }
    private void UpdatePuyoSpeed()
    {
        if (timer <= 0)
        {
            NextPuyoSpeed();
            timer = timeToSpeedUpdate;
        }
        else
        {
            timer -= Time.deltaTime;
        }
    }
    public void NextPuyoSpeed()
    {
        Difficulty++;
        PuyoSpeed -= PuyoSpeedStep;
        if (PuyoSpeed <= PuyoMinSpeed)
        {
            PuyoSpeed = PuyoMaxSpeed;
            Difficulty = 0;
        }

        GameUIController.instance.ChangeBGSprites();
        //yield return new WaitForSeconds(timeToSpeedUpdate);
    }
    private void SetNextPuyos()
    {
        nextPuyoColors[0] = UnityEngine.Random.Range(0, GameBoard.totalColors);
        nextPuyoColors[1] = UnityEngine.Random.Range(0, GameBoard.totalColors);
        GameUIController.instance.UpdateNextPyuos(nextPuyoColors[0], nextPuyoColors[1]);

    }
   
}