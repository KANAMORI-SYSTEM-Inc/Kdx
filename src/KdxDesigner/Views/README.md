# Views

WPF画面（Window/UserControl/ダイアログ）を格納するディレクトリです。

## 概要

XAMLで定義されたUI画面とそのコードビハインドを含みます。各ViewはViewModelとデータバインディングで接続され、MVVMパターンに従います。

## ディレクトリ構造

```
Views/
├── LoginView.xaml              # ログイン画面
├── MainView.xaml               # メイン画面
├── ErrorDialog.xaml            # エラーダイアログ
├── CategoryToColorConverter.cs # カテゴリ色変換コンバーター
├── Common/                     # 共通ダイアログ
│   ├── IOSearchWindow.xaml     # IO検索ウィンドウ
│   └── IOSelectView.xaml       # IO選択ビュー
├── Settings/                   # 設定画面
│   └── SettingsView.xaml       # 設定画面
├── Tools/                      # ツール画面
│   ├── IoEditorView.xaml       # IOエディター
│   ├── IOConversionWindow.xaml # IO変換ウィンドウ
│   ├── LinkDeviceView.xaml     # リンクデバイス設定
│   ├── ControlBoxViews.xaml    # 操作盤設定
│   ├── Timer/                  # タイマー関連
│   └── CylinderManagement/     # シリンダー管理
├── ProcessFlow/                # プロセスフロー関連
│   ├── ProcessPropertiesWindow.xaml
│   ├── ProcessDetailPropertiesWindow.xaml
│   ├── OperationPropertiesWindow.xaml
│   ├── ProcessFlowDetailWindow.xaml
│   └── ...
├── Memory/                     # メモリ設定関連
│   ├── MemoryEditorView.xaml
│   ├── MemorySettingWindow.xaml
│   └── ...
├── Output/                     # 出力関連
│   └── OutputWindow.xaml
└── Interlock/                  # インターロック関連
    ├── InterlockSettingsWindow.xaml
    └── InterlockPreConditionWindow.xaml
```

## 主要画面

### エントリーポイント
| ファイル | 説明 |
|---------|------|
| `LoginView.xaml` | アプリケーション起動時のログイン画面 |
| `MainView.xaml` | メイン操作画面（工程・操作・詳細の一覧表示） |

### プロパティウィンドウ
| ファイル | 説明 |
|---------|------|
| `ProcessPropertiesWindow.xaml` | 工程プロパティ編集ダイアログ |
| `ProcessDetailPropertiesWindow.xaml` | 工程詳細プロパティ編集ダイアログ |
| `OperationPropertiesWindow.xaml` | 操作プロパティ編集ダイアログ |

### 設定画面
| ファイル | 説明 |
|---------|------|
| `SettingsView.xaml` | アプリケーション設定画面 |
| `MemorySettingWindow.xaml` | メモリ設定ウィンドウ |
| `InterlockSettingsWindow.xaml` | インターロック設定ウィンドウ |

## 設計原則

- **MVVM分離**: ViewはUIのみ担当、ロジックはViewModelに委譲
- **データバインディング**: `{Binding}`でViewModelのプロパティと接続
- **コマンド**: ボタンクリックはICommandを通じてViewModelに委譲
- **コードビハインド最小化**: イベントハンドラは可能な限りViewModelに移動
