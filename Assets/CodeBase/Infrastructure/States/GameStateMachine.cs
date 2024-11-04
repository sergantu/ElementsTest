using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using VContainer;

namespace CodeBase.Infrastructure.States
{
    public class GameStateMachine : IStateMachine
    {
        private Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;
        
        private readonly IObjectResolver _objectResolver;

        public GameStateMachine(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
        }

        public void Init()
        {
            _states = new Dictionary<Type, IExitableState> {
                    [typeof(BootstrapState)] = CreateBootstrapState(),
                    [typeof(LoadProgressState)] = CreateLoadProgressState(),
                    [typeof(LoadLevelState)] = CreateLoadLevelState(),
                    [typeof(GameLoopState)] = CreateGameLoopState()
            };
        }

        private BootstrapState CreateBootstrapState()
        {
            var state = _objectResolver.Resolve<BootstrapState>();
            return state;
        }
        
        private LoadProgressState CreateLoadProgressState()
        {
            var state = _objectResolver.Resolve<LoadProgressState>();
            return state;
        }

        private LoadLevelState CreateLoadLevelState()
        {
            var state = _objectResolver.Resolve<LoadLevelState>();
            return state;
        }

        private GameLoopState CreateGameLoopState()
        {
            var state = _objectResolver.Resolve<GameLoopState>();
            return state;
        }

        public async UniTaskVoid Enter<TState>()
                where TState : class, IState
        {
            IState state = ChangeState<TState>();
            state.Enter();
        }

        public async UniTaskVoid Enter<TState, TPayload>(TPayload payload)
                where TState : class, IPayloadedState<TPayload>
        {
            var state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>()
                where TState : class, IExitableState
        {
            _activeState?.Exit();

            var state = GetState<TState>();
            _activeState = state;

            return state;
        }

        private TState GetState<TState>()
                where TState : class, IExitableState =>
                _states[typeof(TState)] as TState;
    }
}