using System.Collections.Generic;

using UnityEngine;

#nullable disable

public class BashelliskBodyController : BashelliskSegment
    {
        public GameObject shootPoint;
        private BashelliskHeadController m_head;
        private BashelliskBodyController.BodyState m_state;
        private BulletScriptSelector m_bulletScript;
        private BulletScriptSource m_bulletSource;

        public BashelliskBodyController.ShootDirection shootDirection { get; set; }

        public bool IsShooting => this.State != BashelliskBodyController.BodyState.Idle;

        public float BaseShootDirection { get; private set; }

        public bool IsBroken { get; set; }

        public void Start()
        {
            if (!(bool) (UnityEngine.Object) this.majorBreakable)
                return;
            this.majorBreakable.OnBreak += new System.Action(this.OnBreak);
        }

        public void Init(BashelliskHeadController headController)
        {
            this.m_head = headController;
            this.healthHaver = this.m_head.healthHaver;
            this.aiActor = this.m_head.aiActor;
            this.aiActor.healthHaver.bodySprites.Add(this.sprite);
        }

        public void Update() => this.UpdateState();

        protected override void OnDestroy()
        {
            this.majorBreakable.OnBreak -= new System.Action(this.OnBreak);
            base.OnDestroy();
        }

        public void Fire(BulletScriptSelector bulletScript)
        {
            if (this.IsBroken)
                return;
            this.m_bulletScript = bulletScript;
            this.State = BashelliskBodyController.BodyState.Intro;
        }

        public void UpdateShootDirection()
        {
            float z = 0.0f;
            if (this.shootDirection == BashelliskBodyController.ShootDirection.NextSegment)
                z = (this.previous.center.position - this.center.position).XY().ToAngle();
            else if (this.shootDirection == BashelliskBodyController.ShootDirection.Head)
                z = this.m_head.aiAnimator.FacingDirection;
            else if (this.shootDirection == BashelliskBodyController.ShootDirection.Average)
            {
                float prevAverage = 0.0f;
                int prevCount = 0;
                double num1 = (double) BraveMathCollege.WeightedAverage(this.m_head.aiAnimator.FacingDirection, ref prevAverage, ref prevCount);
                for (LinkedListNode<BashelliskSegment> next = this.m_head.Body.First.Next; next != null; next = next.Next)
                {
                    double num2 = (double) BraveMathCollege.WeightedAverage(((BashelliskBodyController) next.Value).BaseShootDirection, ref prevAverage, ref prevCount);
                }
                z = prevAverage;
            }
            this.shootPoint.transform.rotation = Quaternion.Euler(0.0f, 0.0f, z);
        }

        public void OnBreak()
        {
            this.IsBroken = true;
            if (this.m_state != BashelliskBodyController.BodyState.Idle)
            {
                this.m_state = BashelliskBodyController.BodyState.Idle;
                if ((bool) (UnityEngine.Object) this.m_bulletSource)
                    this.m_bulletSource.ForceStop();
            }
            this.aiAnimator.SetBaseAnim("broken");
            this.aiAnimator.EndAnimation();
        }

        public override void UpdatePosition(
            PooledLinkedList<Vector2> path,
            LinkedListNode<Vector2> pathNode,
            float totalPathDist,
            float thisNodeDist)
        {
            float num1 = this.PathDist - this.previous.PathDist;
            float num2 = this.previous.attachRadius + this.attachRadius;
            bool flag = false;
            if ((double) num1 < (double) num2)
            {
                num2 = num1;
                flag = true;
            }
            float num3 = -thisNodeDist;
            for (; pathNode.Next != null; pathNode = pathNode.Next)
            {
                float num4 = Vector2.Distance(pathNode.Next.Value, pathNode.Value);
                if ((double) num3 + (double) num4 >= (double) num2)
                {
                    float thisNodeDist1 = num2 - num3;
                    if (!flag)
                    {
                        this.transform.position = (Vector3) Vector2.Lerp(pathNode.Value, pathNode.Next.Value, thisNodeDist1 / num4) - this.center.localPosition;
                        this.specRigidbody.Reinitialize();
                    }
                    this.sprite.UpdateZDepth();
                    this.BaseShootDirection = (this.previous.center.position - this.center.position).XY().ToAngle();
                    this.PathDist = totalPathDist + num2;
                    if ((bool) (UnityEngine.Object) this.next)
                    {
                        this.next.UpdatePosition(path, pathNode, totalPathDist + num2, thisNodeDist1);
                    }
                    else
                    {
                        while (path.Last != pathNode.Next)
                            path.RemoveLast();
                    }
                    this.UpdateShootDirection();
                    break;
                }
                num3 += num4;
            }
        }

        private BashelliskBodyController.BodyState State
        {
            get => this.m_state;
            set
            {
                this.EndState(this.m_state);
                this.m_state = value;
                this.BeginState(this.m_state);
            }
        }

        private void BeginState(BashelliskBodyController.BodyState state)
        {
            switch (state)
            {
                case BashelliskBodyController.BodyState.Intro:
                    this.aiAnimator.PlayUntilCancelled("gun_out");
                    break;
                case BashelliskBodyController.BodyState.Shooting:
                    if (!(bool) (UnityEngine.Object) this.m_bulletSource)
                        this.m_bulletSource = this.shootPoint.GetOrAddComponent<BulletScriptSource>();
                    this.m_bulletSource.BulletManager = this.m_head.bulletBank;
                    this.m_bulletSource.BulletScript = this.m_bulletScript;
                    this.m_bulletSource.Initialize();
                    break;
                case BashelliskBodyController.BodyState.Outro:
                    this.aiAnimator.PlayUntilFinished("gun_in");
                    break;
            }
        }

        private void UpdateState()
        {
            if (this.State == BashelliskBodyController.BodyState.Intro)
            {
                if (this.aiAnimator.IsPlaying("gun_out"))
                    return;
                this.State = BashelliskBodyController.BodyState.Shooting;
            }
            else if (this.State == BashelliskBodyController.BodyState.Shooting)
            {
                if (!this.m_bulletSource.IsEnded)
                    return;
                this.State = BashelliskBodyController.BodyState.Outro;
            }
            else
            {
                if (this.State != BashelliskBodyController.BodyState.Outro || this.aiAnimator.IsPlaying("gun_in"))
                    return;
                this.State = BashelliskBodyController.BodyState.Idle;
            }
        }

        private void EndState(BashelliskBodyController.BodyState state)
        {
        }

        public enum ShootDirection
        {
            NextSegment,
            Head,
            Average,
        }

        private enum BodyState
        {
            Idle,
            Intro,
            Shooting,
            Outro,
        }
    }

