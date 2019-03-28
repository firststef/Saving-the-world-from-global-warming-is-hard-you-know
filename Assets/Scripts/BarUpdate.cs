using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUpdate : MonoBehaviour
{
    private GameManager gameManager;
    private Image childImage;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        childImage = this.gameObject.transform.GetChild(0).GetComponent<Image>();
        
    }

    // Update is called once per frame
    void Update()
    {
        childImage.fillAmount = (float) gameManager.Polution / gameManager.PolutionLimit;
    }
}
