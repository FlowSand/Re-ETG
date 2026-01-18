#nullable disable
namespace FullInspector.LayoutToolkit
{
  public static class fiLayoutUtility
  {
    public static fiLayout Margin(float margin, fiLayout layout)
    {
      return (fiLayout) new fiHorizontalLayout()
      {
        margin,
        (fiLayout) new fiVerticalLayout()
        {
          margin,
          layout,
          margin
        },
        margin
      };
    }
  }
}
