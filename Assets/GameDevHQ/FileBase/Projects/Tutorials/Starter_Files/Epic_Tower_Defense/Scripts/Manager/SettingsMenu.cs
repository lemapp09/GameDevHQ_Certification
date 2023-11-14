using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MetroMayhem.Manager
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private Slider _masterVolumeSilder, _musicVolumeSLider, _sfxVolumeSlider, _cameraControlSpeed;
        [SerializeField] private Button _closeButton, _genreButton1, _genreButton2, _genreButton3, _genreButton4, _howTo;
        [SerializeField] private GameObject _mainCanvas, _settingsCanvas, _howToCanvas;
        [SerializeField] private Toggle _pathwayArrowsToggle;
    
        private void OnEnable() {
            if (_audioMixer.GetFloat(Constants.MasterVolume, out var masterVolume) ) {
                _masterVolumeSilder.value = MixerToSliderValue(masterVolume);
            } if (_audioMixer.GetFloat(Constants.MusicVolume, out var musicVolume) ) {
                _musicVolumeSLider.value = MixerToSliderValue(musicVolume);
            } if (_audioMixer.GetFloat(Constants.SFXVolume, out var sfxVolume) ) {
                _sfxVolumeSlider.value = MixerToSliderValue(sfxVolume);
            }
            _cameraControlSpeed.value = CameraController.Instance.GetCameraControlSpeed();
            SubscribeToEvents();
        }

        private void OnDestroy() {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents() {
            _closeButton.onClick.AddListener(OnCloseClicked);
            _genreButton1.onClick.AddListener(OnGenre1Clicked);
            _genreButton2.onClick.AddListener(OnGenre2Clicked);
            _genreButton3.onClick.AddListener(OnGenre3Clicked);
            _genreButton4.onClick.AddListener(OnGenre4Clicked);
            _howTo.onClick.AddListener(OnHowToClicked);
            _masterVolumeSilder.onValueChanged.AddListener(OnMasterVolumeChanged);
            _musicVolumeSLider.onValueChanged.AddListener(OnMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            _cameraControlSpeed.onValueChanged.AddListener(OnCameraSpeedChanged);
            _pathwayArrowsToggle.onValueChanged.AddListener(OnPathwayArrowsToggle);
        }

        private void OnGenre1Clicked() {
            AudioManager.Instance.PlayCombat1Music();
        }

        private void OnGenre2Clicked() {
            AudioManager.Instance.PlayCombat2Music();
        }

        private void OnGenre3Clicked() {
            AudioManager.Instance.PlayCombat3Music();
        }

        private void OnGenre4Clicked() {
            AudioManager.Instance.PlayPeacefulMusic();
        }

        private void OnHowToClicked() {
            _howToCanvas.SetActive(true);
            _settingsCanvas.SetActive(false);
        }

        private void OnCameraSpeedChanged(float cameraSpeed) {
            CameraController.Instance.SetCameraSpeed(cameraSpeed);
        }
        
        private void UnsubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(OnCloseClicked);
            _genreButton1.onClick.RemoveListener(OnGenre1Clicked);
            _genreButton2.onClick.RemoveListener(OnGenre2Clicked);
            _genreButton3.onClick.RemoveListener(OnGenre3Clicked);
            _genreButton4.onClick.RemoveListener(OnGenre4Clicked);
            _howTo.onClick.RemoveListener(OnHowToClicked);
            _masterVolumeSilder.onValueChanged.RemoveListener(OnMasterVolumeChanged);
            _musicVolumeSLider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            _sfxVolumeSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            _cameraControlSpeed.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            _pathwayArrowsToggle.onValueChanged.RemoveListener(OnPathwayArrowsToggle);
        }

        private void OnMasterVolumeChanged(float sliderValue) {
            _audioMixer.SetFloat(Constants.MasterVolume, SliderToMixerValue(sliderValue));
        }

        private void OnMusicVolumeChanged(float sliderValue) {
            _audioMixer.SetFloat(Constants.MusicVolume, SliderToMixerValue(sliderValue));
        }

        private void OnSFXVolumeChanged(float sliderValue) {
            _audioMixer.SetFloat(Constants.SFXVolume, SliderToMixerValue(sliderValue));
        }

        private void OnCloseClicked() {
            if (Manager.GameManager.Instance != null) {
                Manager.GameManager.Instance.UnpauseCurrentLevel();
            }

            _mainCanvas.SetActive(true);
            _settingsCanvas.SetActive(false);
        }

        private float MixerToSliderValue(float mixerValue) {
            return Mathf.InverseLerp(-80, 20, mixerValue);
        }


        private float SliderToMixerValue(float sliderValue) {
            return Mathf.Lerp(-80, 20, sliderValue);
        }

        private void OnPathwayArrowsToggle(bool isVisible) {
            GameManager.Instance.TogglePathwayArrows(isVisible);
        }
    }
}