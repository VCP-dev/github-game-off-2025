using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deactivatescript : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void deactivate()
    {
        gameObject.SetActive(false);
    }

    void deactivateparent()
    {
        transform.parent.gameObject.SetActive(false);
    }

    void deactivatetopmostparent()
    {
        GameObject obj = gameObject;
        while(obj.transform.parent != null){
            obj = obj.transform.parent.gameObject;
        }
        obj.SetActive(false);
    }

    void Destroyparent()
    {
        Destroy(transform.parent.gameObject,0f);
    }
}
