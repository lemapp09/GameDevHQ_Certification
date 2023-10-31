
using UnityEngine;
using UnityEngine.Serialization;

namespace MetroMayhem.Manager
{
    public class LevelManager : MonoSingleton<LevelManager>
    {
        [SerializeField] public int _maxLevel = 10;
        private int _levelCount; // 1 to 10
        private int _numberOfEnenyKilled;

        private void OnEnable() {
            GameManager.StartLevel += StartLevel;
            GameManager.PauseLevel += PauseCurrentLevel;
            GameManager.UnpauseLevel += UnpauseCurrentLevel;
            GameManager.StopLevel += StopLevel;
            Enemies.EnemyAI.EnemyKilled += EnemyDied;
        }

        private void EnemyDied() {
            _numberOfEnenyKilled++;
        }

        private void StartLevel() {
            _levelCount++;
            UIManager.Instance.UpdateLevelCount(_levelCount);
        }

        private void PauseCurrentLevel() {
            //
        }

        private void UnpauseCurrentLevel() {
            //
        }

        private void StopLevel() {
            _numberOfEnenyKilled = 0;
        }
        
        private void OnDisable() {
            GameManager.StartLevel -= StartLevel;
            GameManager.PauseLevel -= PauseCurrentLevel;
            GameManager.UnpauseLevel -= UnpauseCurrentLevel;
            GameManager.StopLevel -= StopLevel;
            Enemies.EnemyAI.EnemyKilled -= EnemyDied;
        }
    }
}