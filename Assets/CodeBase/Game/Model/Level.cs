using UnityEngine;
using UnityEngine.Tilemaps;

namespace CodeBase.Game.Model
{
    public class Level : MonoBehaviour
    {
        public Tilemap Tilemap;
        public Vector2Int GridSize;
        public Transform TileContainer;
        public Transform BalloonContainer;
        public Transform Grid;
    }
}