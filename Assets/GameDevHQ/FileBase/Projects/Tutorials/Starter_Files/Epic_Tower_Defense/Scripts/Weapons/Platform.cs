using System.Security.Cryptography;
using MetroMayhem.Manager;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    public class Platform : MonoBehaviour
    {
        #region Variables
        [SerializeField] private int _platformID, _weaponID;
        [SerializeField] private GameObject _selectionMedallion;
        [SerializeField] private Collider _boxCollider;
        private GameObject _occupyingWeapon;
        private bool _isOccupied, _isBlinking, _isSelected, _anotherPlatformSelected;
        private float _blinkInterval;
        #endregion
        
        private void OnEnable() {
            _boxCollider.enabled = true;
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
            if (!_isOccupied) {
                _blinkInterval += Time.deltaTime;
                if (_isSelected || _isBlinking)
                {
                    if (_blinkInterval >= 1)
                    {
                        _blinkInterval = 0;
                        _selectionMedallion.SetActive(!_selectionMedallion.activeSelf);
                    }
                }
            }
        }
        
        private void DoNotBlink() {
            _selectionMedallion.SetActive(false);
            _isBlinking = false;
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

        public void SetAsOccupied(GameObject OccupyingWeapon, int weaponID) {
            _weaponID  = weaponID;
            _isOccupied = true;
            _boxCollider.enabled = false;
            _occupyingWeapon = OccupyingWeapon;
            _selectionMedallion.SetActive(false);
        }

        public int GetWeaponID() {
            return _weaponID;
        }

        public GameObject GetOccupyingWeapon() {
            return _occupyingWeapon;
        }

        public void RemoveOccupyingWeapon() {
            _isOccupied = false;
            _boxCollider.enabled = true;
            if ( _occupyingWeapon != null) {
                Destroy(_occupyingWeapon);
            }
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