# please
*plain English commands that require good manners*

## bump
Command for bumping version numbers in files (usually for releasing a NuGet package). Bump can bump the major, minor, or patch versions in AssemblyInfo.cs files and .nuspec files, as well as .nupkg references (.nupkg files contain version numbers in their file name) in text files (e.g. .bat file).

### Usage
* `please bump major version in .\app\TheApp\Properties\AssemblyInfo.cs`
* `please bump minor version in .\release\template\TheApp.nuspec`
* `please bump patch version in .\release\staging.bat`

See release\bump.bat for an example.

### Options
* `major version` will cause the major version to be bumped
* `minor version` will cause the minor version to be bumped
* `patch version` will cause the patch version to be bumped
* `in .\file` specifies the file containing the version reference

## run sql
Command for running a batch of .sql files in a directory on a given database.

### Usage
* `please run sql in .\directory on DATABASE`
* `please run sql with versioning in .\directory on DATABASE`

### Options
* `in .\directory` specifies the directory containing the .sql files
* `on DATABASE` specifies the name of the database connectionString in please.exe.config
* `with versioning` uses the version number prepended to the .sql file (e.g. 20130901000000_create-table.sql) to ensure the .sql file is only ran once on the given database

## Contributing
* New commands can be added to please by adding a Command to the \app\Library\Commands.cs file and corresponding tests to the app\Tests project.
* Run `ci\local` to build the projects and run the tests.
* If you fixed a bug, run `release\bump patch`. If you added a command, run `release\bump minor`.
* Run `build\release` to create a new please.exe in build\output.
* Submit changes using Pull Requests.
