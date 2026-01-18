using System;
using System.Collections.Generic;

using FullInspector;
using UnityEngine;

#nullable disable

public class MirrorImageBehavior : BasicAttackBehavior
    {
        public int NumImages = 2;
        public int MaxImages = 5;
        public float MirrorHealth = 15f;
        public float SpawnDelay = 0.5f;
        public float SplitDelay = 1f;
        public float SplitDistance = 1f;
        [InspectorCategory("Visuals")]
        public string Anim;
        [InspectorCategory("Visuals")]
        public bool AnimRequiresTransparency;
        [InspectorCategory("Visuals")]
        public string MirrorDeathAnim;
        [InspectorCategory("Visuals")]
        public string[] MirroredAnims;
        private MirrorImageBehavior.State m_state;
        private Shader m_cachedShader;
        private AIActor m_enemyPrefab;
        private float m_timer;
        private float m_startAngle;
        private List<AIActor> m_actorsToSplit = new List<AIActor>();
        private List<AIActor> m_allImages = new List<AIActor>();

        public override void Start() => base.Start();

        public override void Upkeep()
        {
            base.Upkeep();
            this.DecrementTimer(ref this.m_timer);
            for (int index = this.m_allImages.Count - 1; index >= 0; --index)
            {
                if (!(bool) (UnityEngine.Object) this.m_allImages[index] || !(bool) (UnityEngine.Object) this.m_allImages[index].healthHaver || this.m_allImages[index].healthHaver.IsDead)
                    this.m_allImages.RemoveAt(index);
            }
        }

        public override BehaviorResult Update()
        {
            BehaviorResult behaviorResult = base.Update();
            if (behaviorResult != BehaviorResult.Continue)
                return behaviorResult;
            if (!this.IsReady())
                return BehaviorResult.Continue;
            this.m_enemyPrefab = EnemyDatabase.GetOrLoadByGuid(this.m_aiActor.EnemyGuid);
            this.m_aiAnimator.PlayUntilFinished(this.Anim, true);
            if (this.AnimRequiresTransparency)
            {
                this.m_cachedShader = this.m_aiActor.renderer.material.shader;
                this.m_aiActor.sprite.usesOverrideMaterial = true;
                this.m_aiActor.SetOutlines(false);
                this.m_aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
            }
            this.m_aiActor.ClearPath();
            this.m_timer = this.SpawnDelay;
            if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(true, nameof (MirrorImageBehavior));
            this.m_aiActor.IsGone = true;
            this.m_aiActor.specRigidbody.CollideWithOthers = false;
            this.m_actorsToSplit.Clear();
            this.m_actorsToSplit.Add(this.m_aiActor);
            this.m_state = MirrorImageBehavior.State.Summoning;
            this.m_updateEveryFrame = true;
            return BehaviorResult.RunContinuous;
        }

        public override ContinuousBehaviorResult ContinuousUpdate()
        {
            if (this.m_state == MirrorImageBehavior.State.Summoning)
            {
                if ((double) this.m_timer <= 0.0)
                {
                    int num = Mathf.Min(this.NumImages, this.MaxImages - this.m_allImages.Count);
                    for (int index1 = 0; index1 < num; ++index1)
                    {
                        AIActor aiActor = AIActor.Spawn(this.m_enemyPrefab, this.m_aiActor.specRigidbody.UnitBottomLeft, this.m_aiActor.ParentRoom, awakenAnimType: AIActor.AwakenAnimationType.Spawn);
                        aiActor.transform.position = this.m_aiActor.transform.position;
                        aiActor.specRigidbody.Reinitialize();
                        aiActor.IsGone = true;
                        aiActor.specRigidbody.CollideWithOthers = false;
                        if (!string.IsNullOrEmpty(this.MirrorDeathAnim))
                            aiActor.aiAnimator.OtherAnimations.Find((Predicate<AIAnimator.NamedDirectionalAnimation>) (a => a.name == "death")).anim.Prefix = this.MirrorDeathAnim;
                        aiActor.PreventBlackPhantom = true;
                        if (aiActor.IsBlackPhantom)
                            aiActor.UnbecomeBlackPhantom();
                        this.m_actorsToSplit.Add(aiActor);
                        this.m_allImages.Add(aiActor);
                        aiActor.aiAnimator.healthHaver.SetHealthMaximum(this.MirrorHealth * AIActor.BaseLevelHealthModifier);
                        MirrorImageController mirrorImageController = aiActor.gameObject.AddComponent<MirrorImageController>();
                        mirrorImageController.SetHost(this.m_aiActor);
                        for (int index2 = 0; index2 < this.MirroredAnims.Length; ++index2)
                            mirrorImageController.MirrorAnimations.Add(this.MirroredAnims[index2]);
                        if (this.AnimRequiresTransparency)
                        {
                            aiActor.sprite.usesOverrideMaterial = true;
                            aiActor.procedurallyOutlined = false;
                            aiActor.SetOutlines(false);
                            aiActor.renderer.material.shader = ShaderCache.Acquire("Brave/LitBlendUber");
                        }
                    }
                    this.m_startAngle = UnityEngine.Random.Range(0.0f, 360f);
                    this.m_state = MirrorImageBehavior.State.Splitting;
                    this.m_timer = this.SplitDelay;
                    return ContinuousBehaviorResult.Continue;
                }
            }
            else if (this.m_state == MirrorImageBehavior.State.Splitting)
            {
                float num = 360f / (float) this.m_actorsToSplit.Count;
                for (int index = 0; index < this.m_actorsToSplit.Count; ++index)
                {
                    this.m_actorsToSplit[index].BehaviorOverridesVelocity = true;
                    this.m_actorsToSplit[index].BehaviorVelocity = BraveMathCollege.DegreesToVector(this.m_startAngle + num * (float) index, this.SplitDistance / this.SplitDelay);
                }
                if ((double) this.m_timer <= 0.0)
                    return ContinuousBehaviorResult.Finished;
            }
            return ContinuousBehaviorResult.Continue;
        }

        public override void EndContinuousUpdate()
        {
            base.EndContinuousUpdate();
            if (this.AnimRequiresTransparency && (bool) (UnityEngine.Object) this.m_cachedShader)
            {
                for (int index = 0; index < this.m_actorsToSplit.Count; ++index)
                {
                    AIActor aiActor = this.m_actorsToSplit[index];
                    if ((bool) (UnityEngine.Object) aiActor)
                    {
                        aiActor.sprite.usesOverrideMaterial = false;
                        aiActor.procedurallyOutlined = true;
                        aiActor.SetOutlines(true);
                        aiActor.renderer.material.shader = this.m_cachedShader;
                    }
                }
                this.m_cachedShader = (Shader) null;
            }
            if (!string.IsNullOrEmpty(this.Anim))
                this.m_aiAnimator.EndAnimationIf(this.Anim);
            if ((bool) (UnityEngine.Object) this.m_aiActor && (bool) (UnityEngine.Object) this.m_aiActor.knockbackDoer)
                this.m_aiActor.knockbackDoer.SetImmobile(false, nameof (MirrorImageBehavior));
            for (int index = 0; index < this.m_actorsToSplit.Count; ++index)
            {
                AIActor aiActor = this.m_actorsToSplit[index];
                if ((bool) (UnityEngine.Object) aiActor)
                {
                    aiActor.BehaviorOverridesVelocity = false;
                    aiActor.IsGone = false;
                    aiActor.specRigidbody.CollideWithOthers = true;
                    if ((UnityEngine.Object) aiActor != (UnityEngine.Object) this.m_aiActor)
                    {
                        aiActor.PreventBlackPhantom = false;
                        if (this.m_aiActor.IsBlackPhantom)
                            aiActor.BecomeBlackPhantom();
                    }
                }
            }
            this.m_actorsToSplit.Clear();
            this.m_state = MirrorImageBehavior.State.Idle;
            this.m_updateEveryFrame = false;
            this.UpdateCooldowns();
        }

        public override bool IsReady()
        {
            return (this.MaxImages <= 0 || this.m_allImages.Count < this.MaxImages) && base.IsReady();
        }

        private enum State
        {
            Idle,
            Summoning,
            Splitting,
        }
    }

