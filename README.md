# JLLG-Mermaid

A full-stack scaffold that mirrors the requested Mermaid diagram: a React frontend talking to an API Gateway, which in turn proxies to three Auth0-protected microservices backed by separate MongoDB databases.

## Structure
- `frontend/` – React + Vite app with Auth0 integration and buttons to query each service through the gateway.
- `api-gateway/` – ASP.NET Core minimal API proxy with JWT validation and fan-out routes to each microservice.
- `src/SvcA` – Microservice A (items with descriptions) with MongoDB driver wiring and Auth0 protection.
- `src/SvcB` – Microservice B (catalog items with prices) with MongoDB driver wiring and Auth0 protection.
- `src/SvcC` – Microservice C (work items with completion flag) with MongoDB driver wiring and Auth0 protection.

## Backend setup
1. Ensure you have the .NET 8 SDK and MongoDB running locally.
2. Set Auth0 values in each `appsettings.json` (`Auth0:Authority` and `Auth0:Audience` per service and gateway).
3. From each backend folder, restore and run (example for Service A):
   ```bash
   cd src/SvcA
   dotnet restore
   dotnet run --urls http://localhost:5001
   ```
4. Repeat for `src/SvcB` (e.g., port 5002), `src/SvcC` (e.g., port 5003), and `api-gateway` (e.g., port 5000).

## Frontend setup
1. Install Node.js 18+.
2. Add a `.env` file in `frontend/` with:
   ```env
   VITE_AUTH0_DOMAIN=your-tenant.us.auth0.com
   VITE_AUTH0_CLIENT_ID=your-client-id
   VITE_AUTH0_AUDIENCE=YOUR_AUTH0_API_IDENTIFIER
   VITE_GATEWAY_URL=http://localhost:5000
   ```
3. Install and run:
   ```bash
   cd frontend
   npm install
   npm run dev
   ```
4. Log in with Auth0, then use the buttons to call each microservice via the gateway.

## Notes
- Each service defines `GET /items` and `POST /items` endpoints secured by JWT Bearer tokens.
- MongoDB collections and database names are configurable per service in their `appsettings.json` files.
- The gateway simply forwards authorized calls to the configured microservice URLs.
