using UniRx;

namespace EcsRx
{
    public interface IEventSystem
    {
        void Publish<T>(T message);
        IObservable<T> OnEvent<T>();
    }
}