# eVote360

Plataforma web de votación electrónica desarrollada como mini proyecto para ITLA.
Permite gestionar todo el ciclo electoral: registro/validación de electores (con
OCR de cédula), configuración de procesos electorales, boletas con candidatos y
partidos, emisión confidencial del voto, y consulta de resultados.

> Este README documenta el estado del proyecto **después de una revisión y
> corrección de brechas frente al documento funcional** (`Mini_proyecto__eVote360.pdf`).
> La sección [Cambios realizados en esta revisión](#cambios-realizados-en-esta-revisión)
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

## Cambios realizados en esta revisión

Se comparó el código contra el documento funcional punto por punto y se
corrigieron las siguientes brechas. **No se pudo compilar el proyecto en
este entorno** (sin acceso a `api.nuget.org` para restaurar paquetes), así
que todo se validó mediante lectura estática cuidadosa del código, incluyendo
verificación de balance de llaves/paréntesis en cada archivo tocado. Se
recomienda correr `dotnet build` localmente antes de dar el proyecto por
definitivo.

### 1. Opción "Ninguno" (voto en blanco) — faltaba por completo
El documento exige que el listado de candidatos incluya siempre una opción
genérica "Ninguno". El dominio ya tenía `VoteItem.CandidateId` como
nullable pero la UI nunca la ofrecía. Se agregó en `CandidatoOpcionVM`,
`SeleccionVM`, `ElectorController.Opciones()` y las vistas
`Opciones.cshtml` / `Boleta.cshtml`, y se refleja en resultados y en el
correo de comprobante.

### 2. Boleta electoral "congelada" por elección (snapshot) — no estaba conectada
El dominio ya incluía las entidades `ElectionBallot` y `BallotOption`
(pensadas exactamente para esto) y estaban registradas en el `DbContext` y
en las migraciones, **pero ningún servicio las usaba nunca**. Esto rompía
tres requisitos explícitos del documento:
- Que un mismo candidato pueda aparecer corriendo por varios partidos
  aliados (`CandidatoPuestoController` ya soportaba esto vía la tabla
  `Candidatura`, pero la boleta del elector y los resultados leían
  directamente `Candidate.PartyId`/`PositionId`, que solo reflejan el
  partido de origen).
- Que los resultados de una elección pasada **no cambien** aunque después
  se inactiven o editen partidos, puestos o candidatos.
- Que el resumen electoral del Admin cuente correctamente "cantidad de
  partidos" vs. "cantidad de candidatos reales".

Se implementó:
- `ICandidaturaRepository` / `CandidaturaRepository` (antes esta tabla solo
  se consultaba con `AppDbContext` inyectado directamente en un controlador
  Web — ver punto 4).
- `IElectionBallotRepository` / `ElectionBallotRepository`.
- `ElectionService.CreateElectionAsync` ahora, al crear la elección, toma
  la "foto" de la boleta (`SnapshotBallotAsync`): un `ElectionBallot` por
  puesto activo y, dentro, un `BallotOption` por cada partido activo con
  una `Candidatura` vigente y candidato activo para ese puesto.
- `ElectionService.ValidateCanCreateElectionAsync` ahora valida la regla
  "cada partido debe tener candidatos para todos los puestos" contra la
  tabla `Candidatura` (incluye candidatos de partidos aliados asignados a
  ese partido), no contra `Candidate.PositionId` en solitario.
- `ElectorController.Opciones()` construye la boleta desde el snapshot, no
  desde el estado en vivo. Para la pantalla de votación se muestra **una
  fila por candidato real** (como pide el documento), priorizando el
  partido de origen como "el partido que representa"; el resto de partidos
  bajo los que corre ese mismo candidato siguen contando para las
  estadísticas de "cantidad de partidos" en el resumen electoral.
- `ElectionService.GetResultadosAsync` y `AdminDashboardService
  .GetResumenElectoralAsync` ahora leen el snapshot en vez de las tablas
  en vivo.

> ⚠️ **Nota de compatibilidad:** elecciones creadas *antes* de este cambio
> no tendrán snapshot y su resumen mostrará 0 partidos/candidatos. Solo las
> elecciones creadas después de esta actualización quedan correctamente
> "congeladas". No se implementó una migración de backfill.

### 3. Violación de arquitectura Onion — `AppDbContext` inyectado en un controlador Web
`CandidatoPuestoController` (área Dirigente) inyectaba `AppDbContext`
directamente y hacía consultas LINQ contra `_db.Candidaturas` — esto viola
la regla del documento de que la arquitectura Onion debe aplicarse al 100%.
Se refactorizó para usar `ICandidaturaRepository` (capa Application),
eliminando toda referencia a Entity Framework desde `eVote360.Web`.

### 4. Mecanismo duplicado/conflictivo para asignar candidato a puesto
Existían **dos** caminos distintos para lo mismo: el correcto
(`CandidatoPuestoController`, usando la tabla `Candidatura`, con las reglas
de alianza) y uno duplicado dentro de `CandidatosController.Asignar` que
modificaba `Candidate.PositionId` directamente, sin crear fila en
`Candidatura` y sin validar las reglas de alianza. Se eliminó el duplicado
(acción, vista y botón "Asignar a Puesto" dentro de "Mis Candidatos") para
evitar estados inconsistentes; el menú del dirigente ahora usa un único
camino, tal como describe el documento ("Asignar candidato a puesto" como
ítem de menú independiente).

### 5. Correo de comprobante de voto — infraestructura lista pero nunca invocada
Toda la infraestructura de correo (SMTP, cola en memoria, plantilla HTML
del comprobante) ya existía, pero `ElectorController.EmitirVoto()` nunca
llamaba a `IEmailService`. Se conectó: al finalizar la votación se arma la
lista de selecciones (incluyendo "Ninguno") y se envía el correo; si el
envío falla, se registra en el log pero **el voto ya quedó guardado y el
flujo del elector no se interrumpe**, como exige el documento.

### 6. Redirección de Admin/Dirigente fuera de la pantalla de votación
El documento exige explícitamente que un administrador logueado que
intente entrar a la pantalla de votación del elector sea redirigido a su
propio home. `AuthMiddleware` no cubría este caso (dejaba pasar `/Elector/*`
como ruta pública sin mirar la sesión). Se agregó la verificación.

### 7. Pantalla "Acceso Denegado" sin enlace al home correspondiente
El documento pide que, ante un intento de acceso indebido, se muestre un
enlace a "su pantalla de inicio correspondiente". La vista solo ofrecía un
botón genérico de "Iniciar sesión". Se actualizó `CuentaController.Denegado`
y su vista para enlazar al home de Admin o Dirigente según el rol en sesión.

### 8. Año seleccionado por defecto en el resumen electoral del Admin
El documento pide que, por defecto, se seleccione **el año más reciente con
elecciones registradas**. El código usaba `DateTime.Now.Year` (el año
calendario actual), que puede no tener ninguna elección. Se corrigió para
usar el año máximo disponible en la lista de años con datos.

### 9. Resumen electoral del Admin — lógica sin implementar
`AdminDashboardService.GetResumenElectoralAsync` tenía un comentario
`// Simplified implementation` y devolvía una lista vacía siempre. Se
implementó completamente usando el snapshot de boleta (ver punto 2).

### 10. Validación server-side de "faltan puestos por votar"
El documento pide un mensaje específico indicando para cuáles puestos aún
falta seleccionar candidato si el elector intenta finalizar sin completar
todos. La UI ya marcaba los radios como `required` (bloqueo en el
navegador), pero no había respaldo en el servidor. Se agregó la validación
en `ElectorController.GuardarBoleta`.

### 11. Sembrado de datos de prueba nunca se ejecutaba
Existía un `DataSeeder` completo con usuarios/puestos/partidos/candidatos/
ciudadanos de ejemplo, pero el archivo se llamaba `DataSeeder copy.cs` (con
un espacio, nunca referenciado desde `Program.cs`) y jamás se invocaba —
es decir, en una base de datos nueva **no existía forma de iniciar sesión**.
Se renombró el archivo, se agregó la creación de las filas `Candidatura`
correspondientes a cada candidato sembrado (necesarias para el nuevo
snapshot de boleta) y se conectó en `Program.cs` para ejecutarse
automáticamente en Development junto con las migraciones.

## Limitaciones conocidas / pendientes

- **No se pudo ejecutar `dotnet build`/`dotnet restore` en este entorno**
  (sin acceso de red a `api.nuget.org`). Todos los cambios se revisaron
  manualmente con mucho cuidado, pero se recomienda compilar localmente
  antes de considerar el proyecto cerrado.
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

## Despliegue en Azure

Ver [`DEPLOY.md`](./DEPLOY.md) para el procedimiento completo (Azure App
Service + Azure SQL Database, scripts de infraestructura con Bicep, CI/CD
con GitHub Actions).
