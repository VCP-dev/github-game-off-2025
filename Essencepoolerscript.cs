using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Essencepoolerscript : PoolerScript
{

    public GameObject essence;

    // Start is called before the first frame update
    void Start()
    {
        objectToPool = essence;
        amountToPool = 100;
        InitialisePooler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnessence(Vector3 pos)
    {
        GameObject spawnedssence = GetPooledObject();
        if(spawnedssence){
            spawnedssence.transform.position = pos;
            spawnedssence.SetActive(true);
            spawnedssence.GetComponent<essencescript>().initialise();
        }
    }
}
