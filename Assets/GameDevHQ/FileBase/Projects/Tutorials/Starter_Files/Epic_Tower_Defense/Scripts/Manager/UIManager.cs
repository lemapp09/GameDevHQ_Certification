using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MetroMayhem.Manager
{
    /// The UIManager class handles the user interface functionality of the game.
    /// It manages various UI elements such as buttons, panels, text, and images.
    /// Attached to a UIManager game object in the scene.
    /// /
    public class UIManager : MonoSingleton<UIManager>
    {
        #region Variables

        /// <summary>
        /// The restart button variable reference.
        /// </summary>
        /// <remarks>
        /// This variable is used to store a reference to the restart button object in the scene.
        /// </remarks>
        [Header("Restart")] [SerializeField] private Button _restartButton;

        /// <summary>
        /// The restart image object that is serialized for editor access.
        /// </summary>
        [SerializeField] private GameObject _restartImage;

        /// <summary>
        /// The panel used for displaying the armory UI.
        /// </summary>
        [Header("Armory")] [SerializeField] private GameObject _armoryPanel;

        /// <summary>
        /// Represents an array of armory pieces.
        /// </summary>
        [SerializeField] private GameObject[] _armoryPieces;

        /// <summary>
        /// An array of Images representing the backgrounds of armory pieces.
        /// </summary>
        /// <remarks>
        /// The elements in this array correspond to the different armory pieces that can be displayed in the game.
        /// Each element represents the background image for a specific armory piece.
        /// </remarks>
        [SerializeField] private Image[] _armoryPieceBackground;

        /// <summary>
        /// The Gatling Gun button used in the game.
        /// </summary>
        [SerializeField] private Button _gatlingGunButton;

        /// <summary>
        /// Serializes the private variable "_missileLauncherButton" for inspector access.
        /// </summary>
        [SerializeField] private Button _missileLauncherButton;

        /// <summary>
        /// The private serialized field that references the Dual Gatling Gun button.
        /// </summary>
        [SerializeField] private Button _dualGatlingGunButton;

        /// <summary>
        /// The private serialized field representing the dual missile launcher button.
        /// </summary>
        [SerializeField] private Button _dualMissileLauncherButton;

        /// <summary>
        /// The TextMeshProUGUI component used to display the war funds.
        /// </summary>
        [Header("WarFunds")] [SerializeField] private TextMeshProUGUI _warFundsText;

        /// <summary>
        /// Serialized field for a TextMeshProUGUI component used to display the status.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _status;

        /// <summary>
        /// Serialized field for controlling the percentage of enemy spawn.
        /// </summary>
        [SerializeField] private Slider _percentageOfEnemy;

        /// <summary>
        /// Represents the image used for displaying the pause icon during playback.
        /// </summary>
        [Header("Playback Speed")]
        [SerializeField] private Image _pauseImage;

        /// <summary>
        /// The private serialized field representing the pause button.
        /// </summary>
        [SerializeField] private Button _pauseButton;

        /// <summary>
        /// Reference to the Image component used for the play button.
        /// </summary>
        [SerializeField] private Image _playImage;

        /// <summary>
        /// The play button in the game.
        /// </summary>
        [SerializeField] private Button _playButton;

        /// <summary>
        /// The fast forward image used for displaying fast forward UI.
        /// </summary>
        [SerializeField] private Image _fastForwardImage;

        /// <summary>
        /// The fast forward button used for advancing playback at an increased speed.
        /// </summary>
        [SerializeField] private Button _fastForwardButton;

        /// <summary>
        /// The settings button.
        /// </summary>
        [SerializeField] private Button _settingsBuitton;

        /// <summary>
        /// The GameObject representing the settings panel.
        /// </summary>
        [SerializeField] private GameObject _settingsPanel;

        /// <summary>
        /// Indicates whether the Pause button is clicked.
        /// </summary>
        private bool _isPausedButtonClicked, _blinkPlayImage;

        /// <summary>
        /// Represents the interval duration at which the blink effect occurs.
        /// </summary>
        private float _blinkInterval;

        /// <summary>
        /// Represents the TextMeshProUGUI component used to display the health level.
        /// </summary>
        [Header("Health Level")] [SerializeField]
        private TextMeshProUGUI _healthText;

        /// <summary>
        /// The private field _levelCountText of type TextMeshProUGUI. </summary>
        /// /
        [SerializeField] private TextMeshProUGUI _levelCountText;

        /// <summary>
        /// The panel that displays the level status.
        /// </summary>
        [Header("Level Status")] [SerializeField]
        private GameObject _levelStatusPanel;

        /// <summary>
        /// The TextMeshProUGUI component used to display the status of the level.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _levelStatusText;

        /// <summary>
        /// Class representing the purchase panel for weapon options. </summary>
        /// /
        [Header("Weapon Options")]
        [SerializeField] private GameObject _purchasePanel;

        /// <summary>
        /// Serialized field representing a TextMeshPro text component used for displaying purchase information.
        /// </summary>
        [SerializeField] private TMP_Text _purchaseText;

        /// <summary>
        /// Reference to the "Yes" button used for confirming a purchase.
        /// </summary>
        [SerializeField] private Button _purchaseYesButton;

        /// <summary>
        /// Serialized field for the "Purchase No" button.
        /// </summary>
        [SerializeField] private Button _purchaseNoButton;

        /// <summary>
        /// Represents the unique identifier of the purchase platform.
        /// </summary>
        private int _purchasePlatformID, _purchaseWeaponID;

        /// <summary>
        /// Reference to the upgrade gun panel GameObject.
        /// </summary>
        [SerializeField] private GameObject _upgradeGunPanel;

        /// <summary>
        /// The yes button used for upgrading the gun.
        /// </summary>
        [SerializeField] private Button _upgradeGunYesButton;

        /// <summary>
        /// Reference to the upgrade gun no button.
        /// </summary>
        [SerializeField] private Button _upgradeGunNoButton;

        /// <summary>
        /// Reference to the upgrade missile panel in the game.
        /// </summary>
        [SerializeField] private GameObject _upgradeMissilePanel;

        /// <summary>
        /// Represents the reference to the button used to confirm the upgrade of a missile.
        /// </summary>
        [SerializeField] private Button _upgradeMissileYesButton;
        [SerializeField] private Button _upgradeMissileNoButton;

        /// <summary>
        /// The ID of the upgrade platform.
        /// </summary>
        private int _upgradePlatformID, _upgradeWeaponID;

        /// <summary>
        /// The GameObject representing the dismantle weapon panel in the scene.
        /// This is a serialized field, meaning it can be assigned in the Unity Inspector.
        /// </summary>
        [SerializeField] private GameObject _dismantleWeaponPanel;

        /// <summary>
        /// Reference to the TextMeshProUGUI component used to display the dismantle price.
        /// </summary>
        [SerializeField] private TextMeshProUGUI _dismantlePriceText;

        /// <summary>
        /// The Yes button used for confirming the dismantling action.
        /// </summary>
        [SerializeField] private Button _dismantleYesButton;

        /// <summary>
        /// Reference to the Button component for the "Dismantle No" button.
        /// The button is used to cancel the dismantling process.
        /// </summary>
        [SerializeField] private Button _dismantleNoButton;

        /// <summary>
        /// Button representing the quit game option in the UI.
        /// </summary>
        [Header("Quit System")]
        [SerializeField ] private Button _quitGameButton;

        /// <summary>
        /// The reference to the quit panel GameObject.
        /// </summary>
        [SerializeField] private GameObject _quitPanel;

        /// <summary>
        /// Reference to the "Yes" button used for quitting the application.
        /// </summary>
        [SerializeField] private Button _quitYesButton;

        /// <summary>
        /// The reference to the "Quit No" button.
        /// </summary>
        [SerializeField] private Button _quitNoButton;

        /// <summary>
        /// The ID of the platform being dismantled.
        /// </summary>
        private int _dismantlePlatformID, _dismantleWeaponID;

        /// <summary>
        /// Array of Image objects used to display level indicators during pause menu.
        /// </summary>
        [SerializeField] private Image[] _pauseLevelIndicators;
        #endregion

        /// <summary>
        /// Starts the process by displaying the afford tower.
        /// </summary>
        private void Start() {
            DisplayAffordTower();
        }

        /// <summary>
        /// Updates the state of the object.
        /// </summary>
        private void Update() {
            if (_blinkPlayImage) {
                _blinkInterval += Time.deltaTime;
                if (_blinkInterval >= 1) {
                    _playImage.gameObject.SetActive(!_playImage.gameObject.activeSelf);
                    _blinkInterval = 0;
                }
            }
        }

        /// <summary>
        /// Displays the afford tower based on the GameManager.AffordTower method.
        /// </summary>
        public void DisplayAffordTower() {
            for (int i = 0; i < _armoryPieces.Length; i++) {
                if (GameManager.Instance.AffordTower(i))  {
                    _armoryPieces[i].SetActive(true);
                    _armoryPieceBackground[i].enabled = false;
                } else {
                    _armoryPieces[i].SetActive(false);
                    _armoryPieceBackground[i].enabled = false;
                }
            }
        }

        /// <summary>
        /// Displays the given message for level completion with a delay.
        /// </summary>
        /// <param name="message">The message to be displayed.</param>
        public void LevelDisplay(string message) {
            StartCoroutine(DelayLevelCompleteDisplay(message));
        }

        /// <summary>
        /// Displays a message on the level status panel for a specific duration.
        /// </summary>
        /// <param name="DisplayMessage">The message to be displayed on the level status panel.</param>
        /// <returns>An IEnumerator to be used with Coroutine.</returns>
        private IEnumerator DelayLevelCompleteDisplay(string DisplayMessage) {
            _levelStatusPanel.SetActive(true);
            _levelStatusText.text = DisplayMessage;
            yield return new WaitForSeconds(5f);
            _levelStatusPanel.SetActive(false);
        }

        /// <summary>
        /// Sets the "_blinkPlayImage" variable to true, triggering a blink effect.
        /// </summary>
        private void Blink() {
            _blinkPlayImage = true;
        }

        /// <summary>
        /// Disables blinking of the play image.
        /// </summary>
        private void DoNotBlink() {
            _blinkPlayImage = false;
        }

        /// <summary>
        /// Method to handle the restart button click event.
        /// </summary>
        /// <remarks>
        /// This method restarts the current level in the game, and also resets the UI.
        /// </remarks>
        public void RestartClicked() {
            GameManager.Instance.RestartCurrentLevel();
            StartCoroutine(ResetUI());
        }

        /// <summary>
        /// Handles the pause button click event.
        /// If the button was not clicked before, it will pause the current level.
        /// If the button was clicked before, it will unpause the current level.
        /// </summary>
        public void PauseClicked() {
            if (_isPausedButtonClicked) {
                _isPausedButtonClicked = false;
                _pauseImage.gameObject.SetActive(false); 
                Manager.GameManager.Instance.UnpauseCurrentLevel();
            } else {
                _isPausedButtonClicked = true;
                _pauseImage.gameObject.SetActive(true); 
                Manager.GameManager.Instance.PauseCurrentLevel();
            }
        }

        /// <summary>
        /// Displays the settings panel and pauses the current level.
        /// </summary>
        private void DisplaySettings() {
            Manager.GameManager.Instance.PauseCurrentLevel();
            _settingsPanel.SetActive(true);
        }

        /// <summary>
        /// Activates the play image game object and starts the game.
        /// </summary>
        public void PlayClicked() {
            _playImage.gameObject.SetActive(true);
            Manager.GameManager.Instance.StartPlayGame();
        }

        /// <summary>
        /// Called when the fast forward button is clicked.
        /// </summary>
        public void FastForwardClicked() {  // 
            if (!_blinkPlayImage) {
                _fastForwardImage.gameObject.SetActive(!_fastForwardImage.gameObject.activeSelf);
                Time.timeScale = Time.timeScale == 1 ? 2 : 1;
            }
        }

        /// <summary>
        /// Resets the fast forward functionality by deactivating the fast forward image and setting the time scale to 1.
        /// </summary>
        public void ResetFastForward() {
            _fastForwardImage.gameObject.SetActive(false);
            Time.timeScale = 1;
        }

        /// <summary>
        /// Event handler for when the Gatling button is clicked.
        /// </summary>
        private void GatlingButtonClicked() {
            HighlightSelectedWeapon(0);
        }

        /// <summary>
        /// Method to handle the event when the Missile button is clicked.
        /// </summary>
        private void MissileButtonClicked() {
            HighlightSelectedWeapon(1);
        }

        /// <summary>
        /// This method is called when the Dual Gatling Button is clicked.
        /// It highlights the selected weapon as the Dual Gatling.
        /// </summary>
        private void DualGatlingButtonClicked() {
            HighlightSelectedWeapon(2);
        }

        /// <summary>
        /// Handles the click event of the dual missile button.
        /// </summary>
        private void DualMissileButtonClicked() {
            HighlightSelectedWeapon(3);
        }

        /// <summary>
        /// Highlights the selected weapon in the armory.
        /// </summary>
        /// <param name="SelectedWeapon">The index of the selected weapon.</param>
        private void HighlightSelectedWeapon(int SelectedWeapon)
        {
            for (int i = 0; i < _armoryPieces.Length; i++) {
                if (i != SelectedWeapon) {
                    _armoryPieceBackground[i].enabled = false;
                } else {
                    _armoryPieceBackground[i].enabled = true;
                }
            }
            GameManager.Instance.CurrentWeaponSelected(SelectedWeapon);
        }

        /// <summary>
        /// Displays the purchase panel for a specific platform and weapon with the given purchase price.
        /// </summary>
        /// <param name="PlatformID">The ID of the platform to be purchased.</param>
        /// <param name="WeaponID">The ID of the weapon to be purchased.</param>
        /// <param name="PurchasePrice">The price of the purchase.</param>
        public void DisplayPurchasePanel(int PlatformID, int WeaponID, int PurchasePrice)
        {
            _purchasePlatformID = PlatformID;
            _purchaseWeaponID = WeaponID;
            _purchasePanel.SetActive(true);
            _purchaseText.text = PurchasePrice.ToString();
        }

        /// <summary>
        /// Removes the purchase panel from the screen.
        /// </summary>
        public void RemovePurchasePanel() {
            _purchasePanel.SetActive(false);
        }

        /// <summary>
        /// Removes the purchase panel and places a tower on the selected platform and with the selected weapon.
        /// </summary>
        private void PurchaseYesButtonClicked() {
            RemovePurchasePanel();
            GameManager.Instance.PlaceTower(_purchasePlatformID, _purchaseWeaponID);
        }

        /// <summary>
        /// Removes the purchase panel and resets the selected weapon and platform in the game manager.
        /// </summary>
        private void PurchaseNoButtonClicked() {
            RemovePurchasePanel();
            GameManager.Instance.CurrentWeaponUnselected();
            GameManager.Instance.UnselectPlatform();
        }

        /// <summary>
        /// Displays the upgrade panel for a given platform and weapon.
        /// </summary>
        /// <param name="platformID">The ID of the platform.</param>
        /// <param name="weaponID">The ID of the weapon.</param>
        public void DisplayUpgradePanel(int platformID, int weaponID) {
            _upgradePlatformID = platformID;
            _upgradeWeaponID = weaponID;
            if (weaponID == 0) {
                _upgradeGunPanel.SetActive(true);
            }   else  {
                _upgradeMissilePanel.SetActive(true);
            }
        }

        /// Disables upgrade gun panel and upgrade missile panel,
        /// and calls the UpgradeYes method in the GameManager instance
        /// with the upgrade platform ID and upgrade weapon ID.
        /// /
        private void UpgradeYes() {
            _upgradeGunPanel.SetActive(false);
            _upgradeMissilePanel.SetActive(false);
            GameManager.Instance.UpgradeYes(_upgradePlatformID, _upgradeWeaponID);
        }

        /// <summary>
        /// Displays the dismantle panel.
        /// </summary>
        /// <param name="platformID">The ID of the platform.</param>
        /// <param name="weaponID">The ID of the weapon.</param>
        /// <param name="price">The price of the weapon.</param>
        public void DisplayDismantlePanel(int platformID, int weaponID, int price)
        {
            _dismantlePlatformID  = platformID;
            _dismantleWeaponID = weaponID;
            _dismantlePriceText.text = price.ToString();
            _dismantleWeaponPanel.SetActive(true);
        }

        /// <summary>
        /// Dismantles a weapon on the specified platform. </summary> <remarks>
        /// This method disables the Dismantle Weapon Panel and calls the DismantleYes method
        /// from the GameManager instance with the specified platform and weapon IDs. </remarks>
        /// /
        private void DismantleYes() {
            _dismantleWeaponPanel.SetActive(false);
            GameManager.Instance.DismantleYes(_dismantlePlatformID, _dismantleWeaponID);
        }

        /// <summary>
        /// Disables the dismantle weapon panel.
        /// </summary>
        private void DismantleNo()
        {
            _dismantleWeaponPanel.SetActive(false);
        }

        /// <summary>
        /// Hides the upgrade gun panel and upgrade missile panel.
        /// </summary>
        private void UpgradeNo() {
            _upgradeGunPanel.SetActive(false);
            _upgradeMissilePanel.SetActive(false);
        }

        /// <summary>
        /// Updates the health text with the provided value.
        /// </summary>
        /// <param name="health">The new health value to display.</param>
        public void UpdateHealth(int health)
        {
            _healthText.text = health.ToString();
        }

        /// <summary>
        /// Updates the war funds and displays it on screen.
        /// </summary>
        /// <param name="funds">The amount of funds to update to.</param>
        public void UpdateWarFunds(int funds)
        {
            _warFundsText.text = funds.ToString();
            DisplayAffordTower();
        }

        /// <summary>
        /// Updates the enemy count based on the number of alive enemies and the total number of enemies in the level.
        /// </summary>
        /// <param name="enemyAlive">The number of enemies that are currently alive.</param>
        /// <param name="enemyThisLevel">The total number of enemies in the level.</param>
        public void UpdateEnemyCount(int enemyAlive, int enemyThisLevel) {
            _percentageOfEnemy.value = ((float)enemyThisLevel - (float)enemyAlive) / (float)enemyThisLevel;
        }

        /// <summary>
        /// Update the level count display.
        /// </summary>
        /// <param name="level">The current level number.</param>
        public void UpdateLevelCount(int level) {
            _levelCountText.text = level.ToString() + "/ 10";
        }

        /// <summary>
        /// Resets the UI elements.
        /// </summary>
        /// <returns>
        /// An IEnumerator to enable usage of yield return.
        /// </returns>
        private IEnumerator ResetUI() {
            _restartImage.SetActive(true);
            _playImage.gameObject.SetActive(false);
            _pauseImage.gameObject.SetActive(false);
            _fastForwardImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            _restartImage.SetActive(false);
        }

        /// <summary>
        /// Displays the quit panel and pauses the current level.
        /// </summary>
        private void DisplayQuitPanel() {
            _quitPanel.SetActive(true);
            GameManager.Instance.PauseCurrentLevel();
        }

        /// <summary>
        /// Method to handle the selection of "Yes" in the quit game panel.
        /// It hides the quit panel and calls the QuitGame method in the GameManager.
        /// </summary>
        private void QuitGameYes() {
            _quitPanel.SetActive(false);
            GameManager.Instance.QuitGame();
        }

        /// <summary>
        /// This method is used to quit the game without confirming the action.
        /// </summary>
        private void QuitGameNo() {
            _quitPanel.SetActive(false);
            GameManager.Instance.UnpauseCurrentLevel();
        }

        /// <summary>
        /// Changes the color of a specific level indicator to indicate if the level is paused or not.
        /// </summary>
        /// <param name="player">The player index. (0 for GameMaster, 1 for SpawnManager)</param>
        /// <param name="indicator">The indicator index. (0 for startLevel, 1 for startPlay, 2 for pauseLevel, 3 for unpauseLevel, 4 for stopLevel, 5 for restartLevel)</param>
        /// <param name="isOn">Specifies whether the level is paused or not. True for paused, False for not paused.</param>
        /// <remarks>
        /// The method changes the color of a specific level indicator based on the provided player and indicator indices.
        /// It uses the isOn parameter to determine whether the level is paused or not.
        /// A value of true for isOn will set the indicator color to red, indicating that the level is paused.
        /// A value of false for isOn will set the indicator color to white, indicating that the level is not paused.
        /// </remarks>
        public void PauseLevel(int player, int indicator, bool isOn) { 
        /*
         Players: (0) GameMaster, (1) SpawnManager,
        Indicators: (0) startLevel, (1) startPlay, (2) pauseLevel, (3) unpauseLevel, (4) stopLevel, (5) restartLevel
        */
            _pauseLevelIndicators[player * 8 + indicator].color = isOn ? Color.red : Color.white;
        }

        /// Method called when the script component is enabled.
        /// It sets the initial state of the level status panel to false and assigns event listeners to various buttons.
        /// Event Listeners:
        /// - PauseButton: Invokes the PauseClicked method.
        /// - PlayButton: Invokes the PlayClicked method.
        /// - SettingsButton: Invokes the DisplaySettings method.
        /// - FastForwardButton: Invokes the FastForwardClicked method.
        /// - RestartButton: Invokes the RestartClicked method.
        /// - GatlingGunButton: Invokes the GatlingButtonClicked method.
        /// - MissileLauncherButton: Invokes the MissileButtonClicked method.
        /// - DualGatlingGunButton: Invokes the DualGatlingButtonClicked method.
        /// - DualMissileLauncherButton: Invokes the DualMissileButtonClicked method.
        /// - PurchaseYesButton: Invokes the PurchaseYesButtonClicked method.
        /// - PurchaseNoButton: Invokes the PurchaseNoButtonClicked method.
        /// - UpgradeGunYesButton: Invokes the UpgradeYes method.
        /// - UpgradeGunNoButton: Invokes the UpgradeNo method.
        /// - UpgradeMissileYesButton: Invokes the UpgradeYes method.
        /// - UpgradeMissileNoButton: Invokes the UpgradeNo method.
        /// - DismantleYesButton: Invokes the DismantleYes method.
        /// - DismantleNoButton: Invokes the DismantleNo method.
        /// - QuitGameButton: Invokes the DisplayQuitPanel method.
        /// - QuitYesButton: Invokes the QuitGameYes method.
        /// - QuitNoButton: Invokes the QuitGameNo method.
        /// Event Subscriptions:
        /// - StartLevel (GameManager): Subscribes to the Blink method.
        /// - StartPlay (GameManager): Subscribes to the DoNotBlink method.
        /// /
        private void OnEnable() {
            _levelStatusPanel.SetActive(false);
            _pauseButton.onClick.AddListener(PauseClicked);
            _playButton.onClick.AddListener(PlayClicked);
            _settingsBuitton.onClick.AddListener(DisplaySettings);
            _fastForwardButton.onClick.AddListener(FastForwardClicked);
            _restartButton.onClick.AddListener(RestartClicked);
            _gatlingGunButton.onClick.AddListener(GatlingButtonClicked);
            _missileLauncherButton.onClick.AddListener(MissileButtonClicked);
            _dualGatlingGunButton.onClick.AddListener(DualGatlingButtonClicked);
            _dualMissileLauncherButton.onClick.AddListener(DualMissileButtonClicked);
            _purchaseYesButton.onClick.AddListener(PurchaseYesButtonClicked);
            _purchaseNoButton.onClick.AddListener(PurchaseNoButtonClicked);
            _upgradeGunYesButton.onClick.AddListener(UpgradeYes);
            _upgradeGunNoButton.onClick.AddListener(UpgradeNo);
            _upgradeMissileYesButton.onClick.AddListener(UpgradeYes);
            _upgradeMissileNoButton.onClick.AddListener(UpgradeNo);
            _dismantleYesButton.onClick.AddListener(DismantleYes);
            _dismantleNoButton.onClick.AddListener(DismantleNo);
            _quitGameButton.onClick.AddListener(DisplayQuitPanel);
            _quitYesButton.onClick.AddListener(QuitGameYes);
            _quitNoButton.onClick.AddListener(QuitGameNo);
            GameManager.StartLevel += Blink;
            GameManager.StartPlay += DoNotBlink;
        }

        /// <summary>
        /// This method is called when the MonoBehaviour is disabled.
        /// It removes all listeners from various buttons and events.
        /// </summary>
        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(PauseClicked);
            _playButton.onClick.RemoveListener(PlayClicked);
            _settingsBuitton.onClick.RemoveListener(DisplaySettings);
            _fastForwardButton.onClick.RemoveListener(FastForwardClicked);
            _restartButton.onClick.RemoveListener(RestartClicked);
            _gatlingGunButton.onClick.RemoveListener(GatlingButtonClicked);
            _missileLauncherButton.onClick.RemoveListener(MissileButtonClicked);
            _dualGatlingGunButton.onClick.RemoveListener(DualGatlingButtonClicked);
            _dualMissileLauncherButton.onClick.RemoveListener(DualMissileButtonClicked);
            _purchaseYesButton.onClick.RemoveListener(PurchaseYesButtonClicked);
            _purchaseNoButton.onClick.RemoveListener(PurchaseNoButtonClicked);
            _upgradeGunYesButton.onClick.RemoveListener(UpgradeYes);
            _upgradeGunNoButton.onClick.RemoveListener(UpgradeNo);
            _upgradeMissileYesButton.onClick.RemoveListener(UpgradeYes);
            _upgradeMissileNoButton.onClick.RemoveListener(UpgradeNo);
            _dismantleYesButton.onClick.RemoveListener(DismantleYes);
            _dismantleNoButton.onClick.RemoveListener(DismantleNo);
            _quitGameButton.onClick.RemoveListener(DisplayQuitPanel);
            _quitYesButton.onClick.RemoveListener(QuitGameYes);
            _quitNoButton.onClick.RemoveListener(QuitGameNo);
            GameManager.StartLevel -= Blink;
            GameManager.StartPlay -= DoNotBlink;
        }
    }
}