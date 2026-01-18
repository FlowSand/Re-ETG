using UnityEngine;

#nullable disable

public class NormalDistributionDecoratorAttribute : PropertyAttribute
    {
        public string MeanProperty;
        public string StdDevProperty;

        public NormalDistributionDecoratorAttribute(string meanPropertyName, string devPropertyName)
        {
            this.MeanProperty = meanPropertyName;
            this.StdDevProperty = devPropertyName;
        }
    }

