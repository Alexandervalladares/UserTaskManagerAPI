# UserTaskManagerAPI

**API REST para la gestión de usuarios y tareas** desarrollada en .NET 10 con arquitectura en capas y buenas prácticas de desarrollo.

Esta solución cumple todos los requerimientos funcionales y técnicos solicitados en la prueba técnica, incorporando mejoras adicionales como paginación, auditoría de timestamps y manejo consistente de errores.

---

## Tabla de Contenidos

- [Características Principales](#características-principales)
- [Tecnologías Utilizadas](#tecnologías-utilizadas)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Documentación de la API](#documentación-de-la-api)
- [Endpoints Principales](#endpoints-principales)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Modelo de Base de Datos](#modelo-de-base-de-datos)

---

## Características Principales

### Gestión de Usuarios

- Creación de usuarios con validación de email único
- Consulta de detalle de usuario con estadísticas de tareas (totales y completadas)
- Actualización de información del usuario
- Eliminación de usuario con borrado en cascada de sus tareas

### Gestión de Tareas

- Creación de tareas asociadas a un usuario
- Listado paginado de tareas por usuario
- Actualización completa de tareas
- Alternar estado de completitud (completada/pendiente) mediante un endpoint dedicado
- Eliminación individual de tareas

### Características Técnicas

- Arquitectura en capas: Controllers → Services → Repositories → Data
- Inyección de dependencias
- Validaciones robustas con Data Annotations y lógica de negocio
- Logging estructurado
- Manejo centralizado y consistente de errores
- Paginación en listados
- Auditoría automática (`CreatedAt` / `UpdatedAt`)
- Documentación interactiva con Swagger/OpenAPI
- Soporte completo para Docker y Docker Compose

---

## Tecnologías Utilizadas

- .NET 10.0 (LTS)
- Entity Framework Core
- SQL Server 2022
- Swagger / OpenAPI
- Docker & Docker Compose

---

## Requisitos Previos

### Ejecución con Docker (recomendado)

- Docker Desktop (versión 4.0 o superior)
- Docker Compose (versión 2.0 o superior)

### Ejecución local (opcional)

- .NET 10 SDK
- SQL Server 2019 o superior
- Herramientas de Entity Framework Core (`dotnet-ef`)

---

## Instalación y Ejecución

### Opción 1: Con Docker (recomendado)

1. Clonar el repositorio:

```bash
git clone https://github.com/Alexandervalladares/UserTaskManagerAPI.git
cd UserTaskManagerAPI
```

2. Levantar los servicios:

```bash
docker-compose up --build -d
```

La aplicación estará disponible en:

- API Base: `http://localhost:8080`
- Documentación Swagger: `http://localhost:8080/api-docs`

Las migraciones de base de datos se aplican automáticamente al iniciar el contenedor.

#### Comandos útiles

```bash
# Ver logs en tiempo real de la API
docker-compose logs -f api

# Ver logs en tiempo real de SQL Server
docker-compose logs -f sqlserver

# Detener los servicios
docker-compose down

# Detener y eliminar volúmenes (limpieza completa)
docker-compose down -v
```

### Opción 2: Ejecución Local sin Docker

1. Configurar la cadena de conexión en `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UserTaskDatabase;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

2. Aplicar migraciones a la base de datos:

```bash
dotnet ef database update
```

3. Ejecutar la aplicación:

```bash
dotnet run
```

La API estará disponible en el puerto indicado en la consola (generalmente `https://localhost:7xxx`) y la documentación en `/api-docs`.

---

## Documentación de la API

La documentación completa e interactiva se genera automáticamente mediante Swagger UI y está disponible en:

**`http://localhost:8080/api-docs`**

Desde Swagger puedes:

- Visualizar todos los endpoints disponibles
- Probar las operaciones directamente en el navegador
- Consultar esquemas detallados de request y response
- Explorar parámetros de paginación (page, pageSize)

---

## Endpoints Principales

### Usuarios (`/api/user`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/user` | Crear nuevo usuario |
| `GET` | `/api/user?page=&pageSize=` | Listar usuarios (paginado) |
| `GET` | `/api/user/{id}` | Detalle de usuario con estadísticas |
| `PUT` | `/api/user/{id}` | Actualizar usuario |
| `DELETE` | `/api/user/{id}` | Eliminar usuario y sus tareas |

### Tareas (`/api/task`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| `POST` | `/api/task/user/{userId}` | Crear tarea para un usuario |
| `GET` | `/api/task/user/{userId}?page=&pageSize=` | Listar tareas del usuario (paginado) |
| `GET` | `/api/task/{id}` | Detalle de tarea |
| `PUT` | `/api/task/{id}` | Actualizar tarea completa |
| `PATCH` | `/api/task/{id}/complete` | Alternar estado completado/pendiente |
| `DELETE` | `/api/task/{id}` | Eliminar tarea |

---

## Estructura del Proyecto

```
UserTaskManagerAPI/
├── Controllers/          # Endpoints REST
├── Services/             # Lógica de negocio e interfaces
├── Repositories/         # Acceso a datos
├── Data/                 # DbContext y configuración EF
├── Models/
│   ├── Entities/         # Entidades de dominio
│   ├── DTOs/             # Objetos de transferencia
│   └── Pagination/       # Clases para paginación
├── Migrations/           # Migraciones de Entity Framework
├── Program.cs            # Configuración de la aplicación
├── appsettings.json      # Configuración
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

## Modelo de Base de Datos

La base de datos consta de dos tablas principales con la siguiente estructura:

### Tabla Users

- `UserId` (int, PK, Identity)
- `FullName` (nvarchar(200), NOT NULL)
- `EmailAddress` (nvarchar(150), NOT NULL, UNIQUE)
- `RegistrationDate` (datetime2, NOT NULL) – Fecha de creación del usuario
- `UpdatedAt` (datetime2, NULL) – Fecha de la última actualización del usuario

### Tabla Tasks

- `TaskId` (int, PK, Identity)
- `TaskDescription` (nvarchar(500), NOT NULL)
- `IsCompleted` (bit, NOT NULL, DEFAULT 0)
- `CreationDate` (datetime2, NOT NULL) – Fecha de creación de la tarea
- `UpdatedAt` (datetime2, NULL) – Fecha de la última actualización de la tarea
- `CompletionDate` (datetime2, NULL) – Fecha en que se marcó como completada (solo cuando IsCompleted = true)
- `UserId` (int, FK → Users(UserId), NOT NULL)

### Relaciones

- **Tipo:** Uno a muchos (un usuario puede tener múltiples tareas)
- **Comportamiento en borrado:** Eliminación en cascada – al eliminar un usuario, se eliminan automátamente todas sus tareas asociadas

---

## Principios de Desarrollo

El proyecto sigue principios SOLID, estándares RESTful y mejores prácticas de .NET, priorizando:

- Mantenibilidad del código
- Escalabilidad de la arquitectura
- Claridad y legibilidad
- Separación de responsabilidades
- Inyección de dependencias

---

## Autor

Alexander Valladares

---

## Licencia

Este proyecto fue desarrollado como parte de una prueba técnica.