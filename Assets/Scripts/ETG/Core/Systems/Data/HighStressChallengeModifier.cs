using System;

#nullable disable

public class HighStressChallengeModifier : ChallengeModifier
    {
        public float StressDuration = 5f;

        private void Start()
        {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                GameManager.Instance.AllPlayers[index].OnReceivedDamage += new Action<PlayerController>(this.OnPlayerReceivedDamage);
        }

        private void OnPlayerReceivedDamage(PlayerController p)
        {
            if (!(bool) (UnityEngine.Object) p || !(bool) (UnityEngine.Object) p.healthHaver)
                return;
            p.TriggerHighStress(this.StressDuration);
        }

        private void OnDestroy()
        {
            for (int index = 0; index < GameManager.Instance.AllPlayers.Length; ++index)
                GameManager.Instance.AllPlayers[index].OnReceivedDamage -= new Action<PlayerController>(this.OnPlayerReceivedDamage);
        }
    }

