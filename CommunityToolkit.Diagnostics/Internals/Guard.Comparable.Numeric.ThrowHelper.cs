// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file under the MIT license.

using System;
using System.Diagnostics.CodeAnalysis;

namespace CommunityToolkit.Diagnostics;

/// <inheritdoc/>
partial class Guard
{
    /// <inheritdoc/>
    partial class ThrowHelper
    {
        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCloseTo(int,int,uint,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCloseTo(int value, int target, uint delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(int).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((double)((long)value - target)))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCloseTo(int,int,uint,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCloseTo(int value, int target, uint delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(int).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((double)((long)value - target)))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCloseTo(long,long,ulong,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCloseTo(long value, long target, ulong delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(long).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((decimal)value - target))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCloseTo(long,long,ulong,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCloseTo(long value, long target, ulong delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(long).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs((decimal)value - target))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCloseTo(float,float,float,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCloseTo(float value, float target, float delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(float).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCloseTo(float,float,float,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCloseTo(float value, float target, float delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(float).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCloseTo(double,double,double,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCloseTo(double value, double target, double delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(double).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCloseTo(double,double,double,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCloseTo(double value, double target, double delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(double).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsCloseTo(nint,nint,nuint,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsCloseTo(nint value, nint target, nuint delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(nint).ToTypeString()}) must be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}.", name);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> when <see cref="IsNotCloseTo(nint,nint,nuint,string)"/> fails.
        /// </summary>
        [DoesNotReturn]
        public static void ThrowArgumentExceptionForIsNotCloseTo(nint value, nint target, nuint delta, string name)
        {
            throw new ArgumentException($"Parameter {AssertString(name)} ({typeof(nint).ToTypeString()}) must not be within a distance of {AssertString(delta)} from {AssertString(target)}, was {AssertString(value)} and had a distance of {AssertString(Math.Abs(value - target))}.", name);
        }
    }
}
