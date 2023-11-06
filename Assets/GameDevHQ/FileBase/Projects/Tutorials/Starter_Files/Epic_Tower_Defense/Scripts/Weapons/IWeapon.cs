
namespace MetroMayhem.Weapons
{
    
    // Interface
    public interface IWeapon
    {
        
        public void Damage(int DamageAmount);
        public void Rotate(bool rotateLeft);
        void Upgrade();
        void Dismantle();

        public void SetPlatformID(int PlatformID) {
           int  _platformID = PlatformID;
        }
    }
}