# Sequence® Microsoft365 Connector

[Sequence®](https://sequence.sh) is a collection of libraries for
automation of cross-application e-discovery and forensic workflows.

This connector contains Steps to interact with the
[Microsoft Graph API](https://developer.microsoft.com/en-us/graph).

## Steps

|         Step          | Description                                    | Result Type |
| :-------------------: | :--------------------------------------------- | :---------: |
| `ConvertJsonToEntity` | Converts a JSON string or stream to an entity. |  `Entity`   |

## Examples

To check if a file exists and print the result:

```scala
- Print (ConvertJsonToEntity '{"Foo":1}')
```

## Settings

To use the Microsoft365 Connector you need to add a `settings` block to
the `microsoft365` connector configuration in the `connectors.json` file:

```json
{
  "Reductech.Sequence.Connectors.Microsoft365": {
    "id": "Reductech.Sequence.Connectors.Microsoft365",
    "version": "0.17.0",
    "enabled": true,
    "settings": {
        ...
    }
  }
}
```

# Documentation

https://sequence.sh

# Download

https://sequence.sh/download

# Try SCL and Core

https://sequence.sh/playground

# Package Releases

Can be downloaded from the [Releases page](https://gitlab.com/reductech/sequence/connectors/filesystem/-/releases).

# NuGet Packages

Release nuget packages are available from [nuget.org](https://www.nuget.org/profiles/Sequence).
