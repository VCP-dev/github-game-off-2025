using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodpoolerscript : PoolerScript
{

    public GameObject bloodeffect;

    // Start is called before the first frame update
    void Start()
    {
        objectToPool = bloodeffect;
        amountToPool = 200;
        InitialisePooler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void spawnbloodeffect(Vector3 pos)
    {
        GameObject spawnedblood = GetPooledObject();
        if(spawnedblood){
            spawnedblood.transform.position = pos;
            spawnedblood.GetComponent<Bloodeffectscript>().initialise();
            spawnedblood.SetActive(true);
        }
    }
}
