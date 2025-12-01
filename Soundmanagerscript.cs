using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundmanagerscript : MonoBehaviour
{

    static AudioSource source;
    static AudioClip enemyexplode,playerspecialready,playerpowerup,playergrabattack,aoeattack,playerattack,playerdash,playerpickupsfx,win,playerdeath,playerhurt;
    static AudioClip enemydamaged1,enemydamaged2,enemydamaged3,enemydamaged4;
    
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        win = Resources.Load<AudioClip>("win");
        playerpickupsfx = Resources.Load<AudioClip>("playerpickupsfx");
        playerdeath = Resources.Load<AudioClip>("playerdeathsfx");
        playerhurt = Resources.Load<AudioClip>("playerhurt");
        playerdash = Resources.Load<AudioClip>("player dash");
        playerattack = Resources.Load<AudioClip>("playerattack");
        enemydamaged1 = Resources.Load<AudioClip>("enemyflushed1");
        enemydamaged2 = Resources.Load<AudioClip>("enemyflushed2");
        enemydamaged3 = Resources.Load<AudioClip>("enemyflushed3");
        enemydamaged4 = Resources.Load<AudioClip>("enemyflushed4");
        aoeattack = Resources.Load<AudioClip>("Explosion120");
        playergrabattack = Resources.Load<AudioClip>("Explosion163");
        playerpowerup = Resources.Load<AudioClip>("Explosion203");
        playerspecialready = Resources.Load<AudioClip>("playerspecialready");
        enemyexplode = Resources.Load<AudioClip>("Explosion205");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void playsound(string name)
    {
        switch(name)
        {
            case "win":
                source.PlayOneShot(win);
                break;
            case "playerpickupsfx":
                source.PlayOneShot(playerpickupsfx);
                break;
            case "playerdeath":
                source.PlayOneShot(playerdeath);
                break;
            case "playerhurt":
                source.PlayOneShot(playerhurt);
                break;
            case "playerdash":
                source.PlayOneShot(playerdash);
                break;
            case "playerattack":
                source.PlayOneShot(playerattack);
                break;
            case "enemydamaged":
                int id = Random.Range(1,5);
                switch(id)
                {
                    case 1:
                        source.PlayOneShot(enemydamaged1);
                        break;
                    case 2:
                        source.PlayOneShot(enemydamaged2);
                        break;
                    case 3:
                        source.PlayOneShot(enemydamaged3);
                        break;
                    case 4:
                        source.PlayOneShot(enemydamaged4);
                        break;
                }
                break;
            case "aoeattack":
                source.PlayOneShot(aoeattack);
                break;
            case "playergrabattack":
                source.PlayOneShot(playergrabattack);
                break;
            case "playerpowerup":
                source.PlayOneShot(playerpowerup);
                break;
            case "playerspecialready":
                source.PlayOneShot(playerspecialready);
                break;
            case "enemyexplode":
                source.PlayOneShot(enemyexplode);
                break;
        }
    }
}
