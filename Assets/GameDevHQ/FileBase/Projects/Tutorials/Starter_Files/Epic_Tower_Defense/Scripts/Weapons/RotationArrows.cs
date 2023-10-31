using System;
using System.Collections;
using System.Collections.Generic;
using MetroMayhem.Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    public class RotationArrows : MonoBehaviour
    {
        [SerializeField] private GameObject _weaponToRotate;
        private IWeapon _iWeaponToRotate;
        [SerializeField] private bool _isLeft;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        
        private void OnEnable()
        {
            if (_weaponToRotate.GetComponent<IWeapon>() != null) {
                _iWeaponToRotate = _weaponToRotate.GetComponent<IWeapon>();
            }
            GameManager.StartLevel += UnPauseRotation;
            GameManager.StartPlay += PauseRotation;
            GameManager.PauseLevel += UnPauseRotation;
            GameManager.UnpauseLevel += PauseRotation;
            GameManager.StopLevel += UnPauseRotation;
            GameManager.RestartLevel += UnPauseRotation;
        }
        private void OnMouseDown() {
            _spriteRenderer.color = Color.red;
            _iWeaponToRotate.Rotate(_isLeft);
        }

        private void OnMouseUp() {
            _spriteRenderer.color = Color.white;
        }

        private void PauseRotation() {
            _spriteRenderer.enabled = false;
        }

        private void UnPauseRotation() {
            _spriteRenderer.enabled = true;
        }

        private void OnDisable() {
            GameManager.StartLevel += UnPauseRotation;
            GameManager.StartPlay += PauseRotation;
            GameManager.PauseLevel += UnPauseRotation;
            GameManager.UnpauseLevel += PauseRotation;
            GameManager.StopLevel += UnPauseRotation;
            GameManager.RestartLevel += UnPauseRotation;
        }
    }
}