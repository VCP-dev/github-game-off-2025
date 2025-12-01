using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemyvalues;

public class enemymanagerscript : MonoBehaviour
{

    public float sethealthamt;
    public GameObject spriteparent;
    public GameObject exclamationpoint;
    public GameObject aoecircle;
    public GameObject aoecircleeffect;
    public GameObject shadow;
    public float chargeattackradius;
    public float aoeattackradius;
    public LayerMask chargeattacklayers;
    public LayerMask aoeattacklayers;
    public slasheffectscript slash;
    public List<GameObject> bloodsplatter;
    public float floorcheckradius;
    public LayerMask floorchecklayer;
    public GameObject bracelet;
    public GameObject shield;
    public GameObject brokenshield;
    public GameObject oneshotoutline;
    public bool istutorial;

    // -------------------------- for retaliating after knockback --------------------------
    public bool attackafterknockback;
    public string attackafterknockbacktype;
    bool retaliate;
    // -------------------------- for retaliating after knockback --------------------------

    Cameramovementscript camscript;
    GameObject player;
    Bloodpoolerscript Bloodpooler;
    Enemydeadexplosionpoolerscript explosionpooler;
    Essencepoolerscript essencepooler;
    Oneshoteffectpoolerscript oneshotpooler;
    playerscript playerscript;
    enemyspawnerscript enemyspawner;
    timerscript timer;
    Animator anim;
    float offsetamt;


    [HideInInspector]
    public bool dead;
    [HideInInspector]
    public bool attacking;
    [HideInInspector]
    public bool charging;
    [HideInInspector]
    public bool knockedback;
    [HideInInspector]
    public bool oneshot;
    [HideInInspector]
    public Vector3 knockeddir;
    [HideInInspector]
    public Vector3 chargedir;
    [HideInInspector]
    public float knockedbackspeed;
    [HideInInspector]
    public float nextattack;
    [HideInInspector]
    public float nextrecover;
    [HideInInspector]
    public float health;
    [HideInInspector]
    public float maxhealth;
    [HideInInspector]
    public HandEnemyType enemytype;
    [HideInInspector]
    public Rigidbody rb;
    [HideInInspector]
    public float distlimit;
    [HideInInspector]
    public float speed;
    [HideInInspector]
    public Vector3 movedir;
    [HideInInspector]
    public bool grabbed;
    [HideInInspector]
    public bool shielded;
    [HideInInspector]
    public bool startenemy;
    [HideInInspector]
    public GameObject grabbedpos;

