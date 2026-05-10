// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;

namespace CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// An attribute that can be used to declare dependencies for calculated properties.
/// When this attribute is used on a property, generated observable properties listed
/// in <see cref="PropertyNames"/> will also notify the annotated property when changed.
/// </summary>
/// <remarks>
/// This attribute is processed by the MVVM Toolkit source generators.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
public sealed class DependsOnAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property this calculated property depends on.</param>
    public DependsOnAttribute(string propertyName)
    {
        PropertyNames = new[] { propertyName };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DependsOnAttribute"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the property this calculated property depends on.</param>
    /// <param name="otherPropertyNames">The other property names this calculated property depends on.</param>
    public DependsOnAttribute(string propertyName, params string[] otherPropertyNames)
    {
        PropertyNames = new[] { propertyName }.Concat(otherPropertyNames).ToArray();
    }

    /// <summary>
    /// Gets the property names this calculated property depends on.
    /// </summary>
    public string[] PropertyNames { get; }

    /// <summary>
    /// Gets or sets whether changes from child <see cref="System.ComponentModel.INotifyPropertyChanged"/> instances should also notify dependents.
    /// </summary>
    public bool NotifyOnSubPropertyChanges { get; set; }
}
