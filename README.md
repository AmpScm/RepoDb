[![MSBuild-CI](https://github.com/AmpScm/RepoDb/actions/workflows/build.yml/badge.svg)](https://github.com/AmpScm/RepoDb/actions/workflows/build.yml)
[![Version](https://img.shields.io/nuget/v/AmpScm.RepoDb?&logo=nuget)](https://www.nuget.org/packages/AmpScm.RepoDb)
[![GitterChat](https://img.shields.io/gitter/room/mikependon/RepoDb?&logo=gitter&color=48B293)](https://gitter.im/RepoDb/community)

# RepoDb – A Hybrid ORM for .NET

RepoDb is an open-source .NET ORM library that bridges micro-ORMs and full-featured ORMs. It provides a simple, high-performance data access layer with support for basic CRUD operations, advanced queries, caching, batch/bulk operations, and more.

**Active Fork** – This is an actively maintained fork of the original RepoDb repository, with improvements including multi-column primary key support, JSON columns, Vector columns, type coercion, and Oracle database support.

## Key Features

- **Easy to Use** – All operations are extension methods on `IDbConnection`. Simple, intuitive API.
- **High Performance** – Caches compiled expressions and generates optimal SQL based on your schema.
- **Memory Efficient** – Extracts and reuses object properties, mappings, and SQL statements throughout execution.
- **Flexible** – Supports atomic, batch, and bulk operations for datasets of any size. Both synchronous and asynchronous methods available.
- **Type Coercion** – Advanced type conversion between databases with AOT-compiled expressions and custom conversion support.
- **JSON Support** – Full support for JSON columns and JSON-backed serialized columns.
- **Vector Support** – Seamless handling of Vector columns via `ReadOnlyMemory<float>`.
- **Comprehensive Testing** – 10,000+ unit and integration tests for production reliability.
- **Open Source** – Free and community-driven with an active development roadmap.

## Fork Improvements

This fork enhances the original RepoDb with:

- **Multi-Column Primary Keys** – Proper detection and handling across all operations
- **Advanced Type Coercion** – JIT-time conversion makes SQLite work as a drop-in replacement for testing
- **JSON Columns** – Native support for JSON and serialized column types
- **Vector Columns** – Support for vector types in databases that provide this feature
- **Oracle Support** – New Oracle provider package ensuring standards compliance
- **Better NULL Handling** – Improved conversions for enums and modern .NET types (DateOnly, TimeOnly)
- **IParsable/IFormattable Support** – Automatic handling of types implementing these interfaces

## Quick Start

Choose your database platform:

- **[SQL Server](docs/getstarted/sqlserver.md)** – Using Microsoft.Data.SqlClient or if really necessary System.Data.SqlClient
- **[PostgreSQL](docs/getstarted/postgresql.md)** – Uses the well-tested Npgsql
- **[MySQL](docs/getstarted/mysql.md)** – Supports for both MySql or MyslConnector
- **[SQLite](docs/getstarted/sqlite.md)** – Microsoft.Data.Sqlite and System.Data.SQLite
- **[Oracle](docs/getstarted/oracle.md)** – Now supported using Oracle.ManagedDataAccess.Core or Oracle.ManagedDataAccess for .Net Framework.

### Example Usage

```csharp
using (var connection = new SqlConnection(connectionString))
{
	connection.Open();

	// Query
	var customers = connection.QueryAll<Customer>();
	var active = connection.Query<Customer>(c => c.IsActive == true);

	// Insert
	var customer = new Customer { Name = "John", Email = "john@example.com" };
	connection.Insert(customer);

	// Update
	customer.Name = "Jane";
	connection.Update(customer);

	// Delete
	connection.Delete(customer);

	// Batch Operations
	var customers = GetLargeDataset();
	connection.InsertAll(customers); // Sets identity values automatically
}
```

## Documentation

For detailed guides on features, operations, and configuration, see the [docs](docs/) directory:

- **[Features](docs/features/README.md)** – Query, Insert, Update, Delete, Merge, Bulk/Batch operations
- **[Caching](docs/features/caching.md)** – 2nd-layer caching for improved performance
- **[Batch Operations](docs/features/batchoperations.md)** – Process large datasets efficiently
- **[Bulk Operations](docs/features/bulkoperations/README.md)** – High-speed data operations with identity tracking
- **[Property Handlers](docs/features/propertyhandlers.md)** – Custom property transformation logic
- **[Tracing](docs/features/tracing.md)** – Query execution diagnostics
- **[Repositories](docs/references/README.md)** – Repository pattern implementation
- **[Class Mapping](docs/features/classmapping.md)** – Entity to database mapping

## Supported Databases

### Core Execute Methods (All Databases)

- `ExecuteQuery` – Execute queries and materialize results
- `ExecuteNonQuery` – Execute commands without returning data
- `ExecuteScalar` – Execute queries returning a single value
- `ExecuteReader` – Execute and get a DataReader
- `ExecuteQueryMultiple` – Execute multiple queries in one call

### Fluent API (Specific Providers)

The fluent LINQ-style API is available for:

- [SQL Server](https://www.nuget.org/packages/AmpScm.RepoDb.SqlServer)
- [SQLite](https://www.nuget.org/packages/AmpScm.RepoDb.SqLite.Microsoft)
- [MySQL](https://www.nuget.org/packages/AmpScm.RepoDb.MySql)
- [PostgreSQL](https://www.nuget.org/packages/AmpScm.RepoDb.PostgreSql)
- [Oracle](https://www.nuget.org/packages/AmpScm.RepoDb.Oracle)

Operations: `Query`, `Insert`, `Update`, `Merge`, `Delete`, `Truncate`, `Exists`, `Average`, `Sum`, etc. etc.

## Advanced Features

### Type Coercion & Conversion

RepoDb provides advanced type conversion between databases:

- **Automatic Conversion** – Smart type mapping with AOT-compiled expressions
- **Property Handlers** – Custom Get/Set transformation logic for specialized types
- **Modern .NET Support** – Built-in support for `DateOnly`, `TimeOnly`, enums and `Nullable` NULL handling on all value types.
- **IParsable/IFormattable** – Automatic handling of types implementing these interfaces
- **Cross-Database Testing** – SQLite works as a drop-in replacement for other databases during testing

**Property Handlers Example:**

```csharp
// Define a custom handler
public class EncryptedStringHandler : IPropertyHandler<string, string>
{
	public string Get(string input, PropertyHandlerGetOptions options)
		=> Decrypt(input);

	public string Set(string input, PropertyHandlerSetOptions options)
		=> Encrypt(input);
}

// Option 1: Via attribute
public class Customer
{
	public int Id { get; set; }

	[PropertyHandler(typeof(EncryptedStringHandler))]
	public string SensitiveData { get; set; }
}

// Option 2: Via fluent mapper (global)
FluentMapper
	.Entity<Customer>()
	.PropertyHandler(c => c.SensitiveData, new EncryptedStringHandler());

// Option 3: Via static mapper
PropertyHandlerMapper.Add<Customer, EncryptedStringHandler>(
	c => c.SensitiveData,
	new EncryptedStringHandler());
```

### Exception Handling

RepoDb provides clear, descriptive exception messages:

- Custom exceptions with detailed context during hydration
- Native exception bubbling from ADO.NET
- No exception swallowing – errors reach you immediately

### Setup, Connection Management

For SQL Server and other providers, initialization is typically done with a single call at startup. However, you can explicitly configure providers:

```csharp
// Initialize RepoDB and register a specific implementation
GlobalConfiguration.Setup().UseSqlServer();

// Or with custom options
GlobalConfiguration.Setup(new()
{
    DateOnlyAndTimeOnly = true,
});
```

If you want to register your own implementations instead of using the standard mappings you could use something like this:

```csharp
// Setup RepoDB
GlobalConfiguration.Setup(new()
{
    DateOnlyAndTimeOnly = true,
});

// Manual provider registration, like for the legacy System.Data.SqlClient
DbSettingMapper.Add<SqlConnection>(new SqlServerDbSetting(), true);
DbHelperMapper.Add<SqlConnection>(new SqlServerDbHelper(), true);
StatementBuilderMapper.Add<SqlConnection>(new SqlServerStatementBuilder(), true);
```

### SSL/Certificate Issues

If using `Microsoft.Data.SqlClient`, you may encounter certificate validation issues:

```csharp
// Add to connection string:
"TrustServerCertificate=true;"
```

**Warning:** Only enable `TrustServerCertificate` when necessary, as it bypasses security validation.

## Performance

RepoDb is highly optimized for data access operations:

- **Expression Caching** – Compiled expressions are cached and reused for maximum performance
- **Memory Efficiency** – Property extraction and mapping is cached throughout execution
- **Minimal Overhead** – Thin layer over ADO.NET with smart compilation strategies

**Performance Note:** The first execution includes AOT compilation overhead. For accurate benchmarking, exclude the first execution from measurements.

## Contributing

We're building RepoDb into a mainstream ORM for .NET. Contributions are welcome!

### How to Contribute

1. Look for [for-grabs](https://github.com/ampscm/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) issues
2. [Create a new issue](https://github.com/ampscm/RepoDb/issues/new) to discuss your contribution
3. Submit a pull request

### Documentation

Documentation improvements? The docs are all hosted in our repository, so please create a PR.

### Spread the Word

Your biggest contribution is sharing this library with other developers:

- Write blog posts
- Share on social media
- Discuss in developer communities
- Document it
- Share it
- Use it

Or, show your support by simply giving a ⭐ on this project.

### Engagements

Please get in touch with us via:

- [GitHub](https://github.com/ampscm/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?tab=newest&q=RepoDb) - for any technical questions.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

Ensure to visit our [Support Policy](docs/support-policy.md) to get more details about our policies when handling the operational support for this library.

### Hints

- [Building the Solution](docs/building.md) - let us build your copies.
- [Issuing a Pull-Request](docs/issuing-a-pull-request.md) - let us be aligned and notified.
- [Reporting an Issue](docs/reporting-an-issue.md) - let us be organized for easy tracking and fixing.

## Credits

Thanks to all the [contributors](https://github.com/ampscm/RepoDb/graphs/contributors) of this project, and to [Scott Hanselman](https://www.hanselman.com/) for [Exploring the .NET open source hybrid ORM library RepoDB](https://www.hanselman.com/blog/ExploringTheNETOpenSourceHybridORMLibraryRepoDB.aspx).

And also, thanks to these awesome OSS projects.

* [AppVeyor](https://www.appveyor.com/) - for the builds and test-executions.
* [GitHub](https://github.com/) - for hosting this project.
* [Gitter](https://gitter.im/) - for the community engagements.
* [Jekyll](https://github.com/jekyll/jekyll) - for powering our website.
* [Just-the-Docs](https://github.com/pmarsceill/just-the-docs) - for being the awesome library documentation template.
* [Moq](https://github.com/moq/moq4) - for being the tests mocking framework.
* [Nuget](https://www.nuget.org/) - for the package deliveries.
* [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) - for measuring the performance and efficiency.
* [SharpLab](https://sharplab.io/) - for helping us on our IL coding.
* [Shields](https://shields.io/) - for the awesome badges.
* [StackEdit](https://stackedit.io) - for being the markdown file editor.
* [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/),  [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite/), [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core/), [MySql.Data](https://www.nuget.org/packages/MySql.Data/), [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/), [Npgsql](https://www.nuget.org/packages/Npgsql/), [Oracle.ManagedDataAccess](https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core) - for being the extended DB provider drivers.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html)

* Copyright © 2019 - 2024 [Michael Camara Pendon](https://github.com/mikependon)
* Copyright © 2025 - now [Bert Huijben](https://github.com/rhuijben)
