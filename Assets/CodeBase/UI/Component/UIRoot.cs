using System;
using CodeBase.Level;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace CodeBase.UI.Component
{
    public class UIRoot : MonoBehaviour
    {
        [SerializeField]
        private Button RestartButton;

        [SerializeField]
        private Button NextLevelButton;

        [SerializeField]
        private GameObject LoadingCuretain;

        [Inject]
        private LevelManager _levelManager;

        private void Start()
        {
            SubscribeRestartButton();
            SubscribeNextLevelButton();
        }

        public void ShowLoadingCurtain()
        {
            LoadingCuretain.SetActive(true);
        }
        
        public void HideLoadingCurtain()
        {
            LoadingCuretain.SetActive(false);
        }

        private void SubscribeRestartButton()
        {
            RestartButton.OnClickAsObservable().Subscribe(_ => _levelManager.RestartLevel().Forget()).AddTo(RestartButton);
        }

        private void SubscribeNextLevelButton()
        {
            NextLevelButton.OnClickAsObservable().Subscribe(_ => _levelManager.LoadNextLevel().Forget()).AddTo(NextLevelButton);
        }
    }
}