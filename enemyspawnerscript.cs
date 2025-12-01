using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemyvalues;

public class enemyspawnerscript : MonoBehaviour
{

    public GameObject music;
    public GameObject tutorial;

    [HideInInspector]
    public bool start;
    [HideInInspector]
    public int wave;
    [HideInInspector]
    public int enemydeathlimit;

    GameObject[] spawnpositionsarray;
    GameObject player;
    float nextspawn;
    Cameramovementscript camscript;

    int enemylimit;
    int nextoneshotenemy;
    float spawnrate;
    float spawndelayuntillessenemies;
    bool chargingenemies;
    bool randomspawn;
    bool hasshield;
    bool randomshield;

    Enemypoolerscript enemypooler;
    Essencepoolerscript essencepooler;
    timerscript timer;

    // Start is called before the first frame update
    void Start()
    {
        spawnpositionsarray = GameObject.FindGameObjectsWithTag("enemyspawnpos");
        player = GameObject.FindGameObjectWithTag("Player");
        enemypooler = GameObject.FindGameObjectWithTag("enemypooler").GetComponent<Enemypoolerscript>();
        camscript = GameObject.FindGameObjectWithTag("camparent").GetComponent<Cameramovementscript>();
        essencepooler = GameObject.FindGameObjectWithTag("Essencepooler").GetComponent<Essencepoolerscript>();
        timer = GameObject.FindGameObjectWithTag("timer").GetComponent<timerscript>();
        nextspawn = 0f;
        start = false;
        enemylimit = 3;
        spawnrate = 0.8f;
        spawndelayuntillessenemies = 1.26f;
        chargingenemies = true;
        randomspawn = false;
        hasshield = false;
        randomshield = false;
        nextoneshotenemy = Random.Range(8,10);
        wave = 1;
        enemydeathlimit = 60;
        music.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(!start){
            return;
        }

        //Debug.Log("chargingenemies: "+chargingenemies+", randomspawn: "+randomspawn+", hasshield: "+hasshield+", randomshield: "+randomshield);

        setenemylimit();

        int enemycount=0;
        foreach(GameObject en in GameObject.FindGameObjectsWithTag("enemy")){
            if(en.activeSelf){
                enemycount+=1;
            }
        }
        if(enemycount<enemylimit){
            if(Time.time > nextspawn){
                spawnenemy();
                nextspawn = Time.time + spawnrate;
            }
        }else{
            nextspawn = Time.time + spawndelayuntillessenemies;
        }
    }

    void setenemylimit()
    {

        // add a wave count for each part just to state which wave the player reached in the gameover screen
        // also state the number of enemies killed in that screen

        if(camscript.enemydeathcount<4){
            wave = 1;
            chargingenemies = true;
            randomspawn = false;
            hasshield = false;
            randomshield = false;
            enemylimit = 5;
        }else if(camscript.enemydeathcount<15){
            wave = 2;
            enemylimit = 6;
            if(camscript.enemydeathcount>=7){
                chargingenemies = false;
                spawnrate = 0.767f;
                spawndelayuntillessenemies = 1.18f;
            }
        }else if(camscript.enemydeathcount<24){
            wave = 3;
            randomspawn = true;
            enemylimit = 7;
            if(camscript.enemydeathcount>=18){
                hasshield = true;
                spawnrate = 0.734f;
                spawndelayuntillessenemies = 1.15f;
            }else{
                hasshield = false;
            }
        }else if(camscript.enemydeathcount<38){
            wave = 4;
            randomshield = true;
            hasshield = false;
            //chargingenemies = true;
            randomspawn = true;
            enemylimit = 8;       
            spawnrate = 0.717f;
            spawndelayuntillessenemies = 0.98f;    
        }else if(camscript.enemydeathcount<50){
            wave = 5;
            //randomspawn = true;
            enemylimit = 9;       
            spawnrate = 0.698f;
            spawndelayuntillessenemies = 0.95f;
        }else if(camscript.enemydeathcount<67){
            spawnrate = 0.687f;
            enemylimit = 10;
        }else if(camscript.enemydeathcount<82){
            enemylimit = 11;
        }else if(camscript.enemydeathcount<100){
            spawnrate = 0.686f;
            spawndelayuntillessenemies = 0.92f;
        }
    }

    public void spawnenemy()
    {
        List<GameObject> spawnpositions = new List<GameObject>(spawnpositionsarray);
        List<GameObject> newpositions = new List<GameObject>();
        foreach(GameObject pos in spawnpositions){
            if(Vector3.Distance(pos.transform.position,player.transform.position)>=0.65f){
                newpositions.Add(pos);
            }
        }
        Vector3 spawnpos = (newpositions.Count > 0 ? (newpositions[Random.Range(0,newpositions.Count-1)].transform.position) : (spawnpositions[Random.Range(0,spawnpositions.Count-1)].transform.position));
        bool oneshotenemy = false;
        // Also maybe add a check for if an explosive enemy is currently present or not maybe, idk
        if(camscript.enemydeathcount >= nextoneshotenemy){
            nextoneshotenemy += Random.Range(8,10);
            oneshotenemy = true;
        }
        HandEnemyType type = (randomspawn ? ((Random.Range(0f,1f) >= 0.39f) ? HandEnemyType.onlyCharge : HandEnemyType.onlyAOE ) : (chargingenemies ? HandEnemyType.onlyCharge : HandEnemyType.onlyAOE ));
        bool shield = (!oneshotenemy && (randomshield ? (Random.Range(0f,1f) >= 0.39f) : hasshield));
        enemypooler.spawnenemy(spawnpos, type, shield, oneshotenemy);
    }

    public void end(bool won = false)
    {
        start = false;
        if(won){
            enemypooler.ResetPool();
            essencepooler.ResetPool();
        }
        timer.countdown = false;
        timer.gameover(won);
        endmusic();
    }

    public void initiate()
    {
        start = true;
        timer.countdown = true;
        nextspawn = Time.time + 1f;
        camscript.starttime = Time.time;
        music.SetActive(true);
        tutorial.SetActive(false);
    }

    public void endmusic()
    {
        music.SetActive(false);
    }
}
