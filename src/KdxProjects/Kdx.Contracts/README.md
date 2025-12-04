# Kdx.Contracts

Data Transfer Objects (DTOs) and interface definitions for KDX Projects.

## 📦 Package Information

- **Package ID**: Kdx.Contracts
- **Description**: Core contracts and DTOs for PLC automation and process flow management
- **License**: MIT
- **Repository**: [GitHub](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects)

## 🎯 Purpose

This package provides:
- Data Transfer Objects (DTOs) for database entities
- Interface definitions for services and repositories
- Shared contracts between application layers

## 📥 Installation

```bash
dotnet add package Kdx.Contracts
```

## 🔧 Usage

```csharp
using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;

// Use DTOs for data transfer
var processFlow = new ProcessFlowDto
{
    ProcessFlowName = "Main Process",
    ProcessFlowCode = "MAIN_001"
};

// Use interfaces for dependency injection
public class MyService
{
    private readonly IProcessFlowService _processFlowService;

    public MyService(IProcessFlowService processFlowService)
    {
        _processFlowService = processFlowService;
    }
}
```

## 📚 Documentation

- [Main README](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/README.md)
- [CHANGELOG](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/CHANGELOG.md)
- [API Documentation](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/wiki)

## 🔗 Related Packages

- `Kdx.Core` - Business logic and application services
- `Kdx.Infrastructure` - Infrastructure implementations
- `Kdx.Infrastructure.Supabase` - Supabase-specific implementations
- `Kdx.Contracts.ViewModels` - WPF ViewModels

## 🤝 Contributing

Please read our [Contributing Guide](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/CONTRIBUTING.md) for details on our code of conduct and the process for submitting pull requests.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/LICENSE.txt) file for details.

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/issues)
- **Discussions**: [GitHub Discussions](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/discussions)