    void Awake()
    {
        camscript = GameObject.FindGameObjectWithTag("camparent").GetComponent<Cameramovementscript>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerscript = player.GetComponent<playerscript>();
        Bloodpooler = GameObject.FindGameObjectWithTag("Bloodpooler").GetComponent<Bloodpoolerscript>();
        explosionpooler = GameObject.FindGameObjectWithTag("Explosionpooler").GetComponent<Enemydeadexplosionpoolerscript>();
        essencepooler = GameObject.FindGameObjectWithTag("Essencepooler").GetComponent<Essencepoolerscript>();
        timer = GameObject.FindGameObjectWithTag("timer").GetComponent<timerscript>();
        oneshotpooler = GameObject.FindGameObjectWithTag("Oneshoteffectpooler").GetComponent<Oneshoteffectpoolerscript>();
        enemyspawner = GameObject.FindGameObjectWithTag("enemyspawner").GetComponent<enemyspawnerscript>();
        rb = GetComponent<Rigidbody>();
        anim = spriteparent.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        attacking = false;
        charging = false;
        knockedback = false;
        grabbed = false;
        retaliate = false;
        startenemy = false;
        exclamationpoint.SetActive(false);
        aoecircle.SetActive(false);
        aoecircleeffect.SetActive(false);
        brokenshield.SetActive(false);
        //shielded = (Random.Range(0f,1f) >= 0.5f);
        //shield.SetActive(shielded);
        nextattack = Time.time + Random.Range(0.3f,0.5f); 
        nextrecover = 0f;
        maxhealth = sethealthamt;
        health = (istutorial) ? 1f : maxhealth;
        /*oneshot = (Random.Range(0f,1f) >= 0.5f);
        if(oneshot){
            health = 1f;
        }*/
        knockedbackspeed = 2.5f;
        offsetamt = 0.038f;//0.068f;
        //enemytype = ((Random.Range(0f,1f) >= 0.5f) ? HandEnemyType.onlyCharge : HandEnemyType.onlyAOE);//true;
        //bracelet.SetActive(enemytype == HandEnemyType.onlyAOE);
        //distlimit = ((enemytype == HandEnemyType.onlyCharge) ? Random.Range(0.378f,0.428f) : Random.Range(0.403f,0.453f));
        movedir = (player.transform.position - transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if(grabbed){
            transform.position = grabbedpos.transform.position;
        }
        shield.SetActive(shielded);
        bracelet.SetActive(enemytype == HandEnemyType.onlyAOE);
        shadow.SetActive(!grabbed && checkfloor());
        oneshotoutline.SetActive(oneshot);
    }


    public void initialiseenemy(HandEnemyType type, bool hasshield, bool oneshotted = false)
    {
        dead = false;
        setenemydodge();
        startenemy = false;
        togglebloodsplatter(false);
        nextrecover = Time.time + 0.79f;
        anim.SetTrigger("initial");
        cancelvals();
        enemytype = type;
        bracelet.SetActive(enemytype == HandEnemyType.onlyAOE);
        speed = (enemytype == HandEnemyType.onlyCharge) ? 0.39f : 0.343f;//0.45f;//0.36f;
        shielded = hasshield;
        oneshot = oneshotted;
        health = oneshotted ? 1f : sethealthamt;//maxhealth;
        distlimit = ((enemytype == HandEnemyType.onlyCharge) ? Random.Range(0.378f,0.428f) : Random.Range(0.403f,0.453f));
        movedir = (player.transform.position - transform.position).normalized;
    }

    public void engageplayer()
    {
        nextrecover = Time.time + 0.79f;
        startenemy = true;
        setenemynotdodge();
    }

    // ----------------------------- attack functions ---------------------------
    public void startslash()
    {
        exclamationpoint.SetActive(true);
        rb.velocity = new Vector3(0,0,0);
        attacking=true;
    }

    public void startaoeattack()
    {
        exclamationpoint.SetActive(false);
        rb.velocity = new Vector3(0,0,0);
        attacking=true;
    }

    public void setdir()
    {
        rb.velocity = new Vector3(0,0,0);
        if(player!=null){
            chargedir = (player.transform.position - transform.position).normalized;
        }
    }

    public void charge()
    {
        activateslash(chargedir);
        exclamationpoint.SetActive(false);
        if(!dead){
            gameObject.layer = LayerMask.NameToLayer("charging");
        }
        charging=true;
    }

    public void setpostoplayer()
    {
        Vector3 targetspos = player.transform.position;
        transform.position = new Vector3(targetspos.x+Random.Range(-0.072f,0.072f),targetspos.y,targetspos.z+Random.Range(-0.072f,0.072f));
        aoecircle.SetActive(true);
    }

    public void aoeattack()
    {
        aoecircle.SetActive(false);
        aoecircleeffect.SetActive(true);
        aoecircleeffect.GetComponent<Animator>().SetTrigger("aoecircleeffect");
        camscript.camsmallshake();
        Soundmanagerscript.playsound("aoeattack");
        Collider[] collist = Physics.OverlapSphere(transform.position,aoeattackradius,aoeattacklayers);
        if(collist.Length>0)
        {
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="Player")
                {
                    col.gameObject.GetComponent<playerscript>().damaged(8.5f);
                    nextattack = Time.time + Random.Range(2.95f,3.1f);
                }
            }
        }
    }

    public void endcharge()
    {
        if(!dead){
            gameObject.layer = LayerMask.NameToLayer("enemy");
        }
        rb.velocity = new Vector3(0,0,0);
        charging=false;
    }

    public void endslash()
    {
        exclamationpoint.SetActive(false);
        aoecircle.SetActive(false);
        if(!dead){
            gameObject.layer = LayerMask.NameToLayer("enemy");
        }
        rb.velocity = new Vector3(0,0,0);
        charging=false;
        attacking=false;
        deactivateslash();
    }

    public void chargeattackfunc()
    {
        Collider[] collist = Physics.OverlapSphere(transform.position,chargeattackradius,chargeattacklayers);
        if(collist.Length>0)
        {
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="Player")
                {
                    col.gameObject.GetComponent<playerscript>().damaged(11.5f);
                    endslash();
                    nextattack = Time.time + Random.Range(1.98f,2.18f);
                }
            }
        }
    }

    public void cancelvals()
    {
        exclamationpoint.SetActive(false);
        aoecircle.SetActive(false);
        charging = false;
        attacking = false;
        deactivateslash();
    }

    public void activateslash(Vector3 dir)
    {
        if(slash){
            slash.activate(dir);
        }
    }

    public void deactivateslash()
    {
        if(slash){
            slash.deactivate();
        }
    }
    // ----------------------------- attack functions ---------------------------


    public void togglebloodsplatter(bool status)
    {
        for(int i=0;i<bloodsplatter.Count;i++)
        {
            bloodsplatter[i].SetActive(status);
        }
    }


    // ----------------------------- dodging functions ---------------------------
    public void setenemydodge()
    {
        gameObject.layer = LayerMask.NameToLayer("enemydodging");
    }

    public void setenemynotdodge()
    {
        gameObject.layer = LayerMask.NameToLayer("enemy");
    }
    // ----------------------------- dodging functions ---------------------------


    // ----------------------------- grabbed by player functions ---------------------------
    public void grabbedbyplayer(GameObject grabposition)
    {
        anim.SetTrigger("damaged");
        grabbedpos = grabposition;
        grabbed = true;
        gameObject.layer = LayerMask.NameToLayer("enemydodging");
        // adding a large delay just in case
        nextrecover = Time.time + 3f;
        nextattack = Time.time + 3f;
        nextrecover = Time.time + 3f;
        cancelvals();
        rb.velocity = new Vector3(0f,0f,0f);
    }

    /**
       Call this when the player takes damage while grabbing and slamming an enemy
    */
    public void droppedbyplayer()
    {
        transform.position = new Vector3(transform.position.x,0f,transform.position.z);
        gameObject.layer = LayerMask.NameToLayer("enemy");
        //nextrecover = Time.time + Random.Range(1.1f,1.2f);
        //nextattack = Time.time + Random.Range(1f,1.3f);
        //nextrecover = Time.time + 1.2f;
        grabbed = false;
        cancelvals();
        rb.velocity = new Vector3(0f,0f,0f);
    }

    public void thrownbyplayer(Vector3 dir, bool slammedbyplayer = false, bool hurtbyplayer = false, bool destroyshield = false)
    {
        cancelvals();
        anim.SetTrigger("damaged");
        transform.position = new Vector3(transform.position.x,0f,transform.position.z);
        nextrecover = Time.time + Random.Range(1.1f,1.2f);
        nextattack = Time.time + Random.Range(1f,1.3f);
        nextrecover = Time.time + 1.2f;
        grabbed = false;
        if(slammedbyplayer){
            retaliate = true;
            // should get hurt by a good amount, but maybe call it from the player when the enemy gets slammed
            togglebloodsplatter(true);
            spawnblood();
            Invoke("spawnblood",0.083f);
            // ------------------- oneshotted enemy when slammed should get insta-killed from this ------------------
            /*if(oneshot){
                cancelvals();
                knockedback = false;
                // not sure yet if you should get lots of health, or lots of super meter tbh
                playerscript.increasehealth(50.85f);
                explosionpooler.spawnexplosion(transform.position);
                spawnblood();
                spawnblood();
                gameObject.SetActive(false);
                camscript.essencepanelsetamt(26.5f);
                return;
            }*/
            // ------------------- oneshotted enemy when slammed should get insta-killed from this ------------------
        }else{
            // hurt somewhat by AOE attack from slamming grabbed enemy
            if(hurtbyplayer){
                togglebloodsplatter(true);
                reducehealth(0.5f);
                Invoke("spawnblood",0.083f);
                Soundmanagerscript.playsound("enemydamaged");
            }
            // knockedback from activating super form
            else if(destroyshield){
                breakshield();
            }
        }
        knockbackenemy(dir);
    }
    // ----------------------------- grabbed by player functions ---------------------------

    public void spawnbloodatposition()
    {
        offsetamt = Random.Range(0.07f,0.083f);
        spawnblood();
    }

    // called when an enemy gets slashed
    public void damaged(Vector3 dir, float amt = 1f)
    {
        retaliate = true;
        anim.SetTrigger("damaged");
        togglebloodsplatter(true);
        breakshield();
        nextrecover = Time.time + 1.1f;
        nextattack += 1f;
        endslash();
        reducehealth(amt);
        knockbackenemy(dir);
        offsetamt = Random.Range(0.033f,0.044f);//Random.Range(0.07f,0.083f);
        spawnblood();
        Invoke("spawnblood",0.083f);
        Soundmanagerscript.playsound("enemydamaged");
    }

    public void reducehealth(float amt)
    {
        if(dead){
            return;
        }
        health = (((health-amt) <= 0f) ? (0f) : (health-amt));
        if(health < 1f)
        {
            dead = true;
        }
    }

    void knockbackenemy(Vector3 dir)
    {
        knockeddir = dir;
        knockedback = true;
        gameObject.layer = LayerMask.NameToLayer("enemydodging");
        if(IsInvoking("stopknockback")){
            CancelInvoke("stopknockback");
        }
        Invoke("stopknockback",0.185f);
    }

    void stopknockback()
    {
        knockedback = false;
        gameObject.layer = LayerMask.NameToLayer("enemy");
        if(dead){
            killenemy();
        } 
        else if(retaliate && attackafterknockback && attackafterknockbacktype != ""){
            // maybe just have only the charge attack ? idk
            anim.SetTrigger(attackafterknockbacktype);
        }
        retaliate = false;
    }

    public void killenemy()
    {
        // kill an enemy properly here
        // spawn an explosion effect or something as well
        // -------------------------------------------------------------------------
        camscript.incrementenemydeathcount();
        cancelvals();
        spawnblood();
        Soundmanagerscript.playsound("enemydamaged");
        explosionpooler.spawnexplosion(transform.position);
        playerscript.increasesuper(checkfloor() ? 1f : 2.5f);
        if(checkfloor()){
            essencepooler.spawnessence(transform.position);
            timer.alterscoreval(100f);
        }else{
            playerscript.increasehealth(7.85f);
            timer.alterscoreval(1000f);
            //timer.altertimerval(2f);
        }
        gameObject.SetActive(false);
        if(oneshot){
            oneshotpooler.spawnoneshoteffect(transform.position);
            Soundmanagerscript.playsound("enemyexplode");
        }
        if(istutorial){
            // start game here
            enemyspawner.initiate();
        }
    }

    public void breakshield()
    {
        if(shielded){
            brokenshield.SetActive(true);
        }
        shielded = false;
    }

    public bool isplayerdead()
    {
        return playerscript.dead;
    }

    public void spawnblood()
    {
        if(!checkfloor()){
            return;
        }
        float amt = (offsetamt);
        Vector3 basepos = transform.position;
        Bloodpooler.spawnbloodeffect(new Vector3(basepos.x+(Random.Range(amt,-amt)),0f,basepos.z+(Random.Range(amt,-amt))));
        Bloodpooler.spawnbloodeffect(new Vector3(basepos.x+(Random.Range(amt,-amt)),0f,basepos.z+(Random.Range(amt,-amt))));
    }

    bool checkfloor()
    {
        return (Physics.OverlapSphere(transform.position,floorcheckradius,floorchecklayer).Length > 0);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, chargeattackradius);
        Gizmos.DrawWireSphere(transform.position, aoeattackradius);
        Gizmos.DrawWireSphere(transform.position, floorcheckradius);
    }
}
