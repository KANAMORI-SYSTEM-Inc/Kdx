# Extensions

拡張メソッドおよびDI（依存性注入）設定を格納するディレクトリです。

## 概要

.NETの拡張メソッドパターンを使用して、既存のクラスに機能を追加します。主にDIコンテナの設定を行うための拡張メソッドを含みます。

## ファイル一覧

| ファイル | 説明 |
|---------|------|
| `ServiceCollectionExtensions.cs` | IServiceCollectionへの拡張メソッド。ViewModelやServiceの依存性注入を設定 |

## 使用例

```csharp
// App.xaml.cs での使用
services.AddKdxDesignerServices();
```
