Please refer to our [Contributions](https://github.com/ampscm/RepoDb#contributions) page for further information.

# Building RepoDB

Call
     `$ dotnet build`

## Test RepoDB

```
# Start database engines in docker
$ docker compose up -d
# Run testsuite
$ dotnet test

# Stop docker containers
$ docker compose down

# To Remove all docker state
$ docker compose down -v
```
Requires docker, and diskspace and some memory to run all engines.

## Release RepoDb:

    `$ ./dotnet pack -c release -p Version=1.2.3 -o release`

This creates nuget packages for version 1.2.3 in the release directory

But it is better to just publish your changes on GitHub and make the GitHub
actions produce binaries in a reproducable way.
