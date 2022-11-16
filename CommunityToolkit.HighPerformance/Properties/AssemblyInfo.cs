// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System.Runtime.CompilerServices;

// We can suppress the .init flag for local variables for the entire module.
// This doesn't affect the correctness of methods in this assembly, as none of them
// are relying on the JIT ensuring that all local memory is zeroed out to work. Doing
// this can provide some minor performance benefits, depending on the workload.
[module: SkipLocalsInit]

// We need to test the RuntimeHelpers polyfills on applicable runtimes
#if !NETSTANDARD2_1_OR_GREATER
[assembly: InternalsVisibleTo("CommunityToolkit.HighPerformance.UnitTests, publickey=002400000480000094000000060200000024000052534131000400000100010041753AF735AE6140C9508567666C51C6AB929806ADB0D210694B30AB142A060237BC741F9682E7D8D4310364B4BBA4EE89CC9D3D5CE7E5583587E8EA44DCA09977996582875E71FB54FA7B170798D853D5D8010B07219633BDB761D01AC924DA44576D6180CDCEAE537973982BB461C541541D58417A3794E34F45E6F2D129E2")]
#endif