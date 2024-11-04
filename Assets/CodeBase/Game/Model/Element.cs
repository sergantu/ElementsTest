using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Game.Model
{
    public class Element : MonoBehaviour
    {
        public string ElementId;

        private static readonly int DestroyTrigger = Animator.StringToHash("destroy");
        private const string IdleAnimName = "Idle";
        public Animator _animator;
        private CancellationTokenSource _cancellationTokenSource;

        private void Start()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;
            float randomStartTime = Random.Range(0f, animationLength); // значение от 0 до 1
            _animator.Play(IdleAnimName, 0, randomStartTime);
        }

        public async UniTask Destroy()
        {
            _animator.SetTrigger(DestroyTrigger);

            try {
                await WaitForAnimationToEnd(_cancellationTokenSource.Token);
            }  finally {
                if (!this.IsDestroyed()) {
                    Destroy(gameObject);
                }
            }
        }

        private async UniTask WaitForAnimationToEnd(CancellationToken cancellationToken)
        {
            AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;
            await UniTask.Delay((int) (animationLength * 1000), cancellationToken: cancellationToken);
        }

        private void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}