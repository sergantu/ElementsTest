using CodeBase.Infrastructure.AssetManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CodeBase.Infrastructure.Factory
{
    public class GameFactory : IService
    {
        private readonly AssetProvider _assets;

        public GameFactory(AssetProvider assets)
        {
            _assets = assets;
        }

        public async UniTask<GameObject> CreateLevel(string path)
        {
            return await _assets.Instantiate(path);
        }
        
        public async UniTask<GameObject> CreateElement(string name, Vector3 position, Transform container)
        {
            return await _assets.Instantiate(name, position, container);
        }
        
        public async UniTask<GameObject> CreateBalloon(string name, Transform container)
        {
            return await _assets.Instantiate(name, container);
        }

        public async UniTask<GameObject> CreateUIRoot()
        {
            return await _assets.Instantiate(AssetAddress.UIRoot);
        }

        public void Cleanup() => _assets.Cleanup();
    }
}