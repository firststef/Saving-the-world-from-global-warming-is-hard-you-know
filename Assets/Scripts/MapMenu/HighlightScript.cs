using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightScript : MonoBehaviour
{
    private MapManager mapM;

    // Start is called before the first frame update
    void Start()
    {
        mapM = GameObject.Find("GameManagerMap").GetComponent<MapManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().enabled = true;

        mapM.HighlightName = name;
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().enabled = false;

        mapM.HighlightName = null;
    }

    
}
