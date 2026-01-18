#nullable disable

public class WeightedItem<T>
  {
    public T value;
    public float weight;

    public WeightedItem(T v, float w)
    {
      this.value = v;
      this.weight = w;
    }
  }

