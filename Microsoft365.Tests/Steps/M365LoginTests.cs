using System.Collections.Generic;
using Reductech.Sequence.Connectors.Microsoft365.Steps;
using Reductech.Sequence.Core;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core.Steps;

namespace Reductech.Sequence.Connectors.Microsoft365.Tests.Steps;

public partial class
    M365LoginTests : Microsoft365StepTestBase<M365UsersRead, Array<Entity>>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield break;
        }
    }

    private const string Token =
        @"eyJ0eXAiOiJKV1QiLCJub25jZSI6IkRiLVJnYjM4b2pPQ2tFay05OGNMLVZfU3dmWUJMUzRPMHk3UF9xSUtGdjgiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9kOTk0ZmFhYy04Y2EwLTRlNzktOTg5Mi00OWJjY2YxNmJiNmQvIiwiaWF0IjoxNjYxMTg0MTkxLCJuYmYiOjE2NjExODQxOTEsImV4cCI6MTY2MTI3MDg5MSwiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFWUUFxLzhUQUFBQW9lOGJBWWJpZ3BXNmZyTndlem5IK0NOQ0h4NWltYzVKa1U3VDBvWmZ3bktDQXBIckkwNG80QXpNZi9sRk5yK1lKbXVDWUVGdm50L2xPNGlJRyswY0JFcGdhbFJWY0lIYUs3cDIwemhMTVlRPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwX2Rpc3BsYXluYW1lIjoiQ29ubmVjdG9yIFRlc3QiLCJhcHBpZCI6IjJlNzZjOTlkLTczM2ItNDVjYi04MWY5LWI2MWVhODdmNjM4OSIsImFwcGlkYWNyIjoiMCIsImZhbWlseV9uYW1lIjoiV2FpbndyaWdodCIsImdpdmVuX25hbWUiOiJNYXJrIiwiaWR0eXAiOiJ1c2VyIiwiaXBhZGRyIjoiOTUuMTQxLjIxLjE0NSIsIm5hbWUiOiJNYXJrIFdhaW53cmlnaHQiLCJvaWQiOiJjMzQxNjI5Ni1mYTdhLTQ4MDktOTY5ZS03ZWE5NjkzMjg1YjAiLCJwbGF0ZiI6IjMiLCJwdWlkIjoiMTAwMzIwMDA5NTNDQkM4MSIsInJoIjoiMC5BUjhBclBxVTJhQ01lVTZZa2ttOHp4YTdiUU1BQUFBQUFBQUF3QUFBQUFBQUFBQWZBQ1EuIiwic2NwIjoiQ2hhdC5SZWFkIE1haWwuUmVhZCBNYWlsLlNlbmQgb3BlbmlkIHByb2ZpbGUgVGVhbS5SZWFkQmFzaWMuQWxsIFVzZXIuUmVhZCBVc2VyLlJlYWRCYXNpYy5BbGwgZW1haWwiLCJzaWduaW5fc3RhdGUiOlsia21zaSJdLCJzdWIiOiJ1TkNyWk44MTNIdDBwVHlvTnBGTVA5YkxnQ2hOcm5OWVlRS1JKbjExb1N3IiwidGVuYW50X3JlZ2lvbl9zY29wZSI6IkVVIiwidGlkIjoiZDk5NGZhYWMtOGNhMC00ZTc5LTk4OTItNDliY2NmMTZiYjZkIiwidW5pcXVlX25hbWUiOiJtYXJrQHJlZHVjLnRlY2giLCJ1cG4iOiJtYXJrQHJlZHVjLnRlY2giLCJ1dGkiOiJRWjBCTzBiT2JVbUFRa0p3RHV3M0FBIiwidmVyIjoiMS4wIiwid2lkcyI6WyJiNzlmYmY0ZC0zZWY5LTQ2ODktODE0My03NmIxOTRlODU1MDkiXSwieG1zX2NjIjpbIkNQMSJdLCJ4bXNfc3NtIjoiMSIsInhtc19zdCI6eyJzdWIiOiIyaWwzOVVaUl90MlNjVHRaeTZEV05fZWUxd0ZvWkFIMnJsN0ROdnhPckxBIn0sInhtc190Y2R0IjoxNTIxMzk1NDE3fQ.NmH9EQ-EOH2M2tG9-LnsODEMM3i1IuH28ABSaYohLooAmmFL7zD5zKoyVWRSjxzAaNdmyAvZ1RMiyMv7mZnKi41RPLMm6oVRwtW-oXpa6jbaNUijzIS5eqTmt1cUe0Ahcg5jNTtuhZi3AEkUNs4c8SYyMs7CCRzba7z-4BOOwaIqAGaykiQj248D13TTFWZr6oPEs7x3aGkSqlxcuXvEFLBubKhXOkZuixz-To8OO708Dk80lpJwNUttMic9bgsIbnrbL_4PeFiGg_85sYdl5OXlktYLfEHWrdM1ZCZDlKSjXkNzkvEzNzqAFVfzFmiTICkFSKOM9uCyHuAWxBJuMg";

    /// <inheritdoc />
    protected override IEnumerable<IntegrationTestCase> M365TestCases
    {
        get
        {
            yield return new IntegrationTestCase(
                "Login with token and list users",
                new M365Login() { Token = new SCLConstant<StringStream>(Token) },
                new Log()
                {
                    Value = new M365UsersRead()
                    {
                        //Stream = StaticHelpers.Constant("{\"Foo\":1}")
                    }
                }
            ) { IgnoreLoggedValues = true, };
        }
    }
}
