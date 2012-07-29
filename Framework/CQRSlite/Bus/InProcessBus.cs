using System;
using System.Linq;
using System.Collections.Generic;
using CQRSlite.Commanding;
using CQRSlite.Config;
using CQRSlite.Eventing;
using CQRSlite.Infrastructure;
using System.Reflection;

namespace CQRSlite.Bus
{
    public class InProcessBus : ICommandSender, IEventPublisher
    {

        private readonly IServiceLocator _serviceLocator;
        private readonly IDictionary<Type, IList<Action<Message>>> _routes = new Dictionary<Type, IList<Action<Message>>>();

        public InProcessBus(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;

            /// we should be safe to scan all assemblies in the currenly loaded 
            /// AppDomain for implementers of IHandle<> as long as we only do it 
            /// once, but how about we filter out assemblies we know won't have any
            /// IHandle implementers.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(w => !(
                w.FullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase)
                || w.FullName.StartsWith("System", StringComparison.OrdinalIgnoreCase)
                || w.FullName.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase)));

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetLoadableTypes().Where(w => w.GetInterfaces().Any(a => a.IsGenericType && a.GetGenericTypeDefinition() == typeof(IHandles<>)));

                foreach (var type in types)
                {
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance).Where(w => w.Name.Equals("Handle", StringComparison.OrdinalIgnoreCase)))
                    {
                        var parameters = method.GetParameters();

                        if (parameters.Length == 1 && typeof(Message).IsAssignableFrom(parameters[0].ParameterType))
                        {
                            RegisterRoute(type, parameters[0].ParameterType);
                        }
                    }
                }
            }
        }

        private void RegisterRoute(Type executorType, Type messageType)
        {
            /// Get the Handle method on executorType for messageType and
            /// create a LateBoundAction.
            var method = executorType.GetMethod("Handle", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { messageType }, null);
            var a = DelegateHelper.CreateAction(method);

            /// Using closure scoping trick to retain access to the
            /// executorType and LateBoundAction created above...
            var del = new Action<Message>(x =>
            {
                var handler = _serviceLocator.GetService(executorType);
                a(handler, new object[] { x });
            });

            IList<Action<Message>> handlers;
            if (!_routes.TryGetValue(messageType, out handlers))
            {
                handlers = new List<Action<Message>>();
                _routes.Add(messageType, handlers);
            }
            handlers.Add(del);
        }

        public void Send<T>(T command) where T : Command
        {
            IList<Action<Message>> handlers;
            if (_routes.TryGetValue(typeof(T), out handlers))
            {
                if (handlers.Count != 1) throw new InvalidOperationException("Cannot send to more than one handler");
                handlers[0](command);
            }
            else
            {
                throw new InvalidOperationException("No handler registered");
            }
        }

        public void Publish<T>(T @event) where T : Event
        {
            IList<Action<Message>> handlers;
            if (!_routes.TryGetValue(@event.GetType(), out handlers)) return;
            foreach (var handler in handlers)
                handler(@event);
        }

    }
}
