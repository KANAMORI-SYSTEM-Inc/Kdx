# Kdx.Infrastructure.Supabase

Supabase/PostgreSQLデータベースへのアクセスを提供するリポジトリパッケージです。

## 📦 Package Information

- **Package ID**: Kdx.Infrastructure.Supabase
- **Description**: Supabase-specific infrastructure implementation for KDX Projects
- **License**: MIT
- **Target Framework**: .NET 8.0

## 📥 Installation

```bash
dotnet add package Kdx.Infrastructure.Supabase
```

## 🎯 Purpose

このパッケージは以下を提供します：
- Supabase（PostgreSQL）データベースへのリポジトリパターン実装
- データベーステーブルに対応するEntityクラス
- CRUD操作を行うリポジトリ

## 📁 ディレクトリ構造

```
Kdx.Infrastructure.Supabase/
├── Entities/                # データベースエンティティ
│   ├── CompanyEntity.cs
│   ├── CycleEntity.cs
│   ├── CylinderEntity.cs
│   ├── ProcessEntity.cs
│   ├── ProcessDetailEntity.cs
│   ├── OperationEntity.cs
│   ├── IOEntity.cs
│   ├── InterlockEntity.cs
│   └── ...
└── Repositories/            # リポジトリ
    ├── ISupabaseRepository.cs
    ├── SupabaseRepository.cs
    └── SupabaseRepository.NewMethods.cs
```

## 🔧 主要クラス

### ISupabaseRepository

Supabaseへのアクセスを抽象化するインターフェース：

- **マスターデータ取得**: 会社、機械、機種、PLC、サイクル等
- **プロセス管理**: 工程、工程詳細、操作のCRUD
- **シリンダー管理**: シリンダー、シリンダーIOのCRUD
- **IO管理**: IO情報の取得・更新
- **インターロック管理**: インターロック条件の管理
- **メモリ管理**: メモリデバイス、タイマーデバイスの管理

### SupabaseRepository

`ISupabaseRepository`の具体的な実装。Supabase C# SDKを使用してデータベースにアクセスします。

## 📊 Entityクラス

| Entity | テーブル | 説明 |
|--------|---------|------|
| `CompanyEntity` | `companies` | 会社 |
| `MachineEntity` | `machines` | 機械 |
| `CycleEntity` | `cycles` | サイクル |
| `ProcessEntity` | `processes` | 工程 |
| `ProcessDetailEntity` | `process_details` | 工程詳細 |
| `OperationEntity` | `operations` | 操作 |
| `CylinderEntity` | `cylinders` | シリンダー |
| `IOEntity` | `ios` | IO |
| `InterlockEntity` | `interlocks` | インターロック |

## 🔧 Usage

```csharp
using Kdx.Infrastructure.Supabase.Repositories;

// リポジトリの初期化
var repository = new SupabaseRepository(supabaseUrl, supabaseKey);

// データ取得
var cycles = await repository.GetCyclesAsync();
var processes = await repository.GetProcessesByCycleIdAsync(cycleId);

// データ更新
await repository.UpdateProcessAsync(process);

// データ追加
int newId = await repository.AddProcessDetailAsync(processDetail);

// データ削除
await repository.DeleteOperationAsync(operationId);
```

## 🔗 Dependencies

- `Kdx.Contracts` - DTOおよびインターフェース
- `supabase-csharp` (0.16.2) - Supabase C# SDK
- `postgrest-csharp` (3.5.1) - PostgREST C# クライアント
- `Npgsql` (9.0.3) - PostgreSQL .NETプロバイダ

## 📝 Notes

- Entity→DTOの変換はリポジトリ内部で自動的に行われます
- 部分クラス構成により、メソッドは複数ファイルに分割されています

## 📚 Documentation

- [Main README](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/README.md)
- [CHANGELOG](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/CHANGELOG.md)

## 📄 License

MIT License - see [LICENSE](https://github.com/KANAMORI-SYSTEM-Inc/KdxProjects/blob/master/LICENSE.txt)
