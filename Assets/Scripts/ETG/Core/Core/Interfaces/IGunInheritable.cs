using System.Collections.Generic;

#nullable disable

public interface IGunInheritable
    {
        void InheritData(Gun sourceGun);

        void MidGameSerialize(List<object> data, int dataIndex);

        void MidGameDeserialize(List<object> data, ref int dataIndex);
    }

