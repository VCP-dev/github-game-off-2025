using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oneshoteffectpoolerscript : PoolerScript
{

    public GameObject oneshot;

    // Start is called before the first frame update
    void Start()
    {
        objectToPool = oneshot;
        amountToPool = 50;
        InitialisePooler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnoneshoteffect(Vector3 pos)
    {
        GameObject spawnedeffect = GetPooledObject();
        if(spawnedeffect){
            spawnedeffect.transform.position = pos;
            spawnedeffect.SetActive(true);
            spawnedeffect.GetComponent<oneshoteffectscript>().explode();
        }
    }
}
