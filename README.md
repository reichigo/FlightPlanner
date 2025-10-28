# 🫢 Before anything else
I sometimes like to joke around and play games too. You might find some issues in my application, but there’s one that’s serious can you figure it out? ❤️

# ✈️ FlightPlanner

A comprehensive flight planning application built with **ASP.NET Core 10.0** and **MongoDB**, following **Clean Architecture** principles. This system allows users to manage airports, aircraft, and flights while automatically calculating distances, flight times, and fuel requirements.

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![MongoDB](https://img.shields.io/badge/MongoDB-Latest-47A248?logo=mongodb)](https://www.mongodb.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📋 Table of Contents

- [Features](#-features)
- [Architecture](#-architecture)
- [Tech Stack](#-tech-stack)
- [Prerequisites](#-prerequisites)
- [Getting Started](#-getting-started)
- [Project Structure](#-project-structure)
- [API Endpoints](#-api-endpoints)
- [Domain Models](#-domain-models)
- [Testing](#-testing)
- [Performance Optimizations](#-performance-optimizations)
- [Contributing](#-contributing)
- [License](#-license)

---

## ✨ Features

### Core Functionality
- **Airport Management**: Create, read, update, and search airports by IATA code, name, city, or country
- **Aircraft Management**: Manage aircraft fleet with specifications (model, cruise speed, fuel capacity)
- **Flight Planning**: Create and manage flights with automatic calculations for:
  - Distance between airports using Haversine formula
  - Estimated flight time based on aircraft cruise speed
  - Fuel requirements with takeoff and cruise consumption
- **Flight Reports**: Generate detailed reports and summaries for flights

### Technical Features
- **Clean Architecture**: Separation of concerns with Domain, Application, Infrastructure, and Presentation layers
- **RESTful API**: Minimal API endpoints with OpenAPI/Swagger documentation
- **Web Interface**: MVC-based web application for user-friendly interaction
- **MongoDB Integration**: NoSQL database with optimized batch queries
- **Global Exception Handling**: Centralized error handling with appropriate HTTP status codes
- **Comprehensive Testing**: Unit tests with xUnit, Moq, and Shouldly
- **Performance Optimized**: Batch database queries to eliminate N+1 query problems

---

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────┐
│                    Presentation Layer                    │
│  ┌──────────────────────┐  ┌──────────────────────────┐ │
│  │   FlightPlanner.Api  │  │  FlightPlanner.Web (MVC) │ │
│  │   (Minimal APIs)     │  │                          │ │
│  └──────────────────────┘  └──────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────┐
│                   Application Layer                      │
│  ┌──────────────┐  ┌──────────┐  ┌──────────────────┐  │
│  │  Use Cases   │  │  Mappers │  │  DTOs & Services │  │
│  └──────────────┘  └──────────┘  └──────────────────┘  │
└─────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────┐
│                     Domain Layer                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │   Entities   │  │  Interfaces  │  │  Exceptions  │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
                            │
┌─────────────────────────────────────────────────────────┐
│                 Infrastructure Layer                     │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐  │
│  │ Repositories │  │ Data Sources │  │    Mappers   │  │
│  │              │  │   (MongoDB)  │  │              │  │
│  └──────────────┘  └──────────────┘  └──────────────┘  │
└─────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

- **Domain**: Core business logic, entities, value objects, and interfaces
- **Application**: Use cases, DTOs, mappers, and application services
- **Infrastructure**: Data access, MongoDB integration, and external services
- **Presentation**: API endpoints and Web UI

---

## 🛠️ Tech Stack

### Backend
- **.NET 10.0** (Preview) - Latest .NET framework
- **ASP.NET Core** - Web framework
- **Minimal APIs** - Lightweight API endpoints
- **MongoDB Driver** - NoSQL database integration

### Frontend
- **ASP.NET Core MVC** - Web interface
- **Razor Views** - Server-side rendering
- **Bootstrap** - UI framework

### Testing
- **xUnit** - Testing framework
- **Moq** - Mocking library for unit tests
- **Shouldly** - Assertion library
- **Testcontainers** - Docker containers for integration tests
- **WebApplicationFactory** - In-memory API testing

### Development Tools
- **Docker** - MongoDB containerization
- **PowerShell** - Automation scripts
- **OpenAPI/Swagger** - API documentation

---

## 📦 Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (Preview)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for MongoDB)
- [Git](https://git-scm.com/)
- A code editor (Visual Studio 2022, Rider, or VS Code)

---

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/reichigo/FlightPlanner.git
cd FlightPlanner
```

### 2. Start MongoDB with Docker

```bash
docker-compose up -d
```

This will start a MongoDB instance on `localhost:27017` with:
- Database: `FlightPlannerDb`
- No authentication required (development mode)

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the Application

#### Option A: Using PowerShell Script (Recommended)

```powershell
.\start-dev.ps1
```

This will start both the API and Web applications in separate windows.

#### Option B: Manual Start

**Terminal 1 - API:**
```bash
dotnet run --project FlightPlanner.Api
```

**Terminal 2 - Web:**
```bash
dotnet run --project FlightPlanner.Web
```

### 6. Access the Applications

- **API**: http://localhost:5062
- **API Documentation (Swagger)**: http://localhost:5062/openapi/v1.json
- **Web Interface**: http://localhost:5181

---

## 📁 Project Structure

```
FlightPlanner/
├── FlightPlanner.Domain/              # Core business logic
│   ├── Entities/                      # Domain entities (Flight, Airport, Aircraft)
│   │   ├── Interfaces/                # Domain service interfaces
│   │   └── ValueObjects/              # Value objects (GeoCoordinate, IataCode, etc.)
│   ├── Repositories/                  # Repository interfaces
│   └── Exceptions/                    # Domain exceptions
│
├── FlightPlanner.Application/         # Application logic
│   ├── UseCases/                      # Use case implementations
│   ├── Dto/                           # Data Transfer Objects
│   │   ├── Request/                   # Request DTOs
│   │   └── Response/                  # Response DTOs
│   ├── Mappers/                       # Mapping extensions
│   └── Services/                      # Application services
│       ├── HaversineDistanceCalculator.cs
│       └── SimpleFuelEstimator.cs
│
├── FlightPlanner.Infrastructure/      # External concerns
│   ├── DataSources/                   # MongoDB data sources
│   │   └── Mongo/                     # MongoDB implementations
│   ├── Repositories/                  # Repository implementations
│   ├── Mappers/                       # Domain ↔ MongoDB mappers
│   └── DependencyInjection.cs         # Infrastructure DI setup
│
├── FlightPlanner.Api/                 # REST API
│   ├── Endpoint/                      # Minimal API endpoints
│   │   ├── AircraftsEndpoint.cs
│   │   ├── AirportsEndpoint.cs
│   │   ├── FlightsEndpoint.cs
│   │   └── ReportsEndpoint.cs
│   ├── Middleware/                    # Custom middleware
│   │   └── GlobalExceptionMiddleware.cs
│   └── Program.cs                     # API startup
│
├── FlightPlanner.Web/                 # Web UI
│   ├── Controllers/                   # MVC controllers
│   ├── Views/                         # Razor views
│   ├── Models/                        # View models
│   ├── Services/                      # API client service
│   └── Program.cs                     # Web startup
│
├── FlightPlanner.Application.Tests/   # Unit tests
│   ├── UseCases/                      # Use case tests
│   ├── Services/                      # Service tests
│   ├── Fixtures/                      # Test fixtures
│   └── Mappers/                       # Mapper tests
│
├── FlightPlanner.Api.Tests/           # Integration tests
│   ├── Endpoints/                     # API endpoint tests
│   │   ├── AirportsEndpointTests.cs
│   │   ├── AircraftsEndpointTests.cs
│   │   ├── FlightsEndpointTests.cs
│   │   └── ReportsEndpointTests.cs
│   ├── IntegrationTestWebAppFactory.cs # Test factory with MongoDB
│   └── IntegrationTestBase.cs         # Base class with helpers
│
├── docker-compose.yml                 # MongoDB container setup
├── start-dev.ps1                      # Start script
└── FlightPlanner.sln                  # Solution file
```

---

## 🔌 API Endpoints

### Airports

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/airports` | Get all airports |
| `GET` | `/api/airports/{id}` | Get airport by ID |
| `GET` | `/api/airports/search?query={query}` | Search airports |
| `POST` | `/api/airports` | Create new airport |
| `PUT` | `/api/airports/{id}` | Update airport |
| `DELETE` | `/api/airports/{id}` | Delete airport |

### Aircraft

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/aircrafts` | Get all aircraft |
| `GET` | `/api/aircrafts/{id}` | Get aircraft by ID |
| `POST` | `/api/aircrafts` | Create new aircraft |
| `PUT` | `/api/aircrafts/{id}` | Update aircraft |
| `DELETE` | `/api/aircrafts/{id}` | Delete aircraft |

### Flights

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/flights` | Get all flights |
| `GET` | `/api/flights/{id}` | Get flight by ID |
| `POST` | `/api/flights` | Create new flight |
| `PUT` | `/api/flights/{id}` | Update flight |
| `DELETE` | `/api/flights/{id}` | Delete flight |

### Reports

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/reports/flights/{id}` | Get detailed flight report |
| `GET` | `/api/reports/summary` | Get flight summary statistics |

---

## 📊 Domain Models

### Airport
```csharp
public class Airport
{
    public Guid Id { get; }
    public IataCode Iata { get; }        // 3-letter IATA code
    public string Name { get; }
    public string City { get; }
    public string Country { get; }
    public GeoCoordinate Location { get; } // Latitude/Longitude
}
```

### Aircraft
```csharp
public class Aircraft
{
    public Guid Id { get; }
    public string Model { get; }
    public int CruiseSpeedKts { get; }    // Cruise speed in knots
    public double FuelCapacityLbs { get; } // Fuel capacity in pounds
}
```

### Flight
```csharp
public class Flight
{
    public Guid Id { get; }
    public Guid OriginId { get; }
    public Guid DestinationId { get; }
    public Guid AircraftId { get; }
    public DateTime? DepartureAt { get; }
}
```

### FlightEstimates (Value Object)
```csharp
public readonly record struct FlightEstimates
{
    public double DistanceKm { get; }
    public TimeSpan EstimatedTime { get; }
    public double FuelRequiredLbs { get; }
}
```

---

## 🧪 Testing

The project includes comprehensive test coverage with both **unit tests** and **integration tests**.

### Unit Tests (FlightPlanner.Application.Tests)

Unit tests cover isolated components with mocked dependencies:
- ✅ All Use Cases (Create, Update, Delete operations)
- ✅ Domain Services (Distance Calculator, Fuel Estimator)
- ✅ Mappers (Domain ↔ DTO conversions)
- ✅ Edge cases and error scenarios

**Run Unit Tests:**
```bash
dotnet test FlightPlanner.Application.Tests
```

**Statistics:**
- **Total Tests**: 28
- **Frameworks**: xUnit, Moq, Shouldly
- **Coverage**: Use cases, services, and mappers

### Integration Tests (FlightPlanner.Api.Tests)

Integration tests verify the entire application stack end-to-end:
- ✅ **Real API Testing**: Tests actual HTTP endpoints using `WebApplicationFactory`
- ✅ **Real Database**: Uses MongoDB in Docker containers via Testcontainers
- ✅ **Complete Workflows**: Tests full request/response cycles
- ✅ **Automatic Cleanup**: Each test runs in isolation with fresh containers

**Test Coverage:**
- **Airports Endpoints** (7 tests): Create, read, update, validation, duplicate detection
- **Aircrafts Endpoints** (7 tests): CRUD operations, validation, conflict handling
- **Flights Endpoints** (11 tests): Flight creation, distance calculations, validations
- **Reports Endpoints** (7 tests): Flight reports, summaries, statistics

**Run Integration Tests:**
```bash
# Run all integration tests
dotnet test FlightPlanner.Api.Tests

# Run specific endpoint tests
dotnet test --filter "FullyQualifiedName~AirportsEndpointTests"
dotnet test --filter "FullyQualifiedName~FlightsEndpointTests"

# Run with detailed output
dotnet test FlightPlanner.Api.Tests --logger "console;verbosity=detailed"
```

**Prerequisites for Integration Tests:**
- Docker Desktop must be running (for Testcontainers)
- No manual setup required - containers are created/destroyed automatically

**Statistics:**
- **Total Tests**: 32
- **Success Rate**: 100%
- **Execution Time**: ~8-10 seconds
- **Frameworks**: xUnit, Testcontainers, WebApplicationFactory, Shouldly

### Run All Tests

```bash
# Run all tests (unit + integration)
dotnet test

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test Architecture

**Unit Tests:**
```
Test → Mock Repository → Use Case → Mock Services → Assertions
```

**Integration Tests:**
```
Test → HTTP Client → Real API → Real Use Cases → Real MongoDB → Assertions
```

---

## ⚡ Performance Optimizations

### Batch Query Optimization

The application implements **batch database queries** to eliminate N+1 query problems:

**Before Optimization:**
```csharp
foreach (var flight in flights)
{
    var origin = await airportRepository.GetByIdAsync(flight.OriginId);
    var destination = await airportRepository.GetByIdAsync(flight.DestinationId);
    var aircraft = await aircraftRepository.GetByIdAsync(flight.AircraftId);
}
// Result: N × 3 database queries
```

**After Optimization:**
```csharp
var airportIds = flights.SelectMany(f => new[] { f.OriginId, f.DestinationId }).Distinct();
var aircraftIds = flights.Select(f => f.AircraftId).Distinct();

var airports = await airportRepository.GetByIdsAsync(airportIds);
var aircrafts = await aircraftRepository.GetByIdsAsync(aircraftIds);

var airportDict = airports.ToDictionary(a => a.Id);
var aircraftDict = aircrafts.ToDictionary(a => a.Id);
// Result: 2 database queries total + O(1) dictionary lookups
```

**Performance Gain:**
- For 100 flights: **300 queries → 2 queries** (99% reduction)
- For 1000 flights: **3000 queries → 2 queries** (99.9% reduction)

---


### Coding Standards
- Follow Clean Architecture principles
- Write unit tests for new features
- Use primary constructors where applicable
- Follow C# naming conventions
- Document public APIs

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Renato Neves**
- GitHub: [@reichigo](https://github.com/reichigo)

---

## 🙏 Acknowledgments

- Haversine formula for distance calculations
- Clean Architecture by Robert C. Martin
- ASP.NET Core team for the excellent framework
- MongoDB team for the robust NoSQL database

**Happy Flight Planning! ✈️**