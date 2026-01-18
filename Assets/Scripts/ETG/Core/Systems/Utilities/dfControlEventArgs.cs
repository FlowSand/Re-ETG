#nullable disable

public class dfControlEventArgs
    {
        internal dfControlEventArgs(dfControl Target) => this.Source = Target;

        public dfControl Source { get; internal set; }

        public bool Used { get; private set; }

        public void Use() => this.Used = true;
    }

