// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace CommunityToolkit.Common.Helpers.Localization;

/// <summary>
/// Enables users to respond to culture changes at runtime.
/// </summary>
public class LocalizationResourceManager : INotifyPropertyChanged
{
	ResourceManager? resourceManager;
	CultureInfo currentCulture;

	/// <summary>
	/// Initialize a new instance of <see cref="LocalizationResourceManager"/>.
	/// </summary>
	LocalizationResourceManager()
	{
		this.currentCulture = CultureInfo.CurrentCulture;
	}

	/// <summary>
	/// Instance of <see cref="LocalizationResourceManager"/>.
	/// </summary>
	public static LocalizationResourceManager Current { get; } = new();

	/// <summary>
	/// Current culture.
	/// </summary>
	public CultureInfo CurrentCulture
	{
		get => this.currentCulture;
		set
		{
			this.currentCulture = value;
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
		}
	}

	/// <summary>
	/// Get resource object by name.
	/// </summary>
	/// <param name="resourceKey">Resource name.</param>
	/// <returns>Resource object if exist, otherwise empty byte array.</returns>
	public object this[string resourceKey] => this.GetValue(resourceKey);

	/// <summary>
	/// Get resource object by name.
	/// </summary>
	/// <param name="resourceKey">Resource name.</param>
	/// <returns>Resource object if exist, otherwise empty byte array.</returns>
	/// <exception cref="NullReferenceException">In case Init method is not called.</exception>
	public object GetValue(string resourceKey)
	{
		if (this.resourceManager is null)
		{
			throw new NullReferenceException("Call Init method first");
		}

		return this.resourceManager.GetObject(resourceKey, this.CurrentCulture) ?? Array.Empty<byte>();
	}

	/// <summary>
	/// <inheritdoc />
	/// </summary>
	public event PropertyChangedEventHandler? PropertyChanged;
	
	/// <summary>
	/// Initialize Resource manager.
	/// </summary>
	/// <param name="manager"><see cref="ResourceManager"/>.</param>
	public void Init(ResourceManager manager)
	{
		this.resourceManager = manager;
	}

	/// <summary>
	/// Initialize Resource manager and its culture.
	/// </summary>
	/// <param name="manager"><see cref="ResourceManager"/>.</param>
	/// <param name="cultureInfo"><see cref="CultureInfo"/>.</param>
	public void Init(ResourceManager manager, CultureInfo cultureInfo)
	{
		this.Init(manager);
		this.CurrentCulture = cultureInfo;
	}
}