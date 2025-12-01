using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class oneshoteffectscript : MonoBehaviour
{

    public GameObject sprite;
    public LayerMask enemyLayers;
    public float radius;

    Animator anim;
    timerscript timer;

    void Awake()
    {
        anim = sprite.GetComponent<Animator>();
        timer = GameObject.FindGameObjectWithTag("timer").GetComponent<timerscript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void explode()
    {
        anim.SetTrigger("effect");
        Collider[] collist = Physics.OverlapSphere(transform.position,radius,enemyLayers);
        if(collist.Length>0)
        {
            float mult = 0f;
            foreach(Collider col in collist)
            {
                if(col.gameObject.tag=="enemy" && col.gameObject.GetComponent<enemymanagerscript>())
                {
                    mult += 1f;
                    Vector3 dir = (col.gameObject.transform.position - transform.position).normalized;
                    enemymanagerscript enemy = col.gameObject.GetComponent<enemymanagerscript>();
                    enemy.damaged(dir, 4f);
                }
            }
            timer.altertimerval(mult);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
