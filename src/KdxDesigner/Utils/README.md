# Utils

ユーティリティクラスおよびコンバーターを格納するディレクトリです。

## 概要

アプリケーション全体で共通して使用されるヘルパークラス、WPFバインディング用コンバーター、エクスポート機能などを定義します。

## ファイル一覧

### エクスポート
| ファイル | 説明 |
|---------|------|
| `CsvExporter.cs` | 汎用CSVエクスポート機能 |
| `LaddrCsvExporter.cs` | ラダープログラム用CSV出力機能 |

### WPFコンバーター
| ファイル | 説明 |
|---------|------|
| `NullToBooleanConverter.cs` | null値をbool値に変換するコンバーター |
| `BoolToColorConverter.cs` | bool値を色に変換するコンバーター |
| `NullCategoryConverter.cs` | nullカテゴリを処理するコンバーター |

### 設定・計算
| ファイル | 説明 |
|---------|------|
| `SettingsManager.cs` | アプリケーション設定の読み込み・保存を管理 |
| `LinkDeviceCalculator.cs` | リンクデバイスのアドレス計算を行うユーティリティ |

## 使用例

### コンバーター

```xml
<TextBox IsEnabled="{Binding SelectedItem, Converter={StaticResource NullToBooleanConverter}}"/>
```

### エクスポート

```csharp
var exporter = new LaddrCsvExporter();
await exporter.ExportAsync(filePath, mnemonicData);
```
