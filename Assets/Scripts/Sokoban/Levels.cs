using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

[System.Serializable]
public class Level
{
    public List<string> m_Rows = new List<string>();

    public int Height { get { return m_Rows.Count; } }
    public int Width
    {
        get
        {
            int maxLength = 0;
            foreach (var r in m_Rows)
                if (r.Length > maxLength) maxLength = r.Length;
            return maxLength;
        }
    }
}

public class Levels : MonoBehaviour
{
    public string filename;
    public List<Level> m_Levels;

    void Awake()
    {
        TextAsset ta = (TextAsset)Resources.Load(filename);
        if (!ta)
        {
            Debug.Log("No");
            return;
        }
        

        string completeText = ta.text;
        string[] lines;
        lines = completeText.Split(new string[] { "\n" }, System.StringSplitOptions.None);
        m_Levels.Add(new Level());

            for(long i = 0; i<lines.LongLength; i++) 
            {
                string line = lines[i];
                if (line.StartsWith(";"))
                {
                    Debug.Log("New Level Added");
                    m_Levels.Add(new Level());
                    continue;
                }
                m_Levels[m_Levels.Count - 1].m_Rows.Add(line);
            }
    }
}
