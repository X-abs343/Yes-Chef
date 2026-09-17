using System;
using UnityEngine;
using YesChef.Stations;

namespace YesChef.Core
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }

        private const string HighScoreKey = "YES_CHEF_HIGH_SCORE";

        public event Action<int> OnScoreChanged;
        public event Action<int> OnHighScoreChanged;

        public int CurrentScore { get; private set; }
        public int HighScore { get; private set; }
        public bool IsNewHighScore { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            LoadHighScore();
        }

        private void OnEnable()
        {
            DeliveryWindow.OnOrderDeliveredScore += AddScore;
        }

        private void OnDisable()
        {
            DeliveryWindow.OnOrderDeliveredScore -= AddScore;
        }

        public void ResetScore()
        {
            CurrentScore = 0;
            IsNewHighScore = false;
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void AddScore(int amount)
        {
            CurrentScore += amount;
            OnScoreChanged?.Invoke(CurrentScore);

            if (CurrentScore > HighScore)
            {
                HighScore = CurrentScore;
                IsNewHighScore = true;
                SaveHighScore();
                OnHighScoreChanged?.Invoke(HighScore);
            }
        }

        private void LoadHighScore()
        {
            HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
            OnHighScoreChanged?.Invoke(HighScore);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }
    }
}