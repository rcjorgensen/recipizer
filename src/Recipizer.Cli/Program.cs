﻿using System.Data.SQLite;
using System.Reflection;

using CommandLine;
using Dapper;
using Microsoft.Extensions.Configuration;
using Recipizer.Cli;
using Recipizer.Cli.Options;

// Here we should handle:
// * Setting global state e.g. configuring Dapper
// * Reading environment variables
// * Reading configuration
// * Creating a composition root i.e. wiring up application with dependencies (we will be using Pure DI https://blog.ploeh.dk/2014/06/10/pure-di/)
// * Parsing command line arguments
// * Writing output

// Global state


DefaultTypeMap.MatchNamesWithUnderscores = true;

// Environment variables


var installDir = Environment.GetEnvironmentVariable("RECIPIZER_INSTALL_DIR");

if (installDir == null)
{
    Console.WriteLine("ERROR: Could not read environment variable `RECIPIZER_INSTALL_DIR`");
    return;
}

// Reading configuration


var appsettings = new ConfigurationBuilder()
    .SetBasePath(installDir)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var configuration = appsettings.Get<Configuration>();

if (configuration == null)
{
    Console.WriteLine("ERROR: Could not read configuration");
    return;
}

var databaseFilePath = configuration.DatabaseFilePath;
if (databaseFilePath == null)
{
    Console.WriteLine("ERROR: Could not get database file path from configuration");
    return;
}

configuration.DatabaseFilePath = Path.Combine(installDir, databaseFilePath);

var dataFilePath = configuration.DataFilePath;
if (dataFilePath != null)
{
    configuration.DataFilePath = Path.Combine(installDir, dataFilePath);
}

// Creating composition root using Pure DI


using var sqlConnection = new SQLiteConnection($"Data Source={configuration.DatabaseFilePath}");

var app = new Application(
    configuration,
    new Repository(sqlConnection),
    new FileSystem(),
    new Deserializer(),
    new Serializer()
);

// Parsing and mapping command line arguments


var result = await Parser.Default
    .ParseArguments<InitOptions, RecipesOptions, IngredientsOptions>(args)
    .MapResult(
        (InitOptions opts) => app.Init(opts),
        (RecipesOptions opts) => app.Recipes(opts),
        (IngredientsOptions opts) => app.Ingredients(opts),
        errs => Task.FromResult(string.Empty)
    );

// Presenting result

if (!string.IsNullOrWhiteSpace(result))
{
    Console.WriteLine(result);
}
