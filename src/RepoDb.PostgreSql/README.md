[![MSBuild-CI](https://github.com/AmpScm/RepoDB/actions/workflows/build.yml/badge.svg)](https://github.com/AmpScm/RepoDB/actions/workflows/build.yml)
[![Version](https://img.shields.io/nuget/v/AmpScm.RepoDb?&logo=nuget)](https://www.nuget.org/packages/AmpScm.RepoDb.PostgreSql)
[![GitterChat](https://img.shields.io/gitter/room/mikependon/RepoDb?&logo=gitter&color=48B293)](https://gitter.im/RepoDb/community)

# [RepoDb.PostgreSql](https://repodb.net/tutorial/get-started-postgresql) - a hybrid .NET ORM library for PostgreSQL.

RepoDB is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

## Important Pages

- [GitHub Home Page](https://github.com/AmpScm/RepoDb) - to learn more about the core library.
- [Website](https://repodb.net) - docs, features, classes, references, releases and blogs.

## Community Engagements

- [GitHub](https://github.com/AmpScm/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## Dependencies

- [Npgsql](https://www.nuget.org/packages/Npgsql/) - the data provider used for PostgreSql.
- [RepoDb](https://www.nuget.org/packages/RepoDb.SqLite/) - the core library of RepoDB.

## License

[Apache-2.0](https://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 -2024 - [Michael Camara Pendon](https://twitter.com/mike_pendon)
[Apache-2.0](https://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2025 - now - [Bert Huijben](https://github.com/rhuijben)

--------

## Installation

At the Package Manager Console, write the command below.

```csharp
> Install-Package AmpScm.RepoDb.PostgreSql
```

Or, visit our [installation](https://repodb.net/tutorial/installation) page for more information.

## Get Started

First, RepoDB must be configured and MySqlConnector support loaded.

```csharp
GlobalConfiguration.Setup().UsePostgreSql();
```

**Note:** The call must be done once.

After the bootstrap initialization, any library operation can then be called.

Or, visit the official [get-started](https://repodb.net/tutorial/get-started-postgresql) page for PostgreSQL.

### Query

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(c => c.Id == 10045);
}
```

### Insert

```csharp
var customer = new Customer
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var id = connection.Insert<Customer>(customer);
}
```

### Update

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	customer.FirstName = "John";
	customer.LastUpdatedUtc = DateTime.UtcNow;
	var affectedRows = connection.Update<Customer>(customer);
}
```

### Delete

```csharp
using (var connection = new NpgsqlConnection(ConnectionString))
{
	var customer = connection.Query<Customer>(10045);
	var deletedCount = connection.Delete<Customer>(customer);
}
```
