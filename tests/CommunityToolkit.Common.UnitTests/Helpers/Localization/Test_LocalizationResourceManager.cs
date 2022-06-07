using System;
using System.Globalization;
using CommunityToolkit.Common.Helpers.Localization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CommunityToolkit.Common.UnitTests.Helpers.Localization;

[TestClass]
public class Test_LocalizationResourceManager
{
	[TestMethod, Priority(0)]
	public void GetResource_ResourceManagerNotInitialized_ThrowException()
	{
		Assert.ThrowsException<NullReferenceException>(
            () => LocalizationResourceManager.Current["Resource"], 
            "Call Init method first");
	}

	[TestMethod, Priority(1)]
    public void GetResource_ResourceDoesNotExist_GetEmptyArray()
	{
		LocalizationResourceManager.Current.Init(new MockResourceManager());
        Assert.AreEqual(LocalizationResourceManager.Current[""], Array.Empty<byte>());
	}

	[TestMethod, Priority(2)]
    public void GetResource_ResourceExists_GetValue()
	{
		LocalizationResourceManager.Current.Init(new MockResourceManager(), new CultureInfo("en-US"));
        Assert.AreEqual(LocalizationResourceManager.Current["Resource"], "English (United States)");
	}

	[TestMethod, Priority(3)]
    public void GetResource_ResourceExistsAndSetCulture_GetValue()
	{
		LocalizationResourceManager.Current.Init(new MockResourceManager());
        Assert.AreEqual(LocalizationResourceManager.Current["Resource"], "English (United States)");
		LocalizationResourceManager.Current.CurrentCulture = new CultureInfo("uk-UA");
        Assert.AreEqual(LocalizationResourceManager.Current.GetValue("Resource"), "Ukrainian (Ukraine)");
	}

	[TestMethod, Priority(4)]
    public void GetResourceWithPredefinedCulture_ResourceExists_GetValue()
	{
		LocalizationResourceManager.Current.Init(new MockResourceManager(), new CultureInfo("uk-UA"));
        Assert.AreEqual(LocalizationResourceManager.Current["Resource"], "Ukrainian (Ukraine)");
	}
}