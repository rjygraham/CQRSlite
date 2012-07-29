using System;
using System.Collections.Generic;
using System.Reflection;
using CQRSlite.Domain.Exception;
using CQRSlite.Eventing;
using CQRSlite.Infrastructure;

namespace CQRSlite.Domain
{
    public abstract class   AggregateRoot
    {
        private readonly List<Event> _changes = new List<Event>();
        private readonly IDictionary<Type, LateBoundAction> _applyCache = new Dictionary<Type, LateBoundAction>();

        public Guid Id { get; protected set; }
        public int Version { get; private set; }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            Version = Version + _changes.Count;
            _changes.Clear();
        }

        public void LoadFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                if (e.Version != Version + 1)
                    throw new EventsOutOfOrderException();
                ApplyChange(e, false);
            }
        }

        protected void ApplyChange(Event @event)
        {
            ApplyChange(@event, true);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            Id = @event.Id;

            var action = GetApply(@event.GetType());
            if (action != null)
            {
                action(this, new object[] { @event });
            }

            if (isNew)
            {
                _changes.Add(@event);
            }
            else
            {
                Version++;
            }
        }

        private LateBoundAction GetApply(Type eventType)
        {
            if (!_applyCache.ContainsKey(eventType))
            {
                LateBoundAction action = null;

                var method = this.GetType().GetMethod("Apply", BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { eventType }, null);
                if (method != null)
                {
                    action = DelegateHelper.CreateAction(method);
                }
                _applyCache.Add(eventType, action);
            }

            return _applyCache[eventType];
        }
    }
}