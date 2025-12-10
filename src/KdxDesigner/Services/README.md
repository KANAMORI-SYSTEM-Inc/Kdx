# Services

ビジネスロジックおよびデータアクセスサービスを格納するディレクトリです。

## 概要

データベース接続、データ変換、プロファイル管理など、アプリケーションの中核となるサービスクラスを定義します。ViewModelから呼び出され、データの取得・加工・保存を担当します。

## ファイル一覧

### 接続ヘルパー
| ファイル | 説明 |
|---------|------|
| `ConnectionHelper.cs` | データベース接続を管理するヘルパークラス |
| `SupabaseConnectionHelper.cs` | Supabaseへの接続を管理するヘルパークラス |

### プロファイル管理
| ファイル | 説明 |
|---------|------|
| `PlcMemoryProfileManager.cs` | PLCメモリプロファイルの読み込み・保存を管理 |
| `CycleMemoryProfileManager.cs` | サイクルメモリプロファイルの読み込み・保存を管理 |

### データ構築・変換
| ファイル | 説明 |
|---------|------|
| `CylinderInterlockDataBuilder.cs` | シリンダーインターロックデータを構築するビルダークラス |
| `MemoryServiceExtensions.cs` | メモリサービス関連の拡張メソッド |

## 設計原則

- **単一責任**: 各サービスは特定の機能領域に特化
- **依存性注入**: IServiceProviderを通じてViewModelに注入
- **非同期処理**: データベースアクセスはasync/awaitパターンを使用
