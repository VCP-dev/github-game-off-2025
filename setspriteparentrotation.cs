using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setspriteparentrotation : MonoBehaviour
{
    GameObject camparent;
    public bool ignoreatfirst;

    // Start is called before the first frame update
    void Start()
    {
        camparent = GameObject.FindGameObjectWithTag("camparent");
    }

    // Update is called once per frame
    void Update()
    {
        if(ignoreatfirst){
            return;
        }
        Vector3 camdir = camparent.transform.forward;
        Vector3 playerparentpos = transform.position;
        transform.forward = new Vector3(camdir.x,playerparentpos.y,camdir.z);// camparent.transform.forward;
    }
}
