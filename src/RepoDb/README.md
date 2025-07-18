[![MSBuild-CI](https://github.com/AmpScm/RepoDB/actions/workflows/build.yml/badge.svg)](https://github.com/AmpScm/RepoDB/actions/workflows/build.yml)
[![Version](https://img.shields.io/nuget/v/AmpScm.RepoDb?&logo=nuget)](https://www.nuget.org/packages/AmpScm.RepoDb)
[![GitterChat](https://img.shields.io/gitter/room/mikependon/RepoDb?&logo=gitter&color=48B293)](https://gitter.im/RepoDb/community)

# [RepoDB](https://repodb.net) - a hybrid ORM library for .NET.

RepoDB is an open-source .NET ORM library that bridges the gaps of micro-ORMs and full-ORMs. It helps you simplify the switch-over of when to use the BASIC and ADVANCE operations during the development.

## Important Pages

- [GitHub Home Page](https://github.com/AmpScm/RepoDb) - to learn more about the core library.
- [Website](https://repodb.net) - docs, features, classes, references, releases and blogs.

## Core Features

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

## Community Engagements

- [GitHub](https://github.com/AmpScm/RepoDb/issues) - for any issues, requests and problems.
- [StackOverflow](https://stackoverflow.com/search?q=RepoDB) - for any technical questions.
- [Twitter](https://twitter.com/search?q=%23repodb) - for the latest news.
- [Gitter Chat](https://gitter.im/RepoDb/community) - for direct and live Q&A.

## License

[Apache-2.0](https://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2019 - 2024 - [Michael Camara Pendon](https://twitter.com/mike_pendon)
[Apache-2.0](https://apache.org/licenses/LICENSE-2.0.html) - Copyright © 2025 - now - [Bert Huijben](https://github.com/rhuijben)

--------

## Installation

At the Package Manager Console, write the command below.

```
Install-Package AmpScm.RepoDb
```

Or, visit our [installation](https://repodb.net/tutorial/installation) page for more information.

## Get Started

After the installation, any library operation can then be called. Please see below for the samples.

### Query

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("SELECT * FROM [dbo].[Customer] WHERE (Id = @Id);", new { Id = 10045 }).FirstOrDefault();
}
```

### Insert

```csharp
var customer = new
{
	FirstName = "John",
	LastName = "Doe",
	IsActive = true
};
using (var connection = new SqlConnection(ConnectionString))
{
	var id = connection.ExecuteScalar<int>("INSERT INTO [dbo].[Customer](FirstName, LastName, IsActive) VALUES (@FirstName, @LastName, @IsActive); SELECT SCOPE_IDENTITY();", customer);
}
```

### Update

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = new
	{
		Id = 10045,
		FirstName = "John",
		LastName = "Doe"
	};
	var affectedRows = connection.ExecuteNonQuery("UPDATE [dbo].[Customer] SET FirstName = @FirstName, LastName = @LastName, LastUpdatedUtc = GETUTCDATE() WHERE (Id = @Id);", customer);
}
```

### Delete

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var deletedRows = connection.ExecuteNonQuery("DELETE FROM [dbo].[Customer] WHERE (Id = @Id)", new { Id = 10045 });
}
```

### StoredProcedure

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("[dbo].[sp_GetCustomer]", new { Id = 10045 }, commandType: CommandType.StoredProcdure).FirstOrDefault();
}
```

Or via direct calls.

```csharp
using (var connection = new SqlConnection(ConnectionString))
{
	var customer = connection.ExecuteQuery<Customer>("EXEC [dbo].[sp_GetCustomer](@Id);", new { Id = 10045 }).FirstOrDefault();
}
```

Or, visit the official [get-started](https://repodb.net/tutorial/get-started-sqlserver) page for SQL Server.
