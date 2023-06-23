using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartSpawner : MonoBehaviour
{

    [SerializeField] private float minXToSpawn;
    [SerializeField] private float maxXToSpawn;
    [SerializeField] private float YToSpawn;
    [SerializeField] private GameObject heart;
    [SerializeField] private float timeToCreateNewHeart;
    private void Start()
    {
        StartCoroutine(HeartFall());
    }

    private IEnumerator HeartFall()
    {
        while(true)
        {
            Vector3 positionToSpawn = new Vector3(Random.Range(minXToSpawn, maxXToSpawn), YToSpawn, 0);
            Instantiate(heart, positionToSpawn,Quaternion.identity);
            yield return new WaitForSeconds(Random.Range(.1f, timeToCreateNewHeart));
        }
    }
}
