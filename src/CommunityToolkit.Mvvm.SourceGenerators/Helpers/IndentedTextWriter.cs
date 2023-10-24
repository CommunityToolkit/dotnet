// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// This file is ported and adapted from ComputeSharp (Sergio0694/ComputeSharp),
// more info in ThirdPartyNotices.txt in the root of the project.

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

#pragma warning disable CS0618

namespace CommunityToolkit.Mvvm.SourceGenerators.Helpers;

/// <summary>
/// A helper type to build sequences of values with pooled buffers.
/// </summary>
internal sealed class IndentedTextWriter : IDisposable
{
    /// <summary>
    /// The default indentation (4 spaces).
    /// </summary>
    private const string DefaultIndentation = "    ";

    /// <summary>
    /// The default new line (<c>'\n'</c>).
    /// </summary>
    private const char DefaultNewLine = '\n';

    /// <summary>
    /// The <see cref="ImmutableArrayBuilder{T}.Writer"/> instance that text will be written to.
    /// </summary>
    private readonly ImmutableArrayBuilder<char>.Writer writer = new();

    /// <summary>
    /// The current indentation level.
    /// </summary>
    private int currentIndentationLevel;

    /// <summary>
    /// The current indentation, as text.
    /// </summary>
    private string currentIndentation;

    /// <summary>
    /// The cached array of available indentations, as text.
    /// </summary>
    private string[] availableIndentations;

    /// <summary>
    /// Creates a new <see cref="IndentedTextWriter"/> object.
    /// </summary>
    public IndentedTextWriter()
    {
        this.currentIndentationLevel = 0;
        this.currentIndentation = "";
        this.availableIndentations = new string[4];
        this.availableIndentations[0] = "";

        for (int i = 1, n = this.availableIndentations.Length; i < n; i++)
        {
            this.availableIndentations[i] = this.availableIndentations[i - 1] + DefaultIndentation;
        }
    }

    /// <summary>
    /// Increases the current indentation level.
    /// </summary>
    public void IncreaseIndent()
    {
        this.currentIndentationLevel++;

        if (this.currentIndentationLevel == this.availableIndentations.Length)
        {
            Array.Resize(ref this.availableIndentations, this.availableIndentations.Length * 2);
        }

        // Set both the current indentation and the current position in the indentations
        // array to the expected indentation for the incremented level (ie. one level more).
        this.currentIndentation = this.availableIndentations[this.currentIndentationLevel]
            ??= this.availableIndentations[this.currentIndentationLevel - 1] + DefaultIndentation;
    }

    /// <summary>
    /// Decreases the current indentation level.
    /// </summary>
    public void DecreaseIndent()
    {
        this.currentIndentationLevel--;
        this.currentIndentation = this.availableIndentations[this.currentIndentationLevel];
    }

    /// <summary>
    /// Writes a block to the underlying buffer.
    /// </summary>
    /// <returns>A <see cref="Block"/> value to close the open block with.</returns>
    public Block WriteBlock()
    {
        WriteLine("{");
        IncreaseIndent();

        return new(this);
    }

    /// <summary>
    /// Writes content to the underlying buffer.
    /// </summary>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void Write(string content, bool isMultiline = false)
    {
        Write(content.AsSpan(), isMultiline);
    }

    /// <summary>
    /// Writes content to the underlying buffer.
    /// </summary>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void Write(ReadOnlySpan<char> content, bool isMultiline = false)
    {
        if (isMultiline)
        {
            while (content.Length > 0)
            {
                int newLineIndex = content.IndexOf(DefaultNewLine);

                if (newLineIndex < 0)
                {
                    // There are no new lines left, so the content can be written as a single line
                    WriteRawText(content);

                    break;
                }
                else
                {
                    // Write the current line
                    WriteRawText(content[..newLineIndex]);
                    WriteLine();

                    // Move past the new line character (the result could be an empty span)
                    content = content[(newLineIndex + 1)..];
                }
            }
        }
        else
        {
            WriteRawText(content);
        }
    }

    /// <summary>
    /// Writes content to the underlying buffer.
    /// </summary>
    /// <param name="handler">The interpolated string handler with content to write.</param>
    public void Write([InterpolatedStringHandlerArgument("")] ref WriteInterpolatedStringHandler handler)
    {
        _ = this;
    }

    /// <summary>
    /// Writes content to the underlying buffer depending on an input condition.
    /// </summary>
    /// <param name="condition">The condition to use to decide whether or not to write content.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void WriteIf(bool condition, string content, bool isMultiline = false)
    {
        if (condition)
        {
            Write(content.AsSpan(), isMultiline);
        }
    }

    /// <summary>
    /// Writes content to the underlying buffer depending on an input condition.
    /// </summary>
    /// <param name="condition">The condition to use to decide whether or not to write content.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void WriteIf(bool condition, ReadOnlySpan<char> content, bool isMultiline = false)
    {
        if (condition)
        {
            Write(content, isMultiline);
        }
    }

