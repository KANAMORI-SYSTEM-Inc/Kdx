# Controls

再利用可能なWPF UserControlコンポーネントを格納するディレクトリです。

## 概要

複数のViewで共通して使用されるUIコンポーネントを定義します。各コントロールはDependencyPropertyを使用してデータバインディングをサポートし、イベントを通じて親Viewと通信します。

## ファイル一覧

| ファイル | 説明 |
|---------|------|
| `CylinderListControl.xaml/.cs` | シリンダー一覧表示・編集用コントロール。検索、追加、編集、削除、フィルタリング機能を提供 |
| `ProcessListControl.xaml/.cs` | 工程一覧表示・編集用コントロール。工程の追加、コピー、削除、保存機能を提供 |
| `ProcessDetailListControl.xaml/.cs` | 工程詳細一覧表示・編集用コントロール。工程詳細の追加、コピー、削除、保存機能を提供 |
| `OperationListControl.xaml/.cs` | 操作一覧表示・編集用コントロール。操作の追加、コピー、削除、保存機能を提供 |

## 使用例

```xml
<controls:ProcessListControl
    Repository="{Binding Repository}"
    Processes="{Binding Processes, Mode=TwoWay}"
    SelectedProcess="{Binding SelectedProcess, Mode=TwoWay}"
    ProcessCategories="{Binding ProcessCategories}"
    CycleId="{Binding SelectedCycle.Id}"
    ProcessSelectionChanged="ProcessListControl_ProcessSelectionChanged"/>
```

## 設計パターン

- **DependencyProperty**: データバインディング用のプロパティを定義
- **イベント駆動**: 選択変更や削除などのアクションを親Viewに通知
- **CRUD操作**: 追加、コピー、削除、保存ボタンを標準装備
