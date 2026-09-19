using System.Collections.Generic;
using UnityEngine;
using Gameplay.Data;
using Gameplay.Weapons.Projectiles;

namespace Gameplay.Weapons.Projectiles
{
    public interface IProjectilePoolService
    {
        Projectile GetProjectile(ProjectileData data);
    }

    public class ProjectilePool : MonoBehaviour, IProjectilePoolService
    {
        // Словник для сортування пулів за типом снаряда (щоб лінкори і крейсери мали свої снаряди)
        private readonly Dictionary<ProjectileData, List<Projectile>> _pools = new();

        // Єдиний плаский список для Update Manager (швидка ітерація)
        private readonly List<Projectile> _activeProjectiles = new(1000);

        private void Awake()
        {
            ServiceLocator.Register<IProjectilePoolService>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<IProjectilePoolService>();
        }

        public Projectile GetProjectile(ProjectileData data)
        {
            if (data == null || data.VisualPrefab == null) return null;

            if (!_pools.TryGetValue(data, out List<Projectile> pool))
            {
                pool = new List<Projectile>();
                _pools[data] = pool;
            }

            // 1. Шукаємо вільний снаряд у пулі (без використання LINQ для Zero GC)
            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].IsActive) return pool[i];
            }

            // 2. Якщо вільних немає — створюємо новий
            GameObject go = Instantiate(data.VisualPrefab, transform);
            Projectile proj = go.GetComponent<Projectile>();

            // Запобіжник: якщо на префабі забули повісити скрипт
            if (proj == null) proj = go.AddComponent<Projectile>();

            pool.Add(proj);
            _activeProjectiles.Add(proj); // Додаємо у глобальний список для Tick()

            return proj;
        }

        // ==========================================
        // UPDATE MANAGER: Рухає ВСІ снаряди у грі
        // ==========================================
        private void Update()
        {
            float dt = Time.deltaTime;

            // Використовуємо класичний for (працює швидше за foreach і не створює сміття)
            for (int i = 0; i < _activeProjectiles.Count; i++)
            {
                if (_activeProjectiles[i].IsActive)
                {
                    _activeProjectiles[i].Tick(dt);
                }
            }
        }
    }
}