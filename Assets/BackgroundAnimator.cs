using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundAnimator : MonoBehaviour
{

    [SerializeField]
    GameObject[] spawnObjsArray;

    [SerializeField]
    GameObject[] spawnPositions;

    float lastTimeSpawned;

    float spawnDeltaTime;

    bool lockedSpawn;
    // Start is called before the first frame update
    void Start()
    {
        lastTimeSpawned = 0f;
        spawnDeltaTime = 3f;
        lockedSpawn = false;


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (lockedSpawn)
            return;
        if (lastTimeSpawned + spawnDeltaTime < Time.time)
        {
            lastTimeSpawned = Time.time;
            int randomChance = Random.Range(0, spawnObjsArray.Length + 1);
            if (randomChance < spawnObjsArray.Length)
            {
                int randPos = Random.Range(0, spawnPositions.Length - 1);
                GameObject go = Instantiate(spawnObjsArray[randomChance], spawnPositions[randPos].transform);
                go.AddComponent<BackgroundDecor>();
                Rigidbody rb = go.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.constraints = RigidbodyConstraints.FreezePositionY;
                rb.drag = 0;
                //lockedSpawn = true;
            }
        }
    }
}
