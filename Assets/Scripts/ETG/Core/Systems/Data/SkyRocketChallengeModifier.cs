using UnityEngine;

#nullable disable

public class SkyRocketChallengeModifier : ChallengeModifier
  {
    public GameObject Rocket;
    public float TimeBetweenRockets = 3f;
    private float m_elapsedSinceRocket;
    private int m_spawnedRockets;

    private void Update()
    {
      this.m_elapsedSinceRocket += BraveTime.DeltaTime;
      if ((double) this.m_elapsedSinceRocket > (double) this.TimeBetweenRockets)
      {
        this.m_elapsedSinceRocket = 0.0f;
        this.FireRocket();
      }
      if (this.m_spawnedRockets <= 0 || !BossKillCam.BossDeathCamRunning && !GameManager.Instance.PreventPausing)
        return;
      this.Cleanup();
    }

    private void OnDestroy() => this.Cleanup();

    private void FireRocket()
    {
      if (BossKillCam.BossDeathCamRunning || GameManager.Instance.PreventPausing)
        return;
      PlayerController randomActivePlayer = GameManager.Instance.GetRandomActivePlayer();
      SkyRocket component = SpawnManager.SpawnProjectile(this.Rocket, Vector3.zero, Quaternion.identity).GetComponent<SkyRocket>();
      component.Target = randomActivePlayer.specRigidbody;
      tk2dSprite componentInChildren = component.GetComponentInChildren<tk2dSprite>();
      component.transform.position = component.transform.position.WithY(component.transform.position.y - componentInChildren.transform.localPosition.y);
      ++this.m_spawnedRockets;
    }

    public void Cleanup()
    {
      this.m_spawnedRockets = 0;
      SkyRocket[] objectsOfType = Object.FindObjectsOfType<SkyRocket>();
      for (int index = 0; index < objectsOfType.Length; ++index)
      {
        if ((bool) (Object) objectsOfType[index])
          objectsOfType[index].DieInAir();
      }
    }
  }

