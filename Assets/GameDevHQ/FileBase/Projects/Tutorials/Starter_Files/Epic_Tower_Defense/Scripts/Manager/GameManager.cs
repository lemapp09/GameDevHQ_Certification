
using System;
using MetroMayhem.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace MetroMayhem.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region Variables
        [SerializeField] private int _playerHealth, _warFunds = 350, _health = 100; // Initialize with your starting WarFunds
        private bool _isPaused;
        public delegate void startLevel();  //The Level Begins
        public static event startLevel StartLevel;
        public delegate void startPlay();  //The Level Begins
        public static event startPlay StartPlay;
        public delegate void pauseLevel();  //Player request a Pause
        public static event pauseLevel PauseLevel; 
        public delegate void unpauseLevel();  //Restart the Level after a Pause
        public static event unpauseLevel UnpauseLevel;
        public delegate void stopLevel();  //Level has ended
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
        private int PlatformID = -1, WeaponID  = -1; // Part of weapon Purchase
        #endregion

        private void OnEnable()
        {
            Enemies.EnemyAI.EnemySurvived += EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled += EnemyDied;
            isPlatformOccupied = new bool[platforms.Length];
        }

        private void Start() {
            StartNextLevel();
        }

        public void StartNextLevel() {
            UIManager.Instance.UpdateWarFunds(_warFunds);
            _isPaused = true;
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

        public void StopCurrentLevel() {
            StopLevel?.Invoke();
        }

        public void SelectPlatform(int platformIndex)
        {
            if (!_platformHasBeenSelected)
            {
                _platformHasBeenSelected = true;
                PlatformID = platformIndex;
                PlatformSelected?.Invoke();
                PurchaseTower();
            }
        }

        public void UnselectPlatform()
        {
            _platformHasBeenSelected = false;
            PlatformID = -1;
            PlatformUnselected?.Invoke();
        }

        public void CurrentWeaponSelected(int WeaponID) {
            if (this.WeaponID == -1 && !_weaponSelected)
            {
                _weaponSelected = true;
                this.WeaponID = WeaponID;
                PurchaseTower();
            }
        }

        public void CurrentWeaponUnselected() {
            _weaponSelected = false;
            WeaponID = -1;
            UIManager.Instance.RemovePurchasePanel();
            UIManager.Instance.DisplayAffordTower();
        }

        private void PurchaseTower()
        {
            if (PlatformID != -1 && WeaponID != -1)
            {
                UIManager.Instance.DisplayPurchasePanel(PlatformID, WeaponID,_towerPrices[WeaponID]);
            }
        }

        public bool AffordTower(int TowerId) {
            if (_towerPrices[TowerId] < _warFunds) {
                return true;
            }
            return false; // (true if WarFunds is greater than or equal to the tower price, false otherwise)
        }
            
        public void PlaceTower(int PlacementPlatformID, int PlacementWeaponID)
        {
            if (_isPaused)
            {
                if (PlacementPlatformID >= 0 && PlacementPlatformID < platforms.Length &&
                    !isPlatformOccupied[PlacementPlatformID])
                {
                    if (PlacementWeaponID >= 0 && PlacementWeaponID < towerPrefabs.Length &&
                        _warFunds >= _towerPrices[PlacementWeaponID])
                    {
                        // Deduct the purchase price from WarFunds
                        _warFunds -= _towerPrices[PlacementWeaponID];
                        UIManager.Instance.UpdateWarFunds(_warFunds);

                        // Instantiate the selected Tower at the platform's position
                        GameObject obj = Instantiate(towerPrefabs[PlacementWeaponID], platforms[PlacementPlatformID].transform.position +
                                new Vector3(0, 0.3f, 0),
                            Quaternion.identity);
                        float _tempRotY = platforms[PlacementPlatformID].GetComponent<Platform>().WhatIsTheDefaulAngle();
                        obj.GetComponent<WeaponID>().SetPlatformID(PlacementPlatformID);
                        obj.transform.localRotation = Quaternion.Euler(0, _tempRotY, 0);
                        
                        // Mark the platform as occupied
                        isPlatformOccupied[PlacementPlatformID] = true;
                        platforms[PlacementPlatformID].GetComponent<Platform>().SetAsOccupied(obj);
                    }
                }
            }
            UnselectPlatform();
            CurrentWeaponUnselected();
        }

        public void UpgradeTower(int platformID, int weaponID) {
            if ( ( weaponID == 0 && _warFunds > 500) || (weaponID == 1 && _warFunds > 750 )) {
                UIManager.Instance.DisplayUpgradePanel(platformID, weaponID);
            }
        }
        
        public void UpgradeYes(int platformID, int weaponID)
        {
            platforms[platformID].GetComponent<Platform>().RemoveOccupyingWeapon();
            isPlatformOccupied[platformID] = false;
            PlaceTower(platformID, weaponID + 2);
        }

        public void DismantleTower(int platformID, int weaponID)
        {
            if (isPlatformOccupied[platformID])
            {
                UIManager.Instance.DisplayDismantlePanel(platformID, weaponID, _towerPrices[weaponID]);
            }
        }

        public void DismantleYes(int dismantlePlatformID, int dismantleWeaponID)
        {
            isPlatformOccupied[dismantlePlatformID] = false;
            platforms[dismantlePlatformID].GetComponent<Platform>().RemoveOccupyingWeapon();
            _warFunds += _towerPrices[dismantleWeaponID];
            UIManager.Instance.UpdateWarFunds(_warFunds);
        }
        
        private void EnemyHasReachedTheEnd() { // EnemySurvived;
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