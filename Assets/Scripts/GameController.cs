using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public float fallSpeed;
    [SerializeField] private float stepToAdd; 

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }



}
