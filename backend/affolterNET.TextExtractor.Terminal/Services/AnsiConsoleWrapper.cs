using System.Globalization;
using affolterNET.TextExtractor.Core.Helpers;
using Spectre.Console;

namespace affolterNET.TextExtractor.Terminal.Services;

public class AnsiConsoleWrapper
{
    private readonly EnumLogLevel _logLevel;
    internal static Style CurrentStyle { get; private set; } = Style.Plain;

    public AnsiConsoleWrapper(EnumLogLevel logLevel)
    {
        _logLevel = logLevel;
    }

    /// <summary>
    /// Writes the specified markup to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void Markup(EnumLogLevel logLevel, string value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.Markup(value);
    }

    /// <summary>
    /// Writes the specified markup to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write.</param>
    public void Markup(EnumLogLevel logLevel, string format, params object[] args)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.Markup(format, args);
    }

    /// <summary>
    /// Writes the specified markup to the console.
    /// <para/>
    /// All interpolation holes which contain a string are automatically escaped so you must not call <see cref="StringExtensions.EscapeMarkup"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// string input = args[0];
    /// string output = Process(input);
    /// AnsiConsole.MarkupInterpolated($"[blue]{input}[/] -> [green]{output}[/]");
    /// </code>
    /// </example>
    /// <param name="logLevel"></param>
    /// <param name="value">The interpolated string value to write.</param>
    public void MarkupInterpolated(EnumLogLevel logLevel, FormattableString value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.MarkupInterpolated(value);
    }

    /// <summary>
    /// Writes the specified markup to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write.</param>
    public void Markup(EnumLogLevel logLevel, IFormatProvider provider, string format, params object[] args)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.Markup(provider, format, args);
    }

    /// <summary>
    /// Writes the specified markup to the console.
    /// <para/>
    /// All interpolation holes which contain a string are automatically escaped so you must not call <see cref="StringExtensions.EscapeMarkup"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// string input = args[0];
    /// string output = Process(input);
    /// AnsiConsole.MarkupInterpolated(CultureInfo.InvariantCulture, $"[blue]{input}[/] -> [green]{output}[/]");
    /// </code>
    /// </example>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The interpolated string value to write.</param>
    public void MarkupInterpolated(EnumLogLevel logLevel, IFormatProvider provider, FormattableString value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.MarkupInterpolated(provider, value);
    }

    /// <summary>
    /// Writes the specified markup, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void MarkupLine(EnumLogLevel logLevel, string value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.MarkupLine(value);
    }

    /// <summary>
    /// Writes the specified markup, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write.</param>
    public void MarkupLine(EnumLogLevel logLevel, string format, params object[] args)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.MarkupLine(format, args);
    }

    /// <summary>
    /// Writes the specified markup, followed by the current line terminator, to the console.
    /// <para/>
    /// All interpolation holes which contain a string are automatically escaped so you must not call <see cref="StringExtensions.EscapeMarkup"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// string input = args[0];
    /// string output = Process(input);
    /// AnsiConsole.MarkupLineInterpolated($"[blue]{input}[/] -> [green]{output}[/]");
    /// </code>
    /// </example>
    /// <param name="logLevel"></param>
    /// <param name="value">The interpolated string value to write.</param>
    public void MarkupLineInterpolated(EnumLogLevel logLevel, FormattableString value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.MarkupLineInterpolated(value);
    }

    /// <summary>
    /// Writes the specified markup, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write.</param>
    public void MarkupLine(EnumLogLevel logLevel, IFormatProvider provider, string format, params object[] args)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.MarkupLine(provider, format, args);
    }

    /// <summary>
    /// Writes the specified markup, followed by the current line terminator, to the console.
    /// <para/>
    /// All interpolation holes which contain a string are automatically escaped so you must not call <see cref="StringExtensions.EscapeMarkup"/>.
    /// </summary>
    /// <example>
    /// <code>
    /// string input = args[0];
    /// string output = Process(input);
    /// AnsiConsole.MarkupLineInterpolated(CultureInfo.InvariantCulture, $"[blue]{input}[/] -> [green]{output}[/]");
    /// </code>
    /// </example>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The interpolated string value to write.</param>
    public void MarkupLineInterpolated(EnumLogLevel logLevel, IFormatProvider provider, FormattableString value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        AnsiConsole.MarkupLineInterpolated(provider, value);
    }

    /// <summary>
    /// Writes an empty line to the console.
    /// </summary>
    public void WriteLine(EnumLogLevel logLevel)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Writes the specified string value, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, string value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value, CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified 32-bit signed integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, int value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified 32-bit signed integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, int value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified 32-bit unsigned integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, uint value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified 32-bit unsigned integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, uint value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified 64-bit signed integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, long value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified 64-bit signed integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, long value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified 64-bit unsigned integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, ulong value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified 64-bit unsigned integer value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, ulong value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified single-precision floating-point
    /// value, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, float value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified single-precision floating-point
    /// value, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, float value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified double-precision floating-point
    /// value, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, double value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified double-precision floating-point
    /// value, followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, double value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified decimal value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, decimal value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified decimal value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, decimal value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the text representation of the specified boolean value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, bool value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the text representation of the specified boolean value,
    /// followed by the current line terminator, to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, bool value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the specified Unicode character, followed by the current
    /// line terminator, value to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, char value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the specified Unicode character, followed by the current
    /// line terminator, value to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, char value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        Console.WriteLine(value.ToString(provider), CurrentStyle);
    }

    /// <summary>
    /// Writes the specified array of Unicode characters, followed by the current
    /// line terminator, value to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, char[] value)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, value);
    }

    /// <summary>
    /// Writes the specified array of Unicode characters, followed by the current
    /// line terminator, value to the console.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">The value to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, char[] value)
    {
        if (Ignore(logLevel))
        {
            return;
        }
        
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        for (var index = 0; index < value.Length; index++)
        {
            Console.Write(value[index].ToString(provider), CurrentStyle);
        }

        Console.WriteLine();
    }

    /// <summary>
    /// Writes the text representation of the specified array of objects,
    /// followed by the current line terminator, to the console
    /// using the specified format information.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write.</param>
    public void WriteLine(EnumLogLevel logLevel, string format, params object[] args)
    {
        WriteLine(logLevel, CultureInfo.CurrentCulture, format, args);
    }

    /// <summary>
    /// Writes the text representation of the specified array of objects,
    /// followed by the current line terminator, to the console
    /// using the specified format information.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="format">A composite format string.</param>
    /// <param name="args">An array of objects to write.</param>
    public void WriteLine(EnumLogLevel logLevel, IFormatProvider provider, string format, params object[] args)
    {
        if (Ignore(logLevel))
        {
            return;
        }

        Console.WriteLine(string.Format(provider, format, args), CurrentStyle);
    }

    private bool Ignore(EnumLogLevel logLevel)
    {
        return (int)logLevel > (int)_logLevel;
    }
}