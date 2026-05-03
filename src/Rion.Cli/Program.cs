// Copyright (c) 2024 Roshaless
// All rights reserved.
//
// This source code is distributed under an MIT license.
// LICENSE file in the root directory of this source tree.

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

using Rion.Cli.Commands;

return await BuildCommandLine()
    .UseDefaults()
    .Build()
    .InvokeAsync(args);

static CommandLineBuilder BuildCommandLine()
{
    // Root command: default behavior is auto-convert files
    var filesArgument = new Argument<string[]>(
        "files",
        () => [],
        "Files to convert. Auto-detects format and converts to the opposite format."
    );

    var outputOption = new Option<string?>(
        aliases: ["--output", "-o"],
        description: "Output file path (single file only)."
    );

    var outputDirOption = new Option<System.IO.DirectoryInfo?>(
        aliases: ["--output-dir", "-d"],
        description: "Output directory for batch conversion."
    );

    var formatOption = new Option<string?>(
        aliases: ["--to", "-t"],
        description: "Target format: 'json' or 'rst'. Auto-detected when omitted."
    );

    var hashesDirOption = new Option<System.IO.DirectoryInfo?>(
        aliases: ["--hashes-dir"],
        description: "Directory containing hash text files to preload."
    );

    var rootCommand = new RootCommand(
        "Rion — Riot String Table CLI tool. Convert, inspect, query, and hash .stringtable files."
    );

    rootCommand.AddArgument(filesArgument);
    rootCommand.AddOption(outputOption);
    rootCommand.AddOption(outputDirOption);
    rootCommand.AddOption(formatOption);
    rootCommand.AddOption(hashesDirOption);

    rootCommand.SetHandler(
        ConvertCommand.HandleFilesAsync,
        filesArgument, outputOption, outputDirOption, formatOption, hashesDirOption);

    rootCommand.AddCommand(InfoCommand.Create());
    rootCommand.AddCommand(LookupCommand.Create());
    rootCommand.AddCommand(HashCommand.Create());

    return new CommandLineBuilder(rootCommand);
}
