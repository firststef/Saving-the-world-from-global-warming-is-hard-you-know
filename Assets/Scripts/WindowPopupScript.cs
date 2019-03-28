using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowPopupScript : MonoBehaviour
{
    private GameObject gM;
    private GameManager gameManager;
    private MouseController mouseController;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameObject.Find("GameManager");
        gameManager = gM.GetComponent<GameManager>();
        mouseController = gM.GetComponent<MouseController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DeleteThis()
    {
        mouseController.windowList.RemoveAt(mouseController.windowList.Count - 1);
        Destroy(this.transform.parent.transform.parent.gameObject);
    }
}
