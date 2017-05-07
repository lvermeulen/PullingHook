namespace PullingHook
{
    public interface IPullingConfiguration<T>
    {
        IPullingSchedule Schedule { get; }
        IPullingSource<T> Source { get; }
        IPullingSink<T> Sink { get; }
    }
}