    /// <summary>
    /// Writes content to the underlying buffer depending on an input condition.
    /// </summary>
    /// <param name="condition">The condition to use to decide whether or not to write content.</param>
    /// <param name="handler">The interpolated string handler with content to write.</param>
    public void WriteIf(bool condition, [InterpolatedStringHandlerArgument("", nameof(condition))] ref WriteIfInterpolatedStringHandler handler)
    {
        _ = this;
    }

    /// <summary>
    /// Writes a line to the underlying buffer.
    /// </summary>
    /// <param name="skipIfPresent">Indicates whether to skip adding the line if there already is one.</param>
    public void WriteLine(bool skipIfPresent = false)
    {
        if (skipIfPresent && this.writer.WrittenSpan is [.., '\n', '\n'])
        {
            return;
        }

        this.writer.Add(DefaultNewLine);
    }

    /// <summary>
    /// Writes content to the underlying buffer and appends a trailing new line.
    /// </summary>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void WriteLine(string content, bool isMultiline = false)
    {
        WriteLine(content.AsSpan(), isMultiline);
    }

    /// <summary>
    /// Writes content to the underlying buffer and appends a trailing new line.
    /// </summary>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void WriteLine(ReadOnlySpan<char> content, bool isMultiline = false)
    {
        Write(content, isMultiline);
        WriteLine();
    }

    /// <summary>
    /// Writes content to the underlying buffer and appends a trailing new line.
    /// </summary>
    /// <param name="handler">The interpolated string handler with content to write.</param>
    public void WriteLine([InterpolatedStringHandlerArgument("")] ref WriteInterpolatedStringHandler handler)
    {
        WriteLine();
    }

    /// <summary>
    /// Writes a line to the underlying buffer depending on an input condition.
    /// </summary>
    /// <param name="condition">The condition to use to decide whether or not to write content.</param>
    /// <param name="skipIfPresent">Indicates whether to skip adding the line if there already is one.</param>
    public void WriteLineIf(bool condition, bool skipIfPresent = false)
    {
        if (condition)
        {
            WriteLine(skipIfPresent);
        }
    }

    /// <summary>
    /// Writes content to the underlying buffer and appends a trailing new line depending on an input condition.
    /// </summary>
    /// <param name="condition">The condition to use to decide whether or not to write content.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void WriteLineIf(bool condition, string content, bool isMultiline = false)
    {
        if (condition)
        {
            WriteLine(content.AsSpan(), isMultiline);
        }
    }

    /// <summary>
    /// Writes content to the underlying buffer and appends a trailing new line depending on an input condition.
    /// </summary>
    /// <param name="condition">The condition to use to decide whether or not to write content.</param>
    /// <param name="content">The content to write.</param>
    /// <param name="isMultiline">Whether the input content is multiline.</param>
    public void WriteLineIf(bool condition, ReadOnlySpan<char> content, bool isMultiline = false)
    {
        if (condition)
        {
            Write(content, isMultiline);
            WriteLine();
        }
    }

