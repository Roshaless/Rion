# Rion

**Rion** is a high-performance .NET library and CLI tool for reading, writing, and converting Riot String Table (`.stringtable`) files — the binary format used by League of Legends and other Riot Games titles for localized string data.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.txt)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)

---

## Features

- **Blazing fast I/O** — Uses memory-mapped files, `unsafe` pointers, and AOT-compatible patterns for zero-copy reads and writes.
- **Multiple hash algorithms** — Supports XxHash3 and XxHash64 with configurable bit masks, matching all known RST file versions (v2–v5).
- **Bidirectional conversion** — Convert between binary `.stringtable` and human-readable `.json` in either direction.
- **Reverse-hash lookup** — Preload known string→hash mappings for human-readable hash resolution.
- **String interning** — `StringRefManager` deduplicates strings across loaded tables for minimal memory usage.
- **AOT ready** — Fully compatible with .NET Native AOT publishing.

---

## Project Structure

```
Rion/
├── Rion.sln
├── README.md
├── LICENSE.txt
├── .gitignore
├── .editorconfig
└── src/
    ├── Rion.Core/          # Core library — read/write/convert .stringtable files
    │   ├── Buffers/        #   Memory buffers, file I/O abstractions
    │   ├── Hashing/        #   RST hash algorithms (XxHash3, XxHash64)
    │   ├── Internal/        #   Internal helpers (extensions, throw helpers)
    │   ├── Metadata/       #   String table metadata and legacy support
    │   └── Serialization/  #   JSON ↔ binary serialization
    ├── Rion.Cli/           # CLI tool — command-line interface for common tasks
    │   └── Commands/       #   Subcommand implementations
    └── Rion.App/           # Desktop GUI (Avalonia) — work in progress
```

---

## Quick Start

### Install

```bash
# Clone the repository
git clone https://github.com/Roshaless/Rion.git
cd Rion

# Build
dotnet build -c Release

# Publish CLI as a standalone native binary
dotnet publish src/Rion.Cli -c Release -o ./publish
```

### CLI Usage

The CLI tool (`Rion.Cli`) provides three main operations:

#### Convert files (default behavior)

```bash
# Auto-detect format and convert
rion data.stringtable              # → data.json
rion data.json                     # → data.stringtable

# Batch conversion
rion *.stringtable -d ./output     # Convert all to JSON in ./output

# Explicit target format
rion data.bin -t json              # Force JSON output
rion data.json -t rst              # Force .stringtable output

# Single file with custom output path
rion data.stringtable -o result.json

# Preload reverse-hash lookup table
rion data.stringtable --hashes-dir ./hashes
```

#### Inspect files

```bash
# Basic metadata
rion info data.stringtable

# Detailed view with sample entries
rion info data.stringtable -d
```

Output example:
```
  File        : D:\data\game.stringtable
  Size        : 1,234,567 bytes
  Version     : 5
  Hash Algo   : XxHash3
  Bit Mask    : 38 bits (0x3FFFFFFFFF)
  Entries     : 12,345
  Avg Key Len : 32.5 chars
```

#### Search entries

```bash
# Find by hash value
rion lookup data.stringtable --hash 0x155C62DCFD

# Find by string value (case-insensitive)
rion lookup data.stringtable --value "champion"

# Limit results
rion lookup data.stringtable --value "skin" -n 20
```

#### Hash strings

```bash
# Hash with default algorithm (latest version)
rion hash "hello"

# Specify a game version
rion hash "hello" -v v5
rion hash "hello" -v v5-1415

# Benchmark mode
rion hash "test" -v latest -n 1000000

# List all supported version identifiers
rion hash --list-versions
```

Supported versions: `v2` | `v3` | `v4` | `v5` / `v5-legacy` | `v5-1415` | `v5-1502` / `latest`

---

## API Overview

### Reading a .stringtable file

