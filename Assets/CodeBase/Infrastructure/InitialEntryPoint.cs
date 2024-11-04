using CodeBase.Infrastructure.States;
using UnityEngine;
using VContainer.Unity;

namespace CodeBase.Infrastructure
{
    public class InitialEntryPoint : IStartable
    {
        private const int TargetFrameRate = 60;
        private readonly GameStateMachine _stateMachine;

        public InitialEntryPoint(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Start()
        {
            /*Application.logMessageReceived += HandleException;
            UniTaskScheduler.UnobservedTaskException += HandleUniTaskException;*/
            Application.targetFrameRate = TargetFrameRate;

            _stateMachine.Init();
            _stateMachine.Enter<BootstrapState>().Forget();
        }
    }
}