using System.Collections;
using System.Collections.Generic;
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
        private bool _isOccupied;
        private bool _isBlinking;
        private float _blinkInterval;
        #endregion
        
        private void OnEnable() {
            GameManager.StartLevel += Blink;
            GameManager.StartPlay += DoNotBlink;
            GameManager.PauseLevel += Blink;
            GameManager.UnpauseLevel += DoNotBlink;
            GameManager.StopLevel += Blink;
            GameManager.RestartLevel += Blink;
        }

        private void Update()
        {
            _blinkInterval += Time.deltaTime;
            if (_isBlinking && !_isOccupied){
                if (_blinkInterval >= 1) {
                    _blinkInterval = 0;
                    _selectionMedallion.SetActive(!_selectionMedallion.activeSelf);
                }
            }
        }
        
        private void DoNotBlink() {
            _selectionMedallion.SetActive(false);
            _isBlinking = false;
        }

        private void Blink() {
            _isBlinking = true;
            _blinkInterval = 1f;
        }

        private void OnDisable() {
            GameManager.StartLevel += Blink;
            GameManager.StartPlay += DoNotBlink;
            GameManager.PauseLevel += Blink;
            GameManager.UnpauseLevel += DoNotBlink;
            GameManager.StopLevel += Blink;
            GameManager.RestartLevel += Blink;
        }
    }
}