using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BenguinController : MonoBehaviour
{
    private GameObject player;
    private GameObject exit;
    private bool isPicked;
    private int index;
    private PlayerController2 cont;

    // Start is called before the first frame update
    void Start()
    {
        exit = GameObject.Find("Exit");
        player = GameObject.Find("Player");
        cont = player.GetComponent<PlayerController2>();
        isPicked = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(isPicked)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 1);
            transform.position = player.transform.position + new Vector3(index*0.5f, 0.7f, 0);
            if(isNearExit())
            {
                //Destroy(this); distrug asta si sageata

                if (cont.BenguinNumber == 0)
                    SceneManager.LoadScene(0);//minigame complete
            }
        }
        else
        {
            isNearPlayer();
        }
    }

    private void isNearPlayer()
    {
        if (Mathf.Abs(player.transform.position.x - transform.position.x) <= 1 && Mathf.Abs(player.transform.position.y - transform.position.y) <= 1)
        {
            isPicked = true;
            index = 2-cont.BenguinNumber;
            cont.BenguinNumber--;
            return;
        }
    }

    private bool isNearExit()
    {
        if (Mathf.Abs(exit.transform.position.x - transform.position.x) <= 1 && Mathf.Abs(exit.transform.position.y - transform.position.y) <= 1)
            return true;
        return false;
    }
}
