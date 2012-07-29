using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CQRSlite.Domain;
using CQRSlite.Eventing;
using CQRSlite.Infrastructure;

namespace CQRSlite.Snapshotting
{
    public class SnapshotRepository : IRepository
    {
        private readonly ISnapshotStore _snapshotStore;
        private readonly ISnapshotStrategy _snapshotStrategy;
        private readonly IRepository _repository;
        private readonly IEventStore _eventStore;

        private readonly IDictionary<Type, LateBoundAction> _restoreSnapshotCache = new Dictionary<Type, LateBoundAction>();
        private readonly IDictionary<Type, LateBoundFunc> _getSnapshotCache = new Dictionary<Type, LateBoundFunc>();

        public SnapshotRepository(ISnapshotStore snapshotStore, ISnapshotStrategy snapshotStrategy, IRepository repository, IEventStore eventStore)
        {
            _snapshotStore = snapshotStore;
            _snapshotStrategy = snapshotStrategy;
            _repository = repository;
            _eventStore = eventStore;
        }

        public void Save<T>(T aggregate, int? exectedVersion = null) where T : AggregateRoot
        {
            TryMakeSnapshot(aggregate);
            _repository.Save(aggregate, exectedVersion);
        }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            var aggregate = AggregateActivator.CreateAggregate<T>();
            var snapshotVersion = TryRestoreAggregateFromSnapshot(aggregateId, aggregate);
            if(snapshotVersion == -1)
            {
                return _repository.Get<T>(aggregateId);
            }
            var events = _eventStore.Get(aggregateId, snapshotVersion).Where(desc => desc.Version > snapshotVersion);
            aggregate.LoadFromHistory(events);

            return aggregate;
        }

        private int TryRestoreAggregateFromSnapshot<T>(Guid id, T aggregate)
        {
            var version = -1;
            if (_snapshotStrategy.IsSnapshotable(typeof(T)))
            {
                var snapshot = _snapshotStore.Get(id);
                if (snapshot != null)
                {
                    var action = GetSnapshotRestoreAction(aggregate.GetType());
                    action(aggregate, new object[] { snapshot });
                    version = snapshot.Version;
                }
            }
            return version;
        }

        private void TryMakeSnapshot(AggregateRoot aggregate)
        {
            if (!_snapshotStrategy.ShouldMakeSnapShot(aggregate))
                return;

            var func = GetSnapshotGetFunc(aggregate.GetType());
            var snapshot = (Snapshot)func(aggregate, new object[] { });

            snapshot.Version = aggregate.Version + aggregate.GetUncommittedChanges().Count();
            _snapshotStore.Save(snapshot);
        }

        private LateBoundAction GetSnapshotRestoreAction(Type aggregateType)
        {
            if (!_restoreSnapshotCache.ContainsKey(aggregateType))
            {
                var method = aggregateType.GetMethod("RestoreFromSnapshot", BindingFlags.NonPublic | BindingFlags.Instance);
                _restoreSnapshotCache.Add(aggregateType, DelegateHelper.CreateAction(method));
            }
            return _restoreSnapshotCache[aggregateType];
        }

        private LateBoundFunc GetSnapshotGetFunc(Type aggregateType)
        {
            if (!_getSnapshotCache.ContainsKey(aggregateType))
            {
                var method = aggregateType.GetMethod("CreateSnapshot", BindingFlags.NonPublic | BindingFlags.Instance);
                _getSnapshotCache.Add(aggregateType, DelegateHelper.CreateFunc(method));
            }
            return _getSnapshotCache[aggregateType];
        }
    }
}