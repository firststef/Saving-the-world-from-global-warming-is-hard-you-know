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
    private Level m_Level;
    private int LevelNumber;
    public GameManager gm;

    GameObject GetPrefab(char c)
    {
        LevelElement levelElement = m_LevelElements.Find(le => le.m_Character == c.ToString());
        if (levelElement != null)
            return levelElement.m_prefab;
        else return null;
    }

    public void Build()
    {
        LevelNumber = GetComponent<Levels>().m_Levels.Count;
        m_Level = GetComponent<Levels>().m_Levels[(int)Random.Range(0,LevelNumber)];

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
                    Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                x++;
            }
            y++;
            x = startx;
        }
    }
}
