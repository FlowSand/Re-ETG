using System;
using UnityEngine;

#nullable disable

public class DodgeRollSynergyProcessor : MonoBehaviour
  {
    public bool LeavesGoopTrail;
    public CustomSynergyType GoopTrailRequiredSynergy;
    public GoopDefinition GoopTrailGoop;
    public float GoopTrailRadius;
    private PassiveItem m_item;
    private PlayerController m_player;

    private void Awake()
    {
      this.m_item = this.GetComponent<PassiveItem>();
      this.m_item.OnPickedUp += new Action<PlayerController>(this.HandlePickedUp);
    }

    private void HandlePickedUp(PlayerController obj)
    {
      this.m_player = obj;
      this.m_player.OnIsRolling += new Action<PlayerController>(this.HandleRollFrame);
    }

    private void HandleRollFrame(PlayerController sourcePlayer)
    {
      if (!this.LeavesGoopTrail || !(bool) (UnityEngine.Object) this.m_player || !this.m_player.HasActiveBonusSynergy(this.GoopTrailRequiredSynergy))
        return;
      if (!(bool) (UnityEngine.Object) this.m_item || (UnityEngine.Object) this.m_item.Owner != (UnityEngine.Object) this.m_player)
        this.m_player.OnIsRolling -= new Action<PlayerController>(this.HandleRollFrame);
      else
        DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(this.GoopTrailGoop).AddGoopCircle(this.m_player.specRigidbody.UnitCenter, this.GoopTrailRadius);
    }
  }

