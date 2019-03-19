using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace UnityEngine.Tilemaps
{
    [Serializable]
    [CreateAssetMenu(fileName = "New Custom Animated Tile", menuName = "Tiles/Custom Animated Tile")]
    public class CustomAnimatedTile : CustomTile
    {
        public Sprite[] m_CustomAnimatedSprites;
        public float m_AMinSpeed = 1f;
        public float m_AMaxSpeed = 1f;
        public float m_CustomAnimationStartTime;

        public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
        {
            tileData.transform = Matrix4x4.identity;
            tileData.color = Color.white;
            if (m_CustomAnimatedSprites != null && m_CustomAnimatedSprites.Length > 0)
            {
                tileData.sprite = m_CustomAnimatedSprites[m_CustomAnimatedSprites.Length - 1];
            }
        }

        public override bool GetTileAnimationData(Vector3Int location, ITilemap tileAMap, ref TileAnimationData tileAnimationData)
        {
            if (m_CustomAnimatedSprites.Length > 0)
            {
                tileAnimationData.animatedSprites = m_CustomAnimatedSprites;
                tileAnimationData.animationSpeed = Random.Range(m_AMinSpeed, m_AMaxSpeed);
                tileAnimationData.animationStartTime = m_CustomAnimationStartTime;
                return true;
            }
            return false;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CustomAnimatedTile))]
    public class CustomAnimatedTileEditor : Editor
    {
        private CustomAnimatedTile tileA { get { return (target as CustomAnimatedTile); } }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            int count = EditorGUILayout.DelayedIntField("Number of Animated Sprites", tileA.m_CustomAnimatedSprites != null ? tileA.m_CustomAnimatedSprites.Length : 0);
            if (count < 0)
                count = 0;

            if (tileA.m_CustomAnimatedSprites == null || tileA.m_CustomAnimatedSprites.Length != count)
            {
                Array.Resize<Sprite>(ref tileA.m_CustomAnimatedSprites, count);
            }

            if (count == 0)
                return;

            EditorGUILayout.LabelField("Place sprites shown based on the order of animation.");
            EditorGUILayout.Space();

            for (int i = 0; i < count; i++)
            {
                tileA.m_CustomAnimatedSprites[i] = (Sprite)EditorGUILayout.ObjectField("Sprite " + (i + 1), tileA.m_CustomAnimatedSprites[i], typeof(Sprite), false, null);
            }

            float minSpeed = EditorGUILayout.FloatField("Minimum Speed", tileA.m_AMinSpeed);
            float maxSpeed = EditorGUILayout.FloatField("Maximum Speed", tileA.m_AMaxSpeed);
            if (minSpeed < 0.0f)
                minSpeed = 0.0f;

            if (maxSpeed < 0.0f)
                maxSpeed = 0.0f;

            if (maxSpeed < minSpeed)
                maxSpeed = minSpeed;

            tileA.m_AMinSpeed = minSpeed;
            tileA.m_AMaxSpeed = maxSpeed;

            tileA.m_CustomAnimationStartTime = EditorGUILayout.FloatField("Start Time", tileA.m_CustomAnimationStartTime);

            tileA.sprite = tileA.m_CustomAnimatedSprites[0];

            EditorGUILayout.Space();

            tileA.displayName = (string)EditorGUILayout.TextField("Display Name", tileA.displayName);

            int zPos = (int) EditorGUILayout.FloatField("Z Position", tileA.zPos);
            tileA.zPos = zPos;

            EditorGUILayout.Space();
            CustomTile.Type type = (CustomTile.Type) EditorGUILayout.EnumPopup("Type", tileA.type);
            tileA.type = type;

            switch (type)
            {
                case CustomTile.Type.Construction:
                    {
                        int polution = (int)EditorGUILayout.IntField("Polution Number", tileA.polutionNumber);
                        tileA.polutionNumber = polution;

                        int revenue = (int)EditorGUILayout.IntField("Revenue Number", tileA.revenueNumber);
                        tileA.revenueNumber = revenue;

                        int cost = (int)EditorGUILayout.IntField("Cost For Action", tileA.costForAction);
                        tileA.costForAction = cost;
                        break;
                    }
                case CustomTile.Type.Event:
                    {
                        //////// Button1

                        tileA.button1 = (string)EditorGUILayout.TextField("Button 1", tileA.button1);

                        int count1 = EditorGUILayout.DelayedIntField("Number of demands", tileA.demands1.Count != 0 ? tileA.demands1.Count : 0);
                        if (count1 < 0)
                            count1 = 0;

                        if (tileA.demands1.Count == 0 || tileA.demands1.Count != count1)
                        {
                            //tileA.demands1 = new List<Demand>(count1);
                            int cur = tileA.demands1.Count;
                            if (count1 < cur)
                                tileA.demands1.RemoveRange(count1, cur - count1);
                            else if (count1 > cur)
                            {
                                tileA.demands1.AddRange(new CustomTile.Demand[count1 - cur]);
                            }
                        }

                        if (count1 == 0)
                            goto skip1;

                        for (int i = 0; i < count1; i++)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Element " + i);
                            tileA.demands1[i].variable = (string)EditorGUILayout.TextField("Variable", tileA.demands1[i].variable);
                            tileA.demands1[i].ctile = (CustomTile)EditorGUILayout.ObjectField("Tile", tileA.demands1[i].ctile, typeof(CustomTile), false, null);
                            tileA.demands1[i].amount = (int)EditorGUILayout.IntField("Amount", tileA.demands1[i].amount);
                            EditorGUILayout.Space();
                        }

                    skip1:
                        EditorGUILayout.Space();
                        //////// Button2

                        tileA.button2 = (string)EditorGUILayout.TextField("Button 2", tileA.button2);

                        int count2 = EditorGUILayout.DelayedIntField("Number of demands", tileA.demands2.Count != 0 ? tileA.demands2.Count : 0);
                        if (count2 < 0)
                            count2 = 0;

                        if (tileA.demands2.Count == 0 || tileA.demands2.Count != count1)
                        {
                            //tileA.demands1 = new List<Demand>(count1);
                            int cur = tileA.demands2.Count;
                            if (count2 < cur)
                                tileA.demands2.RemoveRange(count2, cur - count2);
                            else if (count2 > cur)
                            {
                                if (count2 > tileA.demands2.Capacity)//this bit is purely an optimisation, to avoid multiple automatic capacity changes.
                                    tileA.demands2.Capacity = count2;
                                tileA.demands1.AddRange(new CustomTile.Demand[count2 - cur]);
                            }
                        }

                        if (count2 == 0)
                            goto skip2;

                        for (int i = 0; i < count2; i++)
                        {
                            EditorGUILayout.Space();
                            EditorGUILayout.LabelField("Element " + i);
                            tileA.demands2[i].variable = (string)EditorGUILayout.TextField("Variable", tileA.demands2[i].variable);
                            tileA.demands2[i].ctile = (CustomTile)EditorGUILayout.ObjectField("Tile", tileA.demands2[i].ctile, typeof(CustomTile), false, null);
                            tileA.demands2[i].amount = (int)EditorGUILayout.IntField("Amount", tileA.demands2[i].amount);
                            EditorGUILayout.Space();
                        }

                    skip2:
                        EditorGUILayout.Space();
                        //////// Button3
                        tileA.button3 = (string)EditorGUILayout.TextField("Button 3", tileA.button3);
                        break;
                    }
            }
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tileA);
        }
    }
#endif
}