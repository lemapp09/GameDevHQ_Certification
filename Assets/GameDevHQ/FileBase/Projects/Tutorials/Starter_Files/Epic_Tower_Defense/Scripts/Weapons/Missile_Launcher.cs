using System.Collections;
using UnityEngine;
using MetroMayhem.Manager;

namespace MetroMayhem.Weapons
{
    public class Missile_Launcher : MonoBehaviour , IWeapon
    {
        #region variables
        [SerializeField]
        private GameObject _fireballPrefab; //prefab for the rocket
        [SerializeField]
        private GameObject[] _misslePositionsLeft; //array to hold the rocket positions on the turret
        [SerializeField]
        private GameObject[] _misslePositionsRight; //array to hold the rocket positions on the turret
        [SerializeField]
        private float _fireDelay; //fire delay between rockets
        [SerializeField]
        private float _launchSpeed; //initial launch speed of the rocket
        [SerializeField]
        private float _power; //power to apply to the force of the rocket
        [SerializeField]
        private float _fuseDelay; //fuse delay before the rocket launches
        [SerializeField]
        private float _reloadTime; //time in between reloading the rockets
        [SerializeField]
        private float _destroyTime = 10.0f; //how long till the rockets get cleaned up
        private bool _launched; //bool to check if we launched the rockets
        private MetroMayhemInputSystem _input;
        private bool _isFiring;
        [SerializeField] private bool _isDual;

        private int _platformID;
        [SerializeField] private int _weaponID;
        private bool _isPaused;
        private int _damageAmount;
        private float tempRotY;
        private float _health = 100;
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
            GameManager.RestartLevel += PauseGun;
            _platformID = this.transform.GetComponent<WeaponID>().GetPlatformID();
        }
        
        public void SetPlatformID(int PlatformId) {
            _platformID = PlatformId;
        }
        
        private void Update()
        {
            if (!_isPaused)
            {
                if (_isFiring && _launched == false) //check for space key and if we launched the rockets
                {
                    _launched = true; //set the launch bool to true
                    StartCoroutine(FireRocketsRoutine()); //start a coroutine that fires the rockets. 
                }
            }
        }

        IEnumerator FireRocketsRoutine()
        {
            for (int i = 0; i < _misslePositionsLeft.Length; i++) //for loop to iterate through each missle position
            {
                if (_isDual)
                {
                    GameObject rocketRight = MissilePoolManager.Instance.GetMissile(); //instantiate a rocket
                    rocketRight.transform.parent = _misslePositionsRight[i].transform; //set the rockets parent to the missle launch position 
                    rocketRight.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                    rocketRight.transform.localEulerAngles = new Vector3(0, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                    rocketRight.transform.parent = null; //set the rocket parent to null 
                    rocketRight.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missle>().AssignMissleRules(_launchSpeed,
                        _power, _fuseDelay, _destroyTime - (i * 0.2f)); //assign missle properties
                    _misslePositionsRight[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired 
                    Instantiate(_fireballPrefab, transform.position + transform.forward * (27f  - i * 4.2f),
                        Quaternion.identity); //instantiate fireball
                }
                GameObject rocketLeft = MissilePoolManager.Instance.GetMissile(); //instantiate a rocket
                rocketLeft.transform.parent = _misslePositionsLeft[i].transform; //set the rockets parent to the missle launch position 
                rocketLeft.transform.localPosition = Vector3.zero; //set the rocket position values to zero
                rocketLeft.transform.localEulerAngles = new Vector3(0, 0, 0); //set the rotation values to be properly aligned with the rockets forward direction
                rocketLeft.transform.parent = null; //set the rocket parent to null
                rocketLeft.GetComponent<GameDevHQ.FileBase.Missle_Launcher_Dual_Turret.Missle.Missle>().AssignMissleRules(_launchSpeed,
                    _power, _fuseDelay, _destroyTime - ((i * 0.2f)+ 0.1f)); //assign missle properties
                _misslePositionsLeft[i].SetActive(false); //turn off the rocket sitting in the turret to make it look like it fired
                Instantiate(_fireballPrefab, transform.position + transform.forward * (27f  - (i * 4.2f + 2.1f)),
                    Quaternion.identity); //instantiate fireball

                yield return new WaitForSeconds(_fireDelay); //wait for the firedelay
            }

            for (int i = 0; i < _misslePositionsLeft.Length; i++) //itterate through missle positions
            {
                yield return new WaitForSeconds(_reloadTime); //wait for reload time
                _misslePositionsLeft[i].SetActive(true); //enable fake rocket to show ready to fire
                if (_isDual) {
                    _misslePositionsRight[i].SetActive(true); //enable fake rocket to show ready to fire
                }
            }

            _launched = false; //set launch bool to false
        }
        
        public void Damage(int DamageAmmount)
        {
            _health -= DamageAmmount;
            if(_health <= 0)
            {
                Destroy(gameObject);
            }
        }

        public void Rotate(bool rotateLeft) {
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
        private void PauseGun() {
            _isPaused = true;
        }

        private void UnpauseGun() {
            _isPaused = false;
        }

        private void OnMouseDown() {
            if (_isPaused) {
                if (_input.Towers.Upgrade.IsPressed()) {
                    Upgrade();
                }
                if (_input.Towers.Dismantle.IsPressed()) {
                    Dismantle();
                }
            }

            _isFiring = true;
        }

        private void OnMouseUp() {
            _isFiring = false;
        }

        private void OnDisable() {
            GameManager.StartLevel -= PauseGun;
            GameManager.StartPlay -= UnpauseGun;
            GameManager.PauseLevel -= PauseGun;
            GameManager.UnpauseLevel -= UnpauseGun;
            GameManager.StopLevel -= PauseGun;
            GameManager.RestartLevel -= PauseGun;
        }

    }
}

