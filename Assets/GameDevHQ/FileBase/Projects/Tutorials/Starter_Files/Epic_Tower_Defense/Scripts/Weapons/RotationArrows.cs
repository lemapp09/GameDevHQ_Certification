using MetroMayhem.Manager;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider))]
    public class RotationArrows : MonoBehaviour
    {
        [SerializeField] private GameObject _weaponToRotate;
        private IWeapon _iWeaponToRotate;
        [SerializeField] private bool _isLeft;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private Collider _collider;

        
        private void OnEnable()
        {
            if (_weaponToRotate.GetComponent<IWeapon>() != null) {
                _iWeaponToRotate = _weaponToRotate.GetComponent<IWeapon>();
            }
            if (_spriteRenderer == null) {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
            if (_collider == null) {
                _collider = GetComponent<Collider>();
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
            _collider.enabled = false;
        }

        private void UnPauseRotation() {
            _spriteRenderer.enabled = true;
            _collider.enabled = true;
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