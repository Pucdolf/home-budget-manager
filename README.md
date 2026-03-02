## 🏠 HomeBudgetManager – Kompleksowy system do zarządzania budżetem domowym

**HomeBudgetManager** to nowoczesna aplikacja webowa przeznaczona do monitorowania wydatków i przychodów, umożliwiająca użytkownikom pełną kontrolę nad ich finansami osobistymi oraz wspólnymi budżetami gospodarstw domowych. Projekt został zrealizowany jako część kursu *Programowanie Komputerów* na Politechnice Śląskiej.

---

### 🕹️ Podgląd / Galeria

<p align="center">
  <img src="https://github.com/user-attachments/assets/0bc44495-17dc-48f9-a6c2-dbb122f830be" width="700" alt="Prezentacja działania HomeBudgetManager">
</p>

#### 📄 Podgląd raportu PDF
<p align="center">
  <img src="https://github.com/user-attachments/assets/5ab63780-1fda-4dec-ba35-e5615c0523de" width="200" alt="Raport strona 1">
  <img src="https://github.com/user-attachments/assets/33068a7f-91e1-4e95-a24f-b9ace0378e59" width="200" alt="Raport strona 2">
  <img src="https://github.com/user-attachments/assets/925e9b39-064f-416f-a366-1fb94f6b8fd7" width="200" alt="Raport strona 3">
  <img src="https://github.com/user-attachments/assets/938c3519-e4aa-498a-abd6-bfd332241d2f" width="200" alt="Raport strona 4">
</p>

---

### 🏗️ Architektura i Technologie

Projekt opiera się na architekturze warstwowej, co zapewnia separację logiki biznesowej od prezentacji:

*   **`HomeBudgetManager.Core`** – Biblioteka klas zawierająca logikę domenową, encje bazy danych, serwisy biznesowe oraz migracje. Wykorzystuje **Entity Framework Core** z dostawcą **SQLite**.
*   **`HomeBudgetManager.Web`** – Aplikacja webowa ASP.NET Core wykorzystująca wzorzec **Minimal APIs**. Odpowiada za serwowanie frontendu (HTML/JS) oraz obsługę żądań API. Zawiera również **Background Service** do automatycznego przetwarzania transakcji cyklicznych.
*   **`HomeBudgetManager.Tests`** – Projekt testowy wykorzystujący **xUnit**, służący do weryfikacji poprawności logiki biznesowej (transakcje, autoryzacja, zarządzanie domostwem).

**Wykorzystane technologie:**
*   **Backend:** .NET 9.0, C#, ASP.NET Core
*   **Baza danych:** SQLite, Entity Framework Core (EF Core)
*   **Bezpieczeństwo:** `PasswordHasher` (Microsoft.AspNetCore.Identity) do bezpiecznego hashowania haseł, uwierzytelnianie oparte na ciasteczkach (Cookies).
*   **Raportowanie:** QuestPDF (generowanie dokumentów PDF).
*   **Frontend:** Vanilla HTML5, CSS3, JavaScript (ES6+).
*   **Testy:** xUnit, Moq, FluentAssertions.

---

### 📂 Kluczowe Funkcjonalności

*   **Zarządzanie Transakcjami** – Pełna obsługa transakcji (CRUD) z podziałem na przychody i wydatki. 
*   **Automatyzacja (Transakcje Cykliczne)** – System posiada wbudowany proces tła (**Worker Service**), który co godzinę sprawdza i automatycznie generuje zaplanowane transakcje (dzienne, miesięczne itp.).
*   **Interaktywny Kalendarz** – Wizualizacja historii oraz prognoz finansowych.
*   **Zaawansowane Raporty PDF** – Generowanie profesjonalnych zestawień z wykresami słupkowymi i szczegółową listą operacji.
*   **Gospodarstwa Domowe (Households)** – Współdzielenie budżetu z innymi użytkownikami, wspólne śledzenie wydatków domowych.
*   **Bezpieczeństwo i Role** – System ról (Użytkownik, Admin), bezpieczne przechowywanie poświadczeń i sesji.
*   **Konsola Administratora** – Panel do bezpośredniego zarządzania danymi za pomocą zapytań SQL (tylko dla administratorów systemu).

---

### 🛠️ Wymagania i Konfiguracja

*   **.NET 9.0 SDK**
*   System operacyjny: Windows, Linux lub macOS.
*   Opcjonalnie: Narzędzia wiersza poleceń EF Core (`dotnet-ef`).

**Konfiguracja bazy danych:**
Parametry połączenia znajdują się w pliku `HomeBudgetManager.Web/appsettings.json`. Domyślnie aplikacja korzysta z lokalnego pliku bazy danych SQLite.

---

## 🚀 Kompilacja, Uruchamianie i Testy

### 1. Sklonuj repozytorium
```bash
git clone https://github.com/Pucdolf/home-budget-manager.git
cd home-budget-manager
```

### 2. Przygotowanie bazy danych
Jeśli chcesz zaktualizować bazę danych do najnowszej migracji:
```bash
dotnet ef database update --project HomeBudgetManager.Core --startup-project HomeBudgetManager.Web
```

### 3. Uruchomienie aplikacji
```bash
dotnet run --project HomeBudgetManager.Web
```
Aplikacja będzie dostępna pod adresem `http://localhost:5000` (lub innym wskazanym w konsoli).

### 4. Uruchomienie testów
Aby upewnić się, że wszystko działa poprawnie, uruchom zestaw testów jednostkowych:
```bash
dotnet test
```

---

### 🎨 Zasoby i Licencja
Projekt jest realizowany w celach edukacyjnych jako open-source. Wykorzystuje biblioteki na licencjach typu Community/MIT (m.in. QuestPDF, SQLite).
