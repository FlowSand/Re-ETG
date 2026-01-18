using System;

using UnityEngine;

#nullable disable

public class ArtfulDodgerBumperController : DungeonPlaceableBehaviour
    {
        [Header("Bumper Data")]
        public tk2dBaseSprite mySprite;
        public bool StopsGameProjectileBounces;
        public bool DestroyBumperOnGameCollision;
        public ArtfulDodgerBumperController.DiagonalDirection diagonalDirection;
        public VFXPool BumperPopVFX;
        public string hitAnimation = string.Empty;
        [ShowInInspectorIf("DestroyBumperOnGameCollision", false)]
        public string breakAnimation = string.Empty;
        public string idleAnimation = string.Empty;
        private bool m_canDestroy;

        private void Start()
        {
            this.mySprite.spriteAnimator.AnimationCompleted += new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
            this.specRigidbody.OnRigidbodyCollision += new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
            if (this.diagonalDirection == ArtfulDodgerBumperController.DiagonalDirection.None)
                return;
            this.specRigidbody.ReflectProjectilesNormalGenerator += new Func<Vector2, Vector2, Vector2>(this.ReflectNormalGenerator);
        }

        protected override void OnDestroy()
        {
            if ((bool) (UnityEngine.Object) this.gameObject)
            {
                this.mySprite.spriteAnimator.AnimationCompleted -= new Action<tk2dSpriteAnimator, tk2dSpriteAnimationClip>(this.AnimationCompleted);
                this.specRigidbody.OnRigidbodyCollision -= new SpeculativeRigidbody.OnRigidbodyCollisionDelegate(this.HandleRigidbodyCollision);
                if (this.diagonalDirection != ArtfulDodgerBumperController.DiagonalDirection.None)
                    this.specRigidbody.ReflectProjectilesNormalGenerator -= new Func<Vector2, Vector2, Vector2>(this.ReflectNormalGenerator);
            }
            base.OnDestroy();
        }

        private void AnimationCompleted(tk2dSpriteAnimator animator, tk2dSpriteAnimationClip clip)
        {
            if (this.DestroyBumperOnGameCollision && clip.name == this.breakAnimation && this.m_canDestroy)
            {
                this.BumperPopVFX.SpawnAtPosition(this.gameObject.transform.position, heightOffGround: new float?(1f));
                UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);
            }
            else
            {
                if (!(clip.name == this.hitAnimation))
                    return;
                this.mySprite.spriteAnimator.Play(this.idleAnimation);
            }
        }

        private void HandleRigidbodyCollision(CollisionData rigidbodyCollision)
        {
            if (!((UnityEngine.Object) rigidbodyCollision.OtherRigidbody.projectile != (UnityEngine.Object) null))
                return;
            Projectile projectile = rigidbodyCollision.OtherRigidbody.projectile;
            this.m_canDestroy = projectile.name.StartsWith("ArtfulDodger");
            this.mySprite.spriteAnimator.Play(!this.m_canDestroy || !this.DestroyBumperOnGameCollision ? this.hitAnimation : this.breakAnimation);
            if (!this.StopsGameProjectileBounces)
                return;
            projectile.DieInAir();
        }

        private Vector2 ReflectNormalGenerator(Vector2 contact, Vector2 normal)
        {
            switch (this.diagonalDirection)
            {
                case ArtfulDodgerBumperController.DiagonalDirection.NorthEast:
                    if ((double) normal.x > 0.5 || (double) normal.y > 0.5)
                        return new Vector2(1f, 1f).normalized;
                    break;
                case ArtfulDodgerBumperController.DiagonalDirection.SouthEast:
                    if ((double) normal.x > 0.5 || (double) normal.y < -0.5)
                        return new Vector2(1f, -1f).normalized;
                    break;
                case ArtfulDodgerBumperController.DiagonalDirection.SouthWest:
                    if ((double) normal.x < -0.5 || (double) normal.y < -0.5)
                        return new Vector2(-1f, -1f).normalized;
                    break;
                case ArtfulDodgerBumperController.DiagonalDirection.NorthWest:
                    if ((double) normal.x < -0.5 || (double) normal.y > 0.5)
                        return new Vector2(-1f, 1f).normalized;
                    break;
            }
            return normal;
        }

        public enum DiagonalDirection
        {
            None,
            NorthEast,
            SouthEast,
            SouthWest,
            NorthWest,
        }
    }

