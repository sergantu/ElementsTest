using UnityEngine;

namespace CodeBase.Game.Service
{
    public static class ElementLayer
    {
        private const float Coef = 0.01f;
        public static float CalculateZPos(Vector2Int gridSize, Vector2Int coors) => (-coors.x * gridSize.y - coors.y) * Coef;
    }
}