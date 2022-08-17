# Sequence® Microsoft365 Connector

[Sequence](https://gitlab.com/reductech/Sequence) is a collection of
libraries that automates cross-application e-discovery and forensic workflows.

This connector contains Steps to interact with...

## Steps

|         Step          | Description                                    | Result Type |
| :-------------------: | :--------------------------------------------- | :---------: |
| `ConvertJsonToEntity` | Converts a JSON string or stream to an entity. |  `Entity`   |

## Examples

To check if a file exists and print the result:

```scala
- Print (ConvertJsonToEntity '{"Foo":1}')
```

## Documentation

Documentation is available here: https://sequence.sh

# Releases

Can be downloaded from the [Releases page](https://gitlab.com/reductech/sequence/connectors/microsoft365/-/releases).

# NuGet Packages

Are available in the [Reductech Nuget feed](https://gitlab.com/reductech/nuget/-/packages).
