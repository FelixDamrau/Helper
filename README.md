# Develix.Helper

Small console application that tries to simplify the daily developer life.

## Usage:

```
USAGE:
    Helper [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    package              Copy all nuget packages to the local package cache
    setup <setupName>    Publish setup to the publish directory
    deps                 Check the package references of a solution
```

### AppSettings

You can set some static settings. All settings can be set via console command options as well.  
The command options override the settings from this configuration file.

| Name                     | Description                                                      |
| ------------------------ | ---------------------------------------------------------------- |
| LocalPackageCache        | The path where packages will be copied to                        |
| PublishSetupRoot         | The path where setups will be copied to                          |
| SetupDirectoryIdentifier | The part of the directory the determines the setup source folder |
