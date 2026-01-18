using UnityEngine;

#nullable disable

public class RemoteMineItem : PlayerItem
    {
        public GameObject objectToSpawn;
        public string detonatorSprite = "c4_transmitter_001";
        protected RemoteMineController m_extantEffect;
        protected int m_originalSprite;

        protected override void DoEffect(PlayerController user)
        {
            int num = (int) AkSoundEngine.PostEvent("Play_OBJ_mine_set_01", this.gameObject);
            GameObject gameObject = Object.Instantiate<GameObject>(this.objectToSpawn, (Vector3) user.specRigidbody.UnitCenter, Quaternion.identity);
            this.m_originalSprite = this.sprite.spriteId;
            this.sprite.SetSprite(this.detonatorSprite);
            tk2dBaseSprite component = gameObject.GetComponent<tk2dBaseSprite>();
            this.m_extantEffect = gameObject.GetComponent<RemoteMineController>();
            if ((Object) component != (Object) null)
                component.PlaceAtPositionByAnchor(user.specRigidbody.UnitCenter.ToVector3ZUp(component.transform.position.z), tk2dBaseSprite.Anchor.MiddleCenter);
            this.m_isCurrentlyActive = true;
        }

        public override void Update()
        {
            if ((Object) this.m_extantEffect == (Object) null)
            {
                base.Update();
            }
            else
            {
                if (!TimeTubeCreditsController.IsTimeTubing)
                    return;
                Object.Destroy((Object) this.m_extantEffect.gameObject);
                this.m_extantEffect = (RemoteMineController) null;
            }
        }

        protected override void OnPreDrop(PlayerController user)
        {
            if (this.m_isCurrentlyActive)
                this.DoActiveEffect(user);
            base.OnPreDrop(user);
        }

        protected override void DoActiveEffect(PlayerController user)
        {
            if ((Object) this.m_extantEffect != (Object) null)
            {
                this.m_extantEffect.Detonate();
                this.m_extantEffect = (RemoteMineController) null;
            }
            this.sprite.SetSprite(this.m_originalSprite);
            this.m_isCurrentlyActive = false;
        }

        protected override void OnDestroy() => base.OnDestroy();
    }

