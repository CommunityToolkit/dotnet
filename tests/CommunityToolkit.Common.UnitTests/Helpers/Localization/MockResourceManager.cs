using System.Globalization;
using System.Resources;

namespace CommunityToolkit.Common.UnitTests.Helpers.Localization;

public class MockResourceManager : ResourceManager
{
	public override string GetString(string name, CultureInfo? culture) => culture?.EnglishName ?? string.Empty;
	public override object? GetObject(string name, CultureInfo? culture) => string.IsNullOrEmpty(name) ? null : culture?.EnglishName;
}