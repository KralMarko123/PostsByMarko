# PostsByMarko

> An ASP.NET/React solution that acts as a post sharing website. Users can register and create posts that others can view. They also have the option of chatting with one another. Administrators can manage users and view general statistics about the app's current state. Made with the purpose of showing the practical use of Microsoft's **SignalR**.

## Author: Marko Markovikj

### Coverage

![Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/KralMarko123/d3a54e4ca76749db84b05c1aa272dbe0/raw/postsbymarko-coverage.json)

### Local configuration

Requires the .NET 10 SDK, Node.js 24 LTS, and Docker Desktop with Linux containers. The client uses React 19, TypeScript 6, and Vite 8. EF Core and the Identity EF store remain on 9.0.20 because the stable [Pomelo MariaDB provider](https://www.nuget.org/packages/Pomelo.EntityFrameworkCore.MySql/9.0.0) supports EF Core 9. The application and all test projects target .NET 10.

Copy `.env.example` to `.env` if it does not already exist. Replace the placeholder database password and JWT signing key (at least 32 characters).

#### Persistent development

```powershell
docker compose up --build -d
```

Open the app at http://localhost:3000, API Swagger at http://localhost:7171, and the local email inbox at http://localhost:8025. Register an account, then follow the confirmation link in the inbox. Mailpit captures development email without delivering it externally.

Development uses database migrations and named volumes; accounts, posts, chats, and email confirmation keys survive restarts. Application roles are created on first startup. For an optional administrator, set both `DEVELOPMENT_ADMIN_EMAIL` and `DEVELOPMENT_ADMIN_PASSWORD` in `.env` before starting. The password must satisfy Identity's password policy. This creates a confirmed administrator only in Development. Existing administrator passwords are never reset; an existing non-admin email is rejected. Remove these two settings after successful creation.

```powershell
docker compose down
```

Stopping development preserves its data. Do not add `--volumes` unless you deliberately want to delete it. The legacy cleanup script also now preserves development volumes. Data from the former disposable default stack is not automatically migrated into this new development database.

#### Disposable testing

```powershell
docker compose -f docker-compose.test.yml up --build -d
```

Open the test app at http://localhost:13000 and API at http://localhost:17171. Test MariaDB uses port 13306. The test stack has separate project/container names, networks, and memory-backed database storage, so it can run alongside development. Its API resets and seeds the test database on startup, and stopping the database loses its contents. Email delivery is disabled.

Seeded test-only accounts: administrator `testAdmin@test.com` and user `test@test.com`, both with password `@Marko123`.

```powershell
docker compose -f docker-compose.test.yml down --volumes --remove-orphans
```

The scripts `run_test_docker_compose.ps1` and `remove_test_docker_compose.ps1` perform these test-only operations from any working directory.

For local development outside Docker, keep secrets out of `appsettings*.json` and provide them with ASP.NET Core user secrets or environment variables:

```powershell
dotnet user-secrets --project src/PostsByMarko.Host set "JwtConfig:Secret" "replace-with-at-least-32-random-characters"
```

Outside Compose, email delivery is disabled by the Development and Test configuration files. Production must provide the `EmailConfig` values through the deployment secret store. `SenderAddress` optionally separates the sender email from the SMTP login; it defaults to `Username` when omitted. Configure `Username` and `Password` when SMTP requires authentication.

### Correctness and security checks

The API rejects empty/whitespace post and message content. Request limits are 200 characters for titles, 20,000 for posts, and 4,000 for messages. Registration requires first and last names. Invalid requests return HTTP 400; failed authentication returns 401; denied access returns 403. Unexpected errors return a generic message and a trace ID.

Login and registration share a per-IP limit of 10 requests per minute, configurable with `Authentication:RequestsPerMinute`. Test configuration raises this to 1,000 for automated suites. Identity account lockout is also enforced for lockout-enabled accounts. When deploying behind a proxy, configure trusted forwarded headers and the ingress rate limit explicitly; do not trust arbitrary forwarded IP headers.

Database writes succeed independently of best-effort SignalR delivery. Missed events are reconciled when the client reconnects. Durable event delivery, request idempotency keys, and paginated history remain separate production-hardening work. Direct chat creation uses MariaDB row locks in a transaction to serialize concurrent creation for the same participants.

Run focused checks with:

```powershell
dotnet test test/PostsByMarko.UnitTests
cd src/PostsByMarko.Client
npm test
npm run lint
npm run typecheck
npm run build
```

The integration suite requires only disposable MariaDB. Stop the test stack first if it is already running, then start its database:

```powershell
docker compose -f docker-compose.test.yml up -d --wait database
$env:DB_USER = 'postsbymarko' # Match DB_USER in .env
$env:DB_PASSWORD = '<your .env database password>'
dotnet test test/PostsByMarko.IntegrationTests
```

Integration tests use port 13306 by default (`TEST_SQL_PORT` overrides it; CI uses 3306). They reset `postsbymarko_test`; run them separately from browser tests or manual use of the test app.

The browser suite invokes Docker Compose directly and starts only `docker-compose.test.yml`. Install its browsers first:

```powershell
dotnet build test/PostsByMarko.FrontendTests
pwsh test/PostsByMarko.FrontendTests/bin/Debug/net10.0/playwright.ps1 install --with-deps
dotnet test test/PostsByMarko.FrontendTests
```

Set `IsLocalDevelopment=true` only to reuse a manually started **test** stack at ports 13000/17171; the fixture then leaves its containers running. Keep DB credentials and JWT_SECRET in the root `.env` or environment for Compose. Persistent development is a local setup; production deployment hardening remains separate work.
