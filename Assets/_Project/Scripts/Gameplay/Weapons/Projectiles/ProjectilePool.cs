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
        private readonly Dictionary<ProjectileData, List<Projectile>> _pools = new();

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

            for (int i = 0; i < pool.Count; i++)
            {
                if (!pool[i].IsActive) return pool[i];
            }

            GameObject go = Instantiate(data.VisualPrefab, transform);
            Projectile proj = go.GetComponent<Projectile>();

            if (proj == null) proj = go.AddComponent<Projectile>();

            pool.Add(proj);
            _activeProjectiles.Add(proj); 

            return proj;
        }


        // UPDATE MANAGER: Moves ALL projectiles in the game.
        private void Update()
        {
            float dt = Time.deltaTime;

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