smartHome/
│
├── smartHome.App/                 # Console Application (Frontend)
│   ├── UI/
│   │   └── Menu.cs               # Main dashboard & user interaction
│   │
│   ├── Services/
│   │   └── ApiService.cs         # Backend API calls (HTTP client)
│   │
│   ├── Utilities/
│   │   └── Scheduler.cs          # Background scheduling (auto toggle)
│   │
│   └── Program.cs                # Entry point
│
├── smartHome.Api/                # ASP.NET Core API (Backend)
│   ├── Controllers/
│   │   └── DeviceController.cs   # API endpoints
│   │
│   ├── Services/
│   │   └── DeviceService.cs      # Business logic (armed, validation)
│   │
│   └── Program.cs               # API configuration
│
├── smartHome.Core/              # Shared Layer (Domain)
│   ├── Entities/
│   │   ├── Device.cs
│   │   ├── CommandHistory.cs
│   │   └── DeviceGroup.cs
│   │
│   ├── DTOs/
│   │   ├── DeviceDto.cs
│   │   ├── UpdateDeviceDto.cs
│   │   ├── HistoryDto.cs
│   │   └── ArmedRequest.cs
│   │
│   ├── Interfaces/
│   │   ├── IDeviceService.cs
│   │   └── IDeviceRepository.cs
│   │
│   └── Exceptions/
│       └── Custom exceptions
│
├── smartHome.Infrastructure/    # Data Layer
│   ├── Data/
│   │   └── SmartHomeDbContext.cs
│   │
│   └── Repositories/
│       └── DeviceRepository.cs
│
└── README.md
