namespace PullingHook
{
    public interface IPullingScheduler<T, TKeyProperty>
    {
        void Start(IPullingHookManager<T, TKeyProperty> pullingHookManager);
        void Stop();
    }
}