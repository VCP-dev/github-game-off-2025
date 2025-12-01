using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playeranimationscript : MonoBehaviour
{

    playerscript script;

    // Start is called before the first frame update
    void Start()
    {
        script = transform.parent.parent.parent.gameObject.GetComponent<playerscript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void attackstart()
    {
        script.attackstart();
    }

    void attack()
    {
        script.attack();
    }

    void attackend()
    {
        script.attackend();
    }

    void dodgestart()
    {
        script.dodgestart();
    }

    void dodgeend()
    {
        script.dodgeend();
    }

    void setgrabpos1()
    {
        script.setgrabpos1();
    }

    void setgrabpos2()
    {
        script.setgrabpos2();
    }

    void throwenemy()
    {
        script.throwenemy();
    }
}
