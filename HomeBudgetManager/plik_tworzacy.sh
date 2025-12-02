#!/bin/bash

# 1. Tworzenie pliku rozwiązania (.sln)
dotnet new sln -n HomeBudgetManager

# 2. Tworzenie projektu Core (Logika biznesowa)
dotnet new classlib -n HomeBudgetManager.Core

# 3. Tworzenie projektu Web (Aplikacja z htmx)
dotnet new web -n HomeBudgetManager.Web

# 4. Tworzenie projektu Testów (xUnit)
dotnet new xunit -n HomeBudgetManager.Tests

# 5. Dodawanie referencji (zależności)
# Web musi widzieć Core, żeby korzystać z logiki
dotnet add HomeBudgetManager.Web/HomeBudgetManager.Web.csproj reference HomeBudgetManager.Core/HomeBudgetManager.Core.csproj

# Testy muszą widzieć Core, żeby go testować
dotnet add HomeBudgetManager.Tests/HomeBudgetManager.Tests.csproj reference HomeBudgetManager.Core/HomeBudgetManager.Core.csproj

# 6. Dodawanie projektów do rozwiązania (.sln)
dotnet sln HomeBudgetManager.sln add HomeBudgetManager.Core/HomeBudgetManager.Core.csproj
dotnet sln HomeBudgetManager.sln add HomeBudgetManager.Web/HomeBudgetManager.Web.csproj
dotnet sln HomeBudgetManager.sln add HomeBudgetManager.Tests/HomeBudgetManager.Tests.csproj

# Wyświetlanie komunikatu na zielono (kody ANSI)
echo -e "\e[32mStruktura projektu HomeBudgetManager została utworzona pomyślnie! Otwórz plik HomeBudgetManager.sln w swoim IDE.\e[0m"