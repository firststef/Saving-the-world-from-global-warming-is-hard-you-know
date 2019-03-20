using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;

public class IceBreaking : MonoBehaviour
{
    private Tilemap tilemap;
    public Sprite[] IceSprites;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
        IceSprites = Resources.LoadAll<Sprite>("Ice");
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    
}
