using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace YesChef.Core
{
    public enum GameState
    {
        Warmup,     // Instructions modal shown
        Playing,    // Active 3-minute timer
        Paused,     // TimeScale = 0
        GameOver    // Timer expired, results modal shown
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Match Settings")]
        [SerializeField] private float matchDuration = 180f; 
        public event Action<GameState> OnStateChanged;
        public event Action<float> OnTimerTick;

        public GameState CurrentState { get; private set; } = GameState.Warmup;
        public float TimeRemaining { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            TimeRemaining = matchDuration;
            SetState(GameState.Warmup);
            Time.timeScale = 0f; // Paused while reading instructions
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing)
            {
                TimeRemaining -= Time.deltaTime;
                if (TimeRemaining <= 0f)
                {
                    TimeRemaining = 0f;
                    SetState(GameState.GameOver);
                }
                OnTimerTick?.Invoke(TimeRemaining);
            }
        }

        public void StartGame()
        {
            ScoreManager.Instance?.ResetScore();
            TimeRemaining = matchDuration;
            Time.timeScale = 1f;
            SetState(GameState.Playing);
        }

        public void TogglePause()
        {
            if (CurrentState == GameState.Playing)
            {
                Time.timeScale = 0f;
                SetState(GameState.Paused);
            }
            else if (CurrentState == GameState.Paused)
            {
                Time.timeScale = 1f;
                SetState(GameState.Playing);
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}