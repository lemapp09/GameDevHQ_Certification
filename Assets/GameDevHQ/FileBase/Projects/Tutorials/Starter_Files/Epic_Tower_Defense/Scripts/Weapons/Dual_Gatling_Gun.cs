using System.Collections;
using System.Collections.Generic;
using MetroMayhem.Manager;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace MetroMayhem.Weapons
{
    #region message
    /// <summary>
    /// This script will allow you to view the presentation of the Turret and use it within your project.
    /// Please feel free to extend this script however you'd like. To access this script from another script
    /// (Script Communication using GetComponent) -- You must include the namespace (using statements) at the top. 
    /// "using GameDevHQ.FileBase.Dual_Gatling_Gun" without the quotes. 
    /// 
    /// For more, visit GameDevHQ.com
    /// 
    /// @authors
    /// Al Heck
    /// Jonathan Weinberger
    /// </summary>
    #endregion
    [RequireComponent(typeof(AudioSource))] //Require Audio Source component
    public class Dual_Gatling_Gun : MonoBehaviour , IWeapon
    {
        #region variables
        [SerializeField] private Transform[] _firePoint; // Where bullets are instantiated
        [SerializeField] private Transform[] _gunBarrel; //Reference to hold the gun barrel

        [SerializeField] private GameObject[] _muzzleFlash; //reference to the muzzle flash effect to play when firing

        [SerializeField]
        private ParticleSystem[] _bulletCasings; //reference to the bullet casing effect to play when firing
        
        [SerializeField] private AudioClip _fireSound; //Reference to the audio clip

        [SerializeField] private AudioMixerGroup _mixerGroup;
        private AudioSource _audioSource; //reference to the audio source component
        private bool _startWeaponNoise = true;
        private MetroMayhemInputSystem _input;

        public float viewRange = 5f;
        public float viewAngle = 90f;
        public LayerMask targetMask;
        public LayerMask obstacleMask;
        public List<Transform> visibleTargets = new List<Transform>();

        [SerializeField] private bool _isFiring,  _isPaused;
        [SerializeField] private int _weaponID;
        private int _damageAmount;
        private float _tempRotY;
        private float _health = 100;
        #endregion
        
        private void OnEnable()
        {
            _isPaused = true;
            _input = new MetroMayhemInputSystem();
            _input.Towers.Enable();
            _tempRotY= transform.localRotation.eulerAngles.y;
            GameManager.StartLevel += PauseGun;
            GameManager.StartPlay += UnpauseGun;
            GameManager.PauseLevel += PauseGun;
            GameManager.UnpauseLevel += UnpauseGun;
            GameManager.StopLevel += PauseGun;
            GameManager.RestartLevel += PauseGun;
        }
 
        void Start()
        {
            _muzzleFlash[0].SetActive(false); //setting the initial state of the muzzle flash effect to off
            _muzzleFlash[1].SetActive(false); //setting the initial state of the muzzle flash effect to off
            _audioSource = GetComponent<AudioSource>(); //ssign the Audio Source to the reference variable
            _audioSource.playOnAwake = false; //disabling play on awake
            _audioSource.loop = true; //making sure our sound effect loops
            _audioSource.clip = _fireSound; //assign the clip to play
            _audioSource.outputAudioMixerGroup = _mixerGroup;
            // StartCoroutine("FindVisibleTargets");
        }

        // Update is called once per frame
        void Update() {
            if (!_isPaused) {
                if (_isFiring) {//Check for left click (held) user input
                    RotateBarrel(); //Call the rotation function responsible for rotating our gun barrel

                    //for loop to iterate through all muzzle flash objects
                    for (int i = 0; i < _muzzleFlash.Length; i++) {
                        _muzzleFlash[i].SetActive(true); //enable muzzle effect particle effect
                        _bulletCasings[i].Emit(1); //Emit the bullet casing particle effect  
                        FireBullet(i);
                    }

                    if (_startWeaponNoise == true) {//checking if we need to start the gun sound
                        _audioSource.Play(); //play audio clip attached to audio source
                        _startWeaponNoise =
                            false; //set the start weapon noise value to false to prevent calling it again
                    }
                }
                else if (!_isFiring) {//Check for left click (release) user input
                    //for loop to iterate through all muzzle flash objects
                    for (int i = 0; i < _muzzleFlash.Length; i++) {
                        _muzzleFlash[i].SetActive(false); //enable muzzle effect particle effect
                    }
                    _audioSource.Stop(); //stop the sound effect from playing
                    _startWeaponNoise = true; //set the start weapon noise value to true
                }
            }
        }

        private void FireBullet(int j) {
            if (!_isPaused) {
                for (int i = 0; i < 6; i++) {
                    // Generate a random direction within the spread angle
                    Quaternion fireRotation = Quaternion.LookRotation(transform.forward);
                    Quaternion randomRotation = Random.rotation;
                    fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0.0f, 5f));
                    RaycastHit hit;
                    // Cast the ray in the calculated direction
                    if (Physics.Raycast(_firePoint[j].position, fireRotation * Vector3.forward, out hit, 7f, 1 << 6)) {
                        if (hit.collider.CompareTag("Enemy")) {
                            hit.collider.GetComponent<Enemies.EnemyAI>().Damage(100);
                        }
                    }
                }
            }
        }

        private void DamageEnemy(RaycastHit hit)
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.collider.SendMessage("Damage", 35);
            }
        }
        public void Damage(int DamageAmount)
        {
            _health -= DamageAmount;
            if (_health < 0) {
                Destroy(gameObject);
            }
        }

        public void Rotate(bool rotateLeft) {
            if (rotateLeft) {
                _tempRotY -= 15f;
            } else  {
                _tempRotY += 15f;
            }

            if (_tempRotY <= 0) {
                _tempRotY = 0;}
            else if (_tempRotY >= 360) {
                _tempRotY = 360;
            }
            transform.localRotation = Quaternion.Euler(0, _tempRotY, 0);
        }

        public void Upgrade()
        {
            if (_isPaused) {
                GameManager.Instance.UpgradeTower(this.transform.GetComponent<WeaponID>().GetPlatformID(), _weaponID);
            }
        }

        public void Dismantle()
        {
            if (_isPaused) {
                GameManager.Instance.DismantleTower(this.transform.GetComponent<WeaponID>().GetPlatformID(), _weaponID);
            }
        }

        // Method to rotate gun barrel 
        void RotateBarrel()
        {
            _gunBarrel[0].transform
                .Rotate(-500.0f * Time.deltaTime *
                        Vector3.forward); //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
            _gunBarrel[1].transform
                .Rotate(-500.0f * Time.deltaTime *
                        Vector3.forward); //rotate the gun barrel along the "forward" (z) axis at 500 meters per second
        }

        private void OnMouseDown() {
            if (_isPaused) {
                if (_input.Towers.Upgrade.IsPressed()) {
                    Upgrade();
                }
                if (_input.Towers.Dismantle.IsPressed()) {
                    Dismantle();
                }
            } else {
                _isFiring = true;
            }
        }

        private void OnMouseUp() {
            _isFiring = false;
        }

        private void OnDisable()
        {
            GameManager.StartLevel -= PauseGun;
            GameManager.StartPlay -= UnpauseGun;
            GameManager.PauseLevel -= PauseGun;
            GameManager.UnpauseLevel -= UnpauseGun;
            GameManager.StopLevel -= PauseGun;
            GameManager.RestartLevel -= PauseGun;
        }

        private void PauseGun() {
            _isPaused = true;
            _isFiring = false;
        }

        private void UnpauseGun() {
            _isPaused = false;
        }

        IEnumerator FindVisibleTargets()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);
                FindTargetsInView();
            }
        }

        void FindTargetsInView()
        {
            visibleTargets.Clear();
            Collider[] targetsInView = Physics.OverlapSphere(transform.position, viewRange, targetMask);

            for (int i = 0; i < targetsInView.Length; i++)
            {
                Transform target = targetsInView[i].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;

                if (Vector3.Angle(transform.forward, directionToTarget) < viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, directionToTarget, distanceToTarget, obstacleMask))
                    {
                        visibleTargets.Add(target);
                    }
                }
            }
        }
    }
}