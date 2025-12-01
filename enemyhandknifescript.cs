using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemyvalues;

public class enemyhandknifescript : MonoBehaviour
{

    public GameObject spriteparent;
    public GameObject bloodsprite;
    public List<Sprite> bloodforhand;

    GameObject player;
    Rigidbody rb;
    enemymanagerscript enemymanager;
    Animator anim;

    float damping;
    //float speed;
    float chargespeed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody>();
        enemymanager = GetComponent<enemymanagerscript>();
        anim = spriteparent.GetComponent<Animator>();
        damping = 0.45f;//1.05f;//2.2f;
        //speed = (enemymanager.enemytype == HandEnemyType.onlyCharge) ? 0.39f : 0.343f;//0.45f;//0.36f;
        chargespeed = 1.81f;
    }

    // Update is called once per frame
    void Update()
    {
        setbloodsprite();
        if(/*!enemymanager.start ||*/!enemymanager.startenemy || enemymanager.isplayerdead() || enemymanager.dead || !enemymanager.knockedback || enemymanager.grabbed /*|| enemymanager.firstenemy*/){
            rb.velocity = new Vector3(0f,0f,0f);
            return;
        }
    }

    void FixedUpdate()
    {
        if(enemymanager.isplayerdead() || enemymanager.grabbed /*|| !enemymanager.start*/){
            rb.velocity = new Vector3(0f,0f,0f);
            return;
        }

        if(enemymanager.knockedback){
            enemymanager.cancelvals();
            rb.velocity = enemymanager.knockeddir * enemymanager.knockedbackspeed;
            return;
        }

        if(enemymanager.charging){
            rb.velocity = chargespeed * enemymanager.chargedir;
            enemymanager.chargeattackfunc();
            return;
        }

        if(Time.time < enemymanager.nextrecover || enemymanager.istutorial){
            return;
        }

        if(Vector3.Distance(transform.position,player.transform.position)>enemymanager.distlimit && !enemymanager.attacking)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            enemymanager.movedir = Vector3.Lerp(enemymanager.movedir,dir,Time.deltaTime * damping);
            rb.velocity = enemymanager.movedir * enemymanager.speed; // * Time.deltaTime;
        }
        else if(!enemymanager.attacking && Time.time > enemymanager.nextattack)
        {
            attack();
        }
        else
        {
            rb.velocity = new Vector3(0,0,0);
        }
    }


    void setbloodsprite()
    {
        if(bloodforhand.Count == 0){
            return;
        }
        int j=1;
        float healthamt = enemymanager.health/enemymanager.maxhealth;
        float iteramt = 1f/bloodforhand.Count;
        //Debug.Log(iteramt);
        for(float i=1f;;j+=1,i-=iteramt/*0.25f*/){
            if(i<=healthamt){
                break;
            }
        }
        int index = ((j-1) > -1) ? (j>bloodforhand.Count ? (bloodforhand.Count-1) : (j-1)) : j;
        bloodsprite.GetComponent<SpriteRenderer>().sprite = bloodforhand[index];
    }

    void attack()
    {
        switch(enemymanager.enemytype)
        {
            case HandEnemyType.onlyCharge:
                enemymanager.nextattack = Time.time + Random.Range(2.45f,2.65f);//Random.Range(2.25f,2.45f);
                anim.SetTrigger("chargeattack");
                break;
            case HandEnemyType.onlyAOE:
                enemymanager.nextattack = Time.time + Random.Range(3.15f,3.35f);//Random.Range(2.95f,3.15f);
                anim.SetTrigger("aoeattack");
                break;
            case HandEnemyType.both:
                // only if this enemy will actually get added
                break;
        }
    }
}
