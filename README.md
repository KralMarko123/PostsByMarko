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
