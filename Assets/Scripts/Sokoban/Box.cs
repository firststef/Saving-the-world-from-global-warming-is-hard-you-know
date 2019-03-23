using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    public bool m_OnCross;
    public GameObject go;
    public GameManager gm;
    public Sprite valid;
    private Sprite holder;

    void Start()
    {
        go = GameObject.Find("GameManager");
        gm = go.GetComponent<GameManager>();
        holder = GetComponent<SpriteRenderer>().sprite;
    }

    public bool Move(Vector2 direction)
    {
        if (BoxBlocked(transform.position, direction))
            return false;
        else
        {
            transform.Translate(direction);
            TestForOnCross();
            return true;
        }
    }

    public bool BoxBlocked(Vector3 position, Vector2 direction)
    {
        Vector2 newpos = new Vector2(position.x, position.y) + direction;
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (var wall in walls)
        {
            if (wall.transform.position.x == newpos.x && wall.transform.position.y == newpos.y)
                return true;
        }
        GameObject[] boxes = GameObject.FindGameObjectsWithTag("Box");
        foreach (var box in boxes)
        {
            if (box.transform.position.x == newpos.x && box.transform.position.y == newpos.y)
            {
                return true;
            }
        }
        return false;
    }

    void TestForOnCross()
    {
        GameObject[] crosses = GameObject.FindGameObjectsWithTag("Cross");
        foreach(var cross in crosses)
        {
            if(transform.position.x == cross.transform.position.x && transform.position.y == cross.transform.position.y)
            {
                gm.NumberOfCrosses--;
                GetComponent<SpriteRenderer>().sprite = valid;
                m_OnCross = true;

                return;
            }
        }
        GetComponent<SpriteRenderer>().sprite = holder;
        Debug.Log(holder);
        if (m_OnCross == true)
        {
            gm.NumberOfCrosses++;
            m_OnCross = false;
        }
        
    }
}
