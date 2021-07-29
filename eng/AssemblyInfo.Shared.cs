// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file contains assembly and module attributes that is shared across projects.
// Include it in `Directory.Build.targets` near to all projects that need this file.

using System.Runtime.CompilerServices;

/*
  Using `[module: SkipLocalsInit]` suppresses the .init flag for local variables for the entire module.
  This doesn't affect the correctness of the methods in this assembly, as none of them are relying on
  JIT ensuring that all local memory is zeroed out to work. Doing this can provide some minor
  performance benefits, depending on the workload.
*/
[module: SkipLocalsInit]
