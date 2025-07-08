# Travel Requests API

API REST para la gestión de solicitudes de viaje desarrollada en .NET 9 con arquitectura limpia.

> **Nota:** En el desarrollo de este proyecto se utilizó inteligencia artificial para asistir en la creación de scripts y documentación Swagger por falta de tiempo.

## Características

- **Arquitectura limpia**: Separación en capas (Domain, Application, Infrastructure, API)
- **Autenticación JWT**: Sistema de autenticación basado en tokens
- **Base de datos SQL Server**: Soporte para Entity Framework Core
- **Logging con Serilog**: Registro de eventos en consola y archivos
- **Documentación OpenAPI/Swagger**: Documentación automática de la API
- **Contenedores Docker**: Despliegue simplificado con Docker Compose

## Estructura del proyecto

```
consware-api/
├── src/
│   ├── TravelRequests.Api/          # Capa de presentación (Controllers, Program.cs)
│   ├── TravelRequests.Application/   # Capa de aplicación (Services, DTOs, Interfaces)
│   ├── TravelRequests.Domain/        # Capa de dominio (Entities, Enums)
│   └── TravelRequests.Infrastructure/ # Capa de infraestructura (Data, Config)
├── tests/
│   └── TravelRequests.Tests/         # Pruebas unitarias
├── docker-compose.yml
├── Dockerfile
└── TravelRequests.sln
```

## Requisitos previos

- .NET 9 SDK
- SQL Server (local o remoto)
- Docker y Docker Compose (opcional)

## Configuración del entorno

### Variables de entorno (.env)

Crea un archivo `.env` en la raíz del proyecto con las siguientes variables:

```env
# Base de datos
DB_SERVER=localhost,1433
DB_DATABASE=ConswareDB
DB_USER=SA
DB_PASSWORD=TuPasswordSegura123!
DB_MULTIPLE_ACTIVE_RESULT_SETS=true
DB_TRUST_SERVER_CERTIFICATE=true

# Configuración de entorno
ASPNETCORE_ENVIRONMENT=Development
```

### Configuración de JWT

La configuración JWT se encuentra en `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "TuClaveSecretaMuyLargaParaJWT123456789",
    "Issuer": "consware-api",
    "Audience": "consware-client"
  }
}
```

## Instalación y ejecución

### Opción 1: Ejecución local

1. Clona el repositorio:
```bash
git clone <url-del-repositorio>
cd consware-api
```

2. Restaura las dependencias:
```bash
dotnet restore
```

3. Configura las variables de entorno o actualiza `appsettings.json`

4. Ejecuta las migraciones de base de datos:
```bash
dotnet ef database update --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
```

5. Ejecuta la aplicación:
```bash
dotnet run --project src/TravelRequests.Api
```

La API estará disponible en:
- HTTP: `http://localhost:5156`
- HTTPS: `https://localhost:7156`

### Opción 2: Docker Compose

1. Crea el archivo `.env` con las variables de entorno

2. Ejecuta con Docker Compose:
```bash
docker-compose up -d
```

La API estará disponible en:
- HTTP: `http://localhost:8080`
- HTTPS: `http://localhost:8081`

## Documentación de la API

Una vez que la aplicación esté ejecutándose, puedes acceder a la documentación Swagger en:

- Desarrollo: `http://localhost:5156/swagger`
- Docker: `http://localhost:8080/swagger`

## Endpoints principales

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario

### Usuarios
- `GET /api/users` - Obtener todos los usuarios
- `GET /api/users/{id}` - Obtener usuario por ID
- `PUT /api/users/{id}` - Actualizar usuario
- `DELETE /api/users/{id}` - Eliminar usuario

### Solicitudes de viaje
- `GET /api/travelrequests` - Obtener todas las solicitudes
- `GET /api/travelrequests/{id}` - Obtener solicitud por ID
- `POST /api/travelrequests` - Crear nueva solicitud
- `PUT /api/travelrequests/{id}` - Actualizar solicitud
- `DELETE /api/travelrequests/{id}` - Eliminar solicitud

## Logging

Los logs se almacenan en la carpeta `logs/` con los siguientes archivos:
- `application.log` - Logs de producción
- `application-dev.log` - Logs de desarrollo

### Agregar migraciones

```bash
dotnet ef migrations add NombreMigracion --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
```

### Actualizar base de datos

```bash
dotnet ef database update --project src/TravelRequests.Infrastructure --startup-project src/TravelRequests.Api
```

## Tecnologías utilizadas

- **.NET 9** - Framework principal
- **Entity Framework Core 9** - ORM
- **SQL Server** - Base de datos
- **JWT Bearer Authentication** - Autenticación
- **Serilog** - Logging
- **Swagger/OpenAPI** - Documentación
- **Docker** - Contenedores
