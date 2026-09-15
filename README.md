# eVote360

Plataforma web de votación electrónica desarrollada como mini proyecto para ITLA.
Permite gestionar todo el ciclo electoral: registro/validación de electores (con
OCR de cédula), configuración de procesos electorales, boletas con candidatos y
partidos, emisión confidencial del voto, y consulta de resultados.

> Este README documenta el estado del proyecto **después de una revisión y
> explica exactamente qué se encontró incompleto y qué se corrigió.

---

## Tabla de contenido

- [Stack técnico](#stack-técnico)
- [Arquitectura](#arquitectura)
- [Estructura de carpetas](#estructura-de-carpetas)
- [Cómo correr el proyecto localmente](#cómo-correr-el-proyecto-localmente)
- [Usuarios y datos de prueba](#usuarios-y-datos-de-prueba)
- [Mapeo de funcionalidades del documento funcional](#mapeo-de-funcionalidades-del-documento-funcional)
- [Cambios realizados en esta revisión](#cambios-realizados-en-esta-revisión)
- [Limitaciones conocidas / pendientes](#limitaciones-conocidas--pendientes)
- [Despliegue en Azure](#despliegue-en-azure)

---

## Stack técnico

Cumple los requerimientos técnicos del documento funcional:

| Requerimiento | Implementación |
|---|---|
| ASP.NET Core MVC | .NET 8 |
| ViewModels + validaciones | `DataAnnotations` en los ViewModels de `eVote360.Application/ViewModels` |
| DTOs para transferencia de datos | `eVote360.Application/DTOs` (Request/Response) |
| Entity Framework Core (Code First) | `eVote360.Infrastructure/Persistence` + migraciones |
| AutoMapper | Registrado en `ServicesRegistration.cs` |
| UI | Bootstrap 5 + Bootstrap Icons |
| Arquitectura Onion | Ver [Arquitectura](#arquitectura) |
| Repository + Service genéricos | `IGenericRepository<T>` / `GenericRepository<T>` |
| Capa Shared para correo | `eVote360.Shared/Emails` |

## Arquitectura

El proyecto sigue **arquitectura Onion** con 5 proyectos:

```
eVote360.Domain          ← Entidades y enums puros, sin dependencias externas
eVote360.Application     ← Casos de uso: interfaces (abstracciones), servicios,
                            DTOs, ViewModels, resultados (Result<T>)
eVote360.Infrastructure  ← EF Core (DbContext, migraciones, repositorios),
                            envío de correo (SMTP), OCR (Tesseract)
eVote360.Shared          ← Utilidades transversales (opciones de email, etc.)
eVote360.Web             ← Controladores MVC, Vistas Razor, ViewModels de UI,
                            middlewares
```

Regla de dependencias (de afuera hacia adentro): `Web → Infrastructure →
Application → Domain`. `Web` y `Infrastructure` **solo conocen interfaces**
de `Application` (`I...Service`, `I...Repository`); nunca se referencia
`DbContext` directamente desde `Web` (se corrigió una violación existente,
ver más abajo).

## Estructura de carpetas

```
eVote360.Web/
├── Controllers/            (Login, Cuenta, Elector)
├── Areas/
│   ├── Admin/               → Puestos, Ciudadanos, Partidos, Usuarios,
│   │                           Asignación de dirigentes, Elecciones
│   └── Dirigente/            → Candidatos, CandidatoPuesto (asignar a puesto),
│                                Alianzas políticas
├── Views/
├── Models/                  (ViewModels específicos de la pantalla de votación)
└── Middlewares/AuthMiddleware.cs   (control de acceso basado en sesión)
```

## Cómo correr el proyecto localmente

**Requisitos:** .NET SDK 8, SQL Server (local o contenedor), Node no es necesario.

```bash
# 1. Restaurar dependencias
dotnet restore eVote360.Web.sln

# 2. Configurar la cadena de conexión (ya viene lista para SQL Server local
#    con autenticación de Windows/integrada en appsettings.Development.json)
#    eVote360.Web/appsettings.Development.json
#    "DefaultConnection": "Server=localhost;Database=eVote360;Trusted_Connection=True;TrustServerCertificate=True;"

# 3. Ejecutar (aplica migraciones automáticamente al iniciar)
dotnet run --project eVote360.Web
```

Al iniciar en modo **Development**, el programa:
1. Aplica automáticamente las migraciones pendientes (`Database.MigrateAsync()`).
2. Siembra datos de demostración (`DataSeeder.SeedAsync`) **solo si la base
   está vacía**: usuarios, puestos, partidos, candidatos y ciudadanos de
   ejemplo — ver credenciales abajo. Esto antes existía en el código pero
   nunca se invocaba (ver [Cambios realizados](#cambios-realizados-en-esta-revisión)).

> Si prefieres aplicar las migraciones manualmente:
> ```bash
> dotnet tool install --global dotnet-ef
> dotnet ef database update --project eVote360.Infrastructure --startup-project eVote360.Web
> ```

### Correo (comprobante de voto)

El envío de correo usa SMTP configurado en `EmailSenderOptions` (appsettings).
Para pruebas locales sin un servidor SMTP real, el correo simplemente
fallará silenciosamente y quedará registrado en el log — **el voto igual
se guarda correctamente**, el envío del comprobante nunca bloquea el flujo.

### OCR de cédula

Usa Tesseract OCR (`TesseractOcrService`). Si el motor no está disponible en
el entorno (falta el binario o los datos de idioma), el sistema **aprueba
automáticamente** la validación de identidad para no bloquear el flujo en
desarrollo (`IOcrService.IsAvailable == false`). En producción, instala
Tesseract y configura `Ocr:TessDataPath`.

## Usuarios y datos de prueba

Creados por `DataSeeder` (solo en Development):

| Usuario | Contraseña | Rol | Partido |
|---|---|---|---|
| `admin` | `Admin123!` | Administrador | — |
| `abinader` | `Dirigente123!` | Dirigente | PRM |
| `medina` | `Dirigente123!` | Dirigente | PLD |
| `leonel` | `Dirigente123!` | Dirigente | FP |

Elector de prueba (cédula): `001-1234567-1` (Albertson Terrero López), más
~49 ciudadanos adicionales generados automáticamente.

## Mapeo de funcionalidades del documento funcional

| Sección del documento | Dónde vive en el código |
|---|---|
| Flujo del elector (documento → OCR → boleta → voto) | `ElectorController` + `Views/Elector` |
| Mantenimiento de Puesto electivo | `Areas/Admin/Controllers/PositionsController` |
| Mantenimiento de Ciudadano | `Areas/Admin/Controllers/CitizensController` |
| Mantenimiento de Partidos políticos | `Areas/Admin/Controllers/PartiesController` |
| Mantenimiento de Usuarios | `Areas/Admin/Controllers/UsersController` |
| Asignación de dirigentes políticos | `Areas/Admin/Controllers/PartyAssignmentsController` |
| Elecciones (crear/finalizar/resultados) | `Areas/Admin/Controllers/EleccionesController` |
| Resumen electoral por año | `Areas/Admin/Controllers/HomeController` |
| Mantenimiento de candidatos (dirigente) | `Areas/Dirigente/Controllers/CandidatosController` |
| Alianzas políticas | `Areas/Dirigente/Controllers/AlianzasController` |
| Asignar candidato a puesto (con reglas de alianza) | `Areas/Dirigente/Controllers/CandidatoPuestoController` |
| Control de acceso por rol / redirecciones | `Middlewares/AuthMiddleware.cs` |

## Limitaciones conocidas / pendientes

- El flujo de "listado de puestos disponibles → elegir un puesto → votar →
  volver al listado" que describe el documento se implementó como una
  **única pantalla** con todos los puestos y sus candidatos a la vez
  (`Opciones.cshtml`), en lugar de navegar puesto por puesto. Funcionalmente
  cubre el mismo requisito (un voto por puesto, validación de completitud,
  resumen final), pero es una simplificación de UX respecto al documento.
- Elecciones creadas antes de esta revisión no tienen boleta "congelada"
  (ver nota en el punto 2 de cambios); su resumen mostrará 0 en partidos y
  candidatos hasta que se cree una elección nueva.
- No se implementó backfill/migración de datos históricos.