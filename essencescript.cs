using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class essencescript : MonoBehaviour
{

    public GameObject sprite;

    Animator anim;

    void Awake()
    {
        anim = sprite.GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initialise()
    {
        if(IsInvoking("deactivate")){
            CancelInvoke("deactivate");
        }
        anim.SetTrigger("activate");
        Invoke("deactivate",4.5f);
    }

    public void deactivate()
    {
        anim.SetTrigger("deactivate");
    }

    public void remove()
    {
        if(IsInvoking("deactivate")){
            CancelInvoke("deactivate");
        }
        gameObject.SetActive(false);
    }
}
