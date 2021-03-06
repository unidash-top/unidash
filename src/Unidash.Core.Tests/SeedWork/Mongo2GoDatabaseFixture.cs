﻿using Mongo2Go;
using System;

namespace Unidash.Core.Tests.SeedWork
{
    public class Mongo2GoDatabaseFixture : IDisposable
    {
        public string ConnectionString { get; }

        private readonly MongoDbRunner _runner;

        public Mongo2GoDatabaseFixture()
        {
            _runner = MongoDbRunner.Start();
            ConnectionString = _runner.ConnectionString;
        }

        public void Dispose()
        {
            _runner?.Dispose();
        }
    }
}
