using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace MetroMayhem.Manager
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [Header("Restart")] [SerializeField] private Button _restartButton;
        [SerializeField] private GameObject _restartImage;
        
        [Header("Armory")] [SerializeField] private GameObject _armoryPanel;
        [SerializeField] private GameObject[] _armoryPieces;

        [Header("WarFunds")] [SerializeField] private TextMeshProUGUI _warFundsText;
        [SerializeField] private TextMeshProUGUI _status;

        [Header("Playback Speed")]
        [SerializeField] private Image _pauseImage;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Image _playImage;
        [SerializeField] private Button _playButton;
        [SerializeField] private Image _fastForwardImage;
        [SerializeField] private Button _fastForwardButton;
        [SerializeField] private Button _SettingsBuitton;
        private bool _isPaused;
        
        [Header("Health Level")] [SerializeField]
        private TextMeshProUGUI _healthText;
        [SerializeField] private TextMeshProUGUI _levelCountText;

        [Header("Level Status")] [SerializeField]
        private GameObject _levelStatusPanel;
        [SerializeField] private TextMeshProUGUI _levelStatusText;

        [Header("Weapon Options")] [SerializeField]
        private GameObject _upgradeGunPanel;

        [SerializeField] private GameObject _upgradeMissilePanel;
        [SerializeField] private GameObject _dismantleWeaponPanel;

        public void LevelComplete() {
            StartCoroutine(DelayLevelCompleteDisplay());
        }

        private IEnumerator DelayLevelCompleteDisplay()
        {
            _levelStatusPanel.SetActive(true);
            _levelStatusText.text = "Level Complete!";
            yield return new WaitForSeconds(15f);
            _levelStatusPanel.SetActive(false);
        }

        public void RestartClicked() {
            GameManager.Instance.RestartCurrentLevel();
            StartCoroutine(ResetUI());
        }

        public void PauseClicked()
        {
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

        public void PlayClicked()
        {
            _playImage.gameObject.SetActive(!_playImage.gameObject.activeSelf);
            Manager.GameManager.Instance.StartPlayGame();
        }

        public void FastForwardClicked()
        {
            _fastForwardImage.gameObject.SetActive(!_fastForwardImage.gameObject.activeSelf);
        }
        
        public void UpdateHealth(int health)
        {
            _healthText.text = health.ToString();
        }

        public void UpdateWarFunds(int funds)
        {
            _warFundsText.text = funds.ToString();
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
            _restartButton.onClick.AddListener(RestartClicked);
        }

        private void OnDisable()
        {
            _pauseButton.onClick.RemoveListener(PauseClicked);
            _playButton.onClick.RemoveListener(PlayClicked);
            _restartButton.onClick.RemoveListener(RestartClicked);
        }
    }
}