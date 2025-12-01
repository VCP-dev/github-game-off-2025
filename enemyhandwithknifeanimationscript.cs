using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyhandwithknifeanimationscript : MonoBehaviour
{

    enemymanagerscript script;


    // Start is called before the first frame update
    void Start()
    {
        script = transform.parent.parent.gameObject.GetComponent<enemymanagerscript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void engageplayer()
    {
        script.engageplayer();
    }

    // ----------------------------- attacking functions ---------------------------
    void startslash()
    {
        script.startslash();
    }

    void startaoeattack()
    {
        script.startaoeattack();
    }

    void setdir()
    {
        script.setdir();
    }

    void charge()
    {
        script.charge();
    }

    void setpostoplayer()
    {
        script.setpostoplayer();
    }

    void aoeattack()
    {
        script.aoeattack();
    }

    void endcharge()
    {
        script.endcharge();
    }

    void endslash()
    {
        script.endslash();
    }
    // ----------------------------- attacking functions ---------------------------

    void setenemydodge()
    {
        script.setenemydodge();
    }

    void setenemynotdodge()
    {
        script.setenemynotdodge();
    }

}
