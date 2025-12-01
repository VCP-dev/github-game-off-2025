using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotationscript : MonoBehaviour
{
    
    [HideInInspector]
    public float y;
    [HideInInspector]
    public float z;
    int dir;
    public bool clockwise;
    public float rotationspeed;
    public bool noneedforxrotation;
    public bool shouldrotate;
    public bool zaxisrotation;

    // Start is called before the first frame update
    void Start()
    {
        //dir=(clockwise)?1:-1;
    }

    // Update is called once per frame
    void Update()
    {
        dir=(clockwise)?1:-1;
        
        if(!shouldrotate){
            return;
        }

        y += (Time.deltaTime * rotationspeed * dir);
        z += (Time.deltaTime * rotationspeed * dir);

        if((y>360 && clockwise) || (y<-360 && !clockwise)){
            y=0f;
        }

        if((z>360 && clockwise) || (z<-360 && !clockwise)){
            z=0f;
        }

        if(!zaxisrotation){
            transform.localRotation = Quaternion.Euler((noneedforxrotation)?0f:90f, y, 0f);
        }else{
            transform.localRotation = Quaternion.Euler((noneedforxrotation)?0f:90f, 0f, z);
        }
    }
}
