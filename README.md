_educational demo_

# REST API Error Handling Demo in ASP NET
## proj layout
- Controllers (endpoints and responses)
- IRepositories (data access)
- Data (EF, sqlite)
- Middleware (Custom ErrorHandlingMiddleware)

## error handler flow
- catches exceptions in controller -> repo comms.
- writes custom error msg to response body
- logs error to server
