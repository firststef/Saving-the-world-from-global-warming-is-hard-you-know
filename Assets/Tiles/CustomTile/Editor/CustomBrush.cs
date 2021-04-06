using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor
{
    [CustomGridBrush(true, false, false, "Custom Brush")]
    [CreateAssetMenu(fileName = "New Custom Brush", menuName = "Brushes/Custom Brush")]
    public class CustomBrush : UnityEditor.Tilemaps.GridBrush
    { 
        public CustomTile customTile;
        public int z = 0;

        public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            z = customTile.zPos;
            var zPosition = new Vector3Int(position.x, position.y,z);
            brushTarget.GetComponent<Tilemap>().SetTile(zPosition,customTile);
            customTile.OnInstantiate(position);
        }

        public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            z = customTile.zPos;
            Vector3Int zPosition = new Vector3Int(position.x, position.y, z);
            if (brushTarget.GetComponent<Tilemap>().GetTile(zPosition) != null)
            {
                if (brushTarget.GetComponent<Tilemap>().GetTile(zPosition).name == customTile.name)
                {
                    customTile.OnDelete(position);
                    base.Erase(grid, brushTarget, zPosition);
                }
            }
        }

        public override void FloodFill(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            Debug.Log("FloodFill does not work");
            return;
        }

        public override void BoxFill(GridLayout gridLayout, GameObject brushTarget, BoundsInt position)
        {
            Debug.Log("BoxFill does not work");
            return;
        }

        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pickStart)
        {
            Debug.Log("Pick does not work");
            return;
        }

    }

    [CustomEditor(typeof(CustomBrush))]
    public class CustomBrushEditor : UnityEditor.Tilemaps.GridBrushEditor
    {
        private CustomBrush customBrush { get { return target as CustomBrush; } }
        private Tilemap tilemap;

        public override void PaintPreview(GridLayout grid, GameObject brushTarget, Vector3Int position)
        {
            customBrush.z = customBrush.customTile.zPos;
            var zPosition = new Vector3Int(position.x, position.y, customBrush.z);
            base.PaintPreview(grid, brushTarget, zPosition);
        }

        public override void OnPaintSceneGUI(GridLayout grid, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            base.OnPaintSceneGUI(grid, brushTarget, position, tool, executing);

            tilemap = brushTarget.GetComponent<Tilemap>();// adaugat
            tilemap.SetEditorPreviewTile(new Vector3Int(position.x, position.y, position.z), customBrush.customTile);//adaugat

            if (customBrush.z != 0)
            {
                var zPosition = new Vector3Int(position.min.x, position.min.y, customBrush.z);
                BoundsInt newPosition = new BoundsInt(zPosition, position.size);
                Vector3[] cellLocals = new Vector3[]
                {
                    grid.CellToLocal(new Vector3Int(newPosition.min.x, newPosition.min.y, newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.max.x, newPosition.min.y, newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.max.x, newPosition.max.y, newPosition.min.z)),
                    grid.CellToLocal(new Vector3Int(newPosition.min.x, newPosition.max.y, newPosition.min.z))
                };

                Handles.color = Color.blue;
                int i = 0;
                for (int j = cellLocals.Length - 1; i < cellLocals.Length; j = i++)
                {
                    Handles.DrawLine(cellLocals[j], cellLocals[i]);
                }
            }

            var labelText = "Pos: " + new Vector3Int(position.x, position.y, customBrush.customTile.zPos);
            if (position.size.x > 1 || position.size.y > 1)
            {
                labelText += " Size: " + new Vector2Int(position.size.x, position.size.y);
            }

            Handles.Label(grid.CellToWorld(new Vector3Int(position.x, position.y, customBrush.customTile.zPos)), labelText);
        }

        public override void ClearPreview()
        {
            base.ClearPreview();
            if (tilemap != null)
            {
                tilemap.ClearAllEditorPreviewTiles();
            }
        }
    }
}