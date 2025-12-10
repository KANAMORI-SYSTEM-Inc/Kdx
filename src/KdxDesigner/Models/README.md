# Models

アプリケーション固有のモデルクラスを格納するディレクトリです。

## 概要

KdxProjectsのDTOとは別に、KdxDesignerアプリケーション内でのみ使用されるモデルクラスを定義します。UIの表示用ビューモデル、プロセスフロー関連のノード・接続情報、設定データなどを含みます。

## ファイル一覧

### プロセスフロー関連
| ファイル | 説明 |
|---------|------|
| `ProcessFlowNode.cs` | プロセスフロー図のノード（工程詳細）を表現するモデル |
| `ProcessFlowConnection.cs` | プロセスフロー図のノード間接続を表現するモデル |
| `CompositeProcessGroup.cs` | 複合工程のグループ化を表現するモデル |

### ビューモデル
| ファイル | 説明 |
|---------|------|
| `IOViewModel.cs` | IO情報の表示用ビューモデル |
| `CylinderIOViewModel.cs` | シリンダーIO情報の表示用ビューモデル |
| `OperationIOViewModel.cs` | 操作IO情報の表示用ビューモデル |

### 設定・プロファイル
| ファイル | 説明 |
|---------|------|
| `AppSettings.cs` | アプリケーション設定を保持するモデル |
| `MemoryProfile.cs` | メモリプロファイル情報を保持するモデル |
| `PlcMemoryProfile.cs` | PLCメモリプロファイル情報を保持するモデル |
| `CycleMemoryProfile.cs` | サイクルメモリプロファイル情報を保持するモデル |

### その他
| ファイル | 説明 |
|---------|------|
| `MnemonicTypeInfo.cs` | ニモニックタイプ情報を保持するモデル |
| `CylinderInterlockData.cs` | シリンダーインターロックデータを保持するモデル |
