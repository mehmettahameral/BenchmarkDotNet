﻿using System;
using System.Collections.Generic;
using BenchmarkDotNet.Reports;

namespace BenchmarkDotNet.Analyzers
{
    public class EnvironmentAnalyser : IAnalyser
    {
        public static readonly IAnalyser Default = new EnvironmentAnalyser();

        private EnvironmentAnalyser()
        {
        }

        public IEnumerable<IWarning> Analyze(Summary summary)
        {
            var hostInfo = summary.HostEnvironmentHelper;
            if (hostInfo.HasAttachedDebugger)
                yield return new Warning("AttachedDebugger", "Benchmark was executed with attached debugger.", null);
            if (hostInfo.Configuration.Equals("DEBUG", StringComparison.InvariantCultureIgnoreCase))
                yield return new Warning("DebugConfiguration", "Benchmark was built in DEBUG configuration. Please, build it in RELEASE.", null);
        }
    }
}