LogUtilities
============

A simple library to help with generating logs by using streams in C#.

## Quick Start

Writing Logs to Console:

```csharp
using LogUtilities;

using(var logger = LogStream.ToConsole())
{
	logger.DateTimeFormat = "s";

	logger.Write("Item1");
	logger.Write(" Item2");
	logger.WriteLine();
}
```

Writing all logs to a file:

```csharp
using LogUtilities;

using(var logger = LogStream.ToFile("/path/to/file.log")
{
	logger.Prefix = "MyLog";
	logger.DateTimeFormat = "s";

	logger.WriteLine("Log Entry One");
}
```

Writing Logs to a file with automatic file rotation.

```csharp
using LogUtilities;

using(var logger = new RotatingFileLog("/path/to/file.log")) {
	logger.MaxSize = 1000000;
	logger.MaxAge = 86400;
	logger.Prefix = "RotateLog";

	logger.WriteLine()
}
```

## Installation

Coming Soon.

## License

Copyright 2019 Kevin Gravier

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.