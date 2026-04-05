// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CommunityToolkit.Mvvm.ComponentModel;

/// <summary>
/// A base class for observable objects implementing the <see cref="INotifyDataErrorInfo"/> interface.
/// </summary>
/// <remarks>
/// This type stores validation state and exposes helper APIs to validate individual properties or all
/// properties in an instance. The actual validation logic can be provided either by derived types or by
/// source-generated code overriding the available validation hooks.
/// </remarks>
public abstract class ObservableValidator : ObservableObject, INotifyDataErrorInfo
{
    /// <summary>
    /// The cached <see cref="PropertyChangedEventArgs"/> for <see cref="HasErrors"/>.
    /// </summary>
    private static readonly PropertyChangedEventArgs HasErrorsChangedEventArgs = new(nameof(HasErrors));

    /// <summary>
    /// The optional <see cref="IServiceProvider"/> instance to use when creating a <see cref="ValidationContext"/>.
    /// </summary>
    private readonly IServiceProvider? validationServiceProvider;

    /// <summary>
    /// The optional copied items to use when creating a <see cref="ValidationContext"/>.
    /// </summary>
    private readonly Dictionary<object, object?>? validationItems;

    /// <summary>
    /// The <see cref="ValidationContext"/> instance currently in use.
    /// </summary>
    private ValidationContext? validationContext;

    /// <summary>
    /// The <see cref="Dictionary{TKey,TValue}"/> instance used to store previous validation results.
    /// </summary>
    private readonly Dictionary<string, List<ValidationResult>> errors = new();

    /// <summary>
    /// Indicates the total number of properties with errors (not total errors).
    /// This is used to allow <see cref="HasErrors"/> to operate in O(1) time, as it can just
    /// check whether this value is not 0 instead of having to traverse <see cref="errors"/>.
    /// </summary>
    private int totalErrors;

