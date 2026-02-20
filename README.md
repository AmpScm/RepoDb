[![MSBuild-CI](https://github.com/AmpScm/RepoDB/actions/workflows/build.yml/badge.svg)](https://github.com/AmpScm/RepoDB/actions/workflows/build.yml)
[![Version](https://img.shields.io/nuget/v/AmpScm.RepoDb?&logo=nuget)](https://www.nuget.org/packages/AmpScm.RepoDb)
[![GitterChat](https://img.shields.io/gitter/room/mikependon/RepoDb?&logo=gitter&color=48B293)](https://gitter.im/RepoDb/community)

# [RepoDb](https://repodb.net) - a hybrid ORM Library for .NET.

RepoDb is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

**This is a fork of the now mostly inactive original repository**

## Benefits/Advantages

Like with any other ORMs, RepoDb does provide the preliminary [methods](https://repodb.net/docs#operations) needed for your basic operations. The good thing is, it also does provide the operations that is needed to cater your edge-cases like [2nd-Layer Cache](https://repodb.net/feature/caching), [Tracing](https://repodb.net/feature/tracing), [Repositories](https://repodb.net/feature/repositories), [Property Handlers](https://repodb.net/feature/propertyhandlers) and [Batch](https://repodb.net/feature/batchoperations)/[Bulk Operations](https://repodb.net/feature/bulkoperations).

If you are to use [RepoDb](docs/development-experience.md#repodb), your [development experience](docs/development-experience.md) is as simple as opening a connection and calling the advance operations with a very minimal code. It is the reason that makes this library the simpliest ORM to use.

When you do the [bulk operations](https://repodb.net/feature/bulkoperations), the generated value of the [identity columns](docs/bulk-operation-edge-cases.md#identity-columns) will be set back to the data models, just right after your execution. It is an important use-case that is/may needed by you and/or most developers, and both the [BulkInsert](https://repodb.net/operation/bulkinsert) and [BulkMerge](https://repodb.net/operation/bulkmerge) operations addressed this need.

RepoDb also does support the different way-of-executions (the [atomic](docs/atomic-batch-bulk.md#atomic-operation), the [batch](docs/atomic-batch-bulk.md#batch-operation) and the [bulk](docs/atomic-batch-bulk.md#bulk-operation)). Through this, it is easy for you to establish your repository that can process the smallest-to-the-largest datasets without even affecting the efficiency and the performance of your application.

<details>
<summary><b>Important Attributes</b></summary>

<p>

**Easy to Use** - the operations were all implemented as extension methods of your IDbConnection object. For as long your connection is open, any operations can then be called against your database.

**High Performant** - it caches the already-generated compiled expressions for future reusabilities and executions. It understands your schema to create the most optimal compiled expression AOT.

**Memory Efficient** - it extracts and caches your object properties, execution contexts, object mappings and SQL statements. It is reusing them all throughout the process of transformations and executions.

**Dynamic and Hybrid** - it provides some advance features of the full-fledged ORMs. It significantly help the developers to simplify the experience when context-switching during the development.

**Open-Source Software** - it is an open-source software and will always be free. It is authored to further improve the .NET data access experiences and solutions, together with the collective ideas of the community.

**High Quality** - it is a high-quality micro-ORM supported by 10K+ real-life Unit and Integration Tests. It is highly tested and is used by various critical systems that are running in the Production environment.

</p>

</details>

## Get Started

Please click any of the link below to fast-track your learnings.

- [SqlServer](https://repodb.net/tutorial/get-started-sqlserver)
- [SqLite](https://repodb.net/tutorial/get-started-sqlite)
- [MySql](https://repodb.net/tutorial/get-started-mysql)
- [PostgreSql](https://repodb.net/tutorial/get-started-postgresql)
- [Oracle](https://github.com/AmpScm/RepoDb/tree/master/src/RepoDb.Oracle/)

Or, learn a specific feature.

- [Batch Operations](https://repodb.net/feature/batchoperations)
- [Bulk Operations](https://repodb.net/feature/bulkoperations)
- [Caching](https://repodb.net/feature/caching)
- [Class Handlers](https://repodb.net/feature/classhandlers)
- [Class Mapping](https://repodb.net/feature/classmapping)
- [Dynamics](https://repodb.net/feature/dynamics)
- [Connection Persistency](https://repodb.net/feature/connectionpersistency)
- [Enumeration](https://repodb.net/feature/enumeration)
- [Expression Trees](https://repodb.net/feature/expressiontrees)
- [Hints](https://repodb.net/feature/hints)
- [Implicit Mapping](https://repodb.net/feature/implicitmapping)
- [Multiple Query](https://repodb.net/feature/multiplequery)
- [Property Handlers](https://repodb.net/feature/propertyhandlers)
- [Repositories](https://repodb.net/feature/repositories)
- [Targeted Operations](https://repodb.net/feature/targeted)
- [Tracing](https://repodb.net/feature/tracing)
- [Transaction](https://repodb.net/feature/transaction)
- [Type Mapping](https://repodb.net/feature/typemapping)

Otherwise, please visit our [documentation](https://repodb.net/docs) page to learn more.

## Supported Databases

The execute methods below support all the RDBMS data providers.

- [ExecuteQuery](https://repodb.net/operation/executequery)
- [ExecuteNonQuery](https://repodb.net/operation/executenonquery)
- [ExecuteScalar](https://repodb.net/operation/executescalar)
- [ExecuteReader](https://repodb.net/operation/executereader)
- [ExecuteQueryMultiple](https://repodb.net/operation/executequerymultiple)

Whereas the fluent methods below only support the [SQL Server](https://www.nuget.org/packages/AmpScm.RepoDb.SqlServer), [SQLite](https://www.nuget.org/packages/AmpScm.RepoDb.SqLite.Microsoft), [MySQL](https://www.nuget.org/packages/AmpScm.RepoDb.MySql) and [PostgreSQL](https://www.nuget.org/packages/AmpScm.RepoDb.PostgreSql) RDBMS data providers.

- [Query](https://repodb.net/operation/query)
- [Insert](https://repodb.net/operation/insert)
- [Merge](https://repodb.net/operation/merge)
- [Delete](https://repodb.net/operation/delete)
- [Update](https://repodb.net/operation/update)
 
Click [here](https://repodb.net/operation) to see all the operations.

## Package Referencing

By default, .NET is auto-resolving the references, however, we strongly recommend that you always explicitly reference the [RepoDb](https://www.nuget.org/packages/AmpScm.RepoDb) core library. The rationale behind this is that, the [RepoDb](https://www.nuget.org/packages/AmpScm.RepoDb) core library is a fast-moving package in which all the alpha/beta releases, hotfixes and/or even the actual releases could happen without affecting the extension libraries.

Please note that we are releasing an actual next released-version if the changes are having minimal impact but is important for the other users.

You can always target the version when installing the library, even it is on a semantic release.

```csharp
> Install-Package AmpScm.RepoDb -version 1.x.x-betaX
```

## .NET Type Coercion

By default, RepoDb does not do the automatic .NET CLR Type conversion during the serialization and deserialization process. The coercion support is completely dependent to the ADO.NET coercion capability. But when types like enums are used
additional handling is generated in the entity layer to 'do the right thing'. What the right thing is in your case can be configured using the `EnumHandling` property when initializing using the `GlobalConfigurationOptions` instance
passed to `GlobalConfiguration.Setup()` when initializing RepoDb. Whether strings or integers are used in the database is usually found by identifying the schema on first use, but when using custom queries the `EnumDefaultDatabaseType` setting
is used first.

If needed the library allows you to register custom conversion classes. For recently added .Net classes the AmpScm fork adds DateOnly and TimeOnly support out of the box, and also improves quite a few
of the standard conversions (e.g. enums) with proper `NULL` support.

**Note:** The exception that is being thrown is dependent to what the underlying ADO.NET coercion exception. If the [Automatic](https://repodb.net/enumeration/conversiontype) conversion is used, the extracted value will always be evaluated and an additional conversion logic will be used (if needed). The conversion logic is through the AOT compilation of [System.Linq.Expressions.Expression.Convert](https://docs.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression.convert?view=netcore-3.1) and/or [System.Convert](https://docs.microsoft.com/en-us/dotnet/api/system.convert?view=netcore-3.1).

## Exception Handling

As the compiler exception is a bit low-level and is not descriptive for the native language, therefore, when compiling the process of hydration from/to the database and the application, a customized exception is being thrown to provide a detailed exception messages to the callers.

On the other hand, as part of the standard when writing code in RepoDb (i.e.: respect the default exception handling of .NET, ensure an unharmonized exception when bubbling up the exception messages), RepoDb does not contain a single line of code that catches and rethrowing any exception (try-catch statement). Any exception happens within the library whether it is an ADO.NET exception and/or whatever will be bubble up natively back to the callers.

## System.Data.SqlClient

If you are working with this package, you are required to bootstrap the connection object on the startup.

```csharp
var dbSetting = new SqlServerDbSetting();

DbSettingMapper
	.Add<System.Data.SqlClient.SqlConnection>(dbSetting, true);
DbHelperMapper
	.Add<System.Data.SqlClient.SqlConnection>(new SqlServerDbHelper(), true);
StatementBuilderMapper
	.Add<System.Data.SqlClient.SqlConnection>(new SqlServerStatementBuilder(dbSetting), true);
```

Or, you can replicate the actual [SqlServerBootstrap](src/RepoDb.SqlServer/RepoDb.SqlServer/SqlServerBootstrap.cs) class implementation and attach it to your solution. Then, call the local class initializer method explicitly.

## Trust Server Certificate

For [RepoDb.SqlServer](https://www.nuget.org/packages/AmpScm.RepoDb.SqlServer) package, the [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient) is used, which by default validates the server certificate where System.Data.SqlClient didn't.

In most cases, an exception below is thrown if the mentioned security chain is not enabled.

```csharp
A connection was successfully established with the server, but then an error occurred during the login process.
(provider: SSL Provider, error: 0 - The certificate chain was issued by an authority that is not trusted.)
---> System.ComponentModel.Win32Exception: The certificate chain was issued by an authority that is not trusted..
```

The issue above can be rectified by simply enabling the TLS 1.2. Alternatively, the argument [TrustServerCertificate](https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnectionstringbuilder.trustservercertificate?view=dotnet-plat-ext-6.0) can be used on the connection string.

**Note:** By enabling the [TrustServerCertificate](https://docs.microsoft.com/en-us/dotnet/api/system.data.sqlclient.sqlconnectionstringbuilder.trustservercertificate?view=dotnet-plat-ext-6.0) argument, as per Microsoft, _the transport layer will use SSL to encrypt the channel and bypass walking the certificate chain to validate trust_. Therefore, only enable this flag if needed.

## Library Limitations

It is very important for you and to the community of .NET to learn the things the library is capable and is-not capable of doing, so please spend time reading the [limitation](docs/limitations.md) page before using the library. Some original limitations are lifted in the AmpScm fork. (This fork properly identifies multi-column primary keys and
uses those through the common operations without affecting the public api)

## Benchmark

The benchmark result shown on this section is the result of the community-approved ORM bencher tool, the [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) tool.

Below is the actual recent official execution [result](https://github.com/FransBouma/RawDataAccessBencher/blob/master/Results/20201112_net5_ef5.txt).

<img src="https://raw.githubusercontent.com/mikependon/RepoDb.NET/master/assets/backgrounds/statistics.png" />

RepoDb shows an impressive performance and memory-efficiency if being compared with other ORMs available in the .NET ecosystem. It has positioned itself just right behind the logic-less hand-coded materializer if being benchmark with .NET Core and .NET Framework. However, RepoDb is the fastest and the most-efficient ORM if being benchmark with .NET 5, even beating the hand-coded materializer.

### Important Note

The AOT compilation (IL/Expression) has some degree of performance impact, even just for milliseconds, therefore, if you are to materialize RepoDb, it is highly recommended to always eliminate the first execution.

To avoid the bias, you as well should exclude the first execution of the other ORMs during the benchmarking.

## Contributions

We would like to make RepoDb the mainstream hybrid-ORM library for .NET technology. Please help us build and realize the solution.

To contribute, please find a [for-grabs](https://github.com/ampscm/RepoDb/issues?q=is%3Aissue+is%3Aopen+label%3A%22for+grabs%22) item and issue a PR. Otherwise, you may create a [new issue](https://github.com/ampscm/RepoDb/issues/new) for us to look-at and discuss.

If you wish to contribute to the documentation site, it is hosted in the [RepoDb.NET](https://github.com/ampscm/RepoDb.NET) repository. Your expertise is needed to correct the forms, if needed.

Your biggest contribution is to utilize and share this library to the other developers.

- Blog it
- Discuss it
- Document it
- Share it
- Use it

Or, show your support by simply giving a :star: on this project.

### Engagements

Please get in touch with us via:

- [GitHub](https://github.com/ampscm/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?tab=newest&q=RepoDb) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

Ensure to visit our [Support Policy](docs/support-policy.md) to get more details about our policies when handling the operational support for this library.

### Hints

- [Building the Solutions](https://github.com/ampscm/RepoDb/tree/master/RepoDb.Docs/building-the-solutions.md) - let us build your copies.
- [Coding Standards](https://github.com/ampscm/RepoDb/tree/master/RepoDb.Docs/coding-standards.md) - let us be uniformed.
- [Issuing a Pull-Request](https://github.com/ampscm/RepoDb/tree/master/RepoDb.Docs/issuing-a-pull-request.md) - let us be aligned and notified.
- [Reporting an Issue](https://github.com/ampscm/RepoDb/tree/master/RepoDb.Docs/reporting-an-issue.md) - let us be organized for easy tracking and fixing.

## Credits

Thanks to all the [contributors](https://github.com/ampscm/RepoDb/graphs/contributors) of this project, and to [Scott Hanselman](https://www.hanselman.com/) for [Exploring the .NET open source hybrid ORM library RepoDB](https://www.hanselman.com/blog/ExploringTheNETOpenSourceHybridORMLibraryRepoDB.aspx).

And also, thanks to these awesome OSS projects.

- [AppVeyor](https://www.appveyor.com/) - for the builds and test-executions.
- [GitHub](https://github.com/) - for hosting this project.
- [Gitter](https://gitter.im/) - for the community engagements.
- [Jekyll](https://github.com/jekyll/jekyll) - for powering our website.
- [Just-the-Docs](https://github.com/pmarsceill/just-the-docs) - for being the awesome library documentation template.
- [Moq](https://github.com/moq/moq4) - for being the tests mocking framework.
- [Nuget](https://www.nuget.org/) - for the package deliveries.
- [RawDataAccessBencher](https://github.com/FransBouma/RawDataAccessBencher) - for measuring the performance and efficiency.
- [SharpLab](https://sharplab.io/) - for helping us on our IL coding.
- [Shields](https://shields.io/) - for the awesome badges.
- [StackEdit](https://stackedit.io) - for being the markdown file editor.
- [Microsoft.Data.Sqlite](https://www.nuget.org/packages/Microsoft.Data.Sqlite/), [System.Data.SQLite.Core](https://www.nuget.org/packages/System.Data.SQLite.Core/), [MySql.Data](https://www.nuget.org/packages/MySql.Data/), [MySqlConnector](https://www.nuget.org/packages/MySqlConnector/), [Npgsql](https://www.nuget.org/packages/Npgsql/) - for being the extended DB provider drivers.

## License

[Apache-2.0](http://apache.org/licenses/LICENSE-2.0.html) 
- Copyright © 2019 - 2024 [Michael Camara Pendon](https://github.com/mikependon)
- Copyright © 2025 - now [Bert Huijben](https://github.com/rhuijben)
