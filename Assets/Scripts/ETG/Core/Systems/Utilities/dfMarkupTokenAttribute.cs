#nullable disable

public class dfMarkupTokenAttribute : IPoolable
    {
        public dfMarkupToken Key;
        public dfMarkupToken Value;
        private static dfList<dfMarkupTokenAttribute> pool = new dfList<dfMarkupTokenAttribute>();

        private dfMarkupTokenAttribute()
        {
        }

        public static dfMarkupTokenAttribute Obtain(dfMarkupToken key, dfMarkupToken value)
        {
            dfMarkupTokenAttribute markupTokenAttribute = dfMarkupTokenAttribute.pool.Count <= 0 ? new dfMarkupTokenAttribute() : dfMarkupTokenAttribute.pool.Pop();
            markupTokenAttribute.Key = key;
            markupTokenAttribute.Value = value;
            return markupTokenAttribute;
        }

        public void Release()
        {
            if (this.Key != null)
            {
                this.Key.Release();
                this.Key = (dfMarkupToken) null;
            }
            if (this.Value != null)
            {
                this.Value.Release();
                this.Value = (dfMarkupToken) null;
            }
            if (dfMarkupTokenAttribute.pool.Contains(this))
                return;
            dfMarkupTokenAttribute.pool.Add(this);
        }
    }

