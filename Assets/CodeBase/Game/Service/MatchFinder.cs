using System.Collections.Generic;
using CodeBase.Game.Model;
using UnityEngine;

namespace CodeBase.Game.Utils
{
    public class MatchFinder
    {
        private static readonly Vector2Int[] Directions = { new(0, 1), new(0, -1), new(-1, 0), new(1, 0) };

        private Vector2Int _gridSize;

        public MatchFinder(Vector2Int gridSize)
        {
            _gridSize = gridSize;
        }

        public HashSet<Vector2Int> Find(Element[,] tileObjects)
        {
            var matchedTiles = new HashSet<Vector2Int>();

            for (int i = 0; i < _gridSize.x; i++) {
                for (int j = 0; j < _gridSize.y; j++) {
                    if (tileObjects[j, i] == null) {
                        continue;
                    }

                    var current = tileObjects[j, i];
                    HandleHorizontalMatch(tileObjects, i, j, current, matchedTiles);
                    HandleVerticalMatch(tileObjects, i, j, current, matchedTiles);
                }
            }

            return matchedTiles;
        }

        private void HandleHorizontalMatch(Element[,] tileObjects, int i, int j, Element current, HashSet<Vector2Int> matchedTiles)
        {
            if (i >= _gridSize.x - 2) {
                return;
            }
            var rightElement1 = tileObjects[j, i + 1];
            var rightElement2 = tileObjects[j, i + 2];
            if (IsSameElementType(current, rightElement1) && IsSameElementType(current, rightElement2)) {
                AddMatch(tileObjects, new Vector2Int(i, j), tileObjects[j, i].ElementId, matchedTiles);
            }
        }

        private void HandleVerticalMatch(Element[,] tileObjects, int i, int j, Element current, HashSet<Vector2Int> matchedTiles)
        {
            if (j >= _gridSize.y - 2) {
                return;
            }
            var topElement1 = tileObjects[j + 1, i];
            var topElement2 = tileObjects[j + 2, i];
            if (IsSameElementType(current, topElement1) && IsSameElementType(current, topElement2)) {
                AddMatch(tileObjects, new Vector2Int(i, j), tileObjects[j, i].ElementId, matchedTiles);
            }
        }

        private bool IsSameElementType(Element main, Element right) => right != null && right.ElementId == main.ElementId;

        private void AddMatch(Element[,] tileObjects, Vector2Int position, string elementId, HashSet<Vector2Int> matchedTiles)
        {
            if (matchedTiles.Contains(position)) {
                return;
            }

            matchedTiles.Add(position);
            CheckAdjacentTiles(tileObjects, position, elementId, matchedTiles);
        }

        private void CheckAdjacentTiles(Element[,] tileObjects, Vector2Int position, string elementId, HashSet<Vector2Int> matchedTiles)
        {
            foreach (var dir in Directions) {
                Vector2Int neighborPos = position + dir;
                if (!IsWithinBounds(neighborPos) || tileObjects[neighborPos.y, neighborPos.x] == null) {
                    continue;
                }
                var neighborElement = tileObjects[neighborPos.y, neighborPos.x];
                if (neighborElement != null && neighborElement.ElementId == elementId) {
                    AddMatch(tileObjects, neighborPos, elementId, matchedTiles);
                }
            }
        }

        private bool IsWithinBounds(Vector2Int position) =>
                position.x >= 0 && position.x < _gridSize.x && position.y >= 0 && position.y < _gridSize.y;
    }
}