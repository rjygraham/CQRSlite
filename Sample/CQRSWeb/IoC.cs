using CQRSCode.Domain;
using CQRSCode.ReadModel;
using CQRSlite.Bus;
using CQRSlite.Commanding;
using CQRSlite.Domain;
using CQRSlite.Eventing;
using StructureMap;
using StructureMap.Graph;

namespace CQRSWeb {
    public static class IoC {
        public static IContainer Initialize() {
            ObjectFactory.Initialize(x =>
                        {
                            x.For<InProcessBus>().Singleton().Use<InProcessBus>();
                            x.For<ICommandSender>().Use(y => y.GetInstance<InProcessBus>());
                            x.For<IEventPublisher>().Use(y => y.GetInstance<InProcessBus>());
                            x.For<ISession>().HybridHttpOrThreadLocalScoped().Use<Session>();
                            x.For<IEventStore>().Singleton().Use<EventStore>();
                            x.For<IRepository>().HybridHttpOrThreadLocalScoped().Use<Repository>();

                            x.Scan(s =>
                            {
                                s.TheCallingAssembly();
                                s.AssemblyContainingType<InProcessBus>();
                                s.AssemblyContainingType<ReadModelFacade>();
                                s.Convention<FirstInterfaceConvention>();
                            });
                        });
            return ObjectFactory.Container;
        }
    }
}