```csharp
using Rion.Core;

// Read into a mutable StringTable (Dictionary<ulong, string>)
var table = StringTable.Read("data.stringtable");

// Read as an immutable record (lower memory, no mutation)
IStringTable record = StringTable.ReadAsRecord("data.stringtable");

// Access metadata
Console.WriteLine($"Version: {record.Metadata.Version}");
Console.WriteLine($"Entries: {record.Count()}");

// Look up by hash
if (table.TryGetValue(0x155C62DCFD, out var value))
    Console.WriteLine(value);
```

### Writing a .stringtable file

```csharp
using Rion.Core;
using Rion.Core.Metadata;

var table = StringTable.Create(StringTableMetadata.Latest);
table["champion_name"] = "Ashe";
table["skin_name"] = "Freljord Ashe";

// Write to file
StringTable.Write("output.stringtable", table);
```

### Converting between formats

```csharp
using Rion.Core;
using Rion.Core.Serialization;

// .stringtable → .json
var table = StringTable.ReadAsRecord("data.stringtable");
StringTableSerializer.SerializeToJsonFile("data.json", table);

// .json → .stringtable
var span = File.ReadAllBytes("data.json").AsSpan();
var parsed = StringTableSerializer.DeserializeFromJson(span);
StringTable.Write("data.stringtable", parsed);
```

### Hashing

```csharp
using Rion.Core.Hashing;

var options = RSTHashAlgorithmOptions.Version5Patch1502; // XxHash3, 38-bit
var algorithm = RSTHashAlgorithm.Create(options);

ulong hash = algorithm.Hash("hello");
Console.WriteLine($"Hash: {hash} (0x{hash:X16})");
```

### Reverse-hash lookup

```csharp
using Rion.Core.Hashing;

// Load known strings into the lookup table
var strings = File.ReadAllLines("hashes/strings.txt");
var algo = RSTHashAlgorithm.Create(RSTHashAlgorithmOptions.Version5Patch1502);
RSTHashtable.LoadFromStrings(strings, algo);

// Resolve a hash to its original string (if known)
string name = RSTHashtable.GetNameOrHexString(0x155C62DCFD);
// → "champion_name" or "{155C62DCFD}" if unknown
```

---

## Performance

Rion is designed for extreme throughput on large files. Typical benchmarks on a modern CPU:

| Operation | Throughput |
|-----------|-----------|
| XxHash3 hashing | ~2.9M hashes/s |
| Read .stringtable (100K entries) | ~15 ms |
| Write .stringtable (100K entries) | ~20 ms |
| JSON ↔ RST conversion | ~50 MB/s |

Key optimizations:
- Zero-copy `Span<byte>` / `ReadOnlySpan<byte>` throughout
- `unsafe` pointer arithmetic for hash table traversal
- `NativeMemory.Alloc` for direct memory management
- `[UnsafeAccessor]` for internal .NET API access
- `StringRefManager` with weak-reference interning
- AOT-compatible code paths (no reflection)

---

## File Format

A `.stringtable` file has a simple binary structure:

```
┌──────────────────────────────────────┐
│  Magic (0x52 = 'R')                  │  1 byte
├──────────────────────────────────────┤
│  Version                             │  1 byte
├──────────────────────────────────────┤
│  Hash Algorithm Flags                │  2 bytes
├──────────────────────────────────────┤
│  Entry Count                         │  4 bytes
├──────────────────────────────────────┤
│  Hash Array (EntryCount × 8 bytes)   │
├──────────────────────────────────────┤
│  String Content (null-terminated)    │
└──────────────────────────────────────┘
```

---

## Requirements

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- Windows, Linux, or macOS

---

## License

This project is licensed under the MIT License — see [LICENSE.txt](LICENSE.txt) for details.

---

## Acknowledgments

- [xxHash](https://github.com/Cyan4973/xxHash) — The non-cryptographic hash algorithm used by RST
- [System.IO.Hashing](https://www.nuget.org/packages/System.IO.Hashing) — Microsoft's .NET implementation
- [System.CommandLine](https://github.com/dotnet/command-line-api) — CLI framework
