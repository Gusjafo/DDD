# Domain Driven Design Shop

Este proyecto es un ejemplo sencillo de una tienda en línea construida con principios de **Domain-Driven Design (DDD)** sobre ASP.NET Core.

## Arquitectura general

La solución está organizada en capas bien delimitadas, cada una con una responsabilidad concreta:

- **API (src/DomainDrivenDesignShop.Api)**: capa de presentación. Expone controladores REST que traducen las peticiones HTTP en comandos o consultas para la capa de aplicación.
- **Aplicación (src/DomainDrivenDesignShop.Application)**: contiene los _use cases_ o _application services_. Orquesta el flujo entre el mundo exterior y el dominio puro. No contiene lógica de infraestructura.
- **Dominio (src/DomainDrivenDesignShop.Domain)**: núcleo de negocio. Define entidades, objetos de valor, reglas e invariantes.
- **Infraestructura (src/DomainDrivenDesignShop.Infrastructure)**: implementa detalles técnicos (EF Core, persistencia, repositorios concretos) y se inyecta en la capa de aplicación mediante interfaces.

Esta estructura respeta las dependencias unidireccionales: la API conoce a la aplicación, la aplicación conoce al dominio y a las abstracciones de infraestructura, mientras que el dominio no depende de capas externas.

## Conceptos fundamentales de DDD aplicados

- **Entidades**: objetos con identidad persistente. Ejemplos: `Product` y `Order` en `Domain/Entities`. Contienen reglas de negocio para validar nombre, divisa o el cálculo del total del pedido.
- **Objetos de valor**: representan conceptos inmutables definidos por sus atributos. `Money` asegura divisa ISO-4217 y redondeo de importes; `OrderLine` encapsula cantidad y precio dentro de un pedido.
- **Agregados**: `Order` actúa como agregado raíz controlando la consistencia de sus líneas. Solo la raíz expone métodos para mutar el agregado (`AddProduct`, `UpdateCurrency`).
- **Servicios de dominio**: cuando una regla no pertenece a una entidad, se coloca en `Domain/Abstractions` (por ejemplo `DomainErrors`) para centralizar errores semánticos.
- **Repositorios**: interfaces (`IProductRepository`, `IOrderRepository`) desacoplan la persistencia. Sus implementaciones concretas (`EfProductRepository`, `EfOrderRepository`) viven en infraestructura.
- **Unidad de trabajo**: `IUnitOfWork` coordina la escritura de múltiples agregados en una misma transacción.
- **Casos de uso**: comandos y _handlers_ (`CreateProductHandler`, `ListOrdersHandler`, etc.) que encapsulan escenarios del negocio y se encargan de llamar a los repositorios, validar reglas y materializar DTOs.

## Persistencia

La infraestructura utiliza **Entity Framework Core** sobre SQLite. El contexto `AppDbContext` mapea entidades y objetos de valor. Durante el arranque de la API se invoca `Database.EnsureCreated()` para generar automáticamente la base de datos local `dddshop.db` si no existe.

## API y documentación interactiva

Se agregó **Swagger UI** para explorar y probar los endpoints de forma interactiva.

1. Ejecuta la solución:
   ```bash
   dotnet run --project src/DomainDrivenDesignShop.Api/DomainDrivenDesignShop.Api.csproj
   ```
2. Abre el navegador en `https://localhost:5001/swagger` (o el puerto configurado). Encontrarás la especificación OpenAPI y podrás enviar peticiones de prueba.

La configuración de Swagger vive en `Program.cs`. Solo se habilita en el entorno de desarrollo (`ASPNETCORE_ENVIRONMENT=Development`).

## Próximos pasos sugeridos

- Añadir autenticación/autorización para restringir operaciones.
- Incorporar validación más robusta (por ejemplo con FluentValidation).
- Desplegar en un entorno hospedado y automatizar migraciones de base de datos.
- Implementar pruebas automatizadas para proteger las reglas de negocio.

