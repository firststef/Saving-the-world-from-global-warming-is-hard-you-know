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
            
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(tileA);

            EditorGUILayout.Space();
            int zPos = (int) EditorGUILayout.FloatField("Z Position", tileA.zPos);
            tileA.zPos = zPos;

            EditorGUILayout.Space();
            CustomTile.Type type = (CustomTile.Type) EditorGUILayout.EnumPopup("Type", tileA.type);
            tileA.type = type;
        }
    }
#endif
}