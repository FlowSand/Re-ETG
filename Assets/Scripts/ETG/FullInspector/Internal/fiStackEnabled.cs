#nullable disable
namespace FullInspector.Internal
{
    public class fiStackEnabled
    {
        private int _count;

        public void Push() => ++this._count;

        public void Pop()
        {
            --this._count;
            if (this._count >= 0)
                return;
            this._count = 0;
        }

        public bool Enabled => this._count > 0;
    }
}
