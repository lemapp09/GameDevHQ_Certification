
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace MetroMayhem.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region Variables
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
        
        public delegate void platformSelected();  /// <summary>
        ///  Level has ended
        /// </summary>
        public static event platformSelected PlatformSelected;
        
        public delegate void platformUnselected();  /// <summary>
        ///  Level has ended
        /// </summary>
        public static event platformUnselected PlatformUnselected;
        
            /// <summary>
            /// The four Tower/ Turret Weapons are: Gatling Gun (0),
            /// Missile Launcher (1), Dual Gatling Gun (2),
            /// Dual Missile Launcher (3)
            /// </summary>
        public GameObject[] towerPrefabs;  // Array of Tower prefabs 
        public GameObject[] platforms;     // Array of platform gameobjects with transform positions
        public bool[] isPlatformOccupied;  // Boolean array to track if a platform is occupied
        public int[] _towerPrices = new []{200, 500, 700, 1250};  // Array of tower purchase prices
        private bool _platformHasBeenSelected, _weaponSelected;
        private int warFunds = 1000,        // Initialize with your starting WarFunds
            _currentlySelectedPlatform = -1, _currentSelectedWeapon  = -1; // Part of weapon Purchase
        #endregion

        private void OnEnable()
        {
            Enemies.EnemyAI.EnemySurvived += EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled += EnemyDied;
        }

        private void Start() {
            StartNextLevel();
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

        public void StopCurrentLevel()
        {
            StopLevel?.Invoke();
        }

        public void SelectPlatform(int platformIndex)
        {
            if (!_platformHasBeenSelected)
            {
                _platformHasBeenSelected = true;
                _currentlySelectedPlatform = platformIndex;
                PlatformSelected?.Invoke();
            }
        }

        public void UnselectPlatform(int platformIndex)
        {
            _platformHasBeenSelected = false;
            _currentlySelectedPlatform = -1;
            PlatformUnselected?.Invoke();
        }

        public void CurrentWeaponSelected(int WeaponID) {
            _weaponSelected = true;
            _currentSelectedWeapon = WeaponID;
        }

        public void CurrentWeaponUnselected(int WeaponID) {
            _weaponSelected = false;
            _currentSelectedWeapon = -1;
            UIManager.Instance.DisplayAffordTower();
        }

        private void PurchaseTower()
        {
            if (_currentlySelectedPlatform != -1 && _currentSelectedWeapon != -1)
            {
                
            }
        }

        public bool AffordTower(int TowerId) {
            if (_towerPrices[TowerId] < _warFunds) {
                return true;
            }
            return false; // (true if WarFunds is greater than or equal to the tower price, false otherwise)
        }
            
        public void PlaceTower(int towerIndex, int platformIndex)
        {
            if (_isPaused)
            {
                if (platformIndex >= 0 && platformIndex < platforms.Length && !isPlatformOccupied[platformIndex])
                {
                    if (towerIndex >= 0 && towerIndex < towerPrefabs.Length && warFunds >= _towerPrices[towerIndex])
                    {
                        // Deduct the purchase price from WarFunds
                        warFunds -= _towerPrices[towerIndex];

                        // Instantiate the selected Tower at the platform's position
                        Instantiate(towerPrefabs[towerIndex], platforms[platformIndex].transform.position,
                            Quaternion.identity);

                        // Mark the platform as occupied
                        isPlatformOccupied[platformIndex] = true;
                    }
                }
            }
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