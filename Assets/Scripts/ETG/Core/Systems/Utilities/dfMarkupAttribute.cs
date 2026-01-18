#nullable disable

public class dfMarkupAttribute
  {
    public dfMarkupAttribute(string name, string value)
    {
      this.Name = name;
      this.Value = value;
    }

    public string Name { get; set; }

    public string Value { get; set; }

    public override string ToString() => $"{this.Name}='{this.Value}'";
  }

