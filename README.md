LogUtilities
============

A simple library to help with generating logs by using streams in C#.

## Quick Start

Writing Logs to a custom stream:

```csharp
using LogUtilities;

using (var stream = new MemoryStream()) // Getting a custom stream
{ 
	using (var logger = new LogStream(stream))
	{
		logger.WriteLine("Some Data");
	}
}
```

Writing Logs to Console:

```csharp
using LogUtilities;

using(var logger = LogStream.ToConsole())
{
	logger.DateTimeFormat = "s";

	logger.Write("Item1", "Item2");
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

using(var logger = new RotatingFileLog("/path/to/file.log"))
{
	logger.MaxSize = 1000000;
	logger.MaxAge = 86400;
	logger.Prefix = "RotateLog";

	logger.WriteLine("Item1", "Item2");
}
```

## Installation

Coming Soon.

## Full Documentation

### AbstractLogStream

An abstract representation of LogStream. `AbstractLogStream` extends from Stream, and hence all
derived classes will be streams.

#### Properties

*System.Encoding* **Encoding** - Which encoding to use. Defaults to `System.Encoding.UTF8`

*string* **Prefix** - A string prefix each line with. If not set, no prefix will be prepended.

*string* **DateTimeFormat** A DateTime.ToString format string to prefix each line with. If not set, no prefix will be prepended.

*string* **Separator** - A string to separate the datetime, prefix, and text/text blocks.

#### Methods

*void* **Write(string text)** - Writes a string to the log.

*void* **Write(params string[] textBlocks)** - Writes blocks of strings to the log. Uses `Separator` to delineate the blocks.

*void* **WriteLine(string text)** - Writes a string to the log, followed by a new line.

*void* **WriteLine(params string[] textBlocks)** - Writes blocks of strings to the log, followed by a new line. Uses `Separator` to delineate the blocks.

*void* **WriteLine()** - Writes a newline to the log.

----

### LogStream

`LogStream` is the base class of LogUtilities. It extends `AbstractLogStream`, and hence can be used like any other stream.

#### Static Methods

*LogStream* **ToConsole(LogStreamConsoleType type)** - Creates an instance of `LogStream` which outputs to the console.

*LogStream* **ToFile(string path)** - Creates an instance of `LogStream` which outputs to a file.

#### Constructor

**LogStream(Stream baseStream)** - Pass in any writable stream to have logged output written to that stream.

----

### RotatingFileLogStream

RotatingFileLogStream outputs the log to a file, which is rotated by either size of age of the file. It extends AbstractLogStream, 
and hence can be used like any other stream.

#### Constructor

**RotatingFileLogStream**(string path) - Creates an instance of RotatingFileLogStream which writes to a file.

#### Properties

*long* **MaxSize** - The max size in bytes of the log file.

*int* **MaxAge** - The max age in seconds of the log file.

*int* **MaxFiles** - The maximum number of log files to keep.

*bool* **AutoFlush** - Automatically flush the internal buffer. If not set to true, will hold the log in memory until `Flush()` is called.

## Useful Example

```csharp
using LogUtilities;

var logger = new RotatingFileLogStream("./logs/main.log")
{
	DateTimeFormat = "s";
	MaxSize = 5000000; // 5MB
	AutoFlush = true;
};

var networkLogger = new LogStream(logger) {
	Prefix = "Network";
};

var diskLogger = new LogStream(logger) {
	Prefix = "Disk";
};


/// In Network Code

networkLogger.WriteLine("IN", "Some Data");
networkLogger.WriteLine("OUT", "Some Data");

// In Disk Code

diskLogger.WriteLine("Disk Started");
diskLogger.WriteLine("Disk Stopped");

// In shutdown code

networkLogger.Dispose();
diskLogger.Dispose();
logger.Dispose();
```

## License

Copyright 2019 Kevin Gravier

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.