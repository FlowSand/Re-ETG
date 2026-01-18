using UnityEngine;

#nullable disable

public class PickupIdentifierAttribute : PropertyAttribute, DatabaseIdentifierAttribute
  {
    public System.Type PickupType;

    public PickupIdentifierAttribute() => this.PickupType = typeof (PickupObject);

    public PickupIdentifierAttribute(System.Type type) => this.PickupType = type;
  }

