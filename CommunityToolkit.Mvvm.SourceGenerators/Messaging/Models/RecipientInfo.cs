// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using CommunityToolkit.Mvvm.SourceGenerators.Helpers;

namespace CommunityToolkit.Mvvm.SourceGenerators.Messaging.Models;

/// <summary>
/// A model with gathered info on all message types being handled by a recipient.
/// </summary>
/// <param name="FilenameHint">The filename hint for the current type.</param>
/// <param name="TypeName">The fully qualified type name of the target type.</param>
/// <param name="MessageTypes">The name of messages being received.</param>
internal sealed record RecipientInfo(string FilenameHint, string TypeName, EquatableArray<string> MessageTypes);
