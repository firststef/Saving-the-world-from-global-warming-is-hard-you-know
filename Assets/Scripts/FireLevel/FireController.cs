using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private GameObject player;
    private PlayerController cont;
    private SpriteRenderer spr;
    private int lol;
    private bool isLit;


    void Start()
    {
        player = GameObject.Find("Player");
        cont = player.GetComponent<PlayerController>();
        spr = GetComponent<SpriteRenderer>();
        isLit = true;
        lol = 0;

    }

    void Update()
    {
        if (isLit)
        {
            if (lol / 100 == 0)
                spr.sprite = cont.Sprites[456];
            else if (lol / 100 == 1)
                spr.sprite = cont.Sprites[457];
            else lol = 0;
            lol++;
            isNearPlayer();
        }
    }

    private void isNearPlayer()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= 1 && Mathf.Abs(player.transform.position.y - transform.position.y) <= 1)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (cont.Water > 0)
                {
                    spr.sprite = cont.Sprites[455];
                    cont.Water--;
                    isLit = false;
                    cont.FireNumber--;
                }
            }
        }
    }
}
