// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

/// <summary>
/// A base class for observable objects that also acts as recipients for messages. This class is an extension of
/// <c>ObservableObject</c> which also provides built-in support to use the <c>IMessenger</c> type.
/// </summary>
public abstract class ObservableRecipient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient"/> class.
    /// </summary>
    /// <remarks>
    /// This constructor will produce an instance that will use the <see cref="global::CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default"/> instance
    /// to perform requested operations. It will also be available locally through the <see cref="Messenger"/> property.
    /// </remarks>
    protected ObservableRecipient()
        : this(global::CommunityToolkit.Mvvm.Messaging.WeakReferenceMessenger.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient"/> class.
    /// </summary>
    /// <param name="messenger">The <see cref="global::CommunityToolkit.Mvvm.Messaging.IMessenger"/> instance to use to send messages.</param>
    protected ObservableRecipient(global::CommunityToolkit.Mvvm.Messaging.IMessenger messenger)
    {
        Messenger = messenger;
    }

    /// <summary>
    /// Gets the <see cref="global::CommunityToolkit.Mvvm.Messaging.IMessenger"/> instance in use.
    /// </summary>
    protected global::CommunityToolkit.Mvvm.Messaging.IMessenger Messenger { get; }

    private bool isActive;

    /// <summary>
    /// Gets or sets a value indicating whether the current view model is currently active.
    /// </summary>
    public bool IsActive
    {
        get => this.isActive;

        [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
            "When this property is set to true, the OnActivated() method will be invoked, which will register all necessary message handlers for this recipient. " +
            "This method requires the generated CommunityToolkit.Mvvm.Messaging.__Internals.__IMessengerExtensions type not to be removed to use the fast path. " +
            "If this type is removed by the linker, or if the target recipient was created dynamically and was missed by the source generator, a slower fallback " +
            "path using a compiled LINQ expression will be used. This will have more overhead in the first invocation of this method for any given recipient type. " +
            "Alternatively, OnActivated() can be manually overwritten, and registration can be done individually for each required message for this recipient.")]
        set
        {
            if (SetProperty(ref this.isActive, value, true))
            {
                if (value)
                {
                    OnActivated();
                }
                else
                {
                    OnDeactivated();
                }
            }
        }
    }

    /// <summary>
    /// Invoked whenever the <see cref="IsActive"/> property is set to <see langword="true"/>.
    /// Use this method to register to messages and do other initialization for this instance.
    /// </summary>
    [global::System.Diagnostics.CodeAnalysis.RequiresUnreferencedCode(
        "This method requires the generated CommunityToolkit.Mvvm.Messaging.__Internals.__IMessengerExtensions type not to be removed to use the fast path. " +
        "If this type is removed by the linker, or if the target recipient was created dynamically and was missed by the source generator, a slower fallback " +
        "path using a compiled LINQ expression will be used. This will have more overhead in the first invocation of this method for any given recipient type. " +
        "Alternatively, OnActivated() can be manually overwritten, and registration can be done individually for each required message for this recipient.")]
    protected virtual void OnActivated()
    {
        global::CommunityToolkit.Mvvm.Messaging.IMessengerExtensions.RegisterAll(Messenger, this);
    }

    /// <summary>
    /// Invoked whenever the <see cref="IsActive"/> property is set to <see langword="false"/>.
    /// Use this method to unregister from messages and do general cleanup for this instance.
    /// </summary>
    protected virtual void OnDeactivated()
    {
        Messenger.UnregisterAll(this);
    }

    /// <summary>
    /// Broadcasts a <see cref="global::CommunityToolkit.Mvvm.Messaging.Messages.PropertyChangedMessage{T}"/> with the specified
    /// parameters, without using any particular token (so using the default channel).
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The value of the property before it changed.</param>
    /// <param name="newValue">The value of the property after it changed.</param>
    /// <param name="propertyName">The name of the property that changed.</param>
    /// <remarks>
    /// You should override this method if you wish to customize the channel being
    /// used to send the message (eg. if you need to use a specific token for the channel).
    /// </remarks>
    protected virtual void Broadcast<T>(T oldValue, T newValue, string? propertyName)
    {
        var message = new global::CommunityToolkit.Mvvm.Messaging.Messages.PropertyChangedMessage<T>(this, propertyName, oldValue, newValue);

        _ = global::CommunityToolkit.Mvvm.Messaging.IMessengerExtensions.Send(Messenger, message);
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="broadcast">If <see langword="true"/>, <see cref="Broadcast{T}"/> will also be invoked.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// This method is just like <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{T}(ref T,T,string)"/>, just with the addition
    /// of the <paramref name="broadcast"/> parameter. As such, following the behavior of the base method,
    /// the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> and <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> events
    /// are not raised if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetProperty<T>([global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("newValue")] ref T field, T newValue, bool broadcast, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        T oldValue = field;

        bool propertyChanged = SetProperty(ref field, newValue, propertyName);

        if (propertyChanged && broadcast)
        {
            Broadcast(oldValue, newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> event.
    /// See additional notes about this overload in <see cref="SetProperty{T}(ref T,T,bool,string)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="global::System.Collections.Generic.IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="broadcast">If <see langword="true"/>, <see cref="Broadcast{T}"/> will also be invoked.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    protected bool SetProperty<T>([global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("newValue")] ref T field, T newValue, global::System.Collections.Generic.IEqualityComparer<T> comparer, bool broadcast, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        T oldValue = field;

        bool propertyChanged = SetProperty(ref field, newValue, comparer, propertyName);

        if (propertyChanged && broadcast)
        {
            Broadcast(oldValue, newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> event. Similarly to
    /// the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{T}(T,T,global::System.Action{T},string)"/> method, this overload should only be
    /// used when <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{T}(ref T,T,string)"/> can't be used directly.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="broadcast">If <see langword="true"/>, <see cref="Broadcast{T}"/> will also be invoked.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// This method is just like <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{T}(T,T,global::System.Action{T},string)"/>, just with the addition
    /// of the <paramref name="broadcast"/> parameter. As such, following the behavior of the base method,
    /// the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> and <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> events
    /// are not raised if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetProperty<T>(T oldValue, T newValue, global::System.Action<T> callback, bool broadcast, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        bool propertyChanged = SetProperty(oldValue, newValue, callback, propertyName);

        if (propertyChanged && broadcast)
        {
            Broadcast(oldValue, newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed,
    /// raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> event, updates the property with
    /// the new value, then raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> event.
    /// See additional notes about this overload in <see cref="SetProperty{T}(T,T,global::System.Action{T},bool,string)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="global::System.Collections.Generic.IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="broadcast">If <see langword="true"/>, <see cref="Broadcast{T}"/> will also be invoked.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    protected bool SetProperty<T>(T oldValue, T newValue, global::System.Collections.Generic.IEqualityComparer<T> comparer, global::System.Action<T> callback, bool broadcast, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        bool propertyChanged = SetProperty(oldValue, newValue, comparer, callback, propertyName);

        if (propertyChanged && broadcast)
        {
            Broadcast(oldValue, newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given nested property. If the value has changed,
    /// raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> event, updates the property and then raises the
    /// <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
    /// <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{TModel,T}(T,T,TModel,global::System.Action{TModel,T},string)"/>, with the difference being that this
    /// method is used to relay properties from a wrapped model in the current instance. For more info, see the docs for
    /// <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{TModel,T}(T,T,TModel,global::System.Action{TModel,T},string)"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="model">The model </param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="broadcast">If <see langword="true"/>, <see cref="Broadcast{T}"/> will also be invoked.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, global::System.Action<TModel, T> callback, bool broadcast, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        bool propertyChanged = SetProperty(oldValue, newValue, model, callback, propertyName);

        if (propertyChanged && broadcast)
        {
            Broadcast(oldValue, newValue, propertyName);
        }

        return propertyChanged;
    }

    /// <summary>
    /// Compares the current and new values for a given nested property. If the value has changed,
    /// raises the <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanging"/> event, updates the property and then raises the
    /// <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.PropertyChanged"/> event. The behavior mirrors that of
    /// <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{TModel,T}(T,T,global::System.Collections.Generic.IEqualityComparer{T},TModel,global::System.Action{TModel,T},string)"/>,
    /// with the difference being that this method is used to relay properties from a wrapped model in the
    /// current instance. For more info, see the docs for
    /// <see cref="global::CommunityToolkit.Mvvm.ComponentModel.ObservableObject.SetProperty{TModel,T}(T,T,global::System.Collections.Generic.IEqualityComparer{T},TModel,global::System.Action{TModel,T},string)"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="global::System.Collections.Generic.IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="model">The model </param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="broadcast">If <see langword="true"/>, <see cref="Broadcast{T}"/> will also be invoked.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    protected bool SetProperty<TModel, T>(T oldValue, T newValue, global::System.Collections.Generic.IEqualityComparer<T> comparer, TModel model, global::System.Action<TModel, T> callback, bool broadcast, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        bool propertyChanged = SetProperty(oldValue, newValue, comparer, model, callback, propertyName);

        if (propertyChanged && broadcast)
        {
            Broadcast(oldValue, newValue, propertyName);
        }

        return propertyChanged;
    }
}
