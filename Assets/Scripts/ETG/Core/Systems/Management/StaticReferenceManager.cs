using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using UnityEngine;

using Dungeonator;

#nullable disable

    public static class StaticReferenceManager
    {
        public static List<ClusteredTimeInvariantMonoBehaviour> AllClusteredTimeInvariantBehaviours = new List<ClusteredTimeInvariantMonoBehaviour>();
        public static List<GameObject> AllCorpses = new List<GameObject>();
        public static List<DebrisObject> AllDebris = new List<DebrisObject>();
        public static List<AIActor> AllEnemies = new List<AIActor>();
        public static List<TalkDoerLite> AllNpcs = new List<TalkDoerLite>();
        public static List<ProjectileTrapController> AllProjectileTraps = new List<ProjectileTrapController>();
        public static List<BasicTrapController> AllTriggeredTraps = new List<BasicTrapController>();
        public static List<ForgeHammerController> AllForgeHammers = new List<ForgeHammerController>();
        public static List<BaseShopController> AllShops = new List<BaseShopController>();
        public static List<MajorBreakable> AllMajorBreakables = new List<MajorBreakable>();
        public static List<MinorBreakable> AllMinorBreakables = new List<MinorBreakable>();
        public static List<HealthHaver> AllHealthHavers = new List<HealthHaver>();
        public static List<DeadlyDeadlyGoopManager> AllGoops = new List<DeadlyDeadlyGoopManager>();
        public static List<BroController> AllBros = new List<BroController>();
        public static List<Chest> AllChests = new List<Chest>();
        public static List<InteractableLock> AllLocks = new List<InteractableLock>();
        public static List<BulletScriptSource> AllBulletScriptSources = new List<BulletScriptSource>();
        public static int WeaponChestsSpawnedOnFloor = 0;
        public static int ItemChestsSpawnedOnFloor = 0;
        public static int DChestsSpawnedOnFloor = 0;
        public static int DChestsSpawnedInTotal = 0;
        public static List<PortalGunPortalController> AllPortals = new List<PortalGunPortalController>();
        public static List<TallGrassPatch> AllGrasses = new List<TallGrassPatch>();
        public static List<AdvancedShrineController> AllAdvancedShrineControllers = new List<AdvancedShrineController>();
        public static List<ResourcefulRatMinesHiddenTrapdoor> AllRatTrapdoors = new List<ResourcefulRatMinesHiddenTrapdoor>();
        public static List<Transform> AllShadowSystemDepthHavers = new List<Transform>();
        public static Dictionary<PlayerController, MineCartController> ActiveMineCarts = new Dictionary<PlayerController, MineCartController>();
        public static Dictionary<IntVector2, ElectricMushroom> MushroomMap = new Dictionary<IntVector2, ElectricMushroom>((IEqualityComparer<IntVector2>) new IntVector2EqualityComparer());
        private static List<Projectile> m_allProjectiles = new List<Projectile>();
        private static ReadOnlyCollection<Projectile> m_readOnlyProjectiles = StaticReferenceManager.m_allProjectiles.AsReadOnly();

        public static void ClearStaticPerLevelData()
        {
            StaticReferenceManager.AllForgeHammers.Clear();
            StaticReferenceManager.AllProjectileTraps.Clear();
            StaticReferenceManager.AllTriggeredTraps.Clear();
            StaticReferenceManager.AllShops.Clear();
            StaticReferenceManager.AllMajorBreakables.Clear();
            StaticReferenceManager.AllPortals.Clear();
            StaticReferenceManager.AllLocks.Clear();
            StaticReferenceManager.AllAdvancedShrineControllers.Clear();
            StaticReferenceManager.AllRatTrapdoors.Clear();
            StaticReferenceManager.AllShadowSystemDepthHavers.Clear();
            GlobalDispersalParticleManager.Clear();
            HeartDispenser.ClearPerLevelData();
            SynercacheManager.ClearPerLevelData();
            DecalObject.ClearPerLevelData();
            ExtraLifeItem.ClearPerLevelData();
            Projectile.m_cachedDungeon = (Dungeon) null;
        }

        public static void ForceClearAllStaticMemory()
        {
            StaticReferenceManager.m_allProjectiles.Clear();
            StaticReferenceManager.AllCorpses.Clear();
            StaticReferenceManager.AllDebris.Clear();
            StaticReferenceManager.AllEnemies.Clear();
            StaticReferenceManager.AllNpcs.Clear();
            StaticReferenceManager.AllForgeHammers.Clear();
            StaticReferenceManager.AllProjectileTraps.Clear();
            StaticReferenceManager.AllTriggeredTraps.Clear();
            StaticReferenceManager.AllShops.Clear();
            StaticReferenceManager.AllMajorBreakables.Clear();
            StaticReferenceManager.AllMinorBreakables.Clear();
            StaticReferenceManager.AllHealthHavers.Clear();
            StaticReferenceManager.AllGoops.Clear();
            StaticReferenceManager.AllBros.Clear();
            StaticReferenceManager.AllChests.Clear();
            StaticReferenceManager.AllLocks.Clear();
            StaticReferenceManager.ActiveMineCarts.Clear();
            StaticReferenceManager.AllGrasses.Clear();
            StaticReferenceManager.AllPortals.Clear();
            StaticReferenceManager.MushroomMap.Clear();
            StaticReferenceManager.AllBulletScriptSources.Clear();
            StaticReferenceManager.AllAdvancedShrineControllers.Clear();
            StaticReferenceManager.AllRatTrapdoors.Clear();
            StaticReferenceManager.AllShadowSystemDepthHavers.Clear();
            StaticReferenceManager.WeaponChestsSpawnedOnFloor = 0;
            StaticReferenceManager.ItemChestsSpawnedOnFloor = 0;
            StaticReferenceManager.DChestsSpawnedInTotal = 0;
            StaticReferenceManager.DChestsSpawnedOnFloor = 0;
            if (SecretRoomDoorBeer.AllSecretRoomDoors != null)
                SecretRoomDoorBeer.AllSecretRoomDoors.Clear();
            GlobalSparksDoer.CleanupOnSceneTransition();
            BaseShopController.ClearStaticMemory();
            SilencerInstance.s_MaxRadiusLimiter = new float?();
            CollisionData.Pool.Clear();
            LinearCastResult.Pool.Clear();
            RaycastResult.Pool.Clear();
            TimeTubeCreditsController.ClearPerLevelData();
            HeartDispenser.ClearPerLevelData();
            Projectile.m_cachedDungeon = (Dungeon) null;
            if (StaticReferenceManager.AllClusteredTimeInvariantBehaviours == null)
                return;
            StaticReferenceManager.AllClusteredTimeInvariantBehaviours.Clear();
        }

        public static event Action<Projectile> ProjectileAdded;

        public static event Action<Projectile> ProjectileRemoved;

        public static ReadOnlyCollection<Projectile> AllProjectiles
        {
            get => StaticReferenceManager.m_readOnlyProjectiles;
        }

        public static void AddProjectile(Projectile p)
        {
            StaticReferenceManager.m_allProjectiles.Add(p);
            if (StaticReferenceManager.ProjectileAdded == null)
                return;
            StaticReferenceManager.ProjectileAdded(p);
        }

        public static void RemoveProjectile(Projectile p)
        {
            StaticReferenceManager.m_allProjectiles.Remove(p);
            if (StaticReferenceManager.ProjectileRemoved == null)
                return;
            StaticReferenceManager.ProjectileRemoved(p);
        }

        public static void DestroyAllProjectiles()
        {
            List<Projectile> projectileList = new List<Projectile>();
            for (int index = 0; index < StaticReferenceManager.m_allProjectiles.Count; ++index)
            {
                Projectile allProjectile = StaticReferenceManager.m_allProjectiles[index];
                if ((bool) (UnityEngine.Object) allProjectile)
                    projectileList.Add(allProjectile);
            }
            for (int index = 0; index < projectileList.Count; ++index)
                projectileList[index].DieInAir(allowActorSpawns: false);
        }

        public static void DestroyAllEnemyProjectiles()
        {
            List<Projectile> projectileList = new List<Projectile>();
            for (int index = 0; index < StaticReferenceManager.m_allProjectiles.Count; ++index)
            {
                Projectile allProjectile = StaticReferenceManager.m_allProjectiles[index];
                if ((bool) (UnityEngine.Object) allProjectile && !(allProjectile.Owner is PlayerController) && (allProjectile.collidesWithPlayer || allProjectile.Owner is AIActor))
                    projectileList.Add(allProjectile);
            }
            for (int index = 0; index < projectileList.Count; ++index)
                projectileList[index].DieInAir(allowActorSpawns: false);
        }
    }

