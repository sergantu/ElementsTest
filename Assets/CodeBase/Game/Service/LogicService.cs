using System;
using System.Collections.Generic;
using System.Threading;
using CodeBase.Game.Asset;
using CodeBase.Game.Model;
using CodeBase.Game.Utils;
using CodeBase.Infrastructure.Factory;
using CodeBase.PersistentProgress;
using CodeBase.SaveLoad.Service;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Tilemaps;
using Object = UnityEngine.Object;

namespace CodeBase.Game.Service
{
    public class LogicService
    {
        private const float MoveSpeed = .2f;
        private static readonly Color HideTileColor = new(1f, 1f, 1f, 0f);

        public delegate void Winning();

        public event Winning OnWinning;

        private readonly GameFactory _gameFactory;
        private readonly SaveLoadService _saveLoadService;
        private readonly PersistentProgressService _progressService;

        private Element[,] _tileObjects;
        private bool _isAnimating;

        private Game.Model.Level _level;
        private MatchFinder _matchFinder;

        private CancellationTokenSource _cancellationTokenSource;
        private bool _isWin = false;

        public LogicService(GameFactory gameFactory, SaveLoadService saveLoadService, PersistentProgressService progressService)
        {
            _gameFactory = gameFactory;
            _saveLoadService = saveLoadService;
            _progressService = progressService;
        }

        public void StopGame()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new CancellationTokenSource();
            DestroyAllTiles();
        }

        public async UniTask InitLevel(Game.Model.Level level)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _isAnimating = true;
            _level = level;

            try {
                await InitGrid();
                _matchFinder = new MatchFinder(_level.GridSize);
                await Normalize();
                _isWin = false;
                CheckWin();
            } catch (OperationCanceledException) {
                return;
            }

