using UnityEngine;

#nullable disable

public class RewardManagerResetAttribute : PropertyAttribute
  {
    public string header;
    public string content;
    public string callback;
    public int targetElement;

    public RewardManagerResetAttribute(
      string headerMessage,
      string contentMessage,
      string callbackFunc,
      int targetType)
    {
      this.header = headerMessage;
      this.content = contentMessage;
      this.callback = callbackFunc;
      this.targetElement = targetType;
    }
  }

