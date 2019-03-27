using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class LevelElement
{
    public string m_Character;
    public GameObject m_prefab;
}

public class LevelBuilder : MonoBehaviour
{
    public List<LevelElement> m_LevelElements;
    //private Level m_Level;
    private int LevelNumber;
    public GameManager gm;
    public Sprite valid;

    GameObject GetPrefab(char c)
    {
        LevelElement levelElement = m_LevelElements.Find(le => le.m_Character == c.ToString());
        if (levelElement != null)
            return levelElement.m_prefab;
        else return null;
    }

    public void Build(Level m_Level)
    {
        //LevelNumber = GetComponent<Levels>().m_Levels.Count;
        //m_Level = GetComponent<Levels>().m_Levels[(int)Random.Range(0,LevelNumber)];
        gm.NumberOfCrosses = 0;

        var walls = GameObject.FindGameObjectsWithTag("Wall");
        var box = GameObject.FindGameObjectsWithTag("Box");
        var pl = GameObject.FindGameObjectsWithTag("Player");
        var cro = GameObject.FindGameObjectsWithTag("Cross");

        foreach (GameObject item in walls)
            Destroy(item);
        foreach (GameObject item in box)
            Destroy(item);
        foreach (GameObject item in pl)
            Destroy(item);
        foreach (GameObject item in cro)
            Destroy(item);

        int startx = -m_Level.Width / 2;
        int x = startx;
        int y = -m_Level.Height / 2;
        foreach(var row in m_Level.m_Rows)
        {
            foreach(var ch in row)
            {
                GameObject prefab = GetPrefab(ch);
                if(ch == '.')
                    gm.NumberOfCrosses++;
                if (prefab)
                {
                    GameObject go = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
                    if (go.GetComponent<Box>()) go.GetComponent<Box>().valid = valid;
                }
                x++;
            }
            y++;
            x = startx;
        }
    }
}
