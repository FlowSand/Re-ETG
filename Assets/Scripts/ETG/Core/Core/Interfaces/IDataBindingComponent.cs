#nullable disable

public interface IDataBindingComponent
    {
        bool IsBound { get; }

        void Bind();

        void Unbind();
    }

