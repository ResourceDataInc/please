# please

[![Build Status](https://travis-ci.org/ResourceDataInc/please.svg?branch=master)](https://travis-ci.org/ResourceDataInc/please)

*Please* is a set of commands useful for maintaining software projects.

For example, *please* can be used manage change to databases. Let's say you need to add a column to a table in your Staging database. First, you would create a .sql script that contained the alter table statement and save it to a  directory maybe named `.\migrations`. Next, you would run `please add timestamp in .\migrations` which  prepends a timestamp to the name of the .sql file. Finally, you run `please run sql in .\migrations on Staging` to "migrate" the Staging database and *please* will run any .sql scripts that have not been ran before in the Staging database (in this case your alter table statement). *Please* automatically keeps track of which .sql scripts have ran in the database based on the timestamp values. *Please* also supports running .py files and/or a mix of .sql and .py files as migrations. This gives you the full power of python as a migration which could be used to do something like run a set of arcpy commands to automate an ArcGIS Server task.

However, you don't typically run *please* commands directly from the command line. You probably setup scripts (maybe rake tasks) that call *please* and other necessary commands that can be ran from a continuous integration (CI) server. The plain English style of the *please* commands ensures your CI scripts are easy to read.

## Commands

The word or words that immediately follow `please` designate the command to run, and the command is followed by options. There are currently five commands - `bump`, `run sql`, `run py`, `run all`, and `add timestamp`.

### bump

Command for bumping version numbers in files (usually for releasing a NuGet package). Bump can bump the major, minor, or patch versions in AssemblyInfo.cs files and .nuspec files, as well as .nupkg references (.nupkg files contain version numbers in their file name) in text files (e.g. scripts).

#### Usage

* `please bump major version in .\app\TheApp\Properties\AssemblyInfo.cs`
* `please bump minor version in .\release\template\TheApp.nuspec`
* `please bump patch version in .\release\staging.bat`

See release\bump.bat for an example.

#### Options

* `major version` will cause the major version to be bumped
* `minor version` will cause the minor version to be bumped
* `patch version` will cause the patch version to be bumped
* `in .\file` specifies the file containing the version reference

### run sql

Command for running a single .sql file or a batch of .sql files in a directory on a given database.

#### Usage

* `please run sql file .\script.sql on DATABASE`
* `please run sql in .\directory on DATABASE`
* `please run sql with versioning in .\directory on DATABASE`
* `please run sql include .\whitelist.txt in .\directory on DATABASE`

#### Options

* `file .\script.sql` specifies an individual sql file to run
* `in .\directory` specifies the directory containing the .sql files
* `on DATABASE` specifies the name of the database connectionString in please.exe.config
* `with versioning` uses the version number prepended to the .sql file (e.g. 20130901000000_create-table.sql) to ensure the .sql file is only ran once on the given database
* `include .\whitelist.txt` specifies a file containing the list of .sql files to run if found in the given .\directory

### run py

Command for running a single .py file or a batch of .py files in a directory.

#### Usage

* `please run py file .\script.py`
* `please run py with versioning in .\directory on DATABASE`
* `please run py include .\whitelist.txt in .\directory on DATABASE`

#### Options

* `file .\script.py` specifies an individual sql file to run
* `in .\directory` specifies the directory containing the .py files
* `on DATABASE` specifies the name of the database connectionString in please.exe.config (only applicable if combined with `with versioning`)
* `with versioning` uses the version number prepended to the .py file (e.g. 20130901000000_create-table.py) to ensure the .py file is only ran once
* `include .\whitelist.txt` specifies a file containing the list of .py files to run if found in the given .\directory

### run all

Command for running a batch of .sql and/or .py files in a directory.

### Usage
* `please run all in .\directory on DATABASE`
* `please run all with versioning in .\directory on DATABASE`
* `please run all include .\whitelist.txt in .\directory on DATABASE`

#### Options

* `in .\directory` specifies the directory containing the .sql and/or .py files
* `on DATABASE` specifies the name of the database connectionString in please.exe.config (only applicable to .sql files or if combined with `with versioning`)
* `with versioning` uses the version number prepended to the .sql and/or .py file (e.g. 20130901000000_create-table.sql) to ensure the .sql and/or .py file is only ran once
* `include .\whitelist.txt` specifies a file containing the list of .sql and/or .py files to run if found in the given .\directory

### add timestamp

Command for adding a timestamp to all files in a directory that don't already have them.

#### Usage

* `please add timestamp in .\directory`

#### Options

* `in .\directory` specifies the directory containing the files

## Contributing

* You need to have python installed to run tests.
* New commands can be added to *please* by adding a `Command` to the `\app\Library\Commands.cs` file and corresponding tests to the `app\Tests` project.
* Run `ci\local` to build the projects and run the tests.
* If you fixed a bug, run `release\bump patch`. If you added a command, run `release\bump minor`.
* Run `build\release` to create a new `please.exe` in `build\output`.
* Submit changes using Pull Requests.
