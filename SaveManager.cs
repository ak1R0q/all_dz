using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace chetv_dz
{
    public class SaveManager
    {
        private readonly string _saveFilePath;

        public SaveManager(string saveFilePath)
        {
            _saveFilePath = saveFilePath;
        }

        public async Task SaveAsync(BattleStats stats)
        {
            var save = new GameSave(stats);
            using (var stream = new FileStream(_saveFilePath, FileMode.Create))
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, save);
            }
            await Task.CompletedTask;
        }

        public async Task<GameSave> LoadAsync()
        {
            if (!File.Exists(_saveFilePath))
                return null;

            using (var stream = new FileStream(_saveFilePath, FileMode.Open))
            {
                var formatter = new BinaryFormatter();
                var save = (GameSave)formatter.Deserialize(stream);
                return await Task.FromResult(save);
            }
        }

        public void DeleteSave()
        {
            if (File.Exists(_saveFilePath))
                File.Delete(_saveFilePath);
        }
    }
}