            _isAnimating = false;
        }

        public async UniTask RestartLevel()
        {
            _isAnimating = true;
            DestroyAllTiles();
            try {
                await InitializeLevelGrid();
                SaveGameField(_tileObjects);
                _matchFinder = new MatchFinder(_level.GridSize);
                await Normalize();
                _isWin = false;
                CheckWin();
            } catch (OperationCanceledException) {
                return;
            }

            _isAnimating = false;
        }

        public async UniTask TrySwipe(Vector2Int from, Vector2Int to)
        {
            if (_isAnimating || !IsValidMove(from, to)) {
                return;
            }

            _isAnimating = true;
            try {
                await SwapTiles(from, to);
                await Normalize();
                CheckWin();
                if (!_isWin) {
                    SaveGameField(_tileObjects);
                }
            } catch (OperationCanceledException) {
                return;
            }
            _isAnimating = false;
        }

        public bool HasTileAbove(Vector2Int position)
        {
            var abovePosition = new Vector2Int(position.x, position.y + 1);
            return abovePosition.y < _level.GridSize.y && _tileObjects[abovePosition.y, abovePosition.x] != null;
        }

        public bool IsTileAt(Vector2Int position)
        {
            return position.x >= 0 && position.x < _level.GridSize.x && position.y >= 0 && position.y < _level.GridSize.y
                   && _tileObjects[position.y, position.x] != null;
        }

        private async UniTask InitGrid()
        {
            if (_progressService.Progress.GameData.ElementsField == null) {
                await InitializeLevelGrid();
            } else {
                await InitilizeGridFromProgress();
            }
        }

        private async UniTask InitializeLevelGrid()
        {
            HideTiles();

            _tileObjects = new Element[_level.GridSize.y, _level.GridSize.x];

            var tasks = new List<UniTask>();
            foreach (var pos in _level.Tilemap.cellBounds.allPositionsWithin) {
                tasks.Add(CreateElement(pos));
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask InitilizeGridFromProgress()
        {
            HideTiles();

            List<List<string>> field = _progressService.Progress.GameData.ElementsField;
            _tileObjects = new Element[_level.GridSize.y, _level.GridSize.x];
            var tasks = new List<UniTask>();
            for (int i = 0; i < field.Count; i++) {
                for (int j = 0; j < field[0].Count; j++) {
                    if (string.IsNullOrEmpty(field[i][j])) {
                        continue;
                    }
                    tasks.Add(CreateTileObject(i, j, field[i][j], new Vector3Int(j, i, 0)));
                }
            }

            await UniTask.WhenAll(tasks);
        }

        private async UniTask CreateElement(Vector3Int pos)
        {
            if (pos.x < 0 || pos.x >= _level.GridSize.x || pos.y < 0 || pos.y >= _level.GridSize.y) {
                return;
            }
            var cellPosition = new Vector3Int(pos.x, pos.y, 0);
            if (!_level.Tilemap.HasTile(cellPosition)) {
                return;
            }
            TileBase tile = _level.Tilemap.GetTile(cellPosition);
            if (tile is ElementPlacerTile elementTile && !string.IsNullOrEmpty(elementTile.ElementId)) {
                await CreateTileObject(pos.y, pos.x, elementTile.ElementId, cellPosition);
            }
        }

        private async UniTask CreateTileObject(int y, int x, string elementId, Vector3Int cellPosition)
        {
            if (!string.IsNullOrEmpty(elementId)) {
                Vector3 position = _level.Tilemap.GetCellCenterWorld(cellPosition);
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                GameObject tileObject = await _gameFactory.CreateElement(elementId, position, _level.TileContainer);
                _cancellationTokenSource.Token.ThrowIfCancellationRequested();
                var element = tileObject.GetComponent<Element>();
                tileObject.transform.localScale = _level.Grid.localScale;
                element.ElementId = elementId;
                SetTilePosition(element, x, y);
                _tileObjects[y, x] = element;
            }
        }

        private void HideTiles()
        {
            for (int x = 0; x < _level.GridSize.x; x++) {
                for (int y = 0; y < _level.GridSize.y; y++) {
                    var cellPosition = new Vector3Int(x, y, 0);
                    if (_level.Tilemap.HasTile(cellPosition)) {
                        _level.Tilemap.SetColor(cellPosition, HideTileColor);
                    }
                }
            }
        }

        private void SetTilePosition(Element tileObject, int x, int y)
        {
            if (tileObject == null) {
                return;
            }
            Vector3 position = tileObject.transform.position;
            position.z = ElementLayer.CalculateZPos(_level.GridSize, new Vector2Int(x, y));
            tileObject.transform.position = position;
        }

        private bool IsValidMove(Vector2Int from, Vector2Int to)
        {
            if (to.x < 0 || to.x >= _level.GridSize.x || to.y < 0 || to.y >= _level.GridSize.y) {
                return false;
            }

            if (Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y) != 1) {
                return false;
            }

            return from.y >= to.y || IsTileAt(new Vector2Int(from.x, from.y + 1));
        }

        private async UniTask SwapTiles(Vector2Int from, Vector2Int to)
        {
            Vector3 fromPosition = _level.Tilemap.GetCellCenterWorld(new Vector3Int(from.x, from.y, 0));
            fromPosition.z = ElementLayer.CalculateZPos(_level.GridSize, from);
            Vector3 toPosition = _level.Tilemap.GetCellCenterWorld(new Vector3Int(to.x, to.y, 0));
            toPosition.z = ElementLayer.CalculateZPos(_level.GridSize, to);

            from = new Vector2Int(from.y, from.x);
            to = new Vector2Int(to.y, to.x);

            var moveTasks = new List<UniTask>();
            if (_tileObjects[from.x, from.y] != null) {
                moveTasks.Add(MoveTileObject(_tileObjects[from.x, from.y], toPosition));
            }
            if (_tileObjects[to.x, to.y] != null) {
                moveTasks.Add(MoveTileObject(_tileObjects[to.x, to.y], fromPosition));
            }

            await UniTask.WhenAll(moveTasks);
            (_tileObjects[from.x, from.y], _tileObjects[to.x, to.y]) = (_tileObjects[to.x, to.y], _tileObjects[from.x, from.y]);
        }

        private async UniTask Normalize()
        {
            bool hasChanges;

            do {
                hasChanges = await FallElements();
                await CheckMatches();
            } while (hasChanges);
        }

        private async UniTask<bool> FallElements()
        {
            bool hasChanges = false;
            List<(Element block, Vector3 newPosition, int x, int y)> blocksToMove = new();
            hasChanges = FindFallingElements(blocksToMove, hasChanges);

            if (blocksToMove.Count > 0) {
                await MoveFallingElements(blocksToMove);
            }
            return hasChanges;
        }

        private bool FindFallingElements(List<(Element block, Vector3 newPosition, int x, int y)> blocksToMove, bool hasChanges)
        {
            for (int x = 0; x < _level.GridSize.x; x++) {
                int emptyPositionY = 0;

                for (int y = 0; y < _level.GridSize.y; y++) {
                    if (_tileObjects[y, x] != null) {
                        if (y > emptyPositionY) {
                            AddElementForFalling(x, y, emptyPositionY, blocksToMove);
                            hasChanges = true;
                        }

                        emptyPositionY++;
                    }
                }
            }
            return hasChanges;
        }

        private async UniTask MoveFallingElements(List<(Element block, Vector3 newPosition, int x, int y)> blocksToMove)
        {
            var moveTasks = new List<UniTask>();
            foreach (var (block, newPosition, x, newY) in blocksToMove) {
                moveTasks.Add(MoveTileObject(block, newPosition));
            }

            await UniTask.WhenAll(moveTasks);
        }

        private void AddElementForFalling(int x, int y, int emptyPositionY, List<(Element block, Vector3 newPosition, int x, int y)> blocksToMove)
        {
            Vector3 newPosition = _level.Tilemap.GetCellCenterWorld(new Vector3Int(x, emptyPositionY, 0));
            newPosition.z = ElementLayer.CalculateZPos(_level.GridSize, new Vector2Int(x, emptyPositionY));
            blocksToMove.Add((_tileObjects[y, x], newPosition, x, emptyPositionY));
            _tileObjects[emptyPositionY, x] = _tileObjects[y, x];
            _tileObjects[y, x] = null;
        }

        private async UniTask MoveTileObject(Element tileObject, Vector3 targetPosition)
        {
            await tileObject.transform.DOLocalMove(targetPosition, MoveSpeed).SetEase(Ease.InSine).WithCancellation(_cancellationTokenSource.Token);
        }

        private async UniTask CheckMatches()
        {
            var matchedTiles = _matchFinder.Find(_tileObjects);

            if (matchedTiles.Count > 0) {
                var destroyTasks = new List<UniTask>();
                foreach (var pos in matchedTiles) {
                    destroyTasks.Add(DestroyTileObject(pos));
                }

                await UniTask.WhenAll(destroyTasks);
                await Normalize();
            }
        }

        private async UniTask DestroyTileObject(Vector2Int position)
        {
            if (_tileObjects[position.y, position.x] != null) {
                await _tileObjects[position.y, position.x].Destroy();
                _tileObjects[position.y, position.x] = null;
            }
        }

        private void DestroyAllTiles()
        {
            for (int y = 0; y < _level.GridSize.y; y++) {
                for (int x = 0; x < _level.GridSize.x; x++) {
                    if (_tileObjects[y, x] == null) {
                        continue;
                    }
                    Object.Destroy(_tileObjects[y, x].gameObject);
                    _tileObjects[y, x] = null;
                }
            }
        }

        private void CheckWin()
        {
            if (!_isWin && IsWin()) {
                Win();
            }
        }

        private void Win()
        {
            _isWin = true;
            OnWinning?.Invoke();
        }

        private bool IsWin()
        {
            for (int i = 0; i < _tileObjects.GetLength(0); i++) {
                for (int j = 0; j < _tileObjects.GetLength(1); j++) {
                    if (_tileObjects[i, j] != null) {
                        return false;
                    }
                }
            }

            return true;
        }

        private void SaveGameField(Element[,] tileObjects)
        {
            var elements = new List<List<string>>();

            for (int i = 0; i < tileObjects.GetLength(0); i++) {
                var row = new List<string>();

                for (int j = 0; j < tileObjects.GetLength(1); j++) {
                    string id = tileObjects[i, j] != null ? tileObjects[i, j].ElementId : string.Empty;
                    row.Add(id);
                }

                elements.Add(row);
            }

            _progressService.Progress.GameData.ElementsField = elements;
            _saveLoadService.SaveProgress();
        }

        public Tilemap GetTilemap => _level.Tilemap;
    }
}