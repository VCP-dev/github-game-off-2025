using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemydeadexplosionpoolerscript : PoolerScript
{

    public GameObject explosioneffect;

    // Start is called before the first frame update
    void Start()
    {
        objectToPool = explosioneffect;
        amountToPool = 50;
        InitialisePooler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnexplosion(Vector3 pos)
    {
        GameObject spawnedexplosion = GetPooledObject();
        if(spawnedexplosion){
            spawnedexplosion.transform.position = pos;
            spawnedexplosion.SetActive(true);
        }
    }
}
