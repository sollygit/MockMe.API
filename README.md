[![Build and deploy .NET Core app to Windows WebApp MockMe-API](https://github.com/sollygit/MockMe.Api/actions/workflows/MockMe-Api.yml/badge.svg)](https://github.com/sollygit/MockMe.Api/actions/workflows/MockMe-Api.yml)

# JWT Auth Demo (ASP.NET Core + Angular)

This repository demos an Angular SPA and an ASP.NET Core web API application using JWT auth, and an integration testing project for a set of actions including login, logout, refresh token, impersonation, authentication, and authorization.

## Medium Articles

1. [JWT Auth in ASP.NET Core](https://codeburst.io/jwt-auth-in-asp-net-core-148fb72bed03)

2. [JWT Authentication in Angular](https://codeburst.io/jwt-authentication-in-angular-48cfa882832c)


## Solution Structure

This repository includes two applications: an Angular SPA in the `angular` folder, and an ASP.NET Core web API app in the `webapi` folder. The SPA makes HTTP requests to the server side (the `webapi` app) using an API BaseURL `https://localhost:44389`. The API BaseURL is set in the `environment.ts` file and the `environment.prod.ts` file, which can be modified based on your situation.

- `angular`
  The SPA application demonstrates JWT authorization in the front-end.
- `webapi`
  The ASP.NET Core web API app is served by Kestrel on Docker. This app has implemented HTTPS support.
