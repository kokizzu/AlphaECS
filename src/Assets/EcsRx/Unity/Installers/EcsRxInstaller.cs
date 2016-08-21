using UniRx;
using Zenject;
using EcsRx;

namespace EcsRx.Unity
{
    public class EcsRxInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<IMessageBroker>().To<MessageBroker>().AsSingle();
            Container.Bind<IEventSystem>().To<EventSystem>().AsSingle();
            Container.Bind<IIdentityGenerator>().To<SequentialIdentityGenerator>().AsSingle();
            Container.Bind<IPoolManager>().To<PoolManager>().AsSingle();

			Container.Bind<IGroup> ().To<Group> ();

//            Container.Bind<IReactToDataSystemHandler>().To<ReactToDataSystemHandler>();
//            Container.Bind<IReactToEntitySystemHandler>().To<ReactToEntitySystemHandler>();
//            Container.Bind<IReactToGroupSystemHandler>().To<ReactToGroupSystemHandler>();
//            Container.Bind<ISetupSystemHandler>().To<SetupSystemHandler>();
//            Container.Bind<IManualSystemHandler>().To<ManualSystemHandler>();

//            Container.Bind<ISystemExecutor>().To<SystemExecutor>().AsSingle();
        }
    }
}