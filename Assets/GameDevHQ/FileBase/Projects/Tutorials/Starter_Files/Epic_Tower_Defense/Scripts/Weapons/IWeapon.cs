namespace MetroMayhem.Weapons
{
    public interface IWeapon
    {
        void Damage(int DamageAmount);
        void Rotate(bool rotateLeft);
        void Upgrade();
        void Dismantle();
    }
}