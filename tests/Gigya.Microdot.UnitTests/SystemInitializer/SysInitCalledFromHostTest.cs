﻿using System.Threading.Tasks;
using Gigya.Microdot.Hosting.Validators;
using Gigya.Microdot.SharedLogic;
using Gigya.Microdot.SharedLogic.Measurement.Workload;
using Gigya.Microdot.Testing.Shared.Service;
using NSubstitute;
using NUnit.Framework;

namespace Gigya.Microdot.UnitTests.SystemInitializer
{
    [TestFixture,Parallelizable(ParallelScope.Fixtures)]
    public class SysInitCalledFromHostTest
    {
        [Test]
        public async Task ValidatorCalledOnce()
        {
            IValidator validatorFake = Substitute.For<IValidator>();
            ServiceHostFake<IValidator> srvHost = new ServiceHostFake<IValidator>(validatorFake);
            var args = new ServiceArguments(ServiceStartupMode.CommandLineNonInteractive,
                ConsoleOutputMode.Disabled,
                SiloClusterMode.PrimaryNode,
                DisposablePort.GetPort().Port, initTimeOutSec: 10);
            Task.Run(() => srvHost.Run(args));

            await srvHost.WaitForServiceStartedAsync();
            srvHost.Dispose();

            validatorFake.Received(1).Validate();
        }

        [Test]
        public async Task WorkloadMetricsCalledOnce()
        {
            var args = new ServiceArguments(ServiceStartupMode.CommandLineNonInteractive,
                ConsoleOutputMode.Disabled,
                SiloClusterMode.PrimaryNode,
                DisposablePort.GetPort().Port, initTimeOutSec: 10);
            IWorkloadMetrics workloadMetricsFake = Substitute.For<IWorkloadMetrics>();
            ServiceHostFake<IWorkloadMetrics> srvHost = new ServiceHostFake<IWorkloadMetrics>(workloadMetricsFake);
            Task.Run(() => srvHost.Run(args));
            await srvHost.WaitForServiceStartedAsync();
            srvHost.Dispose();

            workloadMetricsFake.Received(1).Init();
            workloadMetricsFake.Received().Dispose();
        }
    }
}
