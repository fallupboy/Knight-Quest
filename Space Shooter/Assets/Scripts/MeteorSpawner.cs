using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [SerializeField] float minTimeBetweenSpawn = 1f;
    [SerializeField] float maxTimeBetweenSpawn = 6f;
    [SerializeField] GameObject[] meteorObject;
    [SerializeField] float meteorSpeed = 5f;

    float spawnCounter;
    int randMeteor;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountDownAndShot());
    }

    IEnumerator CountDownAndShot()
    {
        while (true)
        {
            spawnCounter = Random.Range(minTimeBetweenSpawn, maxTimeBetweenSpawn);
            yield return new WaitForSeconds(spawnCounter);
            Fire();
        }
    }

    private void Fire()
    {
        randMeteor = Random.Range(0, meteorObject.Length);
        float xPos = Random.Range(-5f, 5f);
        Vector2 desiredPos = new Vector2(xPos, transform.position.y);
        GameObject meteor = Instantiate(meteorObject[randMeteor], desiredPos, Quaternion.identity);
        meteor.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -meteorSpeed);
    }
}
