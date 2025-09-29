#!/usr/bin/env bash
set -e

SOLUTION_NAME="DomainDrivenDesignShop"
SRC_DIR="src"

echo "🚀 Creando solución $SOLUTION_NAME..."
dotnet new sln -n $SOLUTION_NAME

mkdir -p $SRC_DIR

echo "📦 Creando proyectos..."
dotnet new classlib -n $SOLUTION_NAME.Domain -o $SRC_DIR/$SOLUTION_NAME.Domain
dotnet new classlib -n $SOLUTION_NAME.Application -o $SRC_DIR/$SOLUTION_NAME.Application
dotnet new classlib -n $SOLUTION_NAME.Infrastructure -o $SRC_DIR/$SOLUTION_NAME.Infrastructure
dotnet new webapi   -n $SOLUTION_NAME.Api -o $SRC_DIR/$SOLUTION_NAME.Api --use-minimal-apis

echo "🧩 Añadiendo proyectos a la solución..."
dotnet sln add $SRC_DIR/**/$SOLUTION_NAME.*.csproj

echo "🔗 Configurando referencias entre proyectos..."
# Application → Domain
dotnet add $SRC_DIR/$SOLUTION_NAME.Application/$SOLUTION_NAME.Application.csproj reference $SRC_DIR/$SOLUTION_NAME.Domain/$SOLUTION_NAME.Domain.csproj

# Infrastructure → Application, Domain
dotnet add $SRC_DIR/$SOLUTION_NAME.Infrastructure/$SOLUTION_NAME.Infrastructure.csproj reference $SRC_DIR/$SOLUTION_NAME.Application/$SOLUTION_NAME.Application.csproj
dotnet add $SRC_DIR/$SOLUTION_NAME.Infrastructure/$SOLUTION_NAME.Infrastructure.csproj reference $SRC_DIR/$SOLUTION_NAME.Domain/$SOLUTION_NAME.Domain.csproj

# Api → Application, Infrastructure
dotnet add $SRC_DIR/$SOLUTION_NAME.Api/$SOLUTION_NAME.Api.csproj reference $SRC_DIR/$SOLUTION_NAME.Application/$SOLUTION_NAME.Application.csproj
dotnet add $SRC_DIR/$SOLUTION_NAME.Api/$SOLUTION_NAME.Api.csproj reference $SRC_DIR/$SOLUTION_NAME.Infrastructure/$SOLUTION_NAME.Infrastructure.csproj

echo "📥 Instalando paquetes NuGet..."
# EF Core Infra
dotnet add $SRC_DIR/$SOLUTION_NAME.Infrastructure/$SOLUTION_NAME.Infrastructure.csproj package Microsoft.EntityFrameworkCore
dotnet add $SRC_DIR/$SOLUTION_NAME.Infrastructure/$SOLUTION_NAME.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Sqlite
dotnet add $SRC_DIR/$SOLUTION_NAME.Infrastructure/$SOLUTION_NAME.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design

# EF Core Api
dotnet add $SRC_DIR/$SOLUTION_NAME.Api/$SOLUTION_NAME.Api.csproj package Microsoft.EntityFrameworkCore
dotnet add $SRC_DIR/$SOLUTION_NAME.Api/$SOLUTION_NAME.Api.csproj package Microsoft.EntityFrameworkCore.Sqlite

echo "✅ Todo listo."
echo "👉 Ahora puedes ejecutar:"
echo "   dotnet build"
echo "   dotnet run --project $SRC_DIR/$SOLUTION_NAME.Api"
