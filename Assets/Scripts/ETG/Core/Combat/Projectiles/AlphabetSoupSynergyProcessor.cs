// Decompiled with JetBrains decompiler
// Type: AlphabetSoupSynergyProcessor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E27C5245-924B-4031-BFBB-14AA632E24E2
// Assembly location: D:\Github\Re-ETG\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

#nullable disable

namespace ETG.Core.Combat.Projectiles
{
    public class AlphabetSoupSynergyProcessor : MonoBehaviour
    {
      public AlphabetSoupEntry[] Entries;
      private Gun m_gun;
      private string m_currentEntry = "BULLET";
      private int m_currentEntryCount;
      private bool m_hasReplacedProjectileList;
      private bool m_hasPlayedAudioThisShot;
      private string m_currentAudioEvent = "Play_WPN_rgun_bullet_01";

      public void Awake()
      {
        this.m_gun = this.GetComponent<Gun>();
        this.m_gun.PostProcessVolley += new Action<ProjectileVolleyData>(this.HandlePostProcessVolley);
        this.m_gun.PostProcessProjectile += new Action<Projectile>(this.HandlePostProcessProjectile);
        this.m_gun.OnReloadPressed += new Action<PlayerController, Gun, bool>(this.HandleReloadPressed);
        this.m_gun.OnPostFired += new Action<PlayerController, Gun>(this.HandlePostFired);
        this.m_gun.OnFinishAttack += new Action<PlayerController, Gun>(this.HandleFinishAttack);
        this.m_gun.OnBurstContinued += new Action<PlayerController, Gun>(this.HandleBurstContinued);
      }

      private void HandleBurstContinued(PlayerController arg1, Gun arg2)
      {
        if ((bool) (UnityEngine.Object) this.m_gun && this.m_gun.gunClass == GunClass.EXPLOSIVE)
          return;
        this.HandleFinishAttack(arg1, arg2);
      }

      private void HandlePostFired(PlayerController arg1, Gun arg2)
      {
        if ((bool) (UnityEngine.Object) this.m_gun && this.m_gun.gunClass == GunClass.EXPLOSIVE || this.m_hasPlayedAudioThisShot)
          return;
        this.m_hasPlayedAudioThisShot = true;
        int num = (int) AkSoundEngine.PostEvent(this.m_currentAudioEvent, arg2.gameObject);
      }

      private void HandlePostProcessVolley(ProjectileVolleyData obj) => this.m_currentEntryCount = 0;

      private void HandleReloadPressed(PlayerController arg1, Gun arg2, bool arg3)
      {
        this.m_currentEntryCount = 0;
      }

      private string GetLetterForWordPosition(string word)
      {
        if (this.m_currentEntryCount < 0 || this.m_currentEntryCount >= word.Length)
          return "word_projectile_B_001";
        char ch = word[this.m_currentEntryCount];
        switch (ch)
        {
          case 'A':
            return "word_projectile_A_001";
          case 'B':
            return "word_projectile_B_001";
          case 'C':
            return "word_projectile_C_001";
          case 'D':
            return "word_projectile_D_001";
          case 'E':
            return "word_projectile_B_004";
          case 'F':
            return "word_projectile_F_001";
          case 'G':
            return "word_projectile_G_001";
          case 'H':
            return "word_projectile_H_001";
          case 'I':
            return "word_projectile_I_001";
          case 'J':
            return "word_projectile_J_001";
          case 'K':
            return "word_projectile_K_001";
          case 'L':
            return "word_projectile_B_003";
          case 'M':
            return "word_projectile_M_001";
          case 'N':
            return "word_projectile_N_001";
          case 'O':
            return "word_projectile_O_001";
          case 'P':
            return "word_projectile_P_001";
          case 'Q':
            return "word_projectile_Q_001";
          case 'R':
            return "word_projectile_R_001";
          case 'S':
            return "word_projectile_S_001";
          case 'T':
            return "word_projectile_B_005";
          case 'U':
            return "word_projectile_B_002";
          case 'V':
            return "word_projectile_V_001";
          case 'W':
            return "word_projectile_W_001";
          case 'X':
            return "word_projectile_X_001";
          case 'Y':
            return "word_projectile_Y_001";
          case 'Z':
            return "word_projectile_Z_001";
          case 'a':
            return "word_projectile_alpha_001";
          case 'o':
            return "word_projectile_omega_001";
          default:
            if (ch == '+')
              return "word_projectile_+_001";
            return ch == '1' ? "word_projectile_1_001" : "word_projectile_B_001";
        }
      }

      private void HandlePostProcessProjectile(Projectile targetProjectile)
      {
        if (!(bool) (UnityEngine.Object) targetProjectile || !(bool) (UnityEngine.Object) targetProjectile.sprite || (bool) (UnityEngine.Object) this.m_gun && this.m_gun.gunClass == GunClass.EXPLOSIVE)
          return;
        targetProjectile.sprite.SetSprite(this.GetLetterForWordPosition(this.m_currentEntry));
        ++this.m_currentEntryCount;
      }

      private void HandleFinishAttack(PlayerController sourcePlayer, Gun sourceGun)
      {
        if ((bool) (UnityEngine.Object) this.m_gun && this.m_gun.gunClass == GunClass.EXPLOSIVE)
          return;
        this.m_hasPlayedAudioThisShot = false;
        int num = UnityEngine.Random.Range(0, this.Entries.Length);
        AlphabetSoupEntry entry1 = (AlphabetSoupEntry) null;
        for (int index = num; index < num + this.Entries.Length; ++index)
        {
          AlphabetSoupEntry entry2 = this.Entries[index % this.Entries.Length];
          if (sourcePlayer.HasActiveBonusSynergy(entry2.RequiredSynergy))
          {
            entry1 = entry2;
            break;
          }
        }
        if (entry1 != null)
        {
          this.ProcessVolley(this.m_gun.modifiedVolley, entry1);
        }
        else
        {
          this.m_currentEntryCount = 0;
          this.m_currentEntry = "BULLET";
          this.m_currentAudioEvent = "Play_WPN_rgun_bullet_01";
        }
      }

      private void ProcessVolley(ProjectileVolleyData currentVolley, AlphabetSoupEntry entry)
      {
        if ((bool) (UnityEngine.Object) this.m_gun && this.m_gun.gunClass == GunClass.EXPLOSIVE)
          return;
        ProjectileModule projectile = currentVolley.projectiles[0];
        projectile.ClearOrderedProjectileData();
        if (!this.m_hasReplacedProjectileList)
        {
          this.m_hasReplacedProjectileList = true;
          projectile.projectiles = new List<Projectile>();
        }
        projectile.projectiles.Clear();
        int index1 = UnityEngine.Random.Range(0, entry.Words.Length);
        this.m_currentEntry = entry.Words[index1];
        this.m_currentAudioEvent = entry.AudioEvents[index1];
        projectile.burstShotCount = this.m_currentEntry.Length;
        for (int index2 = 0; index2 < this.m_currentEntry.Length; ++index2)
          projectile.projectiles.Add(entry.BaseProjectile);
        this.m_currentEntryCount = 0;
      }
    }

}
