using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YesChef.Core;

namespace YesChef.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;
        [SerializeField] private TextMeshProUGUI matchTimerText;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button quitButton;

        [Header("Modals")]
        [SerializeField] private GameObject instructionsModal;
        [SerializeField] private Button startMatchButton;

        [SerializeField] private GameObject pauseModal;
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button pauseQuitButton;

        [SerializeField] private GameObject gameOverModal;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private GameObject newHighScoreBadge;
        [SerializeField] private Button restartButton;

        private void Start()
        {
            // Bind button click events
            if (startMatchButton != null) startMatchButton.onClick.AddListener(() => GameManager.Instance.StartGame());
            if (pauseButton != null) pauseButton.onClick.AddListener(() => GameManager.Instance.TogglePause());
            if (resumeButton != null) resumeButton.onClick.AddListener(() => GameManager.Instance.TogglePause());
            if (quitButton != null) quitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());
            if (pauseQuitButton != null) pauseQuitButton.onClick.AddListener(() => GameManager.Instance.QuitGame());
            if (restartButton != null) restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());

            // Bind game events
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged += HandleStateChanged;
                GameManager.Instance.OnTimerTick += UpdateTimerDisplay;
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged += UpdateScoreDisplay;
                ScoreManager.Instance.OnHighScoreChanged += UpdateHighScoreDisplay;
                UpdateHighScoreDisplay(ScoreManager.Instance.HighScore);
                UpdateScoreDisplay(0);
            }

            if (InputReader.Instance != null)
            {
                InputReader.Instance.OnPauseEvent += () => GameManager.Instance.TogglePause();
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnStateChanged -= HandleStateChanged;
                GameManager.Instance.OnTimerTick -= UpdateTimerDisplay;
            }

            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnScoreChanged -= UpdateScoreDisplay;
                ScoreManager.Instance.OnHighScoreChanged -= UpdateHighScoreDisplay;
            }
        }

        private void HandleStateChanged(GameState state)
        {
            instructionsModal.SetActive(state == GameState.Warmup);
            pauseModal.SetActive(state == GameState.Paused);
            gameOverModal.SetActive(state == GameState.GameOver);

            if (state == GameState.GameOver)
            {
                if (finalScoreText != null)
                {
                    finalScoreText.text = $"Final Score: {ScoreManager.Instance.CurrentScore}";
                }
                if (newHighScoreBadge != null)
                {
                    newHighScoreBadge.SetActive(ScoreManager.Instance.IsNewHighScore);
                }
                Time.timeScale = 0f;
            }
        }

        private void UpdateTimerDisplay(float secondsRemaining)
        {
            if (matchTimerText == null) return;

            int minutes = Mathf.FloorToInt(secondsRemaining / 60f);
            int seconds = Mathf.FloorToInt(secondsRemaining % 60f);
            matchTimerText.text = $"{minutes:00}:{seconds:00}";

            // Visual feedback pulse when under 30 seconds
            if (secondsRemaining <= 30f)
            {
                matchTimerText.color = new Color(1f, 0.25f, 0.25f);
            }
            else
            {
                matchTimerText.color = Color.white;
            }
        }

        private void UpdateScoreDisplay(int score)
        {
            if (currentScoreText != null) currentScoreText.text = $"Score: {score}";
        }

        private void UpdateHighScoreDisplay(int highScore)
        {
            if (highScoreText != null) highScoreText.text = $"High: {highScore}";
        }
    }
}