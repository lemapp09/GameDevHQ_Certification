using System.Collections;
using MetroMayhem.Weapons;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MetroMayhem.Manager
{
    /// <summary>
    /// Manages the gameplay and logic of the tower defense game.
    /// </summary>
    public class GameManager : MonoSingleton<GameManager>
    {
        #region Variables

        /// <summary>
        /// Delegate representing the start of a level event.
        /// </summary>
        /// <remarks>
        /// This delegate is used to define a method signature for handling the start of a level event.
        /// </remarks>
        /// <example>
        /// This example shows how to subscribe to and handle the start of a level event.
        /// <code>
        /// // Create an instance of the delegate
        /// startLevel levelStartDelegate = () => Console.WriteLine("Level started!");
        /// // Subscribe to the event
        /// GameManager.LevelStarted += levelStartDelegate;
        /// // Trigger the event
        /// GameManager.StartLevel();
        /// // Unsubscribe from the event
        /// GameManager.LevelStarted -= levelStartDelegate;
        /// </code>
        /// </example>
        public delegate void startLevel();  //The Level Begins

        /// <summary>
        /// Represents an event that occurs when a new level starts.
        /// </summary>
        public static event startLevel StartLevel;

        /// <summary>
        /// Delegate representing a method that starts playing.
        /// </summary>
        public delegate void startPlay();  //The Level Begins

        /// <summary>
        /// Event that is raised when the start play action is triggered.
        /// </summary>
        public static event startPlay StartPlay;

        /// <summary>
        /// Represents a delegate to be used for pausing a level in a game.
        /// </summary>
        public delegate void pauseLevel();  //Player request a Pause

        /// <summary>
        /// Represents an event for pausing the level.
        /// </summary>
        public static event pauseLevel PauseLevel;

        /// <summary>
        /// Represents a delegate for unpausing a level.
        /// </summary>
        public delegate void unpauseLevel();  //Restart the Level after a Pause

        /// <summary>
        /// Event triggered when the game is unpaused at a specific level.
        /// </summary>
        /// <remarks>
        /// This event is raised when the game is unpaused at a specific level.
        /// It can be subscribed to in order to perform specific actions when the game is unpaused.
        /// </remarks>
        public static event unpauseLevel UnpauseLevel;

        /// <summary>
        /// Represents a delegate that is used to stop a level.
        /// </summary>
        /// <remarks>
        /// This delegate can be used to define a method signature that will be called when a level needs to be stopped.
        /// </remarks>
        public delegate void stopLevel();  //Level has ended

        /// <summary>
        /// Event that is raised when the stop level is reached.
        /// </summary>
        public static event stopLevel StopLevel;

        /// <summary>
        /// Represents a delegate for restarting a level in a game.
        /// </summary>
        public delegate void restartLevel();

        /// <summary>
        /// Fully restarts the level.
        /// </summary>
        public static event restartLevel RestartLevel;

        /// <summary>
        /// Represents a delegate that is used to handle the event when a platform is selected.
        /// </summary>
        /// <remarks>
        /// This delegate is typically used in UI frameworks where a user can select a platform and an event needs to be triggered when the platform is selected.
        /// </remarks>
        public delegate void platformSelected();

        /// <summary>
        /// Represents an event that is raised when a platform is selected.
        /// </summary>
        /// <remarks>
        /// The PlatformSelected event is triggered when a platform is selected
        /// by the user. Event subscribers can perform custom logic when this
        /// event is raised.
        /// </remarks>
        public static event platformSelected PlatformSelected;

        /// <summary>
        /// Delegate for handling when a platform is unselected.
        /// </summary>
        /// <param name="platformUnselected">The event handler for platform unselection.</param>
        public delegate void platformUnselected();

        /// <summary>
        /// Represents an event that is raised when a platform is unselected.
        /// </summary>
        public static event platformUnselected PlatformUnselected;

        /// <summary>
        /// An array of tower prefabs.
        /// <para>The four Tower/Turret Weapons are:
        /// <list type="bullet">
        /// <item>Gatling Gun (0)</item>
        /// <item>Missile Launcher (1)</item>
        /// <item>Dual Gatling Gun (2)</item>
        /// <item>Dual Missile Launcher (3)</item>
        /// </list>
        /// </para>
        /// </summary>
        [SerializeField] private GameObject[] towerPrefabs; // Array of Tower prefabs 

        /// <summary>
        /// Array of platform GameObjects with transform positions.
        /// </summary>
        [SerializeField] private GameObject[] platforms; // Array of platform gameobjects with transform positions

        /// <summary>
        /// Array of tower purchase prices.
        /// </summary>
        [SerializeField] private int[] _towerPrices = new []{200, 500, 700, 1250};  // Array of tower purchase prices

        /// <summary>
        /// Boolean array used to track if a platform is occupied.
        /// </summary>
        private bool[] isPlatformOccupied;  // Boolean array to track if a platform is occupied

        /// <summary>
        /// The variable representing the amount of war funds available to the player.
        /// </summary>
        [SerializeField] private int _warFunds = 350, _health = 100, _currentLevel = 1; // Initialize with your starting WarFunds

        /// <summary>
        /// The identifier for the platform used in the weapon purchase.
        /// </summary>
        private int PlatformID = -1, WeaponID  = -1; // Part of weapon Purchase

        /// <summary>
        /// Represents the amount of war funds archived by the player.
        /// </summary>
        private int _archiveWarFunds, _enemyThisLevel, _enemyAlive;

        /// Represents an array indicating whether each position in a plat is occupied or not.
        /// </summary>
        private bool[] _archivePlatOccup;

        /// <summary>
        /// Represents an array of archived weapon IDs.
        /// </summary>
        private int[]  _archiveWeaponID;

        /// <summary>
        /// Contains the archived weapon rotations in Vector3 format.
        /// </summary>
        private Vector3[] _archiveWeaponRotation;

        /// <summary>
        /// Indicates whether the program execution is currently paused.
        /// </summary>
        [SerializeField] private bool _isPaused;

        /// <summary>
        /// Indicates whether the platform has been selected or not.
        /// </summary>
        private bool _platformHasBeenSelected, _weaponSelected;

        /// <summary>
        /// The GameObject representing the pathway arrows.
        /// </summary>
        [Header("Pathway Arrows")]
        [SerializeField] private GameObject _pathwayArrows;

        /// <summary>
        /// Represents the input system used by the Metro Mayhem game.
        /// </summary>
        private MetroMayhemInputSystem _input;
        #endregion

        /// <summary>
        /// This method is called when the script component becomes enabled and active.
        /// It is used to register event handlers, initialize variables, and set up the input system.
        /// </summary>
        private void OnEnable() {
            Enemies.EnemyAI.EnemySurvived += EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled += EnemyDied;
            isPlatformOccupied = new bool[platforms.Length];
            _archivePlatOccup = new bool[platforms.Length];
            _archiveWeaponID = new int[platforms.Length];
            _archiveWeaponRotation = new Vector3[platforms.Length];
            _input = new MetroMayhemInputSystem();
            _input.Enable();
            _input.GameControl.Quit.performed += ctx => QuitGame();
        }

        /// <summary>
        /// This method starts the next level of the game.
        /// </summary>
        private void Start() {
            StartNextLevel();
        }

        /// <summary>
        /// Start the next level by resetting the relevant variables, updating UI elements, and archiving level data.
        /// </summary>
        public void StartNextLevel() {
            // Archive  _warFunds, _isPlatformOccupied, weaponIDs[platformID], transform of each weapon
            _health = 100; PlatformID = -1; WeaponID  = -1;
            UIManager.Instance.UpdateHealth(_health);
            UIManager.Instance.UpdateWarFunds(_warFunds);
            UIManager.Instance.LevelDisplay("LEVEL  " + _currentLevel + "\nSTARTING");
            _isPaused = true;
            StartLevel?.Invoke();
            UIManager.Instance.PauseLevel(0,0,true); // GameManager, StartLevel, On
            ArchiveLevelData();
        }

        /// <summary>
        /// Sets the number of enemies for the current level.
        /// </summary>
        /// <param name="NumberOfEnemy">The number of enemies to set.</param>
        public void SetNumberOfEnemy(int NumberOfEnemy) {
            _enemyThisLevel = NumberOfEnemy; 
            _enemyAlive = NumberOfEnemy;
        }

        /// <summary>
        /// Archives level data by storing the current state of war funds, platform occupation, weapon IDs, and weapon rotations.
        /// </summary>
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

        /// <summary>
        /// Restores the level to its previous state.
        /// </summary>
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

        /// <summary>
        /// Starts playing the game.
        /// </summary>
        public void StartPlayGame() {
            _isPaused = false;
            UIManager.Instance.PauseLevel(0,1,true); // GameManager, StartPlay, On
            StartPlay?.Invoke();
        }

        /// <summary>
        /// Pauses the current level.
        /// </summary>
        public void PauseCurrentLevel() {
            _isPaused = true;
            UIManager.Instance.PauseLevel(0,2,true); // GameManager, PauseLevel, On
            PauseLevel?.Invoke();
        }

        /// <summary>
        /// Unpauses the current level.
        /// </summary>
        public void UnpauseCurrentLevel() {
            _isPaused = false;
            UIManager.Instance.PauseLevel(0,3,true); // GameManager, UnpauseLevel, On
            UnpauseLevel?.Invoke();
        }

        /// <summary>
        /// Handles the logic when all enemies are killed.
        /// </summary>
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

        /// <summary>
        /// Loads the game won scene after a delay.
        /// </summary>
        /// <returns>
        /// An IEnumerator object for coroutine execution.
        /// </returns>
        private IEnumerator LoadGameWonScene() {
            yield return new WaitForSeconds(5);
            SceneManager.LoadScene(2);
        }

        /// <summary>
        /// Stops the current level.
        /// </summary>
        public void StopCurrentLevel() {
            UIManager.Instance.PauseLevel(0,4,true); // GameManager, StopLevel, On
            UIManager.Instance.ResetFastForward();
            StopLevel?.Invoke();
        }

        /// <summary>
        /// Restarts the current level.
        /// </summary>
        public void RestartCurrentLevel() {  // Lose
            UIManager.Instance.PauseLevel(0,5,true); // GameManager, StopLevel, On
            UIManager.Instance.LevelDisplay("LEVEL  " + _currentLevel + "\nLOST");
            StopLevel?.Invoke();
            RestoreLevel();
            UIManager.Instance.ResetFastForward();
            RestartLevel?.Invoke();
        }

        /// <summary>
        /// Gets the current level.
        /// </summary>
        /// <returns>The current level.</returns>
        public int GetCurrentLevel() {
            return _currentLevel;
        }

        /// <summary>
        /// Selects the platform at the given index.
        /// </summary>
        /// <param name="platformIndex">The index of the platform to select.</param>
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

        /// <summary>
        /// Unselects the current platform.
        /// </summary>
        public void UnselectPlatform()
        {
            _platformHasBeenSelected = false;
            PlatformID = -1;
            PlatformUnselected?.Invoke();
        }

        /// <summary>
        /// Sets the current selected weapon ID and performs necessary actions.
        /// </summary>
        /// <param name="WeaponID">The ID of the weapon to be set as the current selected weapon.</param>
        public void CurrentWeaponSelected(int WeaponID) {
            if (this.WeaponID == -1 && !_weaponSelected)
            {
                _weaponSelected = true;
                this.WeaponID = WeaponID;
                PurchaseTower();
            }
        }

        /// <summary>
        /// Unselects the current weapon by setting the _weaponSelected flag to false and resetting the WeaponID to -1.
        /// Removes the purchase panel and displays the afford tower UI.
        /// </summary>
        public void CurrentWeaponUnselected() {
            _weaponSelected = false;
            WeaponID = -1;
            UIManager.Instance.RemovePurchasePanel();
            UIManager.Instance.DisplayAffordTower();
        }

        /// <summary>
        /// Method to purchase a tower.
        /// </summary>
        private void PurchaseTower()
        {
            if (PlatformID != -1 && WeaponID != -1)
            {
                UIManager.Instance.DisplayPurchasePanel(PlatformID, WeaponID,_towerPrices[WeaponID]);
            }
        }

        /// <summary>
        /// Determines if the player can afford a tower based on the provided tower ID.
        /// </summary>
        /// <param name="TowerId">The ID of the tower</param>
        /// <returns>True if the player can afford the tower, false otherwise</returns>
        public bool AffordTower(int TowerId) {
            if (_towerPrices[TowerId] < _warFunds) {
                return true;
            }
            return false; // (true if WarFunds is greater than or equal to the tower price, false otherwise)
        }

        /// <summary>
        /// Places a tower on the specified placement platform if the game is not paused, the platform is not occupied,
        /// the weapon ID is valid, and the player has enough war funds. </summary> <param name="PlacementPlatformID">The ID of the placement platform where the tower will be placed.</param> <param name="PlacementWeaponID">The ID of the weapon to be placed as a tower.</param>
        /// /
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

        /// <summary>
        /// Upgrades the selected tower platform with the given weapon.
        /// </summary>
        /// <param name="platformID">The ID of the tower platform to upgrade.</param>
        /// <param name="weaponID">The ID of the weapon to upgrade the tower with.</param>
        public void UpgradeTower(int platformID, int weaponID) {
            if ( ( weaponID == 0 && _warFunds > 500) || (weaponID == 1 && _warFunds > 750 )) {
                UIManager.Instance.DisplayUpgradePanel(platformID, weaponID);
            }
        }

        /// <summary>
        /// Upgrades the platform by removing the occupying weapon and placing a tower with the upgraded weapon.
        /// </summary>
        /// <param name="platformID">The ID of the platform to upgrade.</param>
        /// <param name="weaponID">The ID of the weapon to upgrade.</param>
        public void UpgradeYes(int platformID, int weaponID)
        {
            platforms[platformID].GetComponent<Platform>().RemoveOccupyingWeapon();
            isPlatformOccupied[platformID] = false;
            PlaceTower(platformID, weaponID + 2);
        }

        /// <summary>
        /// Dismantles a tower on a given platform if the platform is occupied,
        /// and displays a dismantle panel to confirm the dismantling process.
        /// </summary>
        /// <param name="platformID">The ID of the platform where the tower is located.</param>
        /// <param name="weaponID">The ID of the tower weapon to be dismantled.</param>
        public void DismantleTower(int platformID, int weaponID)
        {
            if (isPlatformOccupied[platformID])
            {
                UIManager.Instance.DisplayDismantlePanel(platformID, weaponID, _towerPrices[weaponID]);
            }
        }

        /// <summary>
        /// Dismantles a weapon from a platform and updates the war funds.
        /// </summary>
        /// <param name="dismantlePlatformID">The ID of the platform to dismantle the weapon from.</param>
        /// <param name="dismantleWeaponID">The ID of the weapon to dismantle.</param>
        public void DismantleYes(int dismantlePlatformID, int dismantleWeaponID)
        {
            isPlatformOccupied[dismantlePlatformID] = false;
            platforms[dismantlePlatformID].GetComponent<Platform>().RemoveOccupyingWeapon();
            _warFunds += _towerPrices[dismantleWeaponID];
            UIManager.Instance.UpdateWarFunds(_warFunds);
        }

        /// <summary>
        /// Decreases the number of enemy alive, updates the enemy count UI, decreases the health, updates the health UI,
        /// and restarts the current level if the health is zero or less.
        /// </summary>
        private void EnemyHasReachedTheEnd() { // EnemySurvived;
            _enemyAlive--;
            UIManager.Instance.UpdateEnemyCount(_enemyAlive, _enemyThisLevel);
            _health--;
            UIManager.Instance.UpdateHealth(_health);
            if(_health <= 0) {
                RestartCurrentLevel();
            }
        }

        /// <summary>
        /// Decreases the count of alive enemy, updates the enemy count in the UI, increases the war funds and updates the war funds in the UI whenever an enemy dies.
        /// </summary>
        private void EnemyDied()
        {
            _enemyAlive--;
            UIManager.Instance.UpdateEnemyCount(_enemyAlive, _enemyThisLevel);
            _warFunds += 15;
            UIManager.Instance.UpdateWarFunds(_warFunds);
        }

        /// <summary>
        /// Deducts the specified amount from war funds when a weapon is fired.
        /// </summary>
        /// <param name="AmmoCost">The amount of war funds to deduct.</param>
        public void WeaponFired(int AmmoCost)
        {
            _warFunds -= AmmoCost;
        }

        /// <summary>
        /// Toggles the visibility of the pathway arrows.
        /// </summary>
        /// <param name="isVisible">Specifies whether the pathway arrows should be visible or not.</param>
        public void TogglePathwayArrows(bool isVisible) {
            _pathwayArrows.SetActive(isVisible);
        }

        /// <summary>
        /// Quits the game or exits play mode in the editor.
        /// </summary>
        public void QuitGame() {
            /* Quit the Editor
            if (Application.isEditor) {
                    EditorApplication.ExitPlaymode();
            }
            // Quit the game */
            Application.Quit();
        }

        /// <summary>
        /// The OnDisable method is called when the component or GameObject is disabled.
        /// This method is responsible for unregistering event handlers and disabling input.
        /// </summary>
        private void OnDisable() {
            Enemies.EnemyAI.EnemySurvived -= EnemyHasReachedTheEnd;
            Enemies.EnemyAI.EnemyKilled -= EnemyDied;
            _input.Disable();
            _input.GameControl.Quit.performed -= ctx => QuitGame();
        }
    }
} 