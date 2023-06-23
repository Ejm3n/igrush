using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartFalling : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float minSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float minScale;
    [SerializeField] private float maxScale;
    private Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        currentSpeed = Random.Range(minSpeed, maxSpeed) / 100;
        transform.Rotate(new Vector3(-90, 0,0)); // да мне поебать, хз почему он некорректно спавнился
        ScaleCounter();
        ColorRandomizer();
        Destroy(gameObject,10f);
    }
    private void Update()
    {
        transform.Translate(Vector3.back * Time.deltaTime * currentSpeed );
    }

    private void ScaleCounter()
    {
        float largeVar = Random.Range(minScale, maxScale);
        float lowerVar = Random.Range(minScale, maxScale);
        if(largeVar<lowerVar)
        {
            float x = largeVar;
            largeVar = lowerVar;
            lowerVar = x;
        }
        gameObject.GetComponent<AnimationScript>().startScale = new Vector3(lowerVar, lowerVar, lowerVar);
        gameObject.GetComponent<AnimationScript>().endScale = new Vector3(largeVar, largeVar, largeVar);
    }

    private void ColorRandomizer()
    {
        Color color = new Color(Random.Range(0f, 1), Random.Range(0f, 1), Random.Range(0f, 1));
        renderer.material.color =  color;
        
    }
}
