
namespace MetroMayhem.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        private int _playerHealth, _warFunds = 350, _health = 100;
        private bool _isPaused;
        public delegate void startLevel();  /// <summary>
                                            /// The Level Begins
                                            /// </summary>
        public static event startLevel StartLevel;
        public delegate void startPlay();  /// <summary>
        /// The Level Begins
        /// </summary>
        public static event startPlay StartPlay;
        public delegate void pauseLevel();  /// <summary>
                                            /// Player request a Pause
                                            /// </summary>
        public static event pauseLevel PauseLevel; 
        public delegate void unpauseLevel();  /// <summary>
                                              /// Restart the Level after a Pause
                                              /// </summary>
        public static event unpauseLevel UnpauseLevel;
        public delegate void stopLevel();  /// <summary>
                                           ///  Level has ended
                                           /// </summary>
        public static event stopLevel StopLevel;
        public delegate void restartLevel();  /// <summary>
                                              /// Fully Restart the Level
                                              /// </summary>
        public static event restartLevel RestartLevel;

        
        private void Start() {
            Enemies.EnemyAI.EnemySurvived += EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled += EnemyDied;
        }

        public void StartNextLevel() {
            _isPaused = false;
            StartLevel?.Invoke();
        }

        public void StartPlayGame() {
            _isPaused = false;
            StartPlay?.Invoke();
        }

        public void PauseCurrentLevel() {
            _isPaused = true;
            PauseLevel?.Invoke();
        }

        public void UnpauseCurrentLevel() {
            _isPaused = false;
            UnpauseLevel?.Invoke();
        }

        public void RestartCurrentLevel() {
            _isPaused = false;
            RestartLevel?.Invoke();
        }
        
        // EnemySurvived;
        private void EnemyHasReachedTheEnd() {
            _health--;
            UIManager.Instance.UpdateHealth(_health);
        }

        private void EnemyDied()
        {
            _warFunds += 150;
            UIManager.Instance.UpdateWarFunds(_warFunds);
        }

        private void OnDisable() {
            Enemies.EnemyAI.EnemySurvived -= EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled -= EnemyDied;
        }
    }
} 