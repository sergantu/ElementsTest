using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.States
{
    public interface IState : IExitableState
    {
        UniTaskVoid Enter();
    }
    
    public interface IPayloadedState<in TPayload> : IExitableState
    {
        UniTaskVoid Enter(TPayload payload);
    }

    public interface IExitableState
    {
        UniTaskVoid Exit();
    }
}