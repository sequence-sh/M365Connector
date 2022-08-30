# Sequence® Connector for Microsoft 365

The Sequence Connector for Microsoft 365 allows users to automate ediscovery
and forensic workflows that use [Microsoft Graph](https://docs.microsoft.com/en-us/graph/).

This connector has [Steps](https://sequence.sh/steps/Microsoft365) to:

- Read Email
- Read Chats
- List Users
- List Teams
- List Channels of a Team
- Read messages in a channel

## Connector Settings

The Sequence Connector for Microsoft 365® requires additional configuration
which can be provided using the `settings` key in `connectors.json`.

### Supported Settings

| Name               | Required |   Type   | Description                                                                             |
| :----------------- | :------: | :------: | :-------------------------------------------------------------------------------------- |
| TenantId |    ✔     | `string` |Directory Id of the application
| ClientId |    ✔     | `string` | Application Id                                                  |
| GraphUserScopes                |    ✔     | `string[]` | Permission scopes to use. Each step has its own required scopes                                                |

### Example `connectors.json` Entry

```json
"Reductech.Sequence.Connectors.Microsoft365": {
  "id": "Reductech.Sequence.Connectors.Microsoft365",
  "enable": true,
  "version": "0.17.0",
  "settings": {
    "TenantId": "abc123",
    "ClientId": "def456",
    "GraphUserScopes": ["read-mail"],
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

Can be downloaded from the [Releases page](https://gitlab.com/reductech/sequence/connectors/microsoft365/-/releases).

# NuGet Packages

Release nuget packages are available from [nuget.org](https://www.nuget.org/profiles/Sequence).

## Licensing

This product is licensed under the Apache License, Version 2.0.
For further details please see http://www.apache.org/licenses/LICENSE-2.0.

Microsoft does not test, evaluate, endorse or certify this product.
