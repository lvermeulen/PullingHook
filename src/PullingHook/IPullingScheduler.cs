namespace PullingHook
{
    public interface IPullingScheduler<T>
    {
        void Start(IPullingHookManager<T> pullingHookManager);
        void Stop();
    }
}