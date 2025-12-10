# Resources

アプリケーションで使用するリソースファイルを格納するディレクトリです。

## 概要

アイコン、画像、XAMLリソースディクショナリなど、アプリケーションの視覚的要素を定義するファイルを含みます。

## ファイル一覧

| ファイル | 説明 |
|---------|------|
| `app_icon.xaml` | アプリケーションアイコンのXAMLリソース定義 |

## 使用方法

リソースはApp.xamlでマージして使用します：

```xml
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Resources/app_icon.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>
```
