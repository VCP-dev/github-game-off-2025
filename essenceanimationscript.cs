using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class essenceanimationscript : MonoBehaviour
{

    essencescript script;

    // Start is called before the first frame update
    void Start()
    {
        script = transform.parent.parent.gameObject.GetComponent<essencescript>();
    }

    void remove()
    {
        script.remove();
    }


}
