using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSSpawner : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    private Vector2 mousePosition;



    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Instantiate(prefabToSpawn,mousePosition,Quaternion.identity);
        }
    }
}
