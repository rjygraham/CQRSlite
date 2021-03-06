﻿using System;
using System.Linq;
using System.Runtime.Caching;
using CQRSlite.Domain;
using CQRSlite.Domain.Exception;
using CQRSlite.Eventing;

namespace CQRSlite.Caching
{
    public class CachingRepository : IRepository
    {
        private readonly IRepository _repository;
        private readonly IEventStore _eventStore;
        private readonly MemoryCache _cache;
        private readonly Func<CacheItemPolicy> _policyFactory;

        public CachingRepository(IRepository repository, IEventStore eventStore)
        {
            _repository = repository;
            _eventStore = eventStore;
            _cache = MemoryCache.Default;
            _policyFactory = () => new CacheItemPolicy {SlidingExpiration = new TimeSpan(0, 1, 0, 0)};
        }

        public void Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot
        {
            if (!IsTracked(aggregate.Id))
                _cache.Add(aggregate.Id.ToString(), aggregate, _policyFactory.Invoke());
            
            _repository.Save(aggregate,expectedVersion);
        }

        public T Get<T>(Guid aggregateId) where T : AggregateRoot
        {
            T aggregate;
            if (IsTracked(aggregateId))
            {
                aggregate = (T)_cache.Get(aggregateId.ToString());
                var events = _eventStore.Get(aggregateId, aggregate.Version);
                if(events.First().Version != aggregate.Version + 1)
                    throw new EventsOutOfOrderException();
                aggregate.LoadFromHistory(events);
                
                return aggregate;
            }

            aggregate = _repository.Get<T>(aggregateId);
            _cache.Add(aggregateId.ToString(), aggregate, _policyFactory.Invoke());

            return aggregate;
        }

        private bool IsTracked(Guid id)
        {
            return _cache.Contains(id.ToString());
        }
    }
}