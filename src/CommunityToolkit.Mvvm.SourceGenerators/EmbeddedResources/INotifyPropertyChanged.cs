// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/// <summary>
/// A base class for objects implementing <see cref="global::System.ComponentModel.INotifyPropertyChanged"/>.
/// </summary>
public abstract class INotifyPropertyChanged : global::System.ComponentModel.INotifyPropertyChanged
{
    /// <inheritdoc cref="global::System.ComponentModel.INotifyPropertyChanged.PropertyChanged"/>
    public event global::System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="e">The input <see cref="global::System.ComponentModel.PropertyChangedEventArgs"/> instance.</param>
    protected virtual void OnPropertyChanged(global::System.ComponentModel.PropertyChangedEventArgs e)
    {
        PropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    protected void OnPropertyChanged([global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed, updates
    /// the property with the new value, then raises the <see cref="PropertyChanged"/> event.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanged"/> event is not raised if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetProperty<T>([global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("newValue")] ref T field, T newValue, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;

        OnPropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed, updates
    /// the property with the new value, then raises the <see cref="PropertyChanged"/> event.
    /// See additional notes about this overload in <see cref="SetProperty{T}(ref T,T,string)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="field">The field storing the property's value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="global::System.Collections.Generic.IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    protected bool SetProperty<T>([global::System.Diagnostics.CodeAnalysis.NotNullIfNotNull("newValue")] ref T field, T newValue, global::System.Collections.Generic.IEqualityComparer<T> comparer, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (comparer.Equals(field, newValue))
        {
            return false;
        }

        field = newValue;

        OnPropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed, updates
    /// the property with the new value, then raises the <see cref="PropertyChanged"/> event.
    /// This overload is much less efficient than <see cref="SetProperty{T}(ref T,T,string)"/> and it
    /// should only be used when the former is not viable (eg. when the target property being
    /// updated does not directly expose a backing field that can be passed by reference).
    /// For performance reasons, it is recommended to use a stateful callback if possible through
    /// the <see cref="SetProperty{TModel,T}(T,T,TModel,global::System.Action{TModel,T},string?)"/> whenever possible
    /// instead of this overload, as that will allow the C# compiler to cache the input callback and
    /// reduce the memory allocations. More info on that overload are available in the related XML
    /// docs. This overload is here for completeness and in cases where that is not applicable.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanged"/> event is not raised if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetProperty<T>(T oldValue, T newValue, global::System.Action<T> callback, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        callback(newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Compares the current and new values for a given property. If the value has changed, updates
    /// the property with the new value, then raises the <see cref="PropertyChanged"/> event.
    /// See additional notes about this overload in <see cref="SetProperty{T}(T,T,global::System.Action{T},string)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the property that changed.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="global::System.Collections.Generic.IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    protected bool SetProperty<T>(T oldValue, T newValue, global::System.Collections.Generic.IEqualityComparer<T> comparer, global::System.Action<T> callback, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

        callback(newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Compares the current and new values for a given nested property. If the value has changed,
    /// updates the property and then raises the <see cref="PropertyChanged"/> event.
    /// The behavior mirrors that of <see cref="SetProperty{T}(ref T,T,string)"/>,
    /// with the difference being that this method is used to relay properties from a wrapped model in the
    /// current instance. This type is useful when creating wrapping, bindable objects that operate over
    /// models that lack support for notification (eg. for CRUD operations).
    /// Suppose we have this model (eg. for a database row in a table):
    /// <code>
    /// public class Person
    /// {
    ///     public string Name { get; set; }
    /// }
    /// </code>
    /// We can then use a property to wrap instances of this type into our observable model (which supports
    /// notifications), injecting the notification to the properties of that model, like so:
    /// <code>
    /// [INotifyPropertyChanged]
    /// public partial class BindablePerson
    /// {
    ///     public Model { get; }
    ///
    ///     public BindablePerson(Person model)
    ///     {
    ///         Model = model;
    ///     }
    ///
    ///     public string Name
    ///     {
    ///         get => Model.Name;
    ///         set => Set(Model.Name, value, Model, (model, name) => model.Name = name);
    ///     }
    /// }
    /// </code>
    /// This way we can then use the wrapping object in our application, and all those "proxy" properties will
    /// also raise notifications when changed. Note that this method is not meant to be a replacement for
    /// <see cref="SetProperty{T}(ref T,T,string)"/>, and it should only be used when relaying properties to a model that
    /// doesn't support notifications, and only if you can't implement notifications to that model directly (eg. by having
    /// it implement <see cref="global::System.ComponentModel.INotifyPropertyChanged"/>). The syntax relies on passing the target model and a stateless callback
    /// to allow the C# compiler to cache the function, which results in much better performance and no memory usage.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="model">The model containing the property being updated.</param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanged"/> event is not raised if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetProperty<TModel, T>(T oldValue, T newValue, TModel model, global::System.Action<TModel, T> callback, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        if (global::System.Collections.Generic.EqualityComparer<T>.Default.Equals(oldValue, newValue))
        {
            return false;
        }

        callback(model, newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Compares the current and new values for a given nested property. If the value has changed,
    /// updates the property and then raises the <see cref="PropertyChanged"/> event.
    /// The behavior mirrors that of <see cref="SetProperty{T}(ref T,T,string)"/>,
    /// with the difference being that this method is used to relay properties from a wrapped model in the
    /// current instance. See additional notes about this overload in <see cref="SetProperty{TModel,T}(T,T,TModel,global::System.Action{TModel,T},string)"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of model whose property (or field) to set.</typeparam>
    /// <typeparam name="T">The type of property (or field) to set.</typeparam>
    /// <param name="oldValue">The current property value.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="comparer">The <see cref="global::System.Collections.Generic.IEqualityComparer{T}"/> instance to use to compare the input values.</param>
    /// <param name="model">The model containing the property being updated.</param>
    /// <param name="callback">The callback to invoke to set the target property value, if a change has occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    protected bool SetProperty<TModel, T>(T oldValue, T newValue, global::System.Collections.Generic.IEqualityComparer<T> comparer, TModel model, global::System.Action<TModel, T> callback, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        where TModel : class
    {
        if (comparer.Equals(oldValue, newValue))
        {
            return false;
        }

        callback(model, newValue);

        OnPropertyChanged(propertyName);

        return true;
    }

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing field for a property).
    /// If the value has changed, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// The behavior mirrors that of <see cref="SetProperty{T}(ref T,T,string)"/>, with the difference being that
    /// this method will also monitor the new value of the property (a generic <see cref="global::System.Threading.Tasks.Task"/>) and will also
    /// raise the <see cref="PropertyChanged"/> again for the target property when it completes.
    /// This can be used to update bindings observing that <see cref="global::System.Threading.Tasks.Task"/> or any of its properties.
    /// This method and its overload specifically rely on the <see cref="TaskNotifier"/> type, which needs
    /// to be used in the backing field for the target <see cref="global::System.Threading.Tasks.Task"/> property. The field doesn't need to be
    /// initialized, as this method will take care of doing that automatically. The <see cref="TaskNotifier"/>
    /// type also includes an implicit operator, so it can be assigned to any <see cref="global::System.Threading.Tasks.Task"/> instance directly.
    /// Here is a sample property declaration using this method:
    /// <code>
    /// private TaskNotifier myTask;
    ///
    /// public Task MyTask
    /// {
    ///     get => myTask;
    ///     private set => SetPropertyAndNotifyOnCompletion(ref myTask, value);
    /// }
    /// </code>
    /// </summary>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanged"/> event is not raised if the current and new value for the target property are
    /// the same. The return value being <see langword="true"/> only indicates that the new value being assigned to
    /// <paramref name="taskNotifier"/> is different than the previous one, and it does not mean the new
    /// <see cref="global::System.Threading.Tasks.Task"/> instance passed as argument is in any particular state.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion([global::System.Diagnostics.CodeAnalysis.NotNull] ref TaskNotifier? taskNotifier, global::System.Threading.Tasks.Task? newValue, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier(), newValue, null, propertyName);
    }

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing field for a property).
    /// If the value has changed, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// This method is just like <see cref="SetPropertyAndNotifyOnCompletion(ref TaskNotifier,global::System.Threading.Tasks.Task,string)"/>,
    /// with the difference being an extra <see cref="global::System.Action{T}"/> parameter with a callback being invoked
    /// either immediately, if the new task has already completed or is <see langword="null"/>, or upon completion.
    /// </summary>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanged"/> event is not raised if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion([global::System.Diagnostics.CodeAnalysis.NotNull] ref TaskNotifier? taskNotifier, global::System.Threading.Tasks.Task? newValue, global::System.Action<global::System.Threading.Tasks.Task?> callback, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier(), newValue, callback, propertyName);
    }

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing field for a property).
    /// If the value has changed, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// The behavior mirrors that of <see cref="SetProperty{T}(ref T,T,string)"/>, with the difference being that
    /// this method will also monitor the new value of the property (a generic <see cref="global::System.Threading.Tasks.Task"/>) and will also
    /// raise the <see cref="PropertyChanged"/> again for the target property when it completes.
    /// This can be used to update bindings observing that <see cref="global::System.Threading.Tasks.Task"/> or any of its properties.
    /// This method and its overload specifically rely on the <see cref="TaskNotifier{T}"/> type, which needs
    /// to be used in the backing field for the target <see cref="global::System.Threading.Tasks.Task"/> property. The field doesn't need to be
    /// initialized, as this method will take care of doing that automatically. The <see cref="TaskNotifier{T}"/>
    /// type also includes an implicit operator, so it can be assigned to any <see cref="global::System.Threading.Tasks.Task"/> instance directly.
    /// Here is a sample property declaration using this method:
    /// <code>
    /// private TaskNotifier&lt;int&gt; myTask;
    ///
    /// public Task&lt;int&gt; MyTask
    /// {
    ///     get => myTask;
    ///     private set => SetPropertyAndNotifyOnCompletion(ref myTask, value);
    /// }
    /// </code>
    /// </summary>
    /// <typeparam name="T">The type of result for the <see cref="global::System.Threading.Tasks.Task{TResult}"/> to set and monitor.</typeparam>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanged"/> event is not raised if the current and new value for the target property are
    /// the same. The return value being <see langword="true"/> only indicates that the new value being assigned to
    /// <paramref name="taskNotifier"/> is different than the previous one, and it does not mean the new
    /// <see cref="global::System.Threading.Tasks.Task{TResult}"/> instance passed as argument is in any particular state.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion<T>([global::System.Diagnostics.CodeAnalysis.NotNull] ref TaskNotifier<T>? taskNotifier, global::System.Threading.Tasks.Task<T>? newValue, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier<T>(), newValue, null, propertyName);
    }

    /// <summary>
    /// Compares the current and new values for a given field (which should be the backing field for a property).
    /// If the value has changed, updates the field and then raises the <see cref="PropertyChanged"/> event.
    /// This method is just like <see cref="SetPropertyAndNotifyOnCompletion{T}(ref TaskNotifier{T},global::System.Threading.Tasks.Task{T},string)"/>,
    /// with the difference being an extra <see cref="global::System.Action{T}"/> parameter with a callback being invoked
    /// either immediately, if the new task has already completed or is <see langword="null"/>, or upon completion.
    /// </summary>
    /// <typeparam name="T">The type of result for the <see cref="global::System.Threading.Tasks.Task{TResult}"/> to set and monitor.</typeparam>
    /// <param name="taskNotifier">The field notifier to modify.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    /// <remarks>
    /// The <see cref="PropertyChanged"/> event is not raised if the current and new value for the target property are the same.
    /// </remarks>
    protected bool SetPropertyAndNotifyOnCompletion<T>([global::System.Diagnostics.CodeAnalysis.NotNull] ref TaskNotifier<T>? taskNotifier, global::System.Threading.Tasks.Task<T>? newValue, global::System.Action<global::System.Threading.Tasks.Task<T>?> callback, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
    {
        return SetPropertyAndNotifyOnCompletion(taskNotifier ??= new TaskNotifier<T>(), newValue, callback, propertyName);
    }

    /// <summary>
    /// Implements the notification logic for the related methods.
    /// </summary>
    /// <typeparam name="TTask">The type of <see cref="global::System.Threading.Tasks.Task"/> to set and monitor.</typeparam>
    /// <param name="taskNotifier">The field notifier.</param>
    /// <param name="newValue">The property's value after the change occurred.</param>
    /// <param name="callback">(optional) A callback to invoke to update the property value.</param>
    /// <param name="propertyName">(optional) The name of the property that changed.</param>
    /// <returns><see langword="true"/> if the property was changed, <see langword="false"/> otherwise.</returns>
    private bool SetPropertyAndNotifyOnCompletion<TTask>(ITaskNotifier<TTask> taskNotifier, TTask? newValue, global::System.Action<TTask?>? callback, [global::System.Runtime.CompilerServices.CallerMemberName] string? propertyName = null)
        where TTask : global::System.Threading.Tasks.Task
    {
        if (ReferenceEquals(taskNotifier.Task, newValue))
        {
            return false;
        }

        bool isAlreadyCompletedOrNull = newValue?.IsCompleted ?? true;

        taskNotifier.Task = newValue;

        OnPropertyChanged(propertyName);

        if (isAlreadyCompletedOrNull)
        {
            if (callback != null)
            {
                callback(newValue);
            }

            return true;
        }

        async void MonitorTask()
        {
            await global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__TaskExtensions.GetAwaitableWithoutEndValidation(newValue!);

            if (ReferenceEquals(taskNotifier.Task, newValue))
            {
                OnPropertyChanged(propertyName);
            }

            if (callback != null)
            {
                callback(newValue);
            }
        }

        MonitorTask();

        return true;
    }

    /// <summary>
    /// An interface for task notifiers of a specified type.
    /// </summary>
    /// <typeparam name="TTask">The type of value to store.</typeparam>
    private interface ITaskNotifier<TTask>
        where TTask : global::System.Threading.Tasks.Task
    {
        /// <summary>
        /// Gets or sets the wrapped <typeparamref name="TTask"/> value.
        /// </summary>
        TTask? Task { get; set; }
    }

    /// <summary>
    /// A wrapping class that can hold a <see cref="global::System.Threading.Tasks.Task"/> value.
    /// </summary>
    protected sealed class TaskNotifier : ITaskNotifier<global::System.Threading.Tasks.Task>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskNotifier"/> class.
        /// </summary>
        internal TaskNotifier()
        {
        }

        private global::System.Threading.Tasks.Task? task;

        /// <inheritdoc/>
        global::System.Threading.Tasks.Task? ITaskNotifier<global::System.Threading.Tasks.Task>.Task
        {
            get => this.task;
            set => this.task = value;
        }

        /// <summary>
        /// Unwraps the <see cref="global::System.Threading.Tasks.Task"/> value stored in the current instance.
        /// </summary>
        /// <param name="notifier">The input <see cref="TaskNotifier{TTask}"/> instance.</param>
        public static implicit operator global::System.Threading.Tasks.Task?(TaskNotifier? notifier)
        {
            return notifier?.task;
        }
    }

    /// <summary>
    /// A wrapping class that can hold a <see cref="global::System.Threading.Tasks.Task{T}"/> value.
    /// </summary>
    /// <typeparam name="T">The type of value for the wrapped <see cref="global::System.Threading.Tasks.Task{T}"/> instance.</typeparam>
    protected sealed class TaskNotifier<T> : ITaskNotifier<global::System.Threading.Tasks.Task<T>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaskNotifier{TTask}"/> class.
        /// </summary>
        internal TaskNotifier()
        {
        }

        private global::System.Threading.Tasks.Task<T>? task;

        /// <inheritdoc/>
        global::System.Threading.Tasks.Task<T>? ITaskNotifier<global::System.Threading.Tasks.Task<T>>.Task
        {
            get => this.task;
            set => this.task = value;
        }

        /// <summary>
        /// Unwraps the <see cref="global::System.Threading.Tasks.Task{T}"/> value stored in the current instance.
        /// </summary>
        /// <param name="notifier">The input <see cref="TaskNotifier{TTask}"/> instance.</param>
        public static implicit operator global::System.Threading.Tasks.Task<T>?(TaskNotifier<T>? notifier)
        {
            return notifier?.task;
        }
    }
}