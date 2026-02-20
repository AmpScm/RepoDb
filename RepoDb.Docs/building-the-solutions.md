# Building the Solution

In this page, we will guide you on how to build the RepoDB Solution.

## Install the Git

To install the [Git](https://git-scm.com/), please follow this [guide](https://git-scm.com/book/en/v2/Getting-Started-Installing-Git).

## Clone the Repository

```
> mkdir c:\dev
> cd c:\dev
> git clone https://github.com/ampscm/RepoDb.git
```

## Building the [RepoDb.Core](https://github.com/ampscm/RepoDb/tree/master/RepoDb.Core)

```
> cd c:\dev\RepoDb
> dotnet build -v n
```

#### Building and executing the [RepoDb.IntegrationTests](https://github.com/ampscm/RepoDb/tree/master/RepoDb.Core/RepoDb.Tests/RepoDb.IntegrationTests)

To run all the test you would need SqlServer, Postgresql, Mysql, etc, etc. 
You can setup all these manually, but lets assume you have docker..

```
$ docker compose up -d
[+] up 4/4
 ✔ Container repodb-sqlserver-1  Running                                                                            0.0s
 ✔ Container repodb-mysql-1      Running                                                                            0.0s
 ✔ Container repodb-oracle-1     Running                                                                            0.0s
 ✔ Container repodb-postgresql-1 Running                                                                            0.0s
```

Build the integration tests.

```
$ cd c:\dev\RepoDb\src\RepoDb.IntegrationTests
$ dotnet build
```

Execute the integration tests.

```
$ cd c:\dev\RepoDb\src\RepoDb.IntegrationTests
$ dotnet test
```

If you run `dotnet test` in the repository root you will run all tests.


All dependencies are fetched using nuget, so all the previous manual steps are no longer required. If you create a PR
on github, the tests will automatically start on your new code. So you can also use the testresults from there.
