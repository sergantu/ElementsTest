using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.States
{
    public interface IStateMachine
    {
        UniTaskVoid Enter<TState>() where TState : class, IState;
        UniTaskVoid Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>;
    }
}