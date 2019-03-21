using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireController : MonoBehaviour
{
    private GameObject player;
    private PlayerController cont;
    private SpriteRenderer spr;
    private bool isLit = true;


    void Start()
    {
        player = GameObject.Find("Player");
        cont = player.GetComponent<PlayerController>();
        spr = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if (isLit)
        {
            isNearPlayer();
        }
    }

    private void isNearPlayer()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= 1.5 && Mathf.Abs(player.transform.position.y - transform.position.y) <= 1.5)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (cont.Water > 0)
                {
                    GetComponent<Animator>().SetBool("FireExtinguished", true);
                    cont.Water--;
                    isLit = false;
                    cont.FireNumber--;
                }
            }
        }
    }
}
