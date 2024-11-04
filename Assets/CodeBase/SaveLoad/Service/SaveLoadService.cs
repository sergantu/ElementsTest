using CodeBase.Data;
using CodeBase.Infrastructure;
using CodeBase.PersistentProgress;
using Cysharp.Threading.Tasks;
using TigerForge;

namespace CodeBase.SaveLoad.Service
{
    public class SaveLoadService : IService
    {
        private const string CurrentGameProgress = "CurrentGameProgress";
        private const string CurrentGameData = "CurrentGameData";
        private const string Default = "wUH*FW(OKIM";

        private readonly PersistentProgressService _progressService;

        public SaveLoadService(PersistentProgressService progressService)
        {
            _progressService = progressService;
        }

        public void SaveProgress()
        {
            var saveFile = new EasyFileSave(CurrentGameProgress);
            string data = _progressService.Progress.GameData.ToJson();
            saveFile.AddSerialized(CurrentGameData, data);
            saveFile.Save(Default);
        }

        public async UniTask<PlayerProgress> LoadProgress()
        {
            var progress = new PlayerProgress();
            progress.GameData = await LoadCurrentGameProgress();

            return progress;
        }

        private async UniTask<GameData> LoadCurrentGameProgress()
        {
            var saveFile = new EasyFileSave(CurrentGameProgress);
            if (!saveFile.Load(Default)) {
                return new GameData();
            }

            var currentGameData = ((string) saveFile.GetDeserialized(CurrentGameData, typeof(string))).FromJson<GameData>();
            saveFile.Dispose();
            return currentGameData;
        }
    }
}