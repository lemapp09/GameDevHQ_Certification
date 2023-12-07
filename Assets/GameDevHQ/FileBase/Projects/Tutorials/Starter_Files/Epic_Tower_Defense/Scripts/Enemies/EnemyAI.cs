using System.Collections;
using System.Collections.Generic;
using HoaxGames;
using MetroMayhem.Manager;
using UnityEngine;
using UnityEngine.Audio;
using ProjectDawn.Navigation.Hybrid;
using Unity.Mathematics;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MetroMayhem.Enemies
{
    /// The EnemyAI class controls the behavior of the enemy character in the game.
    /// It handles movement, animation, audio, health, and combat functionality.
    /// /
    [RequireComponent(typeof(FootIK))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    
    public class EnemyAI : MonoBehaviour
    {
        #region Variables

        /// <summary>
        /// Represents a NavMesh agent used for navigation on the NavMesh surface.
        /// </summary>
        [Header("NavMesh Agent")]
        [SerializeField] private AgentAuthoring _agent;

        /// <summary>
        /// The list of waypoints.
        /// </summary>
        private List<Transform> _waypoints;
        [SerializeField] private int _currentWayPointIndex = 0;
        
        [Header("Animator")]
        [SerializeField] private Animator _anim;

        /// <summary>
        /// The audio mixer group used for controlling the volume and effects of the attached audio source.
        /// </summary>
        [Header("Audio")]
        [SerializeField] private AudioMixerGroup _mixerGroup;

        /// <summary>
        /// The private variable _audioSource represents the audio source component attached to an object.
        /// </summary>
        private AudioSource _audioSource;

        /// <summary>
        /// Time in seconds for full dissolve.
        /// </summary>
        [FormerlySerializedAs("_dissolveSpeed")]
        [Header("Dissolve")]
        [Tooltip("Time in seconds for full dissolve")]
        [SerializeField] private float _dissolveSpeed; // 10 for Large, 3 for small

        /// <summary>
        /// The dissolve material used for rendering.
        /// </summary>
        [FormerlySerializedAs("_DissovleMaterial")] [SerializeField] private Material _DissolveMaterial;
        public float _dissolveAmount = 1.0f;

        /// <summary>
        /// The health of the object.
        /// </summary>
        [Header("Health")]
        [SerializeField] private float _health = 100f, _dissolveRate;
        
        private bool _isHit, _isDead, _isPaused, _isAttacking;
        private int _idlesHash, _hitsHash, _deathsHash, _idealNumberHash, _deathHash, _hitHash, _speedHash,
            _attackHash,  _dissolveAmmountID;

        /// <summary>
        /// The timer used for tracking the speed check interval.
        /// </summary>
        private float _speedCheckTimer = 0f, _speedCheckInterval = 0.1f;
        private float _unfreezeCharacter = 0f, _unfreezeInterval = 1.0f;

        /// <summary>
        /// The unclump character value.
        /// </summary>
        private float _unclumpCharacter = 0f, _unclumpInterval = 1.0f;

        /// <summary>
        /// Represents a boolean variable indicating whether the objects are clumped together.
        /// </summary>
        private bool _isClumped;

        /// <summary>
        /// The private member variable representing the unfree position in 3D space.
        /// </summary>
        private Vector3 _unfreePosition;

        public delegate void EnemyReachedDestination();

        /// <summary>
        /// Event that is triggered when an enemy successfully reaches its destination.
        /// </summary>
        public static event EnemyReachedDestination EnemySurvived;

        /// <summary>
        /// Represents a delegate that is used to handle the event when an enemy is killed.
        /// </summary>
        public delegate void EnemyWasKilled();
        public static event EnemyWasKilled EnemyKilled;
        #endregion

        /// <summary>
        /// Called when the object is enabled, either at runtime or in the editor.
        /// </summary>
        private void OnEnable() {
            Manager.GameManager.PauseLevel += Pause;
            Manager.GameManager.UnpauseLevel += Unpause;
            Manager.GameManager.StopLevel += Pause;
            InitializeVariables();
        }

        /// Initializes the enemy object by setting up the required components and variables.
        /// /
        void Start() {
            // Get the NavMeshAgent and Animator components
            if (GetComponent<AgentAuthoring>() != null) {
                _agent = GetComponent<AgentAuthoring>();
            } else { Debug.Log("There is no AgentAuthoring component on this Enemy object.");}

            if (GetComponent<Animator>() != null) {
                _anim = GetComponent<Animator>();
            } else {Debug. Log("There is no Animator component of this Enemy object.");}

            if (_waypoints.Count == 0) {
               Debug.Log("There are no Waypoints!");
            }
            
            // Initialize
            _anim.SetFloat(_idealNumberHash, Random.Range(0, 11) * 0.1f);
            // Assign the initial destination for the NavMeshAgent
            if (_waypoints[_currentWayPointIndex] != null)
            {
                Vector3 tempPosition = _waypoints[_currentWayPointIndex].position;
                tempPosition = new Vector3(tempPosition.x + Random.Range(-2.0f, 2.0f), transform.position.y,
                    tempPosition.z + Random.Range(-2.0f, 2.0f));
                _agent.SetDestination(tempPosition);
            }

            StartCoroutine(CheckDistance());
            // Dissolve Initialize
            if (_dissolveSpeed > 0) {
                _dissolveRate = 1 / _dissolveSpeed;
            }
            _dissolveAmmountID = Shader.PropertyToID("_DissolveAmount");
            InitializeAudioSource();
        }

        /// <summary>
        /// Populates the waypoints list and sets the current waypoint index.
        /// </summary>
        /// <param name="waypoints">The list of waypoints to populate.</param>
        public void PopulateWaypoints(List<Transform> waypoints) {
            _waypoints = waypoints;
            _currentWayPointIndex = 0;
        }

        public void InitializeVariables() {
            _currentWayPointIndex = 0;
            _isHit = false; _isDead = false; _isPaused = false;
            _speedCheckTimer = 0f; _speedCheckInterval = 0.1f;
            _unfreezeCharacter = 0f; _unfreezeInterval = 1.0f;
            _unfreePosition = Vector3.zero;
            if (_agent == null) {
                _agent = GetComponent<AgentAuthoring>();
            }
            _agent.enabled = true;
        }

        /// <summary>
        /// Resumes the movement of the agent after it has been paused.
        /// </summary>
        private void Unpause() { 
            // _agent Start
            if (_waypoints[_currentWayPointIndex] != null) {
                _agent.SetDestination(_waypoints[_currentWayPointIndex].position);
            }
            _isPaused = false;
        }

        private void Pause() {
            _agent.SetDestination(this.transform.position);
            _isPaused = true;
            _anim.SetFloat(_speedHash, 0f);
        }

        public void Attack()
        {
            if (!_isPaused && !_isAttacking && !_isDead && !_isHit)
            {
                StartCoroutine(AttackActions());
            }
        }

        /// <summary>
        /// Executes the attack actions.
        /// </summary>
        /// <returns>An enumerator for the coroutine.</returns>
        private IEnumerator AttackActions()
        {
            _isAttacking = true;
            _anim.SetTrigger(_attackHash);
            _anim.SetFloat(_speedHash, 0f);
            yield return new WaitForSeconds(Time.deltaTime);
            _isAttacking = false;
        }

        /// <summary>
        /// Updates the character's movement, animation, unfreezing, and unclumping behavior.
        /// </summary>
        void Update() {
            if (!_isPaused && !_isAttacking && !_isHit && !_isDead)
            {
                _speedCheckTimer += Time.deltaTime;
                _unfreezeCharacter += Time.deltaTime;
                _unclumpCharacter += Time.deltaTime;
                if (_speedCheckTimer >= _speedCheckInterval)
                {
                    if (!_isHit && !_isDead)
                    {
                        // Get the velocity of the NavMeshAgent and set the Speed parameter of the Animator
                        float3 speed1 = _agent.EntityBody.Velocity;
                        Vector3 vector = new Vector3(speed1.x, speed1.y, speed1.z);
                        _anim.SetFloat(_speedHash, vector.magnitude);

                        // Rotate the enemy to face the direction of travel
                        if (-vector.sqrMagnitude > Mathf.Epsilon)
                        {
                            transform.rotation = Quaternion.LookRotation(vector.normalized);
                        }
                    }
                }

                if (_unfreezeCharacter >= _unfreezeInterval)
                {
                    // Check every second to start a frozen character
                    UnFreezeEnemy();
                }

                if (_unclumpCharacter >= _unclumpInterval)
                {
                    // Check every 3 second to un-clump a character
                    UnClumpEnemy();
                }
            }
        }

        private void UnFreezeEnemy() {
            if (!_isHit && !_isDead && _unfreePosition == this.transform.position) {
                if (_waypoints[_currentWayPointIndex] != null) {
                    _agent.SetDestination(_waypoints[_currentWayPointIndex].position);
                }
            }
            _unfreezeCharacter = 0f;
            _unfreePosition = this.transform.position;
        }

        /// <summary>
        /// Unclumps the enemy if it is too close to its destination waypoint.
        /// </summary>
        /// <remarks>
        /// If the enemy is currently clumped and its remaining distance to the destination waypoint is less than 4 units,
        /// it will unclump by setting the _isClumped flag to false and setting the destination to the next available waypoint.
        /// If there are no more waypoints available, the enemy remains clumped.
        /// If the enemy is not clumped, it will clump by setting the _isClumped flag to true.
        /// </remarks>
        private void UnClumpEnemy() {
            if (_agent.EntityBody.RemainingDistance < 4f) {
                if (_isClumped) {
                    _isClumped = false;
                    if (_currentWayPointIndex < _waypoints.Count - 1) {
                        if (_waypoints[_currentWayPointIndex] != null) {
                            _agent.SetDestination(_waypoints[_currentWayPointIndex].position);
                        }
                    }
                }
                else
                {
                    _isClumped = true;
                }
            }
            _unclumpCharacter = 0f;
        }

        /// Decreases the health of this object by the specified amount.
        /// @param amount The amount of damage to be inflicted.
        /// /
        public void Damage(int amount) {
            if (!_isHit) {
                _audioSource.clip = AudioManager.Instance.PlayHurtSound();
                _audioSource.Play();
                _isHit = true;
                _health -= amount;
                _agent.SetDestination(this.transform.position);
                _anim.SetTrigger(_hitHash);
                if (_health < 1) {
                    Death();
                }
            }
        }

        /// <summary>
        /// Performs the death action for the enemy.
        /// </summary>
        private void Death() {
            _audioSource.clip = AudioManager.Instance.PlayDeathSound();
            _audioSource.Play();
            EnemyKilled?.Invoke();
            _isDead = true;
            _agent.SetDestination(this.transform.position);
            _anim.SetTrigger(_deathHash);
            _agent.enabled = false;
        }

        /// <summary>
        /// A coroutine method that handles hitting the target.
        /// </summary>
        /// <returns>An IEnumerator representing the coroutine.</returns>
        IEnumerator HitRoutine() {
            _agent.SetDestination(this.transform.position);
            yield return new WaitForSeconds(1.5f);
            _agent.SetDestination(_waypoints[_currentWayPointIndex].position);
        }

        /// <summary>
        /// CheckDistance method is used to check the distance between the current position of the object and the waypoints.
        /// It uses a coroutine to continuously check the distance and update the position of the object.
        /// When the distance is less than 1f, it moves towards the next waypoint with some random offset. </summary> <returns>
        /// IEnumerator: IEnumerator object to handle coroutine execution. </returns>
        /// /
        private IEnumerator  CheckDistance() {
            while (_currentWayPointIndex < _waypoints.Count - 1) {
                while (Vector3.Distance(transform.position, _waypoints[_currentWayPointIndex].position) > 1f) {
                    yield return new WaitForSeconds(0.1f);
                }
                _currentWayPointIndex++;
                Vector3 tempPosition = _waypoints[_currentWayPointIndex].position;
                tempPosition = new Vector3(tempPosition.x + Random.Range(-2.0f, 2.0f), transform.position.y,
                    tempPosition.z+ Random.Range(-2.0f, 2.0f));
                _agent.SetDestination(tempPosition);
            }
            EnemySurvived?.Invoke();
            PoolManager.Instance.ReturnToPool(this.gameObject);
        }

        public void OnAnimationStateEntered(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _deathsHash) {
                StartCoroutine(DissolveEnemy(stateInfo.length));
            }
            if (stateInfo.shortNameHash == _hitsHash) {
                StartCoroutine(HitRoutine());
            }
        }

        /// <summary>
        /// Called when an animation state has exited.
        /// </summary>
        /// <param name="animator">The animator component in which the animation state has exited.</param>
        /// <param name="stateInfo">Information about the animation state that has exited.</param>
        /// <param name="layerIndex">The index of the animation layer.</param>
        public void OnAnimationStateExited(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.shortNameHash == _hitsHash) {
            }

            if (stateInfo.shortNameHash == _attackHash)
            {
                _isAttacking = false;
                _agent.SetDestination(_waypoints[_currentWayPointIndex].position);
            }
        }

        /// Dissolves the enemy character over a specified animation length.
        /// @param animLength The length of the dissolve animation.
        /// @returns An IEnumerator object.
        /// /
        private IEnumerator DissolveEnemy(float animLength)
        {
            SkinnedMeshRenderer[] _parts = GetComponentsInChildren<SkinnedMeshRenderer>(); 
            yield return  new WaitForSeconds(animLength);
            Material[] temp = new Material[_parts.Length];
            for (int i = 0; i < _parts.Length; i++) {
                temp[i] = _parts[i].material;
            }
            foreach (var part in _parts) {
                part.material = _DissolveMaterial;
            }
            while (_dissolveAmount > 0) {
                _dissolveAmount -= Time.deltaTime * _dissolveRate ;  
                foreach (var part in _parts) {
                    part.material.SetFloat(_dissolveAmmountID, _dissolveAmount);
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
            for (int i = 0; i < _parts.Length; i++) {
                _parts[i].material = temp[i] ;
            }
            _anim.SetFloat(_speedHash, 0f);
            _agent.Stop();
            PoolManager.Instance.ReturnToPool(this.gameObject);
        }

        private void OnDisable(){
            GameManager.PauseLevel -= Pause;
            GameManager.UnpauseLevel -= Unpause;
            GameManager.StopLevel -= Pause;
        }

        /// <summary>
        /// Initializes the hash codes for various properties of an object.
        /// </summary>
        /// <param name="hashCodes">An array containing the hash codes for the properties.</param>
        /// <remarks>
        /// The <paramref name="hashCodes"/> array should contain the following elements, in order:
        /// - The idle hash code
        /// - The hit hash code
        /// - The death hash code
        /// - The ideal number hash code
        /// - The death hash code
        /// - The hit hash code
        /// - The speed hash code
        /// - The attack hash code
        /// The method assigns each hash code to its corresponding property in the object.
        /// </remarks>
        public void InitializeHashCodes(int[] hashCodes)
        {
            _idlesHash = hashCodes[0];
            _hitsHash =  hashCodes[1];
            _deathsHash =  hashCodes[2];
            _idealNumberHash =  hashCodes[3];
            _deathHash =  hashCodes[4];
            _hitHash =  hashCodes[5];
            _speedHash = hashCodes[6];
            _attackHash = hashCodes[7];
        }

        /// <summary>
        /// Initializes the audio source with default settings.
        /// </summary>
        public void InitializeAudioSource() {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.loop = false;
            _audioSource.playOnAwake = false;
            _audioSource.spatialBlend = 1;
            _audioSource.volume = 1;
            _audioSource.outputAudioMixerGroup = _mixerGroup;
        }

        /// <summary>
        /// Invokes the `EnemySurvived` event and returns the game object to the object pool.
        /// </summary>
        public void ReachedEnd() {
            EnemySurvived?.Invoke();
            PoolManager.Instance.ReturnToPool(this.gameObject);
        }
    }
}