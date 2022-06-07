// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.Resources;

namespace CommunityToolkit.Common.UnitTests.Helpers.Localization;

public class MockResourceManager : ResourceManager
{
	public override string GetString(string name, CultureInfo? culture) => culture?.EnglishName ?? string.Empty;
	public override object? GetObject(string name, CultureInfo? culture) => string.IsNullOrEmpty(name) ? null : culture?.EnglishName;
}