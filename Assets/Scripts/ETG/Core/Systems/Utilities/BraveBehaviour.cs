using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class BraveBehaviour : MonoBehaviour
    {
        private BraveCache m_cachedCache;
        private static List<BraveBehaviour> s_braveBehaviours = new List<BraveBehaviour>();

        public void RegenerateCache()
        {
            this.GetComponents<BraveBehaviour>(BraveBehaviour.s_braveBehaviours);
            this.m_cachedCache = new BraveCache();
            this.m_cachedCache.name = this.gameObject.name;
            for (int index = 0; index < BraveBehaviour.s_braveBehaviours.Count; ++index)
                BraveBehaviour.s_braveBehaviours[index].m_cachedCache = this.m_cachedCache;
            BraveBehaviour.s_braveBehaviours.Clear();
        }

        protected virtual void OnDestroy() => this.m_cachedCache = (BraveCache) null;

        public new Transform transform
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasTransform)
                {
                    cache.transform = this.GetComponent<Transform>();
                    cache.hasTransform = true;
                }
                return cache.transform;
            }
        }

        public Renderer renderer
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasRenderer)
                {
                    cache.renderer = this.GetComponent<Renderer>();
                    cache.hasRenderer = true;
                }
                return cache.renderer;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.renderer = value;
                cache.hasRenderer = true;
            }
        }

        public Animation unityAnimation
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasUnityAnimation)
                {
                    cache.unityAnimation = this.GetComponent<Animation>();
                    cache.hasUnityAnimation = true;
                }
                return cache.unityAnimation;
            }
        }

        public ParticleSystem particleSystem
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasParticleSystem)
                {
                    cache.particleSystem = this.GetComponent<ParticleSystem>();
                    cache.hasParticleSystem = true;
                }
                return cache.particleSystem;
            }
        }

        public DungeonPlaceableBehaviour dungeonPlaceable
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasDungeonPlaceable)
                {
                    cache.dungeonPlaceable = this.GetComponent<DungeonPlaceableBehaviour>();
                    cache.hasDungeonPlaceable = true;
                }
                return cache.dungeonPlaceable;
            }
        }

        public AIActor aiActor
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasAiActor)
                {
                    cache.aiActor = this.GetComponent<AIActor>();
                    cache.hasAiActor = true;
                }
                return cache.aiActor;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.aiActor = value;
                cache.hasAiActor = true;
            }
        }

        public AIShooter aiShooter
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasAiShooter)
                {
                    cache.aiShooter = this.GetComponent<AIShooter>();
                    cache.hasAiShooter = true;
                }
                return cache.aiShooter;
            }
        }

        public AIBulletBank bulletBank
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasBulletBank)
                {
                    cache.bulletBank = this.GetComponent<AIBulletBank>();
                    cache.hasBulletBank = true;
                }
                return cache.bulletBank;
            }
        }

        public HealthHaver healthHaver
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasHealthHaver)
                {
                    cache.healthHaver = this.GetComponent<HealthHaver>();
                    cache.hasHealthHaver = true;
                }
                return cache.healthHaver;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.healthHaver = value;
                cache.hasHealthHaver = true;
            }
        }

        public KnockbackDoer knockbackDoer
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasKnockbackDoer)
                {
                    cache.knockbackDoer = this.GetComponent<KnockbackDoer>();
                    cache.hasKnockbackDoer = true;
                }
                return cache.knockbackDoer;
            }
        }

        public HitEffectHandler hitEffectHandler
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasHitEffectHandler)
                {
                    cache.hitEffectHandler = this.GetComponent<HitEffectHandler>();
                    cache.hasHitEffectHandler = true;
                }
                return cache.hitEffectHandler;
            }
        }

        public AIAnimator aiAnimator
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasAiAnimator)
                {
                    cache.aiAnimator = this.GetComponent<AIAnimator>();
                    cache.hasAiAnimator = true;
                }
                return cache.aiAnimator;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.aiAnimator = value;
                cache.hasAiAnimator = true;
            }
        }

        public BehaviorSpeculator behaviorSpeculator
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasBehaviorSpeculator)
                {
                    cache.behaviorSpeculator = this.GetComponent<BehaviorSpeculator>();
                    cache.hasBehaviorSpeculator = true;
                }
                return cache.behaviorSpeculator;
            }
        }

        public GameActor gameActor
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasGameActor)
                {
                    cache.gameActor = this.GetComponent<GameActor>();
                    cache.hasGameActor = true;
                }
                return cache.gameActor;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.gameActor = value;
                cache.hasGameActor = true;
            }
        }

        public MinorBreakable minorBreakable
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasMinorBreakable)
                {
                    cache.minorBreakable = this.GetComponent<MinorBreakable>();
                    cache.hasMinorBreakable = true;
                }
                return cache.minorBreakable;
            }
        }

        public MajorBreakable majorBreakable
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasMajorBreakable)
                {
                    cache.majorBreakable = this.GetComponent<MajorBreakable>();
                    cache.hasMajorBreakable = true;
                }
                return cache.majorBreakable;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.majorBreakable = value;
                cache.hasMajorBreakable = true;
            }
        }

        public Projectile projectile
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasProjectile)
                {
                    cache.projectile = this.GetComponent<Projectile>();
                    cache.hasProjectile = true;
                }
                return cache.projectile;
            }
        }

        public ObjectVisibilityManager visibilityManager
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasVisibilityManager)
                {
                    cache.visibilityManager = this.GetComponent<ObjectVisibilityManager>();
                    cache.hasVisibilityManager = true;
                }
                return cache.visibilityManager;
            }
        }

        public TalkDoerLite talkDoer
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasTalkDoer)
                {
                    cache.talkDoer = this.GetComponent<TalkDoerLite>();
                    cache.hasTalkDoer = true;
                }
                return cache.talkDoer;
            }
        }

        public UltraFortunesFavor ultraFortunesFavor
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasUltraFortunesFavor)
                {
                    cache.ultraFortunesFavor = this.GetComponent<UltraFortunesFavor>();
                    cache.hasUltraFortunesFavor = true;
                }
                return cache.ultraFortunesFavor;
            }
        }

        public DebrisObject debris
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasDebris)
                {
                    cache.debris = this.GetComponent<DebrisObject>();
                    cache.hasDebris = true;
                }
                return cache.debris;
            }
        }

        public EncounterTrackable encounterTrackable
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasEncounterTrackable)
                {
                    cache.encounterTrackable = this.GetComponent<EncounterTrackable>();
                    cache.hasEncounterTrackable = true;
                }
                return cache.encounterTrackable;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.encounterTrackable = value;
                cache.hasEncounterTrackable = true;
            }
        }

        public SpeculativeRigidbody specRigidbody
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasSpecRigidbody)
                {
                    cache.specRigidbody = this.GetComponent<SpeculativeRigidbody>();
                    cache.hasSpecRigidbody = true;
                }
                return cache.specRigidbody;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.specRigidbody = value;
                cache.hasSpecRigidbody = true;
            }
        }

        public tk2dBaseSprite sprite
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasSprite)
                {
                    cache.sprite = this.GetComponent<tk2dBaseSprite>();
                    cache.hasSprite = true;
                }
                return cache.sprite;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.sprite = value;
                cache.hasSprite = true;
            }
        }

        public tk2dSpriteAnimator spriteAnimator
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasSpriteAnimator)
                {
                    cache.spriteAnimator = this.GetComponent<tk2dSpriteAnimator>();
                    cache.hasSpriteAnimator = true;
                }
                return cache.spriteAnimator;
            }
            set
            {
                BraveCache cache = this.GetCache();
                cache.spriteAnimator = value;
                cache.hasSpriteAnimator = true;
            }
        }

        public PlayMakerFSM playmakerFsm
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasPlaymakerFsm)
                {
                    cache.playmakerFsm = this.GetComponent<PlayMakerFSM>();
                    cache.hasPlaymakerFsm = true;
                }
                return cache.playmakerFsm;
            }
        }

        public PlayMakerFSM[] playmakerFsms
        {
            get
            {
                BraveCache cache = this.GetCache();
                if (!cache.hasPlaymakerFsms)
                {
                    cache.playmakerFsms = this.GetComponents<PlayMakerFSM>();
                    cache.hasPlaymakerFsms = true;
                }
                return cache.playmakerFsms;
            }
        }

        public void SendPlaymakerEvent(string eventName)
        {
            PlayMakerFSM[] playmakerFsms = this.playmakerFsms;
            for (int index = 0; index < playmakerFsms.Length; ++index)
            {
                if (playmakerFsms[index].enabled)
                    playmakerFsms[index].SendEvent(eventName);
            }
        }

        public PlayMakerFSM GetDungeonFSM()
        {
            PlayMakerFSM[] playmakerFsms = this.playmakerFsms;
            for (int index = 0; index < playmakerFsms.Length; ++index)
            {
                if (playmakerFsms[index].FsmName.Contains("Dungeon"))
                    return playmakerFsms[index];
            }
            return this.playmakerFsm;
        }

        public PlayMakerFSM GetFoyerFSM()
        {
            PlayMakerFSM[] playmakerFsms = this.playmakerFsms;
            for (int index = 0; index < playmakerFsms.Length; ++index)
            {
                if (playmakerFsms[index].FsmName.Contains("Foyer"))
                    return playmakerFsms[index];
            }
            return this.playmakerFsm;
        }

        private BraveCache GetCache()
        {
            if (this.m_cachedCache == null)
            {
                if (BraveBehaviour.s_braveBehaviours == null)
                    BraveBehaviour.s_braveBehaviours = new List<BraveBehaviour>();
                BraveBehaviour.s_braveBehaviours.Clear();
                this.GetComponents<BraveBehaviour>(BraveBehaviour.s_braveBehaviours);
                for (int index = 0; index < BraveBehaviour.s_braveBehaviours.Count; ++index)
                {
                    if (BraveBehaviour.s_braveBehaviours[index].m_cachedCache != null)
                        this.m_cachedCache = BraveBehaviour.s_braveBehaviours[index].m_cachedCache;
                }
                if (this.m_cachedCache == null)
                {
                    this.m_cachedCache = new BraveCache();
                    this.m_cachedCache.name = this.gameObject.name;
                    for (int index = 0; index < BraveBehaviour.s_braveBehaviours.Count; ++index)
                        BraveBehaviour.s_braveBehaviours[index].m_cachedCache = this.m_cachedCache;
                }
                BraveBehaviour.s_braveBehaviours.Clear();
            }
            return this.m_cachedCache;
        }
    }

