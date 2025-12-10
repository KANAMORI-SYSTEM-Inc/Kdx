# Kdx.Infrastructure

KDXシステムのインフラストラクチャサービス実装を提供するパッケージです。

## 📦 Package Information

- **Package ID**: Kdx.Infrastructure
- **Description**: Infrastructure services and implementations for KDX Projects
- **License**: MIT
- **Target Framework**: .NET 8.0

## 📥 Installation

```bash
dotnet add package Kdx.Infrastructure
```

## 🎯 Purpose

このパッケージは以下を提供します：
- `Kdx.Contracts`で定義されたインターフェースの具体的な実装
- キャッシュ管理
- 設定管理
- 各種サービスの実装

## 📁 ディレクトリ構造

```
Kdx.Infrastructure/
├── Cache/                   # キャッシュ管理
│   ├── MnemonicDeviceMemoryStore.cs
│   └── TimerDeviceCashe.cs
├── Configuration/           # 設定管理
│   ├── SupabaseConfiguration.cs
│   └── SupabaseSettings.cs
├── Extensions/              # 拡張メソッド
│   └── ServiceCollectionExtensions.cs
├── Options/                 # オプションパターン
│   └── DeviceOffsetOptions.cs
└── Services/                # サービス実装
    ├── CylinderIOService.cs
    ├── MemoryService.cs
    ├── OperationIOService.cs
    ├── ProcessFlowService.cs
    ├── InterlockValidationService.cs
    ├── IOConversionService.cs
    ├── DeviceOffsetProvider.cs
    ├── SequenceGenerator.cs
    └── ProsTimeDeviceService.cs
```

## 🔧 主要サービス

### IOサービス
| クラス | 説明 |
|--------|------|
| `CylinderIOService` | シリンダーIOの管理・操作 |
| `OperationIOService` | 操作IOの管理・操作 |
| `IOConversionService` | IO変換処理 |

### メモリサービス
| クラス | 説明 |
|--------|------|
| `MemoryService` | メモリデバイスの管理 |
| `MnemonicDeviceMemoryStore` | ニモニックデバイスのメモリキャッシュ |

### プロセスサービス
| クラス | 説明 |
|--------|------|
| `ProcessFlowService` | プロセスフローの管理 |
| `InterlockValidationService` | インターロック検証 |

### その他
| クラス | 説明 |
|--------|------|
| `DeviceOffsetProvider` | デバイスオフセット提供 |
| `SequenceGenerator` | シーケンス番号生成 |
| `ProsTimeDeviceService` | プロセス時間デバイスサービス |

## 🔧 Usage

```csharp
using Kdx.Infrastructure.Extensions;
using Kdx.Infrastructure.Services;

// DIコンテナへの登録
services.AddKdxInfrastructureServices();

// サービスの使用
var cylinderIOService = serviceProvider.GetService<ICylinderIOService>();
var result = await cylinderIOService.GetCylinderIOsAsync(cylinderId);
```

## 🔗 Dependencies

- `Kdx.Contracts` - DTOおよびインターフェース
- `Kdx.Core` - ビジネスロジック
- `Kdx.Infrastructure.Supabase` - Supabaseリポジトリ
- `EPPlus` (8.2.0) - Excelファイル操作
- `Npgsql` (9.0.3) - PostgreSQLデータ型サポート
- `Microsoft.Extensions.Options` - オプションパターンサポート

## 📚 Documentation

- [Main README](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/README.md)
- [CHANGELOG](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/CHANGELOG.md)

## 📄 License

MIT License - see [LICENSE](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/LICENSE.txt)
