# Agenda Inteligente

Aplicación web para gestionar eventos, categorías, recordatorios, usuarios y
auditorías. Fue desarrollada con **ASP.NET Core MVC**, **Entity Framework
Core** y **SQL Server**.

## 1. Funcionalidades

- Inicio de sesión y registro de usuarios.
- Autenticación mediante cookies.
- Administración de eventos.
- Administración de categorías.
- Administración de recordatorios.
- Consulta de auditorías.
- Control de acceso a las páginas internas.
- Persistencia de información en SQL Server.

## 2. Arquitectura de software

El proyecto utiliza el patrón **MVC (Model-View-Controller)**:

```text
Usuario
   │
   ▼
Controller ───────► Model ───────► SQL Server
   │                  │
   ▼                  ▼
 View             Entity Framework Core
```

### Model

Representa los datos y las reglas básicas del sistema.

Ubicación: `MVC_app_001/Models`

Incluye modelos como:

- `Usuario`
- `Evento`
- `Categoria`
- `Recordatorio`
- `Auditoria`
- `EstadoEvento`

`LoginViewModel` se utiliza exclusivamente para recibir los datos de login y
registro.

### View

Contiene las páginas HTML Razor que ve el usuario.

Ubicación: `MVC_app_001/Views`

Las vistas se organizan por controlador. Por ejemplo:

```text
Views/
├── Login/
├── Eventos/
├── Categoria/
├── Auditoria/
├── Recordatorio/
└── Shared/
```

### Controller

Recibe las solicitudes HTTP, valida los datos y coordina las vistas con la
base de datos.

Ubicación: `MVC_app_001/Controllers`

Controladores principales:

| Controlador | Responsabilidad |
|---|---|
| `LoginController` | Login, registro y cierre de sesión |
| `EventosController` | Crear, consultar, editar y eliminar eventos |
| `CategoriaController` | Gestionar categorías |
| `RecordatorioController` | Gestionar recordatorios |
| `AuditoriaController` | Consultar y gestionar auditorías |
| `UsuarioController` | Gestionar usuarios |

### Acceso a datos

`AgendaDbContext` conecta Entity Framework Core con SQL Server y representa
las tablas mediante `DbSet`.

Ubicación: `MVC_app_001/Data/AgendaDbContext.cs`

Las migraciones se almacenan en:

`MVC_app_001/Migrations`

## 3. Estructura del proyecto

```text
MVC_app_001/
├── MVC_app_001/
│   ├── Controllers/       # Controladores MVC
│   ├── Data/              # Contexto de base de datos
│   ├── Migrations/        # Migraciones de Entity Framework Core
│   ├── Models/            # Entidades y modelos de vista
│   ├── Views/             # Vistas Razor
│   ├── Program.cs         # Configuración e inicio de la aplicación
│   └── MVC_app_001.csproj # Proyecto y paquetes NuGet
├── .env                   # Configuración local, no se publica
└── README.md
```

## 4. Requisitos

- .NET SDK 10.
- SQL Server iniciado.
- Permisos de Windows para acceder a la base de datos.
- Herramienta `dotnet-ef` para administrar migraciones.

Instalación de `dotnet-ef`, si todavía no está disponible:

```bash
dotnet tool install --global dotnet-ef
```

## 5. Configuración de la base de datos

La aplicación utiliza autenticación integrada de Windows. No se requiere un
usuario ni una contraseña de SQL Server.

Crear el archivo `.env` en la raíz del proyecto:

```dotenv
DB_SERVER=ALEX-NITRO
DB_NAME=AgendaInteligenteDb
DB_TRUST_SERVER_CERTIFICATE=True
```

La conexión se construye en `Program.cs` usando:

```text
Server=ALEX-NITRO;
Database=AgendaInteligenteDb;
Integrated Security=True;
TrustServerCertificate=True
```

No incluir contraseñas, tokens ni archivos `.env` en el repositorio.

## 6. Inicio del proyecto

Ejecutar los siguientes pasos desde la raíz del repositorio:

### Paso 1: restaurar paquetes

```bash
dotnet restore
```

### Paso 2: aplicar migraciones

```bash
dotnet ef database update --project .\MVC_app_001\MVC_app_001.csproj --startup-project .\MVC_app_001\MVC_app_001.csproj
```

