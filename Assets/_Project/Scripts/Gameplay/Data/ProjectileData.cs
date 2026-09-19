using UnityEngine;

namespace Gameplay.Data
{
    public enum AmmunitionType
    {
        ArmorPiercing,  // Бронебійний (AP)
        HighExplosive   // Фугасний (HE)
    }

    [CreateAssetMenu(fileName = "NewProjectileData", menuName = "Naval/Projectile Data")]
    public class ProjectileData : ScriptableObject
    {
        [Header("General")]
        public string Name;
        public AmmunitionType Type;

        [Header("Visuals")]
        public GameObject VisualPrefab; // Якщо захочеш різні модельки для різних калібрів

        [Header("Physics & Damage")]
        [Tooltip("Маса в кілограмах")]
        public float Mass = 125f;

        [Tooltip("Калібр у міліметрах")]
        public float Caliber = 203f;

        [Tooltip("Максимальна шкода при пробитті цитаделі")]
        public float MaxDamage = 4500f;
    }
}