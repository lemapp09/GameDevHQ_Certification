using UnityEngine;


namespace MetroMayhem.Weapons
{
    public class WeaponID : MonoBehaviour
    {
        [SerializeField] private int _platformID;
        [SerializeField] private int _weaponID;

        public void SetPlatformID(int PlatformID)
        {
            _platformID = PlatformID;
        }

        public int GetPlatformID()
        {
            return _platformID;
        }

        public int GetWeaponID()
        {
            return _weaponID;
        }
    }
}