using System;
using System.Collections.Generic;

#nullable disable

[Serializable]
public class TalkResponse
  {
    public string response;
    public string followupModuleID;
    public List<TalkResult> resultActions;
  }

