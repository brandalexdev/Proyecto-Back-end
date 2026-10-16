# MVC_app_001 - Agenda Inteligente

Aplicación web ASP.NET Core MVC para administrar usuarios, eventos, categorías,
recordatorios y auditorías mediante Entity Framework Core y SQL Server.

## Estructura principal

```text
MVC_app_001/
├── Controllers/
├── Data/
│   └── AgendaDbContext.cs
├── Models/
├── Views/
├── Migrations/
├── Program.cs
├── appsettings.json
└── appsettings.Development.json
```

## Configuración

La aplicación carga las credenciales desde un archivo `.env` ubicado en la raíz
de la solución. El archivo está ignorado por Git y debe existir localmente:

```dotenv
APP_USER=admin
APP_PASSWORD=cambia-esta-clave

DB_SERVER=localhost
DB_NAME=AgendaInteligenteDb
DB_USER=agenda_user
DB_PASSWORD=cambia-esta-clave
DB_TRUST_SERVER_CERTIFICATE=True
```

También pueden definirse estas variables directamente en el entorno del sistema.
Las variables de entorno existentes tienen prioridad sobre `.env`.

La aplicación construye la conexión SQL Server con `DB_SERVER`, `DB_NAME`,
`DB_USER`, `DB_PASSWORD` y `DB_TRUST_SERVER_CERTIFICATE`. El acceso web está
protegido globalmente mediante autenticación HTTP Basic usando `APP_USER` y
`APP_PASSWORD`, y requiere HTTPS.

## SQL Server

El usuario de base de datos debe crearse en SQL Server antes de iniciar la
aplicación. El script `DatabaseUserAdministration.sql` contiene los permisos
requeridos; reemplaza sus valores de ejemplo por credenciales locales seguras.

## Ejecutar

Desde la raíz de la solución:

```bash
dotnet restore
dotnet build
dotnet run --project MVC_app_001
```

Para aplicar migraciones:

```bash
dotnet ef database update --project MVC_app_001
```

No publiques `.env`, credenciales reales ni cadenas de conexión en el
repositorio. Usa contraseñas diferentes para desarrollo y producción.
