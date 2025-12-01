using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Cameramovementscript : MonoBehaviour
{

    public Camera cam;
    public GameObject parentofcam;
    public GameObject damagepanel;
    public GameObject hitpanel;
    public GameObject superpanel;
    public GameObject essencepanel;


    float minZoom;
    float maxZoom;
    float zoomlimiter;
    float smoothTime;
    float rotsmoothTime;
    float camrotx;
    float rotxval;
    float faceDelay;


    float distlocked;
    float heightlocked;
    float xanglelocked;
    float smoothDistVal;
    float smoothDirVal;
    float damagepanelamt;
    float hitpanelamt;
    float superpanelamt;
    float essencepanelamt;


    [HideInInspector]
    public bool lockontoenemies;
    [HideInInspector]
    public Vector3 facedir;
    [HideInInspector]
    public int enemydeathcount;
    [HideInInspector]
    public int hitcount;
    [HideInInspector]
    public float starttime;
    [HideInInspector]
    public float endtime;
    
    Vector3 velocity;
    Vector3 velocity1;
    Vector3 velocity2;
    Vector3 velocity3;
    float velocityang;
    Vector3 facePosition;
    List<Transform> targets = new List<Transform>();

    GameObject player;
    Animator anim;
    GameObject playerlookatpoint;
    enemyspawnerscript enemyspawner;

    void Awake()
    {
        //QualitySettings.vSyncCount = 1;
        //Application.targetFrameRate = 60;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Cursor.lockState = CursorLockMode.Locked;
        player = GameObject.FindGameObjectWithTag("Player");
        playerlookatpoint = player.GetComponent<playerscript>().lookatpos;
        enemyspawner = GameObject.FindGameObjectWithTag("enemyspawner").GetComponent<enemyspawnerscript>();
        smoothTime = 0.023f;//0.15f;//0.23f;//0.2f;
        anim = cam.GetComponent<Animator>();
        minZoom = 65.12f;        //  minimum FOV
        maxZoom = 55f;          //  max FOV
        zoomlimiter = 0.23f;
        smoothTime = 0.15f;//0.23f;//0.2f;
        rotsmoothTime = 0.165f;
        lockontoenemies = false;
        rotxval = 0f;
        faceDelay = 0f;
        damagepanelamt = 0f;
        hitpanelamt = 0f;
        superpanelamt = 0f;
        essencepanelamt = 0f;
        enemydeathcount = 0;
        hitcount = 0;

        facedir = DirectionBetweenPlayerandCam();

        targets.Add(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            Scene scene = SceneManager.GetActiveScene(); 
            SceneManager.LoadScene(scene.name);
        }

        panelset(ref damagepanel, ref damagepanelamt);
        panelset(ref hitpanel, ref hitpanelamt);
        panelset(ref superpanel, ref superpanelamt);
        panelset(ref essencepanel, ref essencepanelamt);
    }

    void FixedUpdate()
    {

        if(targets.Count==0 || player==null)
            return;

        minZoom = 80f;//70f;//65.12f;
        maxZoom = 70f;//60f;//55f;

        lockontoenemies = targets.Count > 1;

        if(lockontoenemies){
            // code to lock on to enemies when they're near

            // have an overlapsphere thing in the player and check if enemies are in it
            // if they're in it, add them to the targets array here in this script
            // Based on that, set the camera to be at the mid-point of all those objects ( player, enemies etc.)
            // Also have it turn similarly when the angle between the mid-point and the player is greater than a certain angle
            //CamCenterLockon();


            // ignore the above comments, that was a very janky system
            distlocked = 0.6f;
            heightlocked = 0.42f;
            xanglelocked = 26.6f;
            smoothDistVal = 0.215f;
            smoothDirVal = 0.258f;
        }else{
            // lock on to player here
            //CamPlayerLockon();

            distlocked = 0.5f;
            heightlocked = 0.35f;
            xanglelocked = 25f;
            smoothDistVal = 0.12f;
            smoothDirVal = 0.18f;
        }
        CamPlayerLockon();
        
        camrotx = Mathf.SmoothDamp(camrotx,rotxval,ref velocityang,rotsmoothTime);
        parentofcam.transform.localRotation = Quaternion.Euler(camrotx,0f,0f);
        Zoom();
    }

    void panelset(ref GameObject panel, ref float panelsetamt, float delatamt = 140f)
    {
        if(panelsetamt>0){
            panelsetamt -= (delatamt*Time.deltaTime);
        }
        panel.GetComponent<Image>().color = new Color(panel.GetComponent<Image>().color.r,panel.GetComponent<Image>().color.g,panel.GetComponent<Image>().color.b,panelsetamt/255f);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom,minZoom,GetGreatestDistance()/zoomlimiter);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,newZoom,Time.deltaTime * 2.87f);
    }

    public Vector3 DirectionBetweenPlayerandCam()
    {
        Vector3 campos = new Vector3(transform.position.x,0f,transform.position.z);
        Vector3 playerpos = new Vector3(player.transform.position.x,0f,player.transform.position.z);
        return (playerpos - campos).normalized;
    }

    public Vector3 DirectionBetweenPlayerLookatpointandCam()
    {
        Vector3 campos = new Vector3(transform.position.x,0f,transform.position.z);
        Vector3 lookatpos = new Vector3(playerlookatpoint.transform.position.x,0f,playerlookatpoint.transform.position.z);
        return (lookatpos - campos).normalized;
    }

    void CamPlayerLockon()
    {
        if(Time.time > faceDelay){
            // delay added for when camera direction is reset by player
            facedir = DirectionBetweenPlayerandCam();
        }
        rotxval = xanglelocked;
        Vector3 playerpos = player.transform.position;
        Vector3 lockonpos = new Vector3(playerpos.x, 0f, playerpos.z);
        Vector3 setpos1 = lockonpos - (facedir * distlocked);
        Vector3 setpos2 = lockonpos + (facedir * distlocked);
        float distsetpos1 = Vector3.Distance(transform.position,setpos1);
        float distsetpos2 = Vector3.Distance(transform.position,setpos2);
        Vector3 setpos = (distsetpos1<distsetpos2) ? setpos1 : setpos2 ;
        if(Time.time > faceDelay){
            // delay added for when camera direction is reset by player
            facePosition = new Vector3(setpos.x,heightlocked,setpos.z);
        }

        transform.position = Vector3.SmoothDamp(transform.position,facePosition,ref velocity,smoothDistVal);

        transform.forward = Vector3.SmoothDamp(transform.forward,DirectionBetweenPlayerLookatpointandCam()/*facedir*/,ref velocity1,smoothDirVal);
    }

    void CamCenterLockon()
    {
        rotxval = xanglelocked;
        Vector3 center = GetCenterPoint();
        Vector3 centerLookAtPoint = new Vector3(center.x,transform.position.y,center.z);
        Vector3 camfacedir = (centerLookAtPoint - transform.position).normalized;
        Vector3 dirBtwPlayerAndCenter = (center - player.transform.position).normalized;
        float distBtwPlayerAndCenter = Vector3.Distance(center, player.transform.position);
        Vector3 setpos1 = center - (facedir * distlocked);
        Vector3 setpos2 = center + (facedir * distlocked);
        float distsetpos1 = Vector3.Distance(transform.position,setpos1);
        float distsetpos2 = Vector3.Distance(transform.position,setpos2);
        Vector3 setpos = (distsetpos1<distsetpos2) ? setpos1 : setpos2 ;
        Vector3 newPosition = new Vector3(setpos.x,heightlocked,setpos.z);

        transform.position = Vector3.SmoothDamp(transform.position,newPosition,ref velocity,0.385f);

        transform.forward = Vector3.SmoothDamp(transform.forward,camfacedir,ref velocity1,0.258f);


        // turn the camera around when the player is at an angle to the center
        float ang = Vector3.Angle(camfacedir, dirBtwPlayerAndCenter);
        //Debug.Log("Angle : "+ang);
        if(ang >= 167f && distBtwPlayerAndCenter >= 0.127f) 
        {
            facedir = Vector3.Cross(dirBtwPlayerAndCenter, Vector3.up);
        }
    }

    Vector3 GetCenterPoint()
    {
        if(targets.Count == 1)
        {
            return targets[0].position;
        }

        // just return position of player
        // If you come up with a solution that isn't too jank, can use that
        // else maybe should just add a lock-on
        return new Vector3(player.transform.position.x, 0f, player.transform.position.z);

        // for getting center point of all targets at once
        /*float totalX = 0f;
        float totalZ = 0f;
            
        for(int i = 0; i<targets.Count ;i++)
        {
            totalX += targets[i].position.x;
            totalZ += targets[i].position.z;
        }

        float avgX = totalX / targets.Count;
        float avgZ = totalZ / targets.Count;

        return new Vector3(avgX, 0f, avgZ);*/
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position,Vector3.zero);
        for(int i=0;i<targets.Count;i++)
        {
            bounds.Encapsulate(targets[i].position);
        }
        return bounds.size.x;
    }


    public void SetCameraTargets(List<Transform> newObjs)
    {
        targets.Clear();
        targets.Add(player.transform);
        for(int i=0;i<newObjs.Count;i++)
        {
            if(!targets.Contains(newObjs[i])){
                targets.Add(newObjs[i]);
            }
        }
    }

    /** 
        to set the camera behind the player
    */
    public void setCamDirection(Vector3 dir)
    {
        Vector3 dir1 = dir;
        faceDelay = Time.time + 0.35f;
        float ang = Vector3.Angle(facedir, dir1);
        //Debug.Log(ang);
        
        // Just to ensure that the camera actually turns when you're resetting vertically
        if(ang <= 180f && ang >= 160f) {
            Quaternion rotation = Quaternion.AngleAxis(13f, Vector3.up);
            dir1 = rotation * dir;
        }

        facedir = dir1;
        Vector3 playerpos = player.transform.position;
        Vector3 lockonpos = new Vector3(playerpos.x, 0f, playerpos.z);
        Vector3 setpos1 = lockonpos - (facedir * distlocked);
        Vector3 setpos2 = lockonpos + (facedir * distlocked);
        float distsetpos1 = Vector3.Distance(transform.position,setpos1);
        float distsetpos2 = Vector3.Distance(transform.position,setpos2);
        
        Vector3 setpos;

        if(ang < 90){
            setpos = (distsetpos1<distsetpos2) ? setpos1 : setpos2 ;
        }else{
            setpos = (distsetpos1>distsetpos2) ? setpos1 : setpos2 ;
        }        

        facePosition = new Vector3(setpos.x,heightlocked,setpos.z);
    }


    // ------------------------------------ panel functions ------------------------------------

    public void damagedpanel(float amt)
    {
        damagepanelamt = amt;
    }

    public void campanel(float amt)
    {
        hitpanelamt = amt;
    }

    public void superpanelsetamt(float amt)
    {
        superpanelamt = amt;
    }

    public void essencepanelsetamt(float amt)
    {
        essencepanelamt = amt;
    }

    // ------------------------------------ panel functions ------------------------------------


    // for coupling and un-coupling parent
    public void setCamParent(GameObject pos = null)
    {
        if(pos){
            transform.SetParent(pos.transform);
        }else{
            transform.parent = null;
        }
    }
    

    // ------------------------------ animation functions ------------------------------ 

    public void camshake()
    {
        anim.SetTrigger("shake");
    }

    public void camsmallshake()
    {
        anim.SetTrigger("smallshake");
    }

    public void grabslamshake()
    {
        anim.SetTrigger("grabslamshake");
    }

    public void playerdamagedshake()
    {
        anim.SetTrigger("playerdamaged");
    }

    // ------------------------------ animation functions ------------------------------


    public void incrementenemydeathcount()
    {
        enemydeathcount += 1;
        //Debug.Log("enemydeathcount : "+enemydeathcount);
        if(enemydeathcount >= enemyspawner.enemydeathlimit){
            enemydeathcount = enemyspawner.enemydeathlimit;
            if(!enemyspawner.start){
                return;
            }
            enemyspawner.end(true);
            player.GetComponent<playerscript>().health = 100f;
            campanel(50f);
        }
    }

    public void incrementplayerhitcount()
    {
        hitcount += 1;
    }

}
