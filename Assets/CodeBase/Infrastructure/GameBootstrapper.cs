using CodeBase.Balloons.Model;
using CodeBase.Balloons.Service;
using CodeBase.Game.Service;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Factory;
using CodeBase.Infrastructure.States;
using CodeBase.Input.Service;
using CodeBase.Level;
using CodeBase.PersistentProgress;
using CodeBase.SaveLoad.Service;
using CodeBase.UI.Service;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Infrastructure
{
    public class GameBootstrapper : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<InitialEntryPoint>();

            builder.Register<PersistentProgressService>(Lifetime.Singleton);
            builder.Register<SaveLoadService>(Lifetime.Singleton);
            builder.Register<UIService>(Lifetime.Singleton);
            builder.Register<SceneLoader>(Lifetime.Singleton);
            builder.Register<AssetProvider>(Lifetime.Singleton);
            builder.Register<GameFactory>(Lifetime.Singleton);

            builder.Register<GameStateMachine>(Lifetime.Singleton);
            builder.Register<BootstrapState>(Lifetime.Transient);
            builder.Register<LoadProgressState>(Lifetime.Transient);
            builder.Register<LoadLevelState>(Lifetime.Transient);
            builder.Register<GameLoopState>(Lifetime.Transient);
            
            builder.Register<Balloon>(Lifetime.Transient);
            
            builder.Register<LevelManager>(Lifetime.Singleton);
            builder.Register<LogicService>(Lifetime.Singleton);
            builder.Register<BalloonSpawner>(Lifetime.Singleton);
            
            
            builder.Register<SwipeController>(Lifetime.Singleton).AsImplementedInterfaces().AsSelf();

        }

        /*private void HandleException(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception) {
                Debug.LogError("Unhandled exception: " + condition + ", stackTrace: \n" + stackTrace);
            }
        }

        private void HandleUniTaskException(Exception ex) => Debug.LogError("Unhandled UniTask exception: " + ex);*/
    }
}