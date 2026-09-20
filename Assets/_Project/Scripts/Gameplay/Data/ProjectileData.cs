using UnityEngine;

namespace Gameplay.Data
{
    public enum AmmunitionType
    {
        ArmorPiercing,  // (AP)
        HighExplosive   // (HE)
    }

    [CreateAssetMenu(fileName = "NewProjectileData", menuName = "Naval/Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        [Header("General")]
        public string Name;
        public AmmunitionType Type;

        [Header("Visuals")]
        public GameObject VisualPrefab;

        [Header("Physics & Damage")]
        [Tooltip("Mass in kilograms")]
        public float Mass = 125f;

        [Tooltip("Caliber in millimeters")]
        public float Caliber = 203f;

        [Tooltip("Maximum damage from a citadel hit")]
        public float MaxDamage = 4500f;
    }
}