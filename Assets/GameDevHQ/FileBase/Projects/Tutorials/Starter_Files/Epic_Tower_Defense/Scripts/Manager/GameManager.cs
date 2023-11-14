using System.Collections;
using MetroMayhem.Weapons;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroMayhem.Manager
{
    public class GameManager : MonoSingleton<GameManager>
    {
        #region Variables
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
        
        public delegate void platformSelected();  
        public static event platformSelected PlatformSelected;
        
        public delegate void platformUnselected();  
        public static event platformUnselected PlatformUnselected;
        
            /// <summary>
            /// The four Tower/ Turret Weapons are: Gatling Gun (0),
            /// Missile Launcher (1), Dual Gatling Gun (2),
            /// Dual Missile Launcher (3)
            /// </summary>
        [SerializeField] private GameObject[] towerPrefabs;  // Array of Tower prefabs 
        [SerializeField] private GameObject[] platforms;     // Array of platform gameobjects with transform positions
        [SerializeField] private int[] _towerPrices = new []{200, 500, 700, 1250};  // Array of tower purchase prices
        
        private bool[] isPlatformOccupied;  // Boolean array to track if a platform is occupied
        [SerializeField] private int _warFunds = 350, _health = 100, _currentLevel = 1; // Initialize with your starting WarFunds
        private int PlatformID = -1, WeaponID  = -1; // Part of weapon Purchase
        private int _archiveWarFunds, _enemyThisLevel, _enemyAlive;
        private bool[] _archivePlatOccup;
        private int[]  _archiveWeaponID;
        private Vector3[] _archiveWeaponRotation;
        
        [SerializeField] private bool _isPaused;
        private bool _platformHasBeenSelected, _weaponSelected;
        
        [Header("Pathway Arrows")]
        [SerializeField] private GameObject _pathwayArrows;
        #endregion

        private void OnEnable() {
            Enemies.EnemyAI.EnemySurvived += EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled += EnemyDied;
            isPlatformOccupied = new bool[platforms.Length];
            _archivePlatOccup = new bool[platforms.Length];
            _archiveWeaponID = new int[platforms.Length];
            _archiveWeaponRotation = new Vector3[platforms.Length];
        }

        private void Start() {
            StartNextLevel();
        }

        public void StartNextLevel() {
            // Archive  _warFunds, _isPlatformOccupied, weaponIDs[platformID], transform of each weapon
            _health = 100; PlatformID = -1; WeaponID  = -1;
            UIManager.Instance.UpdateHealth(_health);
            UIManager.Instance.UpdateWarFunds(_warFunds);
            UIManager.Instance.LevelDisplay("LEVEL  " + _currentLevel + "\nSTARTING");
            _isPaused = true;
            StartLevel?.Invoke();
            ArchiveLevelData();
        }

        public void SetNumberOfEnemy(int NumberOfEnemy) {
            _enemyThisLevel = NumberOfEnemy; 
            _enemyAlive = NumberOfEnemy;
        }

        public void ArchiveLevelData() {
            _archiveWarFunds = _warFunds;
            for (int i = 0; i < platforms.Length; i++) { 
                _archivePlatOccup[i] = isPlatformOccupied[i];
                _archiveWeaponID[i] = platforms[i].GetComponent<Platform>().GetWeaponID();
                if ( platforms[i].GetComponent<Platform>().GetOccupyingWeapon() != null ) {
                    _archiveWeaponRotation[i] = 
                        platforms[i].GetComponent<Platform>().GetOccupyingWeapon().transform
                        .eulerAngles;
                } else  {
                    _archiveWeaponRotation[i] = Vector3.zero;
                }
            }
        }

        public void RestoreLevel()
        {
            _warFunds = _archiveWarFunds;
            _health = 100;
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i].GetComponent<Platform>().RemoveOccupyingWeapon();
                Debug.Log("Restoring Level: Platform#" + i + " is occupied (" + _archivePlatOccup[i] + ") ");
                isPlatformOccupied[i] = _archivePlatOccup[i];
                if (_archivePlatOccup[i]) {
                    GameObject obj = Instantiate(towerPrefabs[_archiveWeaponID[i]], platforms[i].transform.position +
                        new Vector3(0, 0.3f, 0),
                        Quaternion.identity);
                    obj.transform.eulerAngles = _archiveWeaponRotation[i];
                    platforms[i].GetComponent<Platform>().SetAsOccupied(obj, _archiveWeaponID[i]);
                }
            }
            UIManager.Instance.UpdateHealth(_health);
            UIManager.Instance.UpdateWarFunds(_warFunds);
            _isPaused = true;
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

        public void AllEnemyKilled() {  // Win
            UIManager.Instance.LevelDisplay("LEVEL  " + _currentLevel + "\nWON");
            StopCurrentLevel();
            _currentLevel++;
            if (_currentLevel > 10) {
                UIManager.Instance.LevelDisplay("GAME\nWON");
                StartCoroutine(LoadGameWonScene());
            }
            Invoke(nameof(StartNextLevel), 7f);
        }

        private IEnumerator LoadGameWonScene() {
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(2);
        }

        public void RestartCurrentLevel() {  // Lose
            UIManager.Instance.LevelDisplay("LEVEL  " + _currentLevel + "\nLOST");
            StopLevel?.Invoke();
            RestoreLevel();
            UIManager.Instance.ResetFastForward();
            RestartLevel?.Invoke();
        }

        public void StopCurrentLevel() {
            UIManager.Instance.ResetFastForward();
            StopLevel?.Invoke();
        }

        public int GetCurrentLevel() {
            return _currentLevel;
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
            
        public void PlaceTower(int PlacementPlatformID, int PlacementWeaponID) {
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
                        float _tempRotY = 0;
                        obj.GetComponent<WeaponID>().SetPlatformID(PlacementPlatformID);
                        obj.transform.localRotation = Quaternion.Euler(0, _tempRotY, 0);
                        
                        // Mark the platform as occupied
                        isPlatformOccupied[PlacementPlatformID] = true;
                        platforms[PlacementPlatformID].GetComponent<Platform>().SetAsOccupied(obj, PlacementWeaponID);
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
            _enemyAlive--;
            UIManager.Instance.UpdateEnemyCount(_enemyAlive, _enemyThisLevel);
            _health--;
            UIManager.Instance.UpdateHealth(_health);
            if(_health <= 0) {
                RestartCurrentLevel();
            }
        }

        private void EnemyDied()
        {
            _enemyAlive--;
            UIManager.Instance.UpdateEnemyCount(_enemyAlive, _enemyThisLevel);
            _warFunds += 15;
            UIManager.Instance.UpdateWarFunds(_warFunds);
        }

        public void WeaponFired(int AmmoCost)
        {
            _warFunds -= AmmoCost;
        }

        public void TogglePathwayArrows(bool isVisible) {
            _pathwayArrows.SetActive(isVisible);
        }
        
        private void OnDisable() {
            Enemies.EnemyAI.EnemySurvived -= EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled -= EnemyDied;
        }
    }
} 