using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MetroMayhem.Weapons
{
    
    // Interface
    public interface IWeapon
    {
        public void Damage(int DamageAmount);
        public void Rotate(bool rotateLeft);
    }
}