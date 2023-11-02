using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    public class Fireball : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _particleSystem;
        private IEnumerator Start()
        {
            _particleSystem.Stop();
            yield return new WaitForSeconds(0.65f);
            _particleSystem.Play();
            Destroy(gameObject, 2.5f);
        }
    }
}