    /// <inheritdoc/>
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
    /// This constructor will lazily create a new <see cref="ValidationContext"/> when validation runs.
    /// The created context will reference the current instance and no additional services or validation items.
    /// </summary>
    protected ObservableValidator()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
    /// This constructor will lazily create a new <see cref="ValidationContext"/> when validation runs.
    /// The created context will reference the current instance and expose the specified validation items.
    /// </summary>
    /// <param name="items">A set of key/value pairs to make available to consumers.</param>
    protected ObservableValidator(IDictionary<object, object?>? items)
    {
        if (items is not null)
        {
            this.validationItems = new Dictionary<object, object?>(items);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
    /// This constructor will lazily create a new <see cref="ValidationContext"/> when validation runs.
    /// The created context will reference the current instance and expose the specified services and validation items.
    /// </summary>
    /// <param name="serviceProvider">An <see cref="IServiceProvider"/> instance to make available during validation.</param>
    /// <param name="items">A set of key/value pairs to make available to consumers.</param>
    protected ObservableValidator(IServiceProvider? serviceProvider, IDictionary<object, object?>? items)
    {
        this.validationServiceProvider = serviceProvider;

        if (items is not null)
        {
            this.validationItems = new Dictionary<object, object?>(items);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableValidator"/> class.
    /// This constructor stores the input <see cref="ValidationContext"/> instance for later reuse.
    /// </summary>
    /// <param name="validationContext">
    /// The <see cref="ValidationContext"/> instance to use to validate properties.
    /// <para>
    /// This instance will be reused by the validation helpers in this type. Its <see cref="ValidationContext.MemberName"/>
    /// and <see cref="ValidationContext.DisplayName"/> properties will be updated before validating a property, and they will
    /// keep the values from the last validation operation.
    /// </para>
    /// </param>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="validationContext"/> is <see langword="null"/>.</exception>
    protected ObservableValidator(ValidationContext validationContext)
    {
        ArgumentNullException.ThrowIfNull(validationContext);

        this.validationContext = validationContext;
    }

    /// <inheritdoc/>
    [Display(AutoGenerateField = false)]
    public bool HasErrors => this.totalErrors > 0;

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// This method is just like <see cref="ObservableObject.SetProperty{T}(ref T,T,string)"/>, just with the addition
    /// of the <paramref name="validate"/> parameter. If that is set to <see langword="true"/>, the new value will be
    /// validated and <see cref="ErrorsChanged"/> will be raised if needed. Following the behavior of the base method,
    /// the <see cref="ObservableObject.PropertyChanging"/> and <see cref="ObservableObject.PropertyChanged"/> events
    /// are not raised if the current and new value for the target property are the same.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, bool validate, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        bool propertyChanged = SetProperty(ref field, newValue, propertyName);

        if (propertyChanged && validate)
        {
            ValidateProperty(newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
    /// See additional notes about this overload in <see cref="SetProperty{T}(ref T,T,bool,string)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="comparer"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool SetProperty<T>([NotNullIfNotNull(nameof(newValue))] ref T field, T newValue, IEqualityComparer<T> comparer, bool validate, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(propertyName);

        bool propertyChanged = SetProperty(ref field, newValue, comparer, propertyName);

        if (propertyChanged && validate)
        {
            ValidateProperty(newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event. Similarly to
    /// the <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string)"/> method, this overload should only be
    /// used when <see cref="ObservableObject.SetProperty{T}(ref T,T,string)"/> can't be used directly.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// This method is just like <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string)"/>, just with the addition
    /// of the <paramref name="validate"/> parameter. As such, following the behavior of the base method,
    /// the <see cref="ObservableObject.PropertyChanging"/> and <see cref="ObservableObject.PropertyChanged"/> events
    /// are not raised if the current and new value for the target property are the same.
    /// </remarks>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool SetProperty<T>(T oldValue, T newValue, Action<T> callback, bool validate, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        bool propertyChanged = SetProperty(oldValue, newValue, callback, propertyName);

        if (propertyChanged && validate)
        {
            ValidateProperty(newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="ObservableObject.PropertyChanged"/> event.
    /// See additional notes about this overload in <see cref="SetProperty{T}(T,T,Action{T},bool,string)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="comparer"/>, <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool SetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, bool validate, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        bool propertyChanged = SetProperty(oldValue, newValue, comparer, callback, propertyName);

        if (propertyChanged && validate)
        {
            ValidateProperty(newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given nested property. If the value has changed,
    /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property and then raises the
    /// <see cref="ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
    /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string)"/>, with the difference being that this
    /// method is used to relay properties from a wrapped model in the current instance. For more info, see the docs for
    /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string)"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="model">The model </param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="model"/>, <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, bool validate, [CallerMemberName] string propertyName = null!)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        bool propertyChanged = SetProperty(oldValue, newValue, model, callback, propertyName);

        if (propertyChanged && validate)
        {
            ValidateProperty(newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given nested property. If the value has changed,
    /// raises the <see cref="ObservableObject.PropertyChanging"/> event, updates the property and then raises the
    /// <see cref="ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
    /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string)"/>,
    /// with the difference being that this method is used to relay properties from a wrapped model in the
    /// current instance. For more info, see the docs for
    /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string)"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="model">The model </param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="validate">If <see langword="true"/>, <paramref name="newValue"/> will also be validated.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="comparer"/>, <paramref name="model"/>, <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool SetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, bool validate, [CallerMemberName] string propertyName = null!)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        bool propertyChanged = SetProperty(oldValue, newValue, comparer, model, callback, propertyName);

        if (propertyChanged && validate)
        {
            ValidateProperty(newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Tries to validate a new value for a specified property. If the validation is successful,
    /// <see cref="ObservableObject.SetProperty{T}(ref T,T,string?)"/> is called, otherwise no state change is performed.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="errors">The resulting validation errors, if any.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns>Whether the validation was successful and the property value changed as well.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    protected bool TrySetProperty<T>(ref T field, T newValue, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        return TryValidateProperty(newValue, propertyName, out errors) &&
               SetProperty(ref field, newValue, propertyName);
    }

    /// <summary>
    /// Tries to validate a new value for a specified property. If the validation is successful,
    /// <see cref="ObservableObject.SetProperty{T}(ref T,T,IEqualityComparer{T},string?)"/> is called, otherwise no state change is performed.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="errors">The resulting validation errors, if any.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns>Whether the validation was successful and the property value changed as well.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="comparer"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool TrySetProperty<T>(ref T field, T newValue, IEqualityComparer<T> comparer, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(propertyName);

        return TryValidateProperty(newValue, propertyName, out errors) &&
               SetProperty(ref field, newValue, comparer, propertyName);
    }

    /// <summary>
    /// Tries to validate a new value for a specified property. If the validation is successful,
    /// <see cref="ObservableObject.SetProperty{T}(T,T,Action{T},string?)"/> is called, otherwise no state change is performed.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="errors">The resulting validation errors, if any.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns>Whether the validation was successful and the property value changed as well.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool TrySetProperty<T>(T oldValue, T newValue, Action<T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        return TryValidateProperty(newValue, propertyName, out errors) &&
               SetProperty(oldValue, newValue, callback, propertyName);
    }

    /// <summary>
    /// Tries to validate a new value for a specified property. If the validation is successful,
    /// <see cref="ObservableObject.SetProperty{T}(T,T,IEqualityComparer{T},Action{T},string?)"/> is called, otherwise no state change is performed.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="errors">The resulting validation errors, if any.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns>Whether the validation was successful and the property value changed as well.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="comparer"/>, <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool TrySetProperty<T>(T oldValue, T newValue, IEqualityComparer<T> comparer, Action<T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        return TryValidateProperty(newValue, propertyName, out errors) &&
               SetProperty(oldValue, newValue, comparer, callback, propertyName);
    }

    /// <summary>
    /// Tries to validate a new value for a specified property. If the validation is successful,
    /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,TModel,Action{TModel,T},string?)"/> is called, otherwise no state change is performed.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="model">The model </param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="errors">The resulting validation errors, if any.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns>Whether the validation was successful and the property value changed as well.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="model"/>, <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool TrySetProperty<TModel, T>(T oldValue, T newValue, TModel model, Action<TModel, T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string propertyName = null!)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        return TryValidateProperty(newValue, propertyName, out errors) &&
               SetProperty(oldValue, newValue, model, callback, propertyName);
    }

    /// <summary>
    /// Tries to validate a new value for a specified property. If the validation is successful,
    /// <see cref="ObservableObject.SetProperty{TModel,T}(T,T,IEqualityComparer{T},TModel,Action{TModel,T},string?)"/> is called, otherwise no state change is performed.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="model">The model </param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="errors">The resulting validation errors, if any.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns>Whether the validation was successful and the property value changed as well.</returns>
    /// <exception cref="System.ArgumentNullException">Thrown if <paramref name="comparer"/>, <paramref name="model"/>, <paramref name="callback"/> or <paramref name="propertyName"/> are <see langword="null"/>.</exception>
    protected bool TrySetProperty<TModel, T>(T oldValue, T newValue, IEqualityComparer<T> comparer, TModel model, Action<TModel, T> callback, out IReadOnlyCollection<ValidationResult> errors, [CallerMemberName] string propertyName = null!)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(comparer);
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(callback);
        ArgumentNullException.ThrowIfNull(propertyName);

        return TryValidateProperty(newValue, propertyName, out errors) &&
               SetProperty(oldValue, newValue, comparer, model, callback, propertyName);
    }

    /// <summary>
    /// Clears the validation errors for a specified property or for the entire entity.
    /// </summary>
    /// <param name="propertyName">
    /// The name of the property to clear validation errors for.
    /// If a <see langword="null"/> or empty name is used, all entity-level errors will be cleared.
    /// </param>
    protected void ClearErrors(string? propertyName = null)
    {
        // Clear entity-level errors when the target property is null or empty
        if (string.IsNullOrEmpty(propertyName))
        {
            ClearAllErrors();
        }
        else
        {
            ClearErrorsForProperty(propertyName!);
        }
    }

    /// <inheritdoc cref="INotifyDataErrorInfo.GetErrors(string)"/>
    public IEnumerable<ValidationResult> GetErrors(string? propertyName = null)
    {
        // Get entity-level errors when the target property is null or empty
        if (string.IsNullOrEmpty(propertyName))
        {
            // Local function to gather all the entity-level errors
            [MethodImpl(MethodImplOptions.NoInlining)]
            IEnumerable<ValidationResult> GetAllErrors()
            {
                return this.errors.Values.SelectMany(static errors => errors);
            }

            return GetAllErrors();
        }

        // Property-level errors, if any
        if (this.errors.TryGetValue(propertyName!, out List<ValidationResult>? errors))
        {
            return errors;
        }

        // The INotifyDataErrorInfo.GetErrors method doesn't specify exactly what to
        // return when the input property name is invalid, but given that the return
        // type is marked as a non-nullable reference type, here we're returning an
        // empty array to respect the contract. This also matches the behavior of
        // this method whenever errors for a valid properties are retrieved.
        return Array.Empty<ValidationResult>();
    }

    /// <inheritdoc/>
    IEnumerable INotifyDataErrorInfo.GetErrors(string? propertyName) => GetErrors(propertyName);

    /// <summary>
    /// Validates all properties in the current instance and updates the tracked errors.
    /// </summary>
    /// <remarks>
    /// This method delegates to <see cref="ValidateAllPropertiesCore()"/>.
    /// </remarks>
    protected void ValidateAllProperties()
    {
        ValidateAllPropertiesCore();
    }

    /// <summary>
    /// Validates all properties in the current instance.
    /// </summary>
    /// <remarks>
    /// The default implementation does nothing.
    /// Derived types can override this method to validate all relevant properties and update the tracked errors.
    /// </remarks>
    protected virtual void ValidateAllPropertiesCore() {}

    /// <summary>
    /// Tries to validate a property with the specified name and value, adding any errors to the target collection.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="errors">The target collection for validation errors.</param>
    /// <returns>
    /// A <see cref="ValidationStatus"/> value indicating whether validation succeeded, failed, or was not handled.
    /// </returns>
    /// <remarks>
    /// The default implementation returns <see cref="ValidationStatus.Unhandled"/>.
    /// Derived types can override this method to provide property-specific validation logic.
    /// </remarks>
    protected virtual ValidationStatus TryValidatePropertyCore(object? value, string propertyName, ICollection<ValidationResult> errors)
    {
        return ValidationStatus.Unhandled;
    }

    /// <summary>
    /// Validates a property value through <see cref="Validator.TryValidateValue(object, ValidationContext, ICollection{ValidationResult}, IEnumerable{ValidationAttribute})"/>.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="displayName">The display name to use for validation messages.</param>
    /// <param name="validationAttributes">The explicit validation attributes to use.</param>
    /// <param name="errors">The target collection for validation errors.</param>
    /// <returns><see cref="ValidationStatus.Success"/> if the property is valid, otherwise <see cref="ValidationStatus.Error"/>.</returns>
    /// <remarks>
    /// This helper reuses a shared <see cref="ValidationContext"/> instance, updating its
    /// <see cref="ValidationContext.MemberName"/> and <see cref="ValidationContext.DisplayName"/> properties for the target property.
    /// </remarks>
    protected ValidationStatus TryValidateValue(object? value, string propertyName, string displayName, IEnumerable<ValidationAttribute> validationAttributes, ICollection<ValidationResult> errors)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(displayName);
        ArgumentNullException.ThrowIfNull(validationAttributes);
        ArgumentNullException.ThrowIfNull(errors);
        if (displayName.Length == 0)
        {
            throw new ArgumentException("The display name cannot be empty.", nameof(displayName));
        }

        ValidationContext updatedContext = GetOrCreateUpdatedValidationContext(propertyName, displayName);

        return Validator.TryValidateValue(value, updatedContext, errors, validationAttributes)
            ? ValidationStatus.Success
            : ValidationStatus.Error;
    }

    /// <summary>
    /// Validates a property with a specified name and value and updates the tracked errors for that property.
    /// </summary>
    /// <param name="value">The value to test for the specified property.</param>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the validation request is not handled by <see cref="TryValidatePropertyCore(object?, string, ICollection{ValidationResult})"/>.
    /// </exception>
    protected internal void ValidateProperty(object? value, [CallerMemberName] string propertyName = null!)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        // Check if the property had already been previously validated, and if so retrieve
        // the reusable list of validation errors from the errors dictionary. This list is
        // used to add new validation errors below, if any are produced by the validator.
        // If the property isn't present in the dictionary, add it now to avoid allocations.
        if (!this.errors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors))
        {
            propertyErrors = new List<ValidationResult>();

            this.errors.Add(propertyName, propertyErrors);
        }

        bool errorsChanged = false;

        // Clear the errors for the specified property, if any
        if (propertyErrors.Count > 0)
        {
            propertyErrors.Clear();

            errorsChanged = true;
        }

        ValidationStatus isValid = TryValidatePropertyCore(value, propertyName, propertyErrors);

        // Update the shared counter for the number of errors, and raise the
        // property changed event if necessary. We decrement the number of total
        // errors if the current property is valid but it wasn't so before this
        // validation, and we increment it if the validation failed after being
        // correct before. The property changed event is raised whenever the
        // number of total errors is either decremented to 0, or incremented to 1.
        if (isValid is ValidationStatus.Success)
        {
            if (errorsChanged)
            {
                this.totalErrors--;

                if (this.totalErrors == 0)
                {
                    OnPropertyChanged(HasErrorsChangedEventArgs);
                }
            }
        }
        else if (isValid is ValidationStatus.Unhandled)
        {
            throw new InvalidOperationException($"The requested property {propertyName} was not handled");
        }
        else if (!errorsChanged)
        {
            this.totalErrors++;

            if (this.totalErrors == 1)
            {
                OnPropertyChanged(HasErrorsChangedEventArgs);
            }
        }

        // Only raise the event once if needed. This happens either when the target property
        // had existing errors and is now valid, or if the validation has failed and there are
        // new errors to broadcast, regardless of the previous validation state for the property.
        if (errorsChanged || isValid is not ValidationStatus.Success)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Tries to validate a property with a specified name and value and returns the computed errors, if any.
    /// </summary>
    /// <param name="value">The value to test for the specified property.</param>
    /// <param name="propertyName">The name of the property to validate.</param>
    /// <param name="errors">The resulting validation errors, if any.</param>
    private bool TryValidateProperty(object? value, string propertyName, out IReadOnlyCollection<ValidationResult> errors)
    {
        // Add the cached errors list for later use.
        if (!this.errors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors))
        {
            propertyErrors = new List<ValidationResult>();

            this.errors.Add(propertyName, propertyErrors);
        }

        bool hasErrors = propertyErrors.Count > 0;

        List<ValidationResult> localErrors = new();

        ValidationStatus isValid = TryValidatePropertyCore(value, propertyName, localErrors);

        // We only modify the state if the property is valid and it wasn't so before. In this case, we
        // clear the cached list of errors (which is visible to consumers) and raise the necessary events.
        if ((isValid is ValidationStatus.Success) && hasErrors)
        {
            propertyErrors.Clear();

            this.totalErrors--;

            if (this.totalErrors == 0)
            {
                OnPropertyChanged(HasErrorsChangedEventArgs);
            }

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        errors = localErrors;

        return isValid is ValidationStatus.Success;
    }

    /// <summary>
    /// Clears all the current errors for the entire entity.
    /// </summary>
    private void ClearAllErrors()
    {
        if (this.totalErrors == 0)
        {
            return;
        }

        // Clear the errors for all properties with at least one error, and raise the
        // ErrorsChanged event for those properties. Other properties will be ignored.
        foreach (KeyValuePair<string, List<ValidationResult>> propertyInfo in this.errors)
        {
            bool hasErrors = propertyInfo.Value.Count > 0;

            propertyInfo.Value.Clear();

            if (hasErrors)
            {
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyInfo.Key));
            }
        }

        this.totalErrors = 0;

        OnPropertyChanged(HasErrorsChangedEventArgs);
    }

    /// <summary>
    /// Clears all the current errors for a target property.
    /// </summary>
    /// <param name="propertyName">The name of the property to clear errors for.</param>
    private void ClearErrorsForProperty(string propertyName)
    {
        if (!this.errors.TryGetValue(propertyName, out List<ValidationResult>? propertyErrors) ||
            propertyErrors.Count == 0)
        {
            return;
        }

        propertyErrors.Clear();

        this.totalErrors--;

        if (this.totalErrors == 0)
        {
            OnPropertyChanged(HasErrorsChangedEventArgs);
        }

        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Lazily creates or updates the shared validation context for a target property.
    /// </summary>
    /// <param name="propertyName">The target property name being validated.</param>
    /// <param name="displayName">The display name to expose for the target property.</param>
    /// <returns>The shared <see cref="ValidationContext"/> instance to use.</returns>
    private ValidationContext GetOrCreateUpdatedValidationContext(string propertyName, string displayName)
    {
#pragma warning disable IL2026 // The created ValidationContext object is used in a way that never calls reflection.
        ValidationContext context = this.validationContext ??= new ValidationContext(this, this.validationServiceProvider, this.validationItems);
#pragma warning restore IL2026

        context.MemberName = propertyName;
        context.DisplayName = displayName;

        return context;
    }

    /// <summary>
    /// Indicates the outcome of a property validation request.
    /// </summary>
    protected enum ValidationStatus
    {
        /// <summary>
        /// Validation succeeded.
        /// </summary>
        Success,

        /// <summary>
        /// The requested property was not handled.
        /// </summary>
        Unhandled,

        /// <summary>
        /// Validation failed.
        /// </summary>
        Error
    }
}
