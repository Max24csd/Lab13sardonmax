# Laboratorio 14 - Publicacion y despliegue

Este repositorio reutiliza el Laboratorio 13, desarrollado con .NET 8,
Blazor, API REST, Swagger, Entity Framework Core y una base de datos SQLite.

## Parte 1: publicacion manual en Rider

1. Abrir `Lab13sardonmax.sln`.
2. Hacer clic derecho sobre el proyecto `Lab13sardonmax`.
3. Elegir `Publish`.
4. Seleccionar el perfil `CarpetaRelease`.
5. Confirmar `Release`, `net8.0` y publicar.
6. Revisar la carpeta:
   `Lab13sardonmax/bin/Release/net8.0/publish`.
7. Tomar una captura donde se vean `Lab13sardonmax.dll`,
   `Lab13sardonmax.exe`, `appsettings.json` y la carpeta `wwwroot`.

También se puede ejecutar desde la terminal:

```powershell
dotnet publish .\Lab13sardonmax\Lab13sardonmax.csproj `
  --configuration Release `
  --output .\publicado
```

Para probar los archivos publicados:

```powershell
Set-Location .\publicado
dotnet .\Lab13sardonmax.dll --urls http://localhost:5080
```

Abrir `http://localhost:5080`, `http://localhost:5080/swagger` y
`http://localhost:5080/salud`.

## Parte 2: GitHub Actions

1. Crear un repositorio vacío en GitHub.
2. Desde la raíz de esta solución ejecutar:

```powershell
git init
git add .
git commit -m "Laboratorio 14: publicacion y despliegue"
git branch -M main
git remote add origin URL_DEL_REPOSITORIO
git push -u origin main
```

3. En GitHub abrir la pestaña `Actions`.
4. Entrar al workflow `Compilar, probar y publicar .NET`.
5. Comprobar que restauración, compilación, pruebas y publicación estén verdes.
6. Descargar el artefacto `lab13sardonmax-publicado` si se desea verificarlo.
7. Tomar una captura del workflow completado correctamente.

El archivo usado es `.github/workflows/dotnet.yml`.

## Parte 3: despliegue en Render

La solución incluye `Dockerfile` y `render.yaml`, porque Render despliega la
aplicación .NET mediante un contenedor Docker.

1. Ingresar a Render y conectar la cuenta de GitHub.
2. Elegir `New` y luego `Blueprint`.
3. Seleccionar el repositorio de este proyecto.
4. Render leerá `render.yaml` y creará el servicio `lab13sardonmax`.
5. Iniciar el despliegue y esperar que el estado sea `Live`.
6. Abrir la URL pública, agregar `/swagger` y comprobar `/salud`.
7. Tomar capturas del despliegue exitoso y de la aplicación funcionando.

Cada nuevo `push` a `main` ejecutará GitHub Actions y Render realizará un nuevo
despliegue automático desde el repositorio.

## Nota sobre SQLite

SQLite es suficiente para esta demostración porque la base se crea y se llena
con datos iniciales al arrancar. En el plan gratuito de Render, los cambios
hechos al archivo local no son permanentes después de un reinicio. Para una
aplicación real se debe usar Render Postgres o un disco persistente de pago.

## Conclusiones sugeridas

- La publicación Release genera los archivos necesarios para ejecutar una
  aplicación .NET 8 sin depender del IDE.
- GitHub Actions automatiza la restauración, compilación, pruebas y generación
  del artefacto publicado, reduciendo errores manuales.
- Render permite desplegar automáticamente desde GitHub mediante Docker y
  proporciona una URL pública para verificar la aplicación.
- La integración continua comprueba cada cambio antes del despliegue y mejora
  la confiabilidad del proyecto.
