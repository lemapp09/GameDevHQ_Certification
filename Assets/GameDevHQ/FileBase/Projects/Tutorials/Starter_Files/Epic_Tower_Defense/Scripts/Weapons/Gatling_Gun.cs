﻿using MetroMayhem.Manager;
using UnityEngine;


namespace MetroMayhem.Weapons
{
    #region message
    /// <summary>
    /// This script will allow you to view the presentation of the Turret and use it within your project.
    /// Please feel free to extend this script however you'd like. To access this script from another script
    /// (Script Communication using GetComponent) -- You must include the namespace (using statements) at the top. 
    /// "using GameDevHQ.FileBase.Gatling_Gun" without the quotes. 
    /// 
    /// For more, visit GameDevHQ.com
    /// 
    /// @authors
    /// Al Heck
    /// Jonathan Weinberger
    /// </summary>
    #endregion

    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class Gatling_Gun : MonoBehaviour , IWeapon
    {
        #region variables
        private Transform _gunBarrel; //Reference to hold the gun barrel
        public GameObject Muzzle_Flash; //reference to the muzzle flash effect to play when firing
        public ParticleSystem bulletCasings; //reference to the bullet casing effect to play when firing
        public AudioClip fireSound; //Reference to the audio clip

        private AudioSource _audioSource; //reference to the audio source component
        private bool _startWeaponNoise = true;
        private MetroMayhemInputSystem _input;
        private bool _isFiring;

        private int _platformID;
        [SerializeField] public int _weaponID;
        private bool _isPaused;
        private int _damageAmount;
        private float tempRotY;
        #endregion
        
        private void OnEnable()
        {
            _isPaused = true;
            _input = new MetroMayhemInputSystem();
            _input.Towers.Enable();
            tempRotY= transform.localRotation.eulerAngles.y;
            GameManager.StartLevel += PauseGun;
            GameManager.StartPlay += UnpauseGun;
            GameManager.PauseLevel += PauseGun;
            GameManager.UnpauseLevel += UnpauseGun;
            GameManager.StopLevel += PauseGun;
            GameManager.RestartLevel -= PauseGun;
        }
        
        // Use this for initialization
        void Start()
        {
            _gunBarrel = GameObject.Find("Barrel_to_Spin").GetComponent<Transform>(); //assigning the transform of the gun barrel to the variable
            Muzzle_Flash.SetActive(false); //setting the initial state of the muzzle flash effect to off
            _audioSource = GetComponent<AudioSource>(); //ssign the Audio Source to the reference variable
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = fireSound; //assign the clip to play
        }

        // Update is called once per frame
        void Update()
        {
            if (!_isPaused)
            {
                if (_isFiring) //Check for left click (held) user input
                {
                    RotateBarrel(); //Call the rotation function responsible for rotating our gun barrel
                    Muzzle_Flash.SetActive(true); //enable muzzle effect particle effect
                    bulletCasings.Emit(1); //Emit the bullet casing particle effect  

                    if (_startWeaponNoise == true) //checking if we need to start the gun sound
                    {
                        _audioSource.Play(); //play audio clip attached to audio source
                        _startWeaponNoise =
                            false; //set the start weapon noise value to false to prevent calling it again
                    }

                }
                else if (!_isFiring) //Check for left click (release) user input
                {
                    Muzzle_Flash.SetActive(false); //turn off muzzle flash particle effect
                    _audioSource.Stop(); //stop the sound effect from playing
                    _startWeaponNoise = true; //set the start weapon noise value to true
                }
            }
        }

        // Method to rotate gun barrel 
        void RotateBarrel() 
        {
            _gunBarrel.transform.Rotate(Vector3.forward * Time.deltaTime * -500.0f); //rotate the gun barrel along the "forward" (z) axis at 500 meters per second

        }
        
        public void Damage()
        {
            //
        }

        public void Rotate(bool rotateLeft)
        {
            if (rotateLeft) {
                tempRotY -= 15f;
            } else  {
                tempRotY += 15f;
            }

            if (tempRotY <= 0) {
                tempRotY = 0;}
            else if (tempRotY >= 360) {
                tempRotY = 360;
            }
            transform.localRotation = Quaternion.Euler(0, tempRotY, 0);
        }


        private void OnDisable()
        {
            GameManager.StartLevel -= UnpauseGun;
            GameManager.PauseLevel -= PauseGun;
            GameManager.UnpauseLevel -= UnpauseGun;
            GameManager.StopLevel -= PauseGun;
            GameManager.RestartLevel -= PauseGun;
        }

        private void PauseGun()
        {
            _isPaused = true;
        }

        private void UnpauseGun()
        {
            _isPaused = false;
        }

        private void OnMouseDown()
        {
            _isFiring = true;
        }

        private void OnMouseUp()
        {
            _isFiring = false;
        }

        private void OnMouseDrag()
        {
            //
        }

    }

}
