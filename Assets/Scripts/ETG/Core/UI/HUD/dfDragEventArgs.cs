using UnityEngine;

#nullable disable

public class dfDragEventArgs : dfControlEventArgs
    {
        internal dfDragEventArgs(dfControl source)
            : base(source)
        {
            this.State = dfDragDropState.None;
        }

        internal dfDragEventArgs(
            dfControl source,
            dfDragDropState state,
            object data,
            Ray ray,
            Vector2 position)
            : base(source)
        {
            this.Data = data;
            this.State = state;
            this.Position = position;
            this.Ray = ray;
        }

        public dfDragDropState State { get; set; }

        public object Data { get; set; }

        public Vector2 Position { get; set; }

        public dfControl Target { get; set; }

        public Ray Ray { get; set; }
    }

