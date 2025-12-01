using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemyvalues;

public class Enemypoolerscript : PoolerScript
{
    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        objectToPool = enemy;
        amountToPool = 90;
        InitialisePooler();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void spawnenemy(Vector3 pos, HandEnemyType type, bool hasshield, bool oneshotted = false)
    {
        GameObject spawnedenemy = GetPooledObject();
        if(spawnedenemy){
            spawnedenemy.transform.position = pos;
            enemymanagerscript script = spawnedenemy.GetComponent<enemymanagerscript>();
            script.health = script.maxhealth;
            spawnedenemy.SetActive(true);
            script.initialiseenemy(type, hasshield, oneshotted);
        }
    }
}
