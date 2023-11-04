using MetroMayhem.Manager;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    public class Platform : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int _platformID;
        public int PlatformID => _platformID;
        [SerializeField] private GameObject _selectionMedallion;
        private GameObject _occupyingWeapon;
        private bool _isOccupied, _isBlinking, _isSelected, _anotherPlatformSelected;
        private float _blinkInterval;
        #endregion
        
        private void OnEnable() {
            GameManager.StartLevel += Blink;
            GameManager.StartPlay += DoNotBlink;
            GameManager.PauseLevel += Blink;
            GameManager.UnpauseLevel += DoNotBlink;
            GameManager.StopLevel += Blink;
            GameManager.RestartLevel += Blink;
            GameManager.PlatformSelected += AnotherPlatformSelected;
            GameManager.PlatformUnselected += PlatformUnselected;
        }

        private void Update()
        {
            _blinkInterval += Time.deltaTime;
            if (_isSelected || (_isBlinking && !_isOccupied)){
                if (_blinkInterval >= 1) {
                    _blinkInterval = 0;
                    _selectionMedallion.SetActive(!_selectionMedallion.activeSelf);
                }
            }
        }
        
        private void DoNotBlink() {
            _selectionMedallion.SetActive(false);
            _isBlinking = false;
            _isOccupied = false;
            _anotherPlatformSelected = false;
        }

        private void Blink() {
            _isBlinking = true;
            _isSelected = false;
            _blinkInterval = 1f;
        }

        private void OnMouseDown() {
            if (!_isSelected && !_anotherPlatformSelected && !_isOccupied) {
                Select();
            }
        }
        
        public void Select(){
            if (!_isSelected && !_anotherPlatformSelected) {
                _isSelected = true;
                GameManager.Instance.SelectPlatform(_platformID);
            }
        }

        private void AnotherPlatformSelected() {
            _anotherPlatformSelected = true;
            _isBlinking = false;
            _selectionMedallion.SetActive(false);
        }

        private void PlatformUnselected()
        {
            _anotherPlatformSelected = false;
            _isSelected = false;
            _isBlinking = true;
        }

        private void OnDisable() {
            GameManager.StartLevel -= Blink;
            GameManager.StartPlay -= DoNotBlink;
            GameManager.PauseLevel -= Blink;
            GameManager.UnpauseLevel -= DoNotBlink;
            GameManager.StopLevel -= Blink;
            GameManager.RestartLevel -= Blink;
            GameManager.PlatformSelected -= AnotherPlatformSelected;
            GameManager.PlatformUnselected -= PlatformUnselected;
        }
    }
}