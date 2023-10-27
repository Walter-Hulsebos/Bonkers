namespace Bonkers.Controls
{
    public interface IControl<out T>
    {
        T Value { get; }
    }
}
