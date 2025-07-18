using SplashKitSDK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Swinburne_OOP_HD
{
    public class LevelManager
    {
        private List<Level> _levels;
        private Dictionary<string, int> _levelScores;
        private string _scoreFilePath = "level_scores.txt";

        public LevelManager()
        {
            _levels = new List<Level>();
            _levelScores = new Dictionary<string, int>();
            LoadScores();
        }

        public void AddLevel(Level level, string levelName)
        {
            _levels.Add(level);
            if (!_levelScores.ContainsKey(levelName))
                _levelScores[levelName] = 0;
        }

        public int GetHighestScore(string levelName)
        {
            return _levelScores.ContainsKey(levelName) ? _levelScores[levelName] : 0;
        }

        public void SetHighestScore(string levelName, int score)
        {
            if (!_levelScores.ContainsKey(levelName) || score > _levelScores[levelName])
            {
                _levelScores[levelName] = score;
                SaveScores();
            }
        }

        public void LoadScores()
        {
            if (!File.Exists(_scoreFilePath)) return;
            using (StreamReader reader = new StreamReader(_scoreFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split(' ');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int score))
                    {
                        _levelScores[parts[0]] = score;
                    }
                }
            }
        }

        public void SaveScores()
        {
            using (StreamWriter writer = new StreamWriter(_scoreFilePath, false))
            {
                foreach (var entry in _levelScores)
                {
                    writer.WriteLine($"{entry.Key} {entry.Value}");
                }
            }
        }

        public string GetLevelStatus(string levelName)
        {
            int score = GetHighestScore(levelName);
            if (score == 0)
                return "Incomplete";
            else
                return $"Highest score: {score}";
        }

        public void ClearLevelResource(Level level)
        {
            // Implement resource cleanup for a level
            // For example: free bitmaps, clear lists, etc.
            // This is a placeholder for actual resource management
        }

        public List<Level> Levels
        {
            get { return _levels; }
        }
    }
}
