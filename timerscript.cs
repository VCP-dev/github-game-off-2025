using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class timerscript : MonoBehaviour
{

    public List<Text> timertexts;
    public List<Text> timeaddedtexts;
    public GameObject timeaddedobj;
    public GameObject timertextobj;

    public Text scoretext;
    public List<Text> scoreaddedtexts;
    public GameObject scoreaddedobj;

    public GameObject gameoverpanel;
    public Text titletext;
    public Text wavetext;
    public Text enemieskilledtext;
    public Text hitstakentext;
    public Text timetakentext;
    public Text extratimetext;
    public Text finalscoretext;

    [HideInInspector]
    public bool countdown;

    Animator timeaddedanim;
    Animator timertextanim;
    Animator scoreaddedanim;
    float nexttime;
    float nexttimescore;
    float nextopacity;
    float nextopacityscore;
    float timerval;
    float scoreval;
    float opacity;
    float opacityscore;
    float addedamt;
    float addedscoreamt;
    float opacityreduceval;
    bool killedplayer;

    Cameramovementscript camscript;
    playerscript player;
    enemyspawnerscript enemyspawner;

    // Start is called before the first frame update
    void Start()
    {
        timeaddedanim = timeaddedobj.GetComponent<Animator>();
        timertextanim = timertextobj.GetComponent<Animator>();
        scoreaddedanim = scoreaddedobj.GetComponent<Animator>();
        camscript = GameObject.FindGameObjectWithTag("camparent").GetComponent<Cameramovementscript>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<playerscript>();
        enemyspawner = GameObject.FindGameObjectWithTag("enemyspawner").GetComponent<enemyspawnerscript>();
        timerval = 60f;//99f;
        scoreval = 0f;
        nexttime = 0f;
        nexttimescore = 0f;
        nextopacity = 0f;
        nextopacityscore = 0f;
        opacity = 0f;
        opacityscore = 0f;
        addedamt = 0f;
        addedscoreamt = 0f;
        countdown = false;
        killedplayer = false;
        opacityreduceval = 158.3f;
        gameoverpanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        settimertext();
        settimeaddedopacity();
        setscoreaddedopacity();
        setscoretext();
    }

    void settimertext()
    {
        if(Time.time > nexttime && timerval >= 0f && countdown){
            timerval -= (1.7f * Time.deltaTime);
        }else if(timerval <= 0f && countdown && !killedplayer){
            player.damaged(120f);
            killedplayer = true;
        }
        /*foreach(Text timertext in timertexts)
        {
            timertext.text = ""+(int)timerval;
        }*/
        for(int i=0;i<timertexts.Count;i++)
        {
            Color col = timertexts[i].color;
            Color setcol = (timerval < 21f) ? Color.red : new Color(230f,231f,0f,1f);
            timertexts[i].color = ((i==0) ? setcol : col);
            timertexts[i].text = ""+(int)timerval;
        }
        timertextanim.SetBool("critical",(timerval < 21f));
    }

    void settimeaddedopacity()
    {
        if(opacity <= 10f){
            addedamt = 0f;
        }
        if(Time.time > nextopacity && opacity > 0f){
            opacity -= (opacityreduceval * Time.deltaTime);
        }
        for(int i=0;i<timeaddedtexts.Count;i++)
        {
            Color col = timeaddedtexts[i].color;
            timeaddedtexts[i].color = ((i==0) ? new Color(230f,231f,0f,opacity/255f) : new Color(col.r,col.g,col.g,opacity/255f));
            timeaddedtexts[i].text = "+"+addedamt;
        }
        /*foreach(Text added in timeaddedtexts)
        {
            Color col = added.color;
            added.color = new Color(col.r,col.g,col.g,opacity/255f);
            added.text = "+"+addedamt;
        }*/
    }

    public void altertimerval(float amt = 0f)
    {
        nexttime = Time.time + 0.804f;
        nextopacity = Time.time + 0.41f;//1.25f;
        opacity = 255f;
        timerval = ((timerval+amt >= 99f) ? 99f : (timerval+amt));
        addedamt += amt;
        timeaddedanim.SetTrigger("shake");
        timertextanim.SetTrigger("shake");
    }

    public void timerstatus(bool val)
    {
        countdown = val;
        if(val){
            nexttime = Time.time + 0.83f;
        }
    }

    void setscoreaddedopacity()
    {
        if(opacityscore <= 10f){
            addedscoreamt = 0f;
        }
        if(Time.time > nextopacityscore && opacityscore > 0f){
            opacityscore -= (opacityreduceval * Time.deltaTime);
        }
        for(int i=0;i<scoreaddedtexts.Count;i++)
        {
            Color col = scoreaddedtexts[i].color;
            scoreaddedtexts[i].color = ((i==0) ? new Color(255f,255f,255f,opacityscore/255f) : new Color(col.r,col.g,col.g,opacityscore/255f));
            scoreaddedtexts[i].text = "+"+addedscoreamt;
        }
    }

    public void alterscoreval(float amt = 0f)
    {
        nexttimescore = Time.time + 0.804f;
        nextopacityscore = Time.time + 0.41f;//1.25f;
        opacityscore = 255f;
        scoreval += amt;
        addedscoreamt += amt;
        scoreaddedanim.SetTrigger("shake");
    }

    void setscoretext()
    {
        scoretext.text = ""+scoreval;
    }


    // ------------------------------------ game over functions ------------------------------------ 

    public void gameover(bool won = false)
    {
        if(won){
            alterscoreval(10000f);
            Soundmanagerscript.playsound("win");
        }
        camscript.endtime = Time.time;
        countdown = false;
        titletext.text = won ? "YOU WON !!!" : "GAMEOVER" ;
        wavetext.text = "Wave reached : "+enemyspawner.wave+" / 5";
        enemieskilledtext.text = "Enemies killed : "+camscript.enemydeathcount+" / "+enemyspawner.enemydeathlimit;
        hitstakentext.text = "Hits taken : "+camscript.hitcount;
        int totaltime = (int)(camscript.endtime-camscript.starttime);
        timetakentext.text = "Time taken : "+(totaltime/60)+" min "+(totaltime%60)+" s";
        int inttime = (int)timerval;
        extratimetext.text = "Extra time : "+inttime+" s";
        int finalscore = (((int)scoreval + (inttime * 100))-(camscript.hitcount * 100));
        finalscoretext.text = "Final score : "+finalscore;
        Invoke("showpanel",1.2f);
    }

    void showpanel()
    {
        gameoverpanel.SetActive(true);
    }

    // ------------------------------------ game over functions ------------------------------------ 
}
