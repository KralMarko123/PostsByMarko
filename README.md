# PostsByMarko

> An ASP.NET/React solution that acts as a post sharing website. Users can register and create posts that others can view. They also have the option of chatting with one another. Administrators can manage users and view general statistics about the app's current state. Made with the purpose of showing the practical use of Microsoft's **SignalR**.

## Author: Marko Markovikj

### Coverage

![Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KralMarko123/d3a54e4ca76749db84b05c1aa272dbe0/raw/postsbymarko-coverage.json)

### Local configuration

Copy `.env.example` to `.env`, replace the placeholder database password and JWT signing key, then run `docker compose up --build`. The signing key must contain at least 32 characters.

For local development outside Docker, keep secrets out of `appsettings*.json` and provide them with ASP.NET Core user secrets or environment variables:

```powershell
dotnet user-secrets --project src/PostsByMarko.Host set "JwtConfig:Secret" "replace-with-at-least-32-random-characters"
```

Email delivery is disabled in Development and Test. Production must provide the `EmailConfig` values, including its password, through the deployment secret store.

### Correctness and security checks

The API rejects empty/whitespace post and message content. Request limits are 200 characters for titles, 20,000 for posts, and 4,000 for messages. Registration requires first and last names. Invalid requests return HTTP 400; failed authentication returns 401; denied access returns 403. Unexpected errors return a generic message and a trace ID.

Login and registration share a per-IP limit of 10 requests per minute, configurable with `Authentication:RequestsPerMinute`. Test configuration raises this to 1,000 for automated suites. Identity account lockout is also enforced for lockout-enabled accounts. When deploying behind a proxy, configure trusted forwarded headers and the ingress rate limit explicitly; do not trust arbitrary forwarded IP headers.

Database writes succeed independently of best-effort SignalR delivery. Missed events are reconciled when the client reconnects. Durable event delivery, request idempotency keys, and paginated history remain separate production-hardening work. Direct chat creation uses MariaDB row locks in a transaction to serialize concurrent creation for the same participants.

Run focused checks with:

```powershell
dotnet test test/PostsByMarko.UnitTests
cd src/PostsByMarko.Client
npm test -- --runInBand
npm run build
```

The integration suite requires MariaDB and includes concurrent chat creation coverage. The browser suite requires Docker. The existing default Compose stack is still a **disposable Test environment**: API startup resets its test database. Do not use it to retain real user data; persistent development and production deployment configuration must be set up separately before release.
