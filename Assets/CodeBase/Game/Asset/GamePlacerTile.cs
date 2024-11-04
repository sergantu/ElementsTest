using UnityEngine;
using UnityEngine.Tilemaps;

namespace CodeBase.Game.Asset
{
    [CreateAssetMenu(fileName = "ElementPlacerTile", menuName = "2D Match/Tile/Element Placer")]
    public class ElementPlacerTile : TileBase
    {
        public string ElementId;
        public Sprite PreviewEditorSprite;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            tileData.sprite = PreviewEditorSprite;
        }

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return false;
            }
#endif
            return base.StartUp(position, tilemap, go);
        }
    }
}