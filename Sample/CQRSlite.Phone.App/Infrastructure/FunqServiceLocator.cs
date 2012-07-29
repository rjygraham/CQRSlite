using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CQRSlite.Config;
using Funq;
using CQRSlite.Bus;
using CQRSlite.Commanding;
using CQRSlite.Eventing;
using CQRSCode.Domain;
using CQRSlite.Domain;
using CQRSCode.ReadModel;
using System.Reflection;
using CQRSCode.CommandHandlers;
using CQRSCode.ReadModel.Handlers;
using CQRSlite.Phone.App.ViewModels;

namespace CQRSlite.Phone.App.Infrastructure
{
    public class FunqServiceLocator : IServiceLocator
    {

        private readonly Container _container;

        public FunqServiceLocator()
        {
            _container = new Container();
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            var bus = new InProcessBus(this);
            _container.Register<ICommandSender>(bus);
            _container.Register<IEventPublisher>(bus);
            _container.Register<IEventStore>(c => new EventStore());
            _container.Register<IRepository>(c => new Repository(c.Resolve<IEventStore>(), c.Resolve<IEventPublisher>()));
            _container.Register<ISession>(c => new Session(c.Resolve<IRepository>()));

            _container.Register<IReadModelFacade>(c => new ReadModelFacade());
            _container.Register<InventoryCommandHandlers>(c => new InventoryCommandHandlers(c.Resolve<ISession>()));
            _container.Register<InvenotryItemDetailView>(c => new InvenotryItemDetailView());
            _container.Register<InventoryListView>(c => new InventoryListView());

            _container.Register<MainPageViewModel>(c => new MainPageViewModel(c.Resolve<IReadModelFacade>()));
            _container.Register<AddPageViewModel>(c => new AddPageViewModel(c.Resolve<ICommandSender>()));
            _container.Register<DetailsPageViewModel>(c => new DetailsPageViewModel(c.Resolve<IReadModelFacade>(), c.Resolve<ICommandSender>()));
            _container.Register<RenamePageViewModel>(c => new RenamePageViewModel(c.Resolve<IReadModelFacade>(), c.Resolve<ICommandSender>()));
            _container.Register<CheckInPageViewModel>(c => new CheckInPageViewModel(c.Resolve<IReadModelFacade>(), c.Resolve<ICommandSender>()));
            _container.Register<RemovePageViewModel>(c => new RemovePageViewModel(c.Resolve<IReadModelFacade>(), c.Resolve<ICommandSender>()));
        }

        public T GetService<T>()
        {
            return _container.Resolve<T>();
        }

        public object GetService(Type type)
        {
            return InvokeGenericMethod(type); 
        }

        public object InvokeGenericMethod(Type type)
        {
            Func<object> temp = GetService<object>;
            MethodInfo mi = temp.Method.GetGenericMethodDefinition();
            mi = mi.MakeGenericMethod(new Type[] { type });
            return mi.Invoke(this, new object[] { });
        }
    }
}
