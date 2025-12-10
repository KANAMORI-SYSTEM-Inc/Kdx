# Kdx.Core

KDXシステムのコアビジネスロジックおよびアプリケーションサービスを提供するパッケージです。

## 📦 Package Information

- **Package ID**: Kdx.Core
- **Description**: Core business logic and application services for KDX Projects
- **License**: MIT
- **Target Framework**: .NET 8.0

## 📥 Installation

```bash
dotnet add package Kdx.Core
```

## 🎯 Purpose

このパッケージは以下を提供します：
- ドメインロジックとビジネスルール
- アプリケーションサービス層
- ユースケースの実装
- インターロック戦略パターン

## 📁 ディレクトリ構造

```
Kdx.Core/
├── Application/             # アプリケーション層
│   ├── Strategies/          # インターロック戦略パターン
│   │   ├── On1Strategy.cs
│   │   ├── On2Strategy.cs
│   │   ├── Off1Strategy.cs
│   │   ├── OnMStrategy.cs
│   │   ├── OnOrStrategy.cs
│   │   └── AnyStrategy.cs
│   ├── ErrorAggregator.cs
│   ├── InterlockMnemonicContext.cs
│   ├── InterlockMnemonicOutput.cs
│   ├── IOAddressService.cs
│   └── ServiceCollectionExtensions.cs
└── Domain/                  # ドメイン層
    ├── Factories/
    │   └── MnemonicTimerDeviceFactory.cs
    ├── Interfaces/
    │   └── IMnemonicDeviceMemoryStore.cs
    └── Services/
        └── DeviceOffsets.cs
```

## 🔧 主要機能

### インターロック戦略（Strategy Pattern）

| 戦略クラス | 説明 |
|-----------|------|
| `On1Strategy` | ON1条件の処理 |
| `On2Strategy` | ON2条件の処理 |
| `OnMStrategy` | ONM条件の処理 |
| `OnOrStrategy` | ON-OR条件の処理 |
| `Off1Strategy` | OFF1条件の処理 |
| `AnyStrategy` | ANY条件の処理 |

### アプリケーションサービス

| クラス | 説明 |
|--------|------|
| `IOAddressService` | IOアドレスの解決・変換 |
| `ErrorAggregator` | エラー情報の集約と管理 |
| `InterlockMnemonicOutput` | インターロックニモニック出力生成 |
| `SaveProcessDetailTimerDevicesUseCase` | 工程詳細タイマーデバイス保存 |

## 🔧 Usage

```csharp
using Kdx.Core.Application;

// DIコンテナへの登録
services.AddKdxCoreServices();

// インターロック戦略の使用
var resolver = serviceProvider.GetService<IInterlockMnemonicStrategyResolver>();
var strategy = resolver.Resolve(conditionType);
var output = strategy.Execute(context);
```

## 🔗 Dependencies

- `Kdx.Contracts` - DTOおよびインターフェース
- `Kdx.Infrastructure.Supabase` - Supabaseリポジトリ
- `Npgsql` (9.0.3) - PostgreSQLデータ型サポート

## 📚 Documentation

- [Main README](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/README.md)
- [CHANGELOG](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/CHANGELOG.md)

## 📄 License

MIT License - see [LICENSE](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/LICENSE.txt)
