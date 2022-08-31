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

## Authentication

Using this connector requires authenticating with Microsoft 365.  
There are two ways to do this:

- Use steps as normal. The first time you use a step which requires authentication, a message
  will be logged containing a url and a code. Follow the url, enter the code and login to authenticate.
- Create a token by logging in previously and supply this token as a parameter to `M365Login`

## Connector Settings

The Sequence Connector for Microsoft 365® requires additional configuration
which can be provided using the `settings` key in `connectors.json`.

### Supported Settings

| Name            | Required |    Type    | Description                                                     |
| :-------------- | :------: | :--------: | :-------------------------------------------------------------- |
| TenantId        |    ✔     |  `string`  | Directory Id of the application                                 |
| ClientId        |    ✔     |  `string`  | Application Id                                                  |
| GraphUserScopes |    ✔     | `string[]` | Permission scopes to use. Each step has its own required scopes |

If you do not have the Tenant and Client Ids you may have to create an application, follow the
instructions [on this page](https://docs.microsoft.com/en-us/graph/tutorials/dotnet?tabs=aad&tutorial-step=1)

### Example `connectors.json` Entry

```json
"Reductech.Sequence.Connectors.Microsoft365": {
  "id": "Reductech.Sequence.Connectors.Microsoft365",
  "enable": true,
  "version": "0.17.0",
  "settings": {
    "TenantId": "abc123",
    "ClientId": "def456",
    "GraphUserScopes": ["Mail.Read","User.ReadBasic.All", "Team.ReadBasic.All", "Chat.Read", "Channel.ReadBasic.All","ChannelMessage.Read.All"]
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
