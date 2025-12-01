using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slasheffectscript : MonoBehaviour
{

    public GameObject slashsprite;

    public void activate(Vector3 dir)
    {
        slashsprite.SetActive(false);
        transform.forward = dir;
        slashsprite.SetActive(true);
    }

    public void deactivate()
    {
        slashsprite.SetActive(false);
    }
}