Este comando crea o actualiza las tablas de `AgendaInteligenteDb`, incluida
la tabla `Auditoria`.

### Paso 3: compilar

```bash
dotnet build .\MVC_app_001\MVC_app_001.csproj
```

### Paso 4: ejecutar

```bash
dotnet run --project .\MVC_app_001\MVC_app_001.csproj
```

Abrir la URL HTTPS indicada en la consola. Normalmente:

`https://localhost:7056/Login/Index`

### Paso 5: crear el primer usuario

1. Entrar a **Crear usuario**.
2. Completar nombre, correo y contraseña.
3. Volver a la pantalla de login.
4. Iniciar sesión.

Después del login, la aplicación muestra el administrador de eventos.

## 7. Rutas principales

| Ruta | Descripción |
|---|---|
| `/` | Redirige al login |
| `/Login/Index` | Inicio de sesión |
| `/Login/Register` | Registro de usuario |
| `/Eventos/Index` | Gestión de eventos |
| `/Categoria/Index` | Gestión de categorías |
| `/Recordatorio/Index` | Gestión de recordatorios |
| `/Auditoria/Index` | Gestión de auditorías |
| `/Login/Logout` | Cierre de sesión |

## 8. Actualizar librerías y paquetes

## 8. API REST JSON

La aplicación también expone operaciones CRUD mediante controladores
`[ApiController]` y respuestas JSON. Las rutas disponibles son:

| Recurso | Ruta |
|---|---|
| Eventos | `/api/eventos` |
| Recordatorios | `/api/recordatorios` |
| Usuarios | `/api/usuarios` |
| Auditorías | `/api/auditorias` |

Cada recurso admite `GET` para listar, `GET /{id}` para consultar por
identificador, `POST` para crear, `PUT /{id}` para modificar y
`DELETE /{id}` para eliminar. Los eventos no requieren usuario y su estado es
opcional. Las respuestas de usuarios nunca incluyen la contraseña.

Ejemplo de creación de evento:

```json
{
  "titulo": "Reunión de proyecto",
  "descripcion": "Revisión semanal",
  "fechaInicio": "2026-11-01T10:00:00",
  "fechaFin": "2026-11-01T11:00:00",
  "lugar": "Sala 1",
  "categoriaId": 1,
  "estadoEventoId": null
}
```

## 9. Actualizar librerías y paquetes

Los paquetes del proyecto se encuentran en:

`MVC_app_001/MVC_app_001.csproj`

Para consultar los paquetes instalados:

```bash
dotnet list .\MVC_app_001\MVC_app_001.csproj package
```

Para revisar paquetes con versiones más nuevas:

```bash
dotnet list .\MVC_app_001\MVC_app_001.csproj package --outdated
```

Para actualizar un paquete específico:

```bash
dotnet add .\MVC_app_001\MVC_app_001.csproj package Nombre.Del.Paquete --version X.Y.Z
```

Ejemplo:

```bash
dotnet add .\MVC_app_001\MVC_app_001.csproj package Microsoft.EntityFrameworkCore.SqlServer --version 10.0.0
```

Después de actualizar paquetes:

```bash
dotnet restore
dotnet build .\MVC_app_001\MVC_app_001.csproj
```

No actualizar paquetes individualmente a versiones incompatibles. Los paquetes
de Entity Framework Core deben mantenerse en la misma versión principal y,
preferentemente, en la misma versión exacta.

## 10. Actualizar la base de datos después de cambiar modelos

Cuando se modifique una entidad en `Models`, crear una nueva migración:

```bash
dotnet ef migrations add NombreDelCambio --project .\MVC_app_001\MVC_app_001.csproj --startup-project .\MVC_app_001\MVC_app_001.csproj
```

Después aplicar la migración:

```bash
dotnet ef database update --project .\MVC_app_001\MVC_app_001.csproj --startup-project .\MVC_app_001\MVC_app_001.csproj
```

No modificar manualmente las migraciones que ya fueron aplicadas en la base de
datos. Para revisar las migraciones disponibles:

```bash
dotnet ef migrations list --project .\MVC_app_001\MVC_app_001.csproj --startup-project .\MVC_app_001\MVC_app_001.csproj
```
