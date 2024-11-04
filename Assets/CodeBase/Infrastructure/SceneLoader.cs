using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CodeBase.Infrastructure
{
    public class SceneLoader : IService
    {
        public async UniTaskVoid LoadAsync(string nextScene, Action onLoaded)
        {
            if (SceneManager.GetActiveScene().name == nextScene) {
                onLoaded?.Invoke();
                return;
            }

            var cancellationToken = new CancellationTokenSource();
            AsyncOperation waitNextScene = SceneManager.LoadSceneAsync(nextScene);

            bool isCancelled = await UniTask.WaitWhile(() => !waitNextScene.isDone, cancellationToken: cancellationToken.Token)
                                            .SuppressCancellationThrow();

            if (isCancelled) {
                Debug.LogWarning("Loading scene is cancelled");
                return;
            }

            onLoaded?.Invoke();
        }
    }
}