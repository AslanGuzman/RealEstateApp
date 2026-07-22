# RealEstateApp

Gestión inmobiliaria. ASP.NET Core MVC + Web API (.NET 9), EF Core Code First, Onion, ASP.NET Core Identity.

## Requisitos

- .NET SDK 9.0
- SQL Server (Docker / instancia / LocalDB)

## Configuración — `RealEstateApp/appsettings.json`

- `ConnectionStrings:DefaultConnection` / `IdentityConnection` — cadena SQL Server. Usar un `Database` único.
- `MailSettings` — SMTP para activación de cuenta y restablecimiento de contraseña: `SmtpHost`, `SmtpPort`, `SmtpUser`, `SmtpPass`, `EmailFrom`, `DisplayName`. Gmail requiere *App Password*.

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RealEstateApp_<id>;Trusted_Connection=True;TrustServerCertificate=True",
  "IdentityConnection": "Server=(localdb)\\MSSQLLocalDB;Database=RealEstateApp_<id>;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Docker opcional: `docker compose up -d` levanta SQL Server en el puerto 1433 (contraseña en `.env`).

Las migraciones se aplican al arrancar.

## Ejecución

```bash
dotnet run --project RealEstateApp
```

`http://localhost:5089` · `https://localhost:7243`

## Acceso

Registro público. La cuenta se activa con el enlace enviado al correo; sin activar no se puede iniciar sesión.
