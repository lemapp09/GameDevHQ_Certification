using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MetroMayhem.Manager
{
    public class UIManager : MonoSingleton<UIManager>
    {
        #region Variables
        [Header("Restart")] [SerializeField] private Button _restartButton;
        [SerializeField] private GameObject _restartImage;
        
        [Header("Armory")] [SerializeField] private GameObject _armoryPanel;
        [SerializeField] private GameObject[] _armoryPieces;
        [SerializeField] private Image[] _armoryPieceBackground;
        [SerializeField] private Button _gatlingGunButton;
        [SerializeField] private Button _missileLauncherButton;
        [SerializeField] private Button _dualGatlingGunButton;
        [SerializeField] private Button _dualMissileLauncherButton;

        [Header("WarFunds")] [SerializeField] private TextMeshProUGUI _warFundsText;
        [SerializeField] private TextMeshProUGUI _status;
        [SerializeField] private Slider _percentageOfEnemy;

        [Header("Playback Speed")]
        [SerializeField] private Image _pauseImage;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Image _playImage;
        [SerializeField] private Button _playButton;
        [SerializeField] private Image _fastForwardImage;
        [SerializeField] private Button _fastForwardButton;
        [SerializeField] private Button _settingsBuitton;
        [SerializeField] private GameObject _settingsPanel;
        private bool _isPaused, _blinkPlayImage;
        private float _blinkInterval;
        
        [Header("Health Level")] [SerializeField]
        private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _levelCountText;

        [Header("Level Status")] [SerializeField]
        private GameObject _levelStatusPanel;
        [SerializeField] private TextMeshProUGUI _levelStatusText;

        [Header("Weapon Options")]
        [SerializeField] private GameObject _purchasePanel;
        [SerializeField] private TMP_Text _purchaseText;
        [SerializeField] private Button _purchaseYesButton;
        [SerializeField] private Button _purchaseNoButton;
        private int _purchasePlatformID, _purchaseWeaponID;
        
        [SerializeField] private GameObject _upgradeGunPanel;
        [SerializeField] private Button _upgradeGunYesButton;
        [SerializeField] private Button _upgradeGunNoButton;

        [SerializeField] private GameObject _upgradeMissilePanel;
        [SerializeField] private Button _upgradeMissileYesButton;
        [SerializeField] private Button _upgradeMissileNoButton;
        private int _upgradePlatformID, _upgradeWeaponID;
        
        [SerializeField] private GameObject _dismantleWeaponPanel;
        [SerializeField] private TextMeshProUGUI _dismantlePriceText;
        [SerializeField] private Button _dismantleYesButton;
        [SerializeField] private Button _dismantleNoButton;
        private int _dismantlePlatformID, _dismantleWeaponID;
        #endregion
        private void Start() {
            DisplayAffordTower();
        }

        private void Update() {
            if (_blinkPlayImage) {
                _blinkInterval += Time.deltaTime;
                if (_blinkInterval >= 1) {
                    _playImage.gameObject.SetActive(!_playImage.gameObject.activeSelf);
                    _blinkInterval = 0;
                }
            }
        }

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
        
        public void LevelDisplay(string message) {
            StartCoroutine(DelayLevelCompleteDisplay(message));
        }

        private IEnumerator DelayLevelCompleteDisplay(string DisplayMessage) {
            _levelStatusPanel.SetActive(true);
            _levelStatusText.text = DisplayMessage;
            yield return new WaitForSeconds(5f);
            _levelStatusPanel.SetActive(false);
        }

        private void Blink() {
            _blinkPlayImage = true;
        }

        private void DoNotBlink() {
            _blinkPlayImage = false;
        }

        public void RestartClicked() {
            GameManager.Instance.RestartCurrentLevel();
            StartCoroutine(ResetUI());
        }

        public void PauseClicked() {
            if (_isPaused) {
                _isPaused = false;
                _pauseImage.gameObject.SetActive(false); 
                Manager.GameManager.Instance.UnpauseCurrentLevel();
            } else {
                _isPaused = true;
                _pauseImage.gameObject.SetActive(true); 
                Manager.GameManager.Instance.PauseCurrentLevel();
            }
        }

        private void DisplaySettings() {
            Manager.GameManager.Instance.PauseCurrentLevel();
            _settingsPanel.SetActive(true);
        }

        public void PlayClicked() {
            _playImage.gameObject.SetActive(true);
            Manager.GameManager.Instance.StartPlayGame();
        }

        public void FastForwardClicked() {  // 
            if (!_blinkPlayImage) {
                _fastForwardImage.gameObject.SetActive(!_fastForwardImage.gameObject.activeSelf);
                Time.timeScale = Time.timeScale == 1 ? 2 : 1;
            }
        }

        public void ResetFastForward() {
            _fastForwardImage.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
        
        private void GatlingButtonClicked() {
            HighlightSelectedWeapon(0);
        }

        private void MissileButtonClicked() {
            HighlightSelectedWeapon(1);
        }

        private void DualGatlingButtonClicked() {
            HighlightSelectedWeapon(2);
        }

        private void DualMissileButtonClicked() {
            HighlightSelectedWeapon(3);
        }

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

        public void DisplayPurchasePanel(int PlatformID, int WeaponID, int PurchasePrice)
        {
            _purchasePlatformID = PlatformID;
            _purchaseWeaponID = WeaponID;
            _purchasePanel.SetActive(true);
            _purchaseText.text = PurchasePrice.ToString();
        }

        public void RemovePurchasePanel() {
            _purchasePanel.SetActive(false);
        }

        private void PurchaseYesButtonClicked() {
            RemovePurchasePanel();
            GameManager.Instance.PlaceTower(_purchasePlatformID, _purchaseWeaponID);
        }

        private void PurchaseNoButtonClicked() {
            RemovePurchasePanel();
            GameManager.Instance.CurrentWeaponUnselected();
            GameManager.Instance.UnselectPlatform();
        }

        public void DisplayUpgradePanel(int platformID, int weaponID) {
            _upgradePlatformID = platformID;
            _upgradeWeaponID = weaponID;
            if (weaponID == 0) {
                _upgradeGunPanel.SetActive(true);
            }   else  {
                _upgradeMissilePanel.SetActive(true);
            }
        }

        private void UpgradeYes() {
            _upgradeGunPanel.SetActive(false);
            _upgradeMissilePanel.SetActive(false);
            GameManager.Instance.UpgradeYes(_upgradePlatformID, _upgradeWeaponID);
        }

        public void DisplayDismantlePanel(int platformID, int weaponID, int price)
        {
            _dismantlePlatformID  = platformID;
            _dismantleWeaponID = weaponID;
            _dismantlePriceText.text = price.ToString();
            _dismantleWeaponPanel.SetActive(true);
        }

        private void DismantleYes() {
            _dismantleWeaponPanel.SetActive(false);
            GameManager.Instance.DismantleYes(_dismantlePlatformID, _dismantleWeaponID);
        }

        private void DismantleNo() {
            _dismantleWeaponPanel.SetActive(false);
        }
        
        private void UpgradeNo() {
            _upgradeGunPanel.SetActive(false);
            _upgradeMissilePanel.SetActive(false);
        }
        
        public void UpdateHealth(int health)
        {
            _healthText.text = health.ToString();
        }

        public void UpdateWarFunds(int funds)
        {
            _warFundsText.text = funds.ToString();
            DisplayAffordTower();
        }

        public void UpdateEnemyCount(int enemyAlive, int enemyThisLevel) {
            _percentageOfEnemy.value = ((float)enemyThisLevel - (float)enemyAlive) / (float)enemyThisLevel;
        }

        public void UpdatePercentageOfEnemies(float percentAlive) {
                _percentageOfEnemy.value = percentAlive;
        }

        public void UpdateLevelCount(int level) {
            _levelCountText.text = level.ToString() + "/ 10";
        }

        private IEnumerator ResetUI() {
            _restartImage.SetActive(true);
            _playImage.gameObject.SetActive(false);
            _pauseImage.gameObject.SetActive(false);
            _fastForwardImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            _restartImage.SetActive(false);
        }

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
            GameManager.StartLevel += Blink;
            GameManager.StartPlay += DoNotBlink;
        }

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
            GameManager.StartLevel -= Blink;
            GameManager.StartPlay -= DoNotBlink;
        }
    }
}