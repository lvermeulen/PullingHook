namespace PullingHook
{
    public interface IPullingConfiguration<T, TKeyProperty>
    {
        IPullingSchedule Schedule { get; }
        IPullingSource<T> Source { get; }
        IPullingSink<T, TKeyProperty> Sink { get; }
    }
}
