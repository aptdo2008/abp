﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.ExtensionMethods.Collections.Generic;

namespace Volo.Abp.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        public IServiceProvider ServiceProvider { get; }

        private readonly Dictionary<string, IDatabaseApi> _databaseApis;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            _databaseApis = new Dictionary<string, IDatabaseApi>();
        }

        public void Dispose()
        {
            //TODO: Remove itself from IUnitOfWorkManager
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var databaseApi in _databaseApis.Values)
            {
                await databaseApi.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task CompleteAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            //TODO: We need a transaction management system.

            foreach (var databaseApi in _databaseApis.Values)
            {
                await databaseApi.SaveChangesAsync(cancellationToken);
            }
        }

        public IDatabaseApi FindDatabaseApi(string id)
        {
            return _databaseApis.GetOrDefault(id);
        }

        public IDatabaseApi GetOrAddDatabaseApi(string id, Func<IDatabaseApi> factory)
        {
            Check.NotNull(id, nameof(id));
            Check.NotNull(factory, nameof(factory));

            return _databaseApis.GetOrAdd(id, factory);
        }
        
        public IDatabaseApi AddDatabaseApi(string id, IDatabaseApi databaseApi)
        {
            Check.NotNull(id, nameof(id));
            Check.NotNull(databaseApi, nameof(databaseApi));

            if (_databaseApis.ContainsKey(id))
            {
                throw new AbpException($"There is already a database api with same id: {id}");
            }

            return _databaseApis[id] = databaseApi;
        }
    }
}