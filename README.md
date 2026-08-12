# MiniDocs

Gestor documental básico construido como primer sistema de práctica para el SIGD Empresarial.

## Tecnologías

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity

## Estado

Proyecto inicial creado. La autenticación, los roles y la migración inicial se implementarán por etapas.

## Ejecución local

1. Configura SQL Server o LocalDB.
2. Revisa la cadena `DefaultConnection` en `appsettings.json`.
3. Configura las credenciales del administrador mediante User Secrets:

   ```powershell
   dotnet user-secrets set "AdminSeed:Email" "admin@minidocs.local"
   dotnet user-secrets set "AdminSeed:Password" "Cambia_Esta_Clave_123!"
   ```

4. Ejecuta `dotnet run` dentro de esta carpeta.

Para crear la cuenta única de SuperAdministrador, configura también:

```powershell
dotnet user-secrets set "SuperAdminSeed:Email" "superadmin@minidocs.local"
dotnet user-secrets set "SuperAdminSeed:Password" "Cambia_Esta_Clave_123!"
```
