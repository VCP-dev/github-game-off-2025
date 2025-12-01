using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodeffectscript : MonoBehaviour
{
    public GameObject bloodsprite;
    [HideInInspector]
    public float delaytillfade;
    [HideInInspector]
    public float bloodspriteamt;

    public List<Sprite> bloodsprites;

    // Start is called before the first frame update
    void Start()
    {
        //initialise();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > delaytillfade){
            if(bloodspriteamt>0){
                bloodspriteamt -= (140*Time.deltaTime);
            }
            bloodsprite.GetComponent<SpriteRenderer>().color = new Color(bloodsprite.GetComponent<SpriteRenderer>().color.r,bloodsprite.GetComponent<SpriteRenderer>().color.g,bloodsprite.GetComponent<SpriteRenderer>().color.b,bloodspriteamt/255f);
        }
    }

    void deactivate()
    {
        gameObject.SetActive(false);
    }

    public void initialise()
    {
        if(IsInvoking("setup")){
            CancelInvoke("setup");
        }
        Invoke("setup",0.14f/*0.275f*/);
    }

    void setup()
    {
        bloodsprite.GetComponent<SpriteRenderer>().sprite = bloodsprites[Random.Range(0,bloodsprites.Count)];
        float delayamt = Random.Range(0.9f,1.1f);//Random.Range(1.8f,2.8f);//Random.Range(9.5f,11.5f);//2.6f;
        delaytillfade = Time.time + delayamt;//2.6f;
        bloodspriteamt = 255f;
        bloodsprite.GetComponent<SpriteRenderer>().color = new Color(bloodsprite.GetComponent<SpriteRenderer>().color.r,bloodsprite.GetComponent<SpriteRenderer>().color.g,bloodsprite.GetComponent<SpriteRenderer>().color.b,bloodspriteamt/255f);
        if(IsInvoking("deactivate")){
            CancelInvoke("deactivate");
        }
        Invoke("deactivate",(delayamt+1.3f)/*3.9f*/);
    }
}