    /// <summary>
    /// Writes content to the underlying buffer and appends a trailing new line depending on an input condition.
    /// </summary>
    /// <param name="condition">The condition to use to decide whether or not to write content.</param>
    /// <param name="handler">The interpolated string handler with content to write.</param>
    public void WriteLineIf(bool condition, [InterpolatedStringHandlerArgument("", nameof(condition))] ref WriteIfInterpolatedStringHandler handler)
    {
        if (condition)
        {
            WriteLine();
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return this.writer.WrittenSpan.Trim().ToString();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.writer.Dispose();
    }

    /// <summary>
    /// Writes raw text to the underlying buffer, adding leading indentation if needed.
    /// </summary>
    /// <param name="content">The raw text to write.</param>
    private void WriteRawText(ReadOnlySpan<char> content)
    {
        if (this.writer.Count == 0 || this.writer.WrittenSpan[^1] == DefaultNewLine)
        {
            this.writer.AddRange(this.currentIndentation.AsSpan());
        }

        this.writer.AddRange(content);
    }

    /// <summary>
    /// A delegate representing a callback to write data into an <see cref="IndentedTextWriter"/> instance.
    /// </summary>
    /// <typeparam name="T">The type of data to use.</typeparam>
    /// <param name="value">The input data to use to write into <paramref name="writer"/>.</param>
    /// <param name="writer">The <see cref="IndentedTextWriter"/> instance to write into.</param>
    public delegate void Callback<T>(T value, IndentedTextWriter writer);

    /// <summary>
    /// Represents an indented block that needs to be closed.
    /// </summary>
    public struct Block : IDisposable
    {
        /// <summary>
        /// The <see cref="IndentedTextWriter"/> instance to write to.
        /// </summary>
        private IndentedTextWriter? writer;

        /// <summary>
        /// Creates a new <see cref="Block"/> instance with the specified parameters.
        /// </summary>
        /// <param name="writer">The input <see cref="IndentedTextWriter"/> instance to wrap.</param>
        [Obsolete("Use IndentedTextWriter.OpenBlock() instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Block(IndentedTextWriter writer)
        {
            this.writer = writer;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            IndentedTextWriter? writer = this.writer;

            this.writer = null;

            if (writer is not null)
            {
                writer.DecreaseIndent();
                writer.WriteLine("}");
            }
        }
    }

    /// <summary>
    /// Provides a handler used by the language compiler to append interpolated strings into <see cref="IndentedTextWriter"/> instances.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    public readonly ref struct WriteInterpolatedStringHandler
    {
        /// <summary>The associated <see cref="IndentedTextWriter"/> to which to append.</summary>
        private readonly IndentedTextWriter writer;

        /// <summary>Creates a handler used to append an interpolated string into a <see cref="StringBuilder"/>.</summary>
        /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
        /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
        /// <param name="writer">The associated <see cref="IndentedTextWriter"/> to which to append.</param>
        /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
        public WriteInterpolatedStringHandler(int literalLength, int formattedCount, IndentedTextWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>Writes the specified string to the handler.</summary>
        /// <param name="value">The string to write.</param>
        public void AppendLiteral(string value)
        {
            this.writer.Write(value);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        public void AppendFormatted(string? value)
        {
            AppendFormatted<string?>(value);
        }

        /// <summary>Writes the specified character span to the handler.</summary>
        /// <param name="value">The span to write.</param>
        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            this.writer.Write(value);
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value)
        {
            if (value is not null)
            {
                this.writer.Write(value.ToString());
            }
        }

        /// <summary>Writes the specified value to the handler.</summary>
        /// <param name="value">The value to write.</param>
        /// <param name="format">The format string.</param>
        /// <typeparam name="T">The type of the value to write.</typeparam>
        public void AppendFormatted<T>(T value, string? format)
        {
            if (value is IFormattable)
            {
                this.writer.Write(((IFormattable)value).ToString(format, CultureInfo.InvariantCulture));
            }
            else if (value is not null)
            {
                this.writer.Write(value.ToString());
            }
        }
    }

    /// <summary>
    /// Provides a handler used by the language compiler to conditionally append interpolated strings into <see cref="IndentedTextWriter"/> instances.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [InterpolatedStringHandler]
    public readonly ref struct WriteIfInterpolatedStringHandler
    {
        /// <summary>The associated <see cref="WriteInterpolatedStringHandler"/> to use.</summary>
        private readonly WriteInterpolatedStringHandler handler;

        /// <summary>Creates a handler used to append an interpolated string into a <see cref="StringBuilder"/>.</summary>
        /// <param name="literalLength">The number of constant characters outside of interpolation expressions in the interpolated string.</param>
        /// <param name="formattedCount">The number of interpolation expressions in the interpolated string.</param>
        /// <param name="writer">The associated <see cref="IndentedTextWriter"/> to which to append.</param>
        /// <param name="condition">The condition to use to decide whether or not to write content.</param>
        /// <param name="shouldAppend">A value indicating whether formatting should proceed.</param>
        /// <remarks>This is intended to be called only by compiler-generated code. Arguments are not validated as they'd otherwise be for members intended to be used directly.</remarks>
        public WriteIfInterpolatedStringHandler(int literalLength, int formattedCount, IndentedTextWriter writer, bool condition, out bool shouldAppend)
        {
            if (condition)
            {
                this.handler = new WriteInterpolatedStringHandler(literalLength, formattedCount, writer);

                shouldAppend = true;
            }
            else
            {
                this.handler = default;

                shouldAppend = false;
            }
        }

        /// <inheritdoc cref="WriteInterpolatedStringHandler.AppendLiteral(string)"/>
        public void AppendLiteral(string value)
        {
            this.handler.AppendLiteral(value);
        }

        /// <inheritdoc cref="WriteInterpolatedStringHandler.AppendFormatted(string?)"/>
        public void AppendFormatted(string? value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="WriteInterpolatedStringHandler.AppendFormatted(ReadOnlySpan{char})"/>
        public void AppendFormatted(ReadOnlySpan<char> value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="WriteInterpolatedStringHandler.AppendFormatted{T}(T)"/>
        public void AppendFormatted<T>(T value)
        {
            this.handler.AppendFormatted(value);
        }

        /// <inheritdoc cref="WriteInterpolatedStringHandler.AppendFormatted{T}(T, string?)"/>
        public void AppendFormatted<T>(T value, string? format)
        {
            this.handler.AppendFormatted(value, format);
        }
    }
}