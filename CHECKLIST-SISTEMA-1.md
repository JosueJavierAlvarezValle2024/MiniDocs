# Checklist de MiniDocs (Sistema 1)

## Funcionalidad

- [x] Proyecto ASP.NET Core MVC ejecutable localmente.
- [x] Base de datos SQL Server LocalDB configurada.
- [x] Migraciones de Entity Framework Core creadas y aplicadas.
- [x] Inicio y cierre de sesión.
- [x] Roles `SuperAdministrador`, `Administrador` y `Usuario`.
- [x] Protección de la cuenta única de SuperAdministrador.
- [x] Gestión de usuarios.
- [x] Gestión de departamentos.
- [x] Creación de documentos.
- [x] Edición de metadatos del documento.
- [x] Listado y filtrado por título y departamento.
- [x] Descarga de archivos.
- [x] Eliminación lógica de documentos.
- [x] Validación de extensiones permitidas.
- [x] Límite de archivos de 10 MB.
- [x] Auditoría de creación, edición, descarga y eliminación.
- [x] Vista de auditoría para administradores.

## Seguridad

- [x] Rutas administrativas protegidas por roles.
- [x] Documentos protegidos por autenticación.
- [x] Un usuario normal solo consulta sus propios documentos.
- [x] Nombres físicos de archivos generados con GUID.
- [x] Nombre original del archivo saneado con `Path.GetFileName`.
- [x] Protección antiforgery en operaciones POST.
- [x] Credenciales configuradas mediante User Secrets.

## Pruebas

- [x] 12 pruebas automatizadas de validación y autorización.
- [x] Smoke tests HTTP para inicio, login y rutas protegidas.
- [x] Prueba manual de login y roles.
- [x] Prueba manual de departamentos y usuarios.
- [x] Prueba manual de carga, edición, descarga y eliminación.
- [x] Prueba manual de rechazo de extensiones no permitidas.
- [x] Prueba manual de auditoría.

## Pendiente para una versión futura

- [ ] Recuperación de contraseña por correo.
- [ ] Cambio de contraseña desde el perfil.
- [ ] Paginación de listados grandes.
- [ ] Registro de auditoría de usuarios y departamentos.
- [ ] Versionado de documentos.
- [ ] Flujos de aprobación.
- [ ] API REST.
- [ ] Docker y despliegue.

## Resultado

MiniDocs cumple el objetivo de ser un primer sistema práctico para aprender autenticación,
autorización, CRUD, almacenamiento de archivos, validación, pruebas y auditoría con ASP.NET
Core MVC, Entity Framework Core y SQL Server.
