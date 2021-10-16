# TimeZoneLab ReadMe

The objective of the two C# projects included in this repository is to document
the behavior of the `System.TimeZoneInfo` object and its properties, especially
when applied to times near the Daylight Saving Time transition times.

Another feature is that the character-mode project, `TimeZoneLab.exe`, provides a
comprehensive list of the time zones defined by the computer on which the
program executes.

This project relies upon several class libraries, primarily the __WizardWrx .NET API__
available at [https://github.com/txwizard/WizardWrx_NET_API](https://github.com/txwizard/WizardWrx_NET_API)
under a three-clause BSD license, by adding capabilities that meet the
specialized requirements of console programs and as a set of NuGet packages, all
of which belong to the `WizardWrx` namespace.

Though its relevance is peripheral, the project relies on another NuGet package,
`WizardWrx.ConsoleAppAids3`, which is fully documented at [https://txwizard.github.io/ConsoleAppAids3](https://txwizard.github.io/ConsoleAppAids3).

## Internal Documentation

The source code includes comprehenisve technical documentation, including XML to
generate IntelliSense help, from which the build engine generates XML documents,
which are included herein. Argument names follow Hungarian notation, to make the
type immediately evident in most cases. A lower case "p" precedes a type prefix,
to differentiate arguments from local variables, followed by a lower case "a" to
designate arguments that are arrays. Object variables have an initial underscore
and static variables begin with "s _ "; this naming scheme makes variable scope
crystal clear.

The classes in this code and that of the dependent libraries are thoroughly cross
referenced, and many properties and methods have working links to relevant MSDN pages.

## Versioning

The assemblies in this project implement semantic versioning.

## Road Map

Since this project is fundamentally a guinea pig for testing new code concepts,
there is no formal road map.

## Contributing

Though I created these assemblies to meet my individual development needs, I have
put a good bit of thought and care into their design. Moreover, since I will not
live forever, and I want these libraries to outlive me, I would be honored to
add contributions from others to it. My expectations are few, simple, easy to
meet, and intended to maintain the consistency of the code base and its API.

1.	__Naming Conventions__: I use Hungarian notation. Some claim that it has
outlived its usefulness. I think it remains useful because it encodes data
about the objects to which the names are applied that follows them wherever they
go, and convey it without help from IntelliSense.

2.	__Coding Style__: I have my editor set to leave spaces around every token.
This spacing has helped me quicly spot misplaced puncuation, such as the right
bracket that closes an array initializer that is in the wrong place, and it
makes mathematical expressions easier to read and mentally parse.

3.	__Comments__: I comment liberally and very deliberately. Of particular
importance are the comments that I append to the bracket that closes a block. It
does either or both of two things: link it to the opening statement, and
document which of two paths an __if__ statement is expected to follow most of
the time. When blocks get nested two, three, or four deep, they really earn
their keep.

4.	__Negative Conditions__: I do my best to avoid them, because they almost
always cause confusion. Astute observers will notice that they occur from time
to time, because there are _a few cases_ where they happen to be less confusing.

5.	__Array Initializers__: Arrays that have more than a very few initializers,
or that are initialized to long strings, are laid out as lists, with line
comments, if necessary, that describe the intent of each item.

6.	__Format Item Lists__: Lists of items that are paired with format items in
calls to `string.Format`, `Console.WriteLine`, and their relatives, are laid out
as arrays, even if there are too few to warrant one, and the comments show the
corresponding format item in context. This helps ensure that the items are
listed in the correct order, and that all format items are covered.

7.	__Symbolic Constants__: I use symbolic constants to document what a literal
value means in the context in which it appears, and to disambiguate tokens that
are easy to confuse, suzh as `1` and `l` (lower case L), `0` and `o` (lower case O),
literal spaces (1 and 2 spaces are common), underscores, the number `-1`, and so
forth. Literals that are widely applicable are defined in a set of classes that
comprise the majority of the root `WizardWrx` namespace.

8.	__Argument Lists__: I treat argument lists as arrays, and often comment each
argument with the name of the paramter that it represents. These lists help
guarantee that a long list of positional arguments is specified in the correct
order, especially when several are of the same type (e. g., two or more integer
arguments).

9.	__Triple-slash Comments__: These go on _everything_, even private members and
methods, so that everything supports IntelliSense, and it's easy to apply a tool
(I use DocFX.) to generate reference documentation.

With respect to the above items, you can expect me to be a nazi, though I shall
endeavor to give contributors a fair hearing when they have a good case.
Otherwise, please exercise your imagination, and submit your pull requests.
Contributors can expect prominent credit on the package page in the official
public repository. If you skim the headnotes of the code, you'll see that I take
great pains to give others credit when I icorporate their code into my projects,
even to the point of disclaiming copyright or leaving their copyright notice
intact. Along the same lines, the comments are liberally sprinkled with
references to articles and Stack Overflow discussions that contributed to the
work.