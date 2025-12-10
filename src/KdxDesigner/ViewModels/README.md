# ViewModels

MVVMパターンのViewModelクラスを格納するディレクトリです。

## 概要

CommunityToolkit.Mvvmを使用したViewModelクラスを定義します。ObservableObject継承、[ObservableProperty]属性、[RelayCommand]属性を活用してプロパティ変更通知とコマンドを実装します。

## ディレクトリ構造

```
ViewModels/
├── LoginViewModel.cs           # ログイン画面のViewModel
├── ProcessFlowDetailViewModel.cs # プロセスフロー詳細画面のViewModel
├── MainView/                   # メイン画面関連のViewModel
│   ├── MainViewModel.cs        # メイン画面のViewModel（部分クラス）
│   ├── MainViewModel.Properties.cs
│   ├── MainViewModel.Commands.cs
│   ├── InterlockViewModels/    # インターロック設定関連
│   ├── IOEditor/               # IOエディター関連
│   ├── Memory/                 # メモリ設定関連
│   ├── MemonicMemorySetting/   # ニモニックメモリ設定関連
│   ├── OutputViewModels/       # 出力関連
│   └── Settings/               # 設定関連
├── PropertyWindow/             # プロパティウィンドウ用ViewModel
├── PropertyList/               # プロパティリスト用ViewModel
└── Managers/                   # 状態管理マネージャー
```

## 主要ファイル

### ルートレベル
| ファイル | 説明 |
|---------|------|
| `LoginViewModel.cs` | ログイン認証とSupabase接続を管理 |
| `ProcessFlowDetailViewModel.cs` | プロセスフロー図の表示・編集を管理 |

### MainView（部分クラス構成）
| ファイル | 説明 |
|---------|------|
| `MainViewModel.cs` | メインViewModelのコア部分 |
| `MainViewModel.Properties.cs` | プロパティ定義 |
| `MainViewModel.Commands.cs` | コマンド定義 |

### Managers
| ファイル | 説明 |
|---------|------|
| `SelectionStateManager.cs` | 選択状態の管理 |
| `ServiceInitializer.cs` | サービス初期化の管理 |
| `MemoryConfigurationManager.cs` | メモリ設定の管理 |
| `DeviceConfigurationManager.cs` | デバイス設定の管理 |

## 設計パターン

- **部分クラス**: 大規模なViewModelは機能ごとにファイル分割
- **ObservableProperty**: プロパティ変更通知の自動生成
- **RelayCommand**: コマンドの宣言的定義
- **依存性注入**: コンストラクタでリポジトリを受け取る
