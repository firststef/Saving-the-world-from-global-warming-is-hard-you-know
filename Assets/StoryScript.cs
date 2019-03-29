using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryScript : MonoBehaviour
{
    public GameObject video;
    public GameObject menu;
    private int state = 0;
    private Text t;
    private Animator animator;
    private bool clic = false;
    

    // Start is called before the first frame update
    void Start()
    {
        t = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        clic = true;

        if (Input.GetMouseButtonUp(0)&&clic)
        {
             state = state + 1;
        }

        switch(state)
        {
            case 0:
                t.text = "In the year 2030, global warming has the leaders of the world desperate.";
                break;
            case 1:
                animator.SetBool("ClickOnce",true);
                t.text = "They seek the help of a professional. This job requires someone with special skills.";
                break;
            case 2:
                t.text = "Someone like you.";
                break;
            case 3:
                t.text = "But don't take this lightly. Solving this problem is a complex task. One game wouldn't suffice ;) ";
                break;
            case 4:
                video.gameObject.SetActive(true);
                StartCoroutine(DestroyIn());
                
            break;
        }

    }

    IEnumerator DestroyIn()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
        menu.gameObject.SetActive(true);
    }
}
