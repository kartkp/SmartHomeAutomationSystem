##PROJECT STRUCTURE:
smartHome/
│
├── smartHome.App/                # Console Application (Frontend)
│   ├── UI/
│   │   └── Menu.cs              # Main dashboard & user interaction
│   │
│   ├── Services/
│   │   └── ApiService.cs        # Backend API calls (HTTP client)
│   │
│   ├── Utilities/
│   │   └── Scheduler.cs         # Background scheduling (auto toggle)
│   │
│   └── Program.cs               # Entry point (main flow control)
│
├── smartHome.Api/               # ASP.NET Core Web API (Backend)
│   ├── Controllers/
│   │   └── DeviceController.cs  # API endpoints (GET, PUT, POST, DELETE)
│   │
│   ├── Services/
│   │   └── DeviceService.cs     # Business logic (rules, validation, armed system)
│   │
│   ├── DTOs/                   # (Optional if separate)
│   │   └── Request/Response models
│   │
│   └── Program.cs              # API startup & configuration
│
├── smartHome.Core/             # Shared Layer (Domain)
│   ├── Entities/
│   │   ├── Device.cs           # Device model (DB table)
│   │   ├── CommandHistory.cs   # Logs/history table
│   │   └── DeviceGroup.cs      # Group/room model
│   │
│   ├── DTOs/
│   │   ├── DeviceDto.cs        # Device response model
│   │   ├── UpdateDeviceDto.cs  # Update request
│   │   ├── HistoryDto.cs       # Recent activity model
│   │   └── ArmedRequest.cs     # Armed system request
│   │
│   ├── Interfaces/
│   │   ├── IDeviceService.cs
│   │   └── IDeviceRepository.cs
│   │
│   └── Exceptions/
│       └── Custom exceptions (InvalidCommandException, etc.)
│
├── smartHome.Infrastructure/   # Data Layer
│   ├── Data/
│   │   └── SmartHomeDbContext.cs   # EF Core DB context
│   │
│   └── Repositories/
│       └── DeviceRepository.cs    # DB operations (CRUD)
│
└── README.md
