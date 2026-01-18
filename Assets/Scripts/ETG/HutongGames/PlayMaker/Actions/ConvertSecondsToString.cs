using System;

#nullable disable
namespace HutongGames.PlayMaker.Actions
{
  [HelpUrl("http://hutonggames.com/playmakerforum/index.php?topic=1711.0")]
  [Tooltip("Converts Seconds to a String value representing the time.")]
  [ActionCategory(ActionCategory.Convert)]
  public class ConvertSecondsToString : FsmStateAction
  {
    [Tooltip("The seconds variable to convert.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmFloat secondsVariable;
    [Tooltip("A string variable to store the time value.")]
    [UIHint(UIHint.Variable)]
    [RequiredField]
    public FsmString stringVariable;
    [Tooltip("Format. 0 for days, 1 is for hours, 2 for minutes, 3 for seconds and 4 for milliseconds. 5 for total days, 6 for total hours, 7 for total minutes, 8 for total seconds, 9 for total milliseconds, 10 for two digits milliseconds. so {2:D2} would just show the seconds of the current time, NOT the grand total number of seconds, the grand total of seconds would be {8:F0}")]
    [RequiredField]
    public FsmString format;
    [Tooltip("Repeat every frame. Useful if the seconds variable is changing.")]
    public bool everyFrame;

    public override void Reset()
    {
      this.secondsVariable = (FsmFloat) null;
      this.stringVariable = (FsmString) null;
      this.everyFrame = false;
      this.format = (FsmString) "{1:D2}h:{2:D2}m:{3:D2}s:{10}ms";
    }

    public override void OnEnter()
    {
      this.DoConvertSecondsToString();
      if (this.everyFrame)
        return;
      this.Finish();
    }

    public override void OnUpdate() => this.DoConvertSecondsToString();

    private void DoConvertSecondsToString()
    {
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) this.secondsVariable.Value);
      string str = timeSpan.Milliseconds.ToString("D3").PadLeft(2, '0').Substring(0, 2);
      this.stringVariable.Value = string.Format(this.format.Value, (object) timeSpan.Days, (object) timeSpan.Hours, (object) timeSpan.Minutes, (object) timeSpan.Seconds, (object) timeSpan.Milliseconds, (object) timeSpan.TotalDays, (object) timeSpan.TotalHours, (object) timeSpan.TotalMinutes, (object) timeSpan.TotalSeconds, (object) timeSpan.TotalMilliseconds, (object) str);
    }
  }
}
