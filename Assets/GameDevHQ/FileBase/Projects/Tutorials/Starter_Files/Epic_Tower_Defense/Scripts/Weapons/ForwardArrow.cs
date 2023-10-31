using System;
using System.Collections;
using System.Collections.Generic;
using MetroMayhem.Manager;
using Unity.VisualScripting;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    public class ForwardArrow : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        
        private void OnEnable() {
            GameManager.StartLevel += UnpauseRotation;
            GameManager.StartPlay += PauseRotation;
            GameManager.PauseLevel += UnpauseRotation;
            GameManager.UnpauseLevel += PauseRotation;
            GameManager.StopLevel += UnpauseRotation;
            GameManager.RestartLevel += UnpauseRotation;
        }

        private void PauseRotation() {
            _spriteRenderer.enabled = false;
        }

        private void UnpauseRotation() {
            _spriteRenderer.enabled = true;
        }

        private void OnDisable() {
            GameManager.StartLevel += UnpauseRotation;
            GameManager.StartPlay += PauseRotation;
            GameManager.PauseLevel += UnpauseRotation;
            GameManager.UnpauseLevel += PauseRotation;
            GameManager.StopLevel += UnpauseRotation;
            GameManager.RestartLevel += UnpauseRotation;
        }
    }
}