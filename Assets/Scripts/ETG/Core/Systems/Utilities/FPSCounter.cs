using UnityEngine;

#nullable disable

[AddComponentMenu("Daikon Forge/Examples/General/FPS Counter")]
[RequireComponent(typeof (dfLabel))]
public class FPSCounter : MonoBehaviour
  {
    public float updateInterval = 0.1f;
    private float accum;
    private int frames;
    private float timeleft;
    private dfLabel label;

    private void Start()
    {
      this.label = this.GetComponent<dfLabel>();
      if ((Object) this.label == (Object) null)
        Debug.LogError((object) "FPS Counter needs a Label component!");
      this.timeleft = this.updateInterval;
      this.label.Text = string.Empty;
    }

    private void Update()
    {
      if ((Object) this.label == (Object) null)
        return;
      this.timeleft -= BraveTime.DeltaTime;
      this.accum += UnityEngine.Time.timeScale / BraveTime.DeltaTime;
      ++this.frames;
      if ((double) this.timeleft > 0.0)
        return;
      float num = this.accum / (float) this.frames;
      this.label.Text = $"{num:F2} FPS";
      if ((double) num < 30.0)
        this.label.Color = (Color32) Color.yellow;
      else if ((double) num < 10.0)
        this.label.Color = (Color32) Color.red;
      else
        this.label.Color = (Color32) Color.green;
      this.timeleft = this.updateInterval;
      this.accum = 0.0f;
      this.frames = 0;
    }
  }

