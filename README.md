# 🚀 UserTask Manager API

Sistema robusto de gestión de usuarios y tareas desarrollado con .NET 8, implementando arquitectura en capas y mejores prácticas de desarrollo.

## 📋 Tabla de Contenidos

- [Características](#características)
- [Tecnologías](#tecnologías)
- [Requisitos Previos](#requisitos-previos)
- [Instalación y Ejecución](#instalación-y-ejecución)
- [Estructura del Proyecto](#estructura-del-proyecto)
- [Endpoints Disponibles](#endpoints-disponibles)
- [Base de Datos](#base-de-datos)

---

## ✨ Características

### Gestión de Usuarios
- ✅ Crear nuevos usuarios con validación de email único
- ✅ Consultar detalles de usuario con estadísticas de tareas
- ✅ Actualizar información de usuario
- ✅ Eliminar usuarios (con eliminación en cascada de tareas)

### Gestión de Tareas
- ✅ Crear tareas asociadas a usuarios
- ✅ Listar todas las tareas de un usuario
- ✅ Actualizar estado de completitud de tareas
- ✅ Alternar estado completado/pendiente
- ✅ Eliminar tareas individuales

### Características Técnicas
- ✅ Arquitectura en capas (Controllers, Services, Repositories, Data)
- ✅ Inyección de dependencias
- ✅ Validación de datos con Data Annotations
- ✅ Logging estructurado
- ✅ Manejo de errores centralizado
- ✅ Documentación automática con Swagger
- ✅ Soporte completo para Docker

---

## 🛠️ Tecnologías

- **.NET 8** (C#)
- **Entity Framework Core 8** (ORM)
- **SQL Server 2022**
- **Swagger/OpenAPI** (Documentación)
- **Docker & Docker Compose**

---

## 📦 Requisitos Previos

### Para ejecución con Docker (Recomendado):
- **Docker Desktop** 4.0 o superior
- **Docker Compose** 2.0 o superior

### Para ejecución local:
- **.NET 8 SDK**
- **SQL Server 2022** o **SQL Server 2019**
- **Visual Studio 2022** o **VS Code** (opcional)

---

## 🚀 Instalación y Ejecución

### 🐳 Opción 1: Con Docker (Recomendado)
```bash
# 1. Clonar el repositorio
git clone <url-del-repositorio>
cd UserTaskManagerAPI

# 2. Levantar los servicios con Docker Compose
docker-compose up --build -d

# 3. Esperar 30-45 segundos para que SQL Server inicialice

# 4. Acceder a la documentación Swagger
# Abrir navegador en: http://localhost:8080/api-docs
```

**Comandos útiles:**
```bash
# Ver logs de la API
docker-compose logs -f api

# Ver logs de SQL Server
docker-compose logs -f sqlserver

# Detener los servicios
docker-compose down

# Detener y eliminar volúmenes (limpieza completa)
docker-compose down -v
```

---

### 💻 Opción 2: Ejecución Local (Sin Docker)

#### Paso 1: Configurar SQL Server

Actualiza la cadena de conexión en `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=UserTaskDatabase;User Id=tu_usuario;Password=tu_password;TrustServerCertificate=True;"
  }
}
```

#### Paso 2: Instalar herramientas de Entity Framework
```bash
dotnet tool install --global dotnet-ef --version 8.0.0
```

#### Paso 3: Crear la base de datos
```bash
# Crear la migración inicial (si no existe)
dotnet ef migrations add InitialCreate

# Aplicar las migraciones
dotnet ef database update
```

#### Paso 4: Ejecutar la aplicación
```bash
# Restaurar dependencias
dotnet restore

# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run
```

**La API estará disponible en:**
- API Base: `http://localhost:5000`
- Swagger UI: `http://localhost:5000/api-docs`

---

## 📁 Estructura del Proyecto
```
UserTaskManagerAPI/
├── Controllers/              # Endpoints REST
│   ├── UserController.cs
│   └── TaskController.cs
├── Services/                 # Lógica de negocio
│   ├── IUserService.cs
│   ├── UserService.cs
│   ├── ITaskService.cs
│   └── TaskService.cs
├── Repositories/             # Acceso a datos
│   ├── IUserRepository.cs
│   ├── UserRepository.cs
│   ├── ITaskRepository.cs
│   └── TaskRepository.cs
├── Data/                     # Contexto de base de datos
│   └── ApplicationDbContext.cs
├── Models/                   # Entidades y DTOs
│   ├── Entities/
│   │   ├── UserEntity.cs
│   │   └── TaskEntity.cs
│   └── DTOs/
│       ├── UserDTOs.cs
│       └── TaskDTOs.cs
├── Migrations/               # Migraciones de EF Core
├── Program.cs                # Configuración principal
├── appsettings.json          # Configuración
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

## 🌐 Endpoints Disponibles

### 👤 Usuarios (`/api/user`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/user` | Crear un nuevo usuario |
| GET | `/api/user/{id}` | Obtener detalles de un usuario |
| GET | `/api/user` | Listar todos los usuarios |
| PUT | `/api/user/{id}` | Actualizar información del usuario |
| DELETE | `/api/user/{id}` | Eliminar un usuario |

### 📋 Tareas (`/api/task`)

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/task/user/{userId}` | Crear tarea para un usuario |
| GET | `/api/task/{id}` | Obtener detalles de una tarea |
| GET | `/api/task/user/{userId}` | Obtener todas las tareas de un usuario |
| PUT | `/api/task/{id}` | Actualizar una tarea |
| PATCH | `/api/task/{id}/complete` | Alternar estado de completitud |
| DELETE | `/api/task/{id}` | Eliminar una tarea |

---

## 📚 Documentación Swagger

Accede a la documentación interactiva en: **`http://localhost:8080/api-docs`**

Desde Swagger puedes:
- Ver todos los endpoints disponibles
- Probar las peticiones directamente
- Ver los esquemas de request/response
- Consultar códigos de estado HTTP

---

## 🗄️ Base de Datos

### Modelo de Datos

**Tabla: Users**

| Campo | Tipo | Restricciones |
|-------|------|---------------|
| UserId | int | PRIMARY KEY, IDENTITY |
| FullName | nvarchar(200) | NOT NULL |
| EmailAddress | nvarchar(150) | NOT NULL, UNIQUE |
| RegistrationDate | datetime2 | NOT NULL |

**Tabla: Tasks**

| Campo | Tipo | Restricciones |
|-------|------|---------------|
| TaskId | int | PRIMARY KEY, IDENTITY |
| TaskDescription | nvarchar(500) | NOT NULL |
| IsCompleted | bit | NOT NULL, DEFAULT 0 |
| CreationDate | datetime2 | NOT NULL |
| CompletionDate | datetime2 | NULL |
| UserId | int | FOREIGN KEY → Users(UserId) |

**Relación:** Un usuario puede tener múltiples tareas (1:N). Al eliminar un usuario, se eliminan todas sus tareas en cascada.

---

## 🧪 Ejemplos de Uso

### Crear Usuario
```bash
curl -X POST http://localhost:8080/api/user \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "María González",
    "emailAddress": "maria.gonzalez@empresa.com"
  }'
```

### Crear Tarea
```bash
curl -X POST http://localhost:8080/api/task/user/1 \
  -H "Content-Type: application/json" \
  -d '{
    "taskDescription": "Revisar documentación del proyecto"
  }'
```

### Marcar Tarea como Completada
```bash
curl -X PATCH http://localhost:8080/api/task/1/complete
```

### Listar Tareas de un Usuario
```bash
curl -X GET http://localhost:8080/api/task/user/1
```

---

## 🐛 Solución de Problemas

### Error: "No se puede conectar a SQL Server"

**Con Docker:**
```bash
# Verificar estado de contenedores
docker-compose ps

# Ver logs de SQL Server
docker-compose logs sqlserver

# Reiniciar servicios
docker-compose restart
```

**Sin Docker:**
- Verifica que SQL Server esté corriendo
- Comprueba las credenciales en `appsettings.json`

### Puerto ya en uso

Edita `docker-compose.yml` y cambia el puerto:
```yaml
ports:
  - "8081:8080"  # Cambiar 8080 por otro puerto disponible
```

---

## 👨‍💻 Desarrollo

Este proyecto fue desarrollado siguiendo:
- Principios SOLID
- Clean Architecture
- Mejores prácticas de .NET
- Estándares RESTful
- Código limpio y bien documentado

---

## 📄 Licencia

Proyecto desarrollado como prueba técnica - Uso educativo y demostrativo.

---

**✨ ¡Gracias por revisar este proyecto!**