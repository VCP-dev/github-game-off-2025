using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerscript : MonoBehaviour
{

    [HideInInspector]
    public bool dead;
    [HideInInspector]
    public bool dodging;
    [HideInInspector]
    public bool attacking;
    [HideInInspector]
    public bool facingright;
    [HideInInspector]
    public Vector3 direction;
    [HideInInspector]
    public bool playerdead;
    [HideInInspector]
    public bool grabbing;
    [HideInInspector]
    public bool specialstate;  // essentially devil trigger
    [HideInInspector]
    public bool specialready;
    [HideInInspector]
    public float health;
    [HideInInspector]
    public float specialamt;
    [HideInInspector]
    public float maxspecial;
    [HideInInspector]
    public List<Transform> surroundingEnemies;
    [HideInInspector]
    public GameObject grabbedenemy;

    public GameObject sprite;
    public GameObject playerParent;
    public LayerMask enemyLayers;
    public LayerMask surroundEnemyLayers;
    public float surroundingEnemyRadius;
    public slasheffectscript slash;
    public slasheffectscript superslash;
    public GameObject attackpos;
    public GameObject superattackpos;
    public float attackradius; // 0.14f
    public float specialattackradius; // 0.195f;
    public GameObject grabposparent;
    public GameObject grabenemypos;
    public float grabenemyradius;
    public GameObject grabpos1;
    public GameObject grabpos2;
    public GameObject aoecircleeffect;
    public float slamradius;
    public GameObject slampos;
    public GameObject slamparent;
    public GameObject lookatparent;
    public GameObject lookatpos;
    public List<GameObject> supercircleeffects;
    public GameObject superformcircle;
    public float superradius;
    public Image specialbar;
    public Image healtbar;
    public Image specialbarbackground;

    Color specialidlecolor;
    Color specialreadycolor;
    Color origcolor;
    Color specialcolor;
    
    Animator anim;
    Animator specialbarbackgroundanim;
    float maxhealth;
    Rigidbody rb;
    float horizontal;
    float vertical;
    float speed;
    float dodgespeed;
    float nextrecover;
    float nextattack;
    float nextcamfocus;
    float nextdodge;
    float delaytomovement;
    float nextgrab;
    float nextspecial;
    Vector3 dodgedir;
    Vector3 localscale;
    Vector3 facingDir;
    Vector3 throwDir;
    GameObject camparent;
    Cameramovementscript camscript;
    Bloodpoolerscript Bloodpooler;
    enemyspawnerscript enemyspawner;
    timerscript timer;


    // Start is called before the first frame update
    void Start()
    {
        surroundingEnemies = new List<Transform>();
        dead = false;
        playerdead = false;
        dodging = false;
        attacking = false;
        facingright = false;
        grabbing = false;
        specialstate = false;
        specialready = false;
        health = maxhealth = 100f;
        specialamt = 0f;
        maxspecial = 100f;
        camparent = GameObject.FindGameObjectWithTag("camparent");
        camscript = camparent.GetComponent<Cameramovementscript>();
        Bloodpooler = GameObject.FindGameObjectWithTag("Bloodpooler").GetComponent<Bloodpoolerscript>();
        timer = GameObject.FindGameObjectWithTag("timer").GetComponent<timerscript>();
        enemyspawner = GameObject.FindGameObjectWithTag("enemyspawner").GetComponent<enemyspawnerscript>();
        rb = GetComponent<Rigidbody>();
        anim = sprite.GetComponent<Animator>();
        aoecircleeffect.SetActive(false);
        supercircles(false);
        superformcircle.SetActive(false);
        localscale = playerParent.transform.localScale;
        speed = 0.81f;//0.79f;
        dodgespeed = 1.673f;
        nextrecover = 0f;
        nextattack = 0f;
        grabposparent.transform.forward = facingDir = transform.forward;
        nextcamfocus = 0f;
        nextdodge = 0f;
        nextgrab = 0f;
        nextspecial = 0f;
        delaytomovement = 0f;
        grabbedenemy = null;

        specialbarbackgroundanim = specialbarbackground.GetComponent<Animator>();
        specialidlecolor = new Color32(0,98,76,255);
        specialreadycolor = new Color32(232,0,0,255);

        origcolor = sprite.GetComponent<SpriteRenderer>().color;
        specialcolor = new Color32(255,0,144,255);
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log("attacking: "+attacking+", dodging: "+dodging+", grabbing: "+grabbing+", grabbedenemy : "+grabbedenemy);
        specialbar.fillAmount = (specialamt/maxspecial);
        healtbar.fillAmount = (health/maxhealth);

        specialready = ((specialamt >= (0.98f * maxspecial)) && !specialstate);
        specialbarbackground.color = specialready ? specialreadycolor : specialidlecolor;
        specialbarbackgroundanim.SetBool("specialready",specialready);

        if(specialstate && !dead){
            specialamt -= (10f*Time.deltaTime);
            if(specialamt <= 0f){
                specialstate = false;
            }
        }

        superformcircle.SetActive(specialstate && !dead);
        sprite.GetComponent<SpriteRenderer>().color = specialstate ? specialcolor : origcolor;

        if(dead){
            surroundingEnemies.Clear();
            camscript.SetCameraTargets(surroundingEnemies);
            anim.SetBool("move",false);
            if(!playerdead){
                spawnblood(transform.position);
                playerdead = true;
                timer.timerstatus(false);
                enemyspawner.end(false);
                anim.SetTrigger("dead");
                Soundmanagerscript.playsound("playerdeath");
            }
            return;
        }

        SurroundingEnemyDetector();

        lookatparent.transform.forward = direction.magnitude > 0 ? direction.normalized : facingDir;

        // placing input for super form in here
        // to ensure that a player can do it even if they were being damaged by enemies
        otherinputs();

        if(Time.time < nextrecover){
            return;
        }

        PCmovement();
        PCactions();

        direction = getMovementBasedOnCam();
        if(direction.magnitude > 0){
            grabposparent.transform.forward = direction;
        }
    }

    void PCactions()
    {
        if(grabbing){
            return;
        }
        if((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift)) && Time.time > nextdodge && !dodging && direction.magnitude > 0){
            attackend();
            nextdodge = Time.time + 0.41f;
            dodgedir = direction;
            anim.SetTrigger("dodge");
            Soundmanagerscript.playsound("playerdash");
        }
        if(Input.GetKeyDown(KeyCode.J) && Time.time > nextattack && !dodging){
            dodgeend();
            nextdodge = Time.time + 0.26f;
            nextattack = Time.time + 0.43f;
            if(specialstate){
                superslash.activate(facingDir);
            }else{
                slash.activate(facingDir);
            }
            anim.SetTrigger(specialstate ? "specialattack" : "attack");
            Soundmanagerscript.playsound("playerattack");
        }
        if(Input.GetKeyDown(KeyCode.K) && Time.time > nextattack && !dodging && !attacking && !grabbedenemy && Time.time > nextgrab) {
            grabenemy();
        }
    }

    void PCmovement()
    {
        horizontal = horizontalaxis();
        vertical = verticalaxis();
    }

    void otherinputs()
    {
        if(Input.GetKeyDown(KeyCode.L) && Time.time > nextcamfocus && !dodging){
            camscript.setCamDirection(facingDir);
            nextcamfocus = Time.time + 0.363f;
        }
        if(Input.GetKeyDown(KeyCode.H) && specialready && !specialstate && !dodging && !attacking && !grabbing && !grabbedenemy && Time.time > nextspecial){
            gameObject.layer = LayerMask.NameToLayer("playerdodging");
            camscript.grabslamshake();
            camscript.superpanelsetamt(30f);
            specialstate = true;
            superknockback();
            nextdodge = Time.time + 0.25f;
            nextattack = Time.time + 0.25f;
            nextrecover = Time.time + 0.25f;
            nextspecial = Time.time + 0.9f;
            rb.velocity = new Vector3(0f,0f,0f);
            anim.SetTrigger("superform");
            supercircles(true);
            Soundmanagerscript.playsound("playerpowerup");
            Invoke("dodgeend", 0.54f);
        }else if(Input.GetKeyDown(KeyCode.H) && specialstate && Time.time > nextspecial){
            specialstate = false;
            nextspecial = Time.time + 0.9f;
        }
    }

    void FixedUpdate()
    {
        if(dead || Time.time < nextrecover){
            anim.SetBool("move",false); 
            rb.velocity = direction = new Vector3(0f,0f,0f);
            return;
        }

        // just to ensure that the player actually turns when you re-direct a throw
        if(((!dodging && !attacking) || grabbing) && direction.magnitude > 0f){
            if(horizontal>0){
                facingright=true;
            }else if(horizontal<0){
                facingright=false;
            }
        }

        if(Time.time < delaytomovement){
            anim.SetBool("move",false); 
            rb.velocity = new Vector3(0f,0f,0f);
            return;
        }

        if(dodging){
            rb.velocity = dodgedir * dodgespeed;
            return;
        }

        if(direction.magnitude>0 && !attacking && Time.time > nextrecover && !grabbing)
        {
            facingDir = direction.normalized;
            rb.velocity = new Vector3(direction.x*speed,0f,direction.z*speed);
            anim.SetBool("move",true);
        }
        else
        {
            rb.velocity = new Vector3(0f,0f,0f);  
            anim.SetBool("move",false); 
        }
    }

    void LateUpdate()
    {
        localscale = playerParent.transform.localScale;
        localscale.x = (facingright ? 1 : -1);
        playerParent.transform.localScale = localscale;
    }

    public void damaged(float amt = 5f)
    {
        if(dead){
            return;
        }
        if(grabbedenemy){
            grabbedenemy.GetComponent<enemymanagerscript>().droppedbyplayer();
        }
        camscript.damagedpanel(60f);
        camscript.playerdamagedshake();
        camscript.incrementplayerhitcount();
        Soundmanagerscript.playsound("playerhurt");
        grabbedenemy = null;
        grabbing = false;
        aoecircleeffect.SetActive(false);
        slash.deactivate();
        spawnblood();
        attackend();
        dodgeend();
        rb.velocity = direction = dodgedir = new Vector3(0f,0f,0f);  
        nextdodge = nextattack = nextrecover = Time.time + 0.3f;
        reducehealth(amt);
        increasesuper(2f);
        anim.SetTrigger("damaged");
    }


    // ----------------------------------- attacking functions -----------------------------------

    public void attackstart()
    {
        attacking = true;
    }

    public void attack()
    {
        Collider[] collist = Physics.OverlapSphere(specialstate ? superattackpos.transform.position : attackpos.transform.position,specialstate ? specialattackradius : attackradius,enemyLayers);
        if(collist.Length>0)
        {
            camscript.campanel(6f);
            camscript.camshake();
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="enemy" && col.gameObject.GetComponent<enemymanagerscript>())
                {
                    enemymanagerscript enemy = col.gameObject.GetComponent<enemymanagerscript>();
                    increasesuper( enemy.charging ? 8.5f : 5f);
                    if(enemy.charging){
                        timer.alterscoreval(200f);
                    }
                    enemy.damaged(facingDir);
                    if(specialstate){
                        increasehealth(2.5f/*3.5f*/);
                    }
                }
            }
        }
    }

    public void attackend()
    {
        attacking = false;
        grabbing = false;
    }

    // ----------------------------------- attacking functions -----------------------------------



    // ----------------------------------- dodging functions -----------------------------------

    public void dodgestart()
    {
        gameObject.layer = LayerMask.NameToLayer("playerdodging");
        dodging = true;
    }

    public void dodgeend()
    {
        if(IsInvoking("dodgeend")){
            CancelInvoke("dodgeend");
        }
        gameObject.layer = LayerMask.NameToLayer("player");
        dodging = false;
    }

    // ----------------------------------- dodging functions -----------------------------------



    // ----------------------------------- grabbing functions -----------------------------------

    public void grabenemy()
    {
        Collider[] collist = Physics.OverlapSphere(grabenemypos.transform.position,grabenemyradius,enemyLayers);
        if(collist.Length>0 && !grabbedenemy)
        {
            /*Debug.ClearDeveloperConsole();
            for(int i=0;i<collist.Length;i++){
                Debug.Log("enemy "+i+" : "+collist[i].gameObject.name);
            }*/
            List<Collider> newcols = new List<Collider>();
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="enemy" && col.gameObject.GetComponent<enemymanagerscript>())
                {
                    if(!col.gameObject.GetComponent<enemymanagerscript>().shielded)
                    {
                        newcols.Add(col);
                    }
                }
            }
            if(newcols.Count == 0)
            {
                return;
            }
            grabbing = true;
            nextgrab = Time.time + 0.76f;
            GameObject chosenenemy = newcols[0].gameObject;
            foreach(Collider col in newcols)
            {
                if(Vector3.Distance(col.gameObject.transform.position, transform.position) <= Vector3.Distance(chosenenemy.transform.position, transform.position))
                {
                    chosenenemy = col.gameObject;
                }
            }
            grabbedenemy = chosenenemy;
            if(grabbedenemy.GetComponent<enemymanagerscript>().charging){
                increasesuper(11f);
                timer.altertimerval(8f);
                timer.alterscoreval(350f);
            }
            grabbedenemy.GetComponent<enemymanagerscript>().grabbedbyplayer(grabpos1);
            nextdodge = Time.time + 0.685f;
            nextattack = Time.time + 0.685f;
            delaytomovement = Time.time + 0.63f;
            anim.SetTrigger(specialstate ? "multigrab" : "grab");
        }
    }

    public void setgrabpos1()
    {
        if(grabbedenemy){
            grabbedenemy.GetComponent<enemymanagerscript>().grabbedpos = grabpos1;
        }
    }

    public void setgrabpos2()
    {
        throwDir = direction.magnitude > 0 ? direction.normalized : facingDir;
        slamparent.transform.forward = throwDir;
        camscript.grabslamshake();
        if(!grabbedenemy){
            return;
        }
        grabbedenemy.GetComponent<enemymanagerscript>().grabbedpos = grabpos2;
        grabbedenemy.GetComponent<enemymanagerscript>().togglebloodsplatter(true);
        // hurt the grabbed enemy quite a lot when slammed
        grabbedenemy.GetComponent<enemymanagerscript>().reducehealth(1f);
        spawnblood(new Vector3(slampos.transform.position.x,0f,slampos.transform.position.z));
        aoecircleeffect.SetActive(true);
        aoecircleeffect.GetComponent<Animator>().SetTrigger("aoecircleeffect");
        camscript.campanel(6.5f);
        increasesuper(5f);
        if(specialstate){
            increasehealth(3.5f/*4.5f*/);
        }
        Soundmanagerscript.playsound("playergrabattack");
        Soundmanagerscript.playsound("enemydamaged");
        Collider[] collist = Physics.OverlapSphere(slampos.transform.position,slamradius,enemyLayers);
        if(collist.Length>0)
        {
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="enemy" && col.gameObject.GetComponent<enemymanagerscript>() && (col.gameObject != grabbedenemy))
                {
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    enemymanagerscript enemy = col.gameObject.GetComponent<enemymanagerscript>();
                    increasesuper(enemy.charging ? 4.5f : 2f);
                    if(enemy.charging){
                        timer.alterscoreval(200f);
                    }
                    enemy.thrownbyplayer(dir, false, true);
                    if(specialstate){
                        increasehealth(1.5f);
                    }
                }
            }
        }
    }

    public void throwenemy()
    {
        if(grabbedenemy){
            grabbedenemy.GetComponent<enemymanagerscript>().thrownbyplayer(throwDir, true);
        }
        grabbedenemy = null;
        grabbing = false;
    }

    // ----------------------------------- grabbing functions -----------------------------------


    // ----------------------------------- super functions ----------------------------------- 

    void superknockback()
    {
        Collider[] collist = Physics.OverlapSphere(transform.position,superradius,enemyLayers);
        if(collist.Length>0)
        {
            float count = 0f;
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="enemy" && col.gameObject.GetComponent<enemymanagerscript>())
                {
                    count += 1f;
                    Vector3 dir = (col.transform.position - transform.position).normalized;
                    col.gameObject.GetComponent<enemymanagerscript>().thrownbyplayer(dir, false, false, true);
                }
            }
            if(count > 0f){
                timer.altertimerval(count);
                timer.alterscoreval(count * 100f);
            }
        }
    }

    void supercircles(bool activate)
    {
        for(int i=0;i<supercircleeffects.Count;i++)
        {
            supercircleeffects[i].SetActive(activate);
            if(activate){
                supercircleeffects[i].GetComponent<Animator>().SetTrigger("aoecircleeffect");
            }
        }
    }


    // ----------------------------------- super functions ----------------------------------- 



    // ----------------------------------- Movement systems -----------------------------------

    int verticalaxis()
    {
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)){
            return 1;
        }
        else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            return -1;
        }
        else if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow)) && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))){
            return 0;
        }
        else{
            return 0;
        }
    }

    int horizontalaxis()
    {
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            return 1;
        }
        else if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)){
            return -1;
        }
        else if((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow)) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))){
            return 0;
        }
        else{
            return 0;
        }
    }

    Vector3 getMovementBasedOnCam()
    {
        Vector3 camforwaddir = new Vector3(camparent.transform.forward.x,0f,camparent.transform.forward.z).normalized;
        Vector3 camrightdir = new Vector3(camparent.transform.right.x,0f,camparent.transform.right.z).normalized;

        Vector3 forwardRelative = (camforwaddir * vertical).normalized;
        Vector3 rightRelative = (camrightdir * horizontal).normalized;

        return (forwardRelative + rightRelative).normalized;
    }

    // ----------------------------------- Movement systems -----------------------------------

    void SurroundingEnemyDetector()
    {
        Collider[] collist = Physics.OverlapSphere(transform.position,surroundingEnemyRadius,surroundEnemyLayers);
        surroundingEnemies.Clear();
        if(collist.Length>0)
        {
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="enemy")
                {
                    if(!surroundingEnemies.Contains(col.transform)){
                        surroundingEnemies.Add(col.transform);
                    }
                }
            }
        }
        camscript.SetCameraTargets(surroundingEnemies);
    }

    void spawnblood(Vector3? pos = null)
    {
        float amt = 0.077f;
        Vector3 basepos = (pos == null) ? transform.position : ((Vector3)pos);
        Bloodpooler.spawnbloodeffect(new Vector3(basepos.x+(Random.Range(amt,-amt)),0f,basepos.z+(Random.Range(amt,-amt))));
        Bloodpooler.spawnbloodeffect(new Vector3(basepos.x+(Random.Range(amt,-amt)),0f,basepos.z+(Random.Range(amt,-amt))));
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "essence")
        {
            col.gameObject.GetComponent<essencescript>().remove();
            camscript.essencepanelsetamt(26.5f);
            increasesuper(2.5f, specialstate);
            increasehealth(2f);
            timer.altertimerval(8f);
            timer.alterscoreval(50f);
            Soundmanagerscript.playsound("playerpickupsfx");
        }
    }

    // -------------------------------------- bar and meter functions ----------------------------------

    public void increasehealth(float amt)
    {
        health = (((health+amt) >= maxhealth ) ? maxhealth : (health+amt));
    }

    public void reducehealth(float amt)
    {
        health = (((health-amt) <= 0f ) ? 0f : (health-amt));
        if(health <= 0f){
            dead = true;
        }
    }

    public void increasesuper(float amt, bool ignorespecialstate = false)
    {
        if(specialstate && !ignorespecialstate){
            return;
        }
        float setamt = (ignorespecialstate) ? (amt + 10.5f) : amt;
        if((specialamt+setamt) >= maxspecial && !specialready && !specialstate){
            Soundmanagerscript.playsound("playerspecialready");
        }
        specialamt = (((specialamt+setamt) >= maxspecial ) ? maxspecial : (specialamt+setamt));
    }

    // -------------------------------------- bar and meter functions ----------------------------------

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, surroundingEnemyRadius);

        if(attackpos){
            Gizmos.DrawWireSphere(attackpos.transform.position, attackradius);
        }
        if(superattackpos){
            Gizmos.DrawWireSphere(superattackpos.transform.position, specialattackradius);
        }
        if(grabenemypos){
            Gizmos.DrawWireSphere(grabenemypos.transform.position, grabenemyradius);
        }
        if(slampos){
            Gizmos.DrawWireSphere(slampos.transform.position, slamradius);
        }
        Gizmos.DrawWireSphere(transform.position, superradius);
    }

}
