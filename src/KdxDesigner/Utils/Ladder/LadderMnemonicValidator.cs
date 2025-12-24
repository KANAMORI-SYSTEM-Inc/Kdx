using Kdx.Contracts.DTOs;

namespace KdxDesigner.Utils.Ladder
{
    /// <summary>
    /// ラダーバリデーションの問題を表すレコード
    /// </summary>
    public record LadderValidationIssue(
        int Key,
        string StepNo,
        string Command,
        string Address,
        string? FileName,  // 出力元のCSVファイル名
        string Level,      // "Error" or "Warn"
        string Message
    );

    /// <summary>
    /// ラダーニーモニックのバリデーションを行うクラス
    /// </summary>
    public static class LadderMnemonicValidator
    {
        // 命令分類
        private static readonly HashSet<string> LoadCommands = new(StringComparer.OrdinalIgnoreCase)
        { "LD", "LDI", "LDP", "LD=", "LDF", "LD<>" };

        private static readonly HashSet<string> LogicCommands = new(StringComparer.OrdinalIgnoreCase)
        { "AND", "ANI", "OR", "ORI", "ORP", "AND<>", "ORF", "OR=", "MEP", "MEF" };

        private static readonly HashSet<string> BlockCommands = new(StringComparer.OrdinalIgnoreCase)
        { "ORB", "ANB" };

        private static readonly HashSet<string> RungSeparators = new(StringComparer.OrdinalIgnoreCase)
        { "NOPLF" };

        // スタック操作命令
        private static readonly HashSet<string> StackCommands = new(StringComparer.OrdinalIgnoreCase)
        { "MPS", "MRD", "MPP" };

        // アドレス指定が不要な命令（MEP, MEF, ORB, ANB, MPS, MRD, MPP, NOPLFなど）
        private static readonly HashSet<string> NoAddressCommands = new(StringComparer.OrdinalIgnoreCase)
        { "MEP", "MEF", "ORB", "ANB", "MPS", "MRD", "MPP", "NOPLF" };

        // 「直前の条件式が必要」な命令
        private static readonly HashSet<string> RequiresExprCommands = new(StringComparer.OrdinalIgnoreCase)
        { "OUT", "SET", "RST", "MOV", "DMOV", "INC", "DEC", "MOVP", "INC", "PLS", "OUTH", "CJ" };

        public static List<LadderValidationIssue> Validate(List<LadderCsvRow> rows)
        {
            var issues = new List<LadderValidationIssue>();

            bool hasCurrentExpr = false;      // 現在"評価中"の式があるか
            var exprStack = new Stack<bool>(); // "過去の枝"を積む（ORB/ANB用）
            var exprStackStartRows = new Stack<LadderCsvRow>(); // exprStackの開始位置を記録
            var mpsStack = new Stack<bool>();  // MPS/MRD/MPP用スタック
            var mpsStackStartRows = new Stack<LadderCsvRow>(); // MPSスタックの開始位置を記録
            string? lastMeaningfulCommand = null;
            int? lastMeaningfulKey = null;

            LadderCsvRow? prevMeaningfulRow = null;

            foreach (var row in rows)
            {
                var cmd = Unquote(row.Command)?.Trim();
                var addr = Unquote(row.Address)?.Trim();
                var stepNo = Unquote(row.StepNo)?.Trim();
                var stepComment = Unquote(row.StepComment)?.Trim();

                bool isCommentRow = !string.IsNullOrWhiteSpace(stepComment) && stepComment.StartsWith("*");
                bool isBlankCommand = string.IsNullOrWhiteSpace(cmd);

                // --- パラメータ行（K10など） ---
                if (isBlankCommand && !string.IsNullOrWhiteSpace(addr) && addr.StartsWith("K", StringComparison.OrdinalIgnoreCase))
                {
                    if (prevMeaningfulRow == null)
                    {
                        issues.Add(Warn(row, "パラメータ行(K値)が単独で出現しています。直前に対応する命令が必要です。"));
                    }
                    continue;
                }

                // --- ステップコメントで段区切り ---
                if (isCommentRow)
                {
                    ResetExpr();
                    continue;
                }

                // --- 段区切り命令 ---
                if (!string.IsNullOrWhiteSpace(cmd) && RungSeparators.Contains(cmd))
                {
                    ResetExpr();
                    MarkMeaningful(row);
                    continue;
                }

                // --- ロード系 ---
                if (!string.IsNullOrWhiteSpace(cmd) && LoadCommands.Contains(cmd))
                {
                    // すでに式がある状態でLDが来たら「枝」を作ったとみなしてスタックへ退避
                    if (hasCurrentExpr)
                    {
                        exprStack.Push(true);
                        exprStackStartRows.Push(row); // スタック開始位置を記録
                    }
                    hasCurrentExpr = true;

                    // 住所が空はエラー
                    if (string.IsNullOrWhiteSpace(addr))
                    {
                        issues.Add(Error(row, "LD/LDI のアドレスが空です。"));
                    }

                    MarkMeaningful(row);
                    continue;
                }

                // --- 論理命令 ---
                if (!string.IsNullOrWhiteSpace(cmd) && LogicCommands.Contains(cmd))
                {
                    if (!hasCurrentExpr)
                    {
                        issues.Add(Error(row, $"{cmd} の前に LD/LDI がありません（条件式が未開始）。"));
                    }

                    // アドレス不要な命令でない場合のみアドレスチェック
                    if (!NoAddressCommands.Contains(cmd) && string.IsNullOrWhiteSpace(addr))
                    {
                        issues.Add(Error(row, $"{cmd} のアドレスが空です。"));
                    }

                    MarkMeaningful(row);
                    continue;
                }

                // --- ブロック結合 ORB/ANB ---
                if (!string.IsNullOrWhiteSpace(cmd) && BlockCommands.Contains(cmd))
                {
                    if (!hasCurrentExpr)
                    {
                        issues.Add(Error(row, $"{cmd} の実行には「現在式」が必要ですが存在しません。"));
                    }

                    if (exprStack.Count < 1)
                    {
                        issues.Add(Error(row, $"{cmd} の実行には「一つ前の枝(式)」が必要ですが、スタックが空です。"));
                    }
                    else
                    {
                        // 結合して1つの式になる
                        _ = exprStack.Pop();
                        if (exprStackStartRows.Count > 0)
                        {
                            _ = exprStackStartRows.Pop(); // 対応する開始位置も削除
                        }
                        hasCurrentExpr = true;
                    }

                    MarkMeaningful(row);
                    continue;
                }

                // --- スタック操作 MPS/MRD/MPP ---
                if (!string.IsNullOrWhiteSpace(cmd) && StackCommands.Contains(cmd))
                {
                    if (cmd.Equals("MPS", StringComparison.OrdinalIgnoreCase))
                    {
                        // MPS: 現在の演算結果をスタックに保存
                        if (!hasCurrentExpr)
                        {
                            issues.Add(Error(row, "MPS の前に条件式(LD/LDI...)がありません。"));
                        }
                        else
                        {
                            mpsStack.Push(hasCurrentExpr);
                            mpsStackStartRows.Push(row); // MPS開始位置を記録
                            // hasCurrentExprは維持される（MPSは演算結果を保持）
                        }
                    }
                    else if (cmd.Equals("MRD", StringComparison.OrdinalIgnoreCase))
                    {
                        // MRD: スタックの最上位を読み出す（Popしない）
                        if (mpsStack.Count == 0)
                        {
                            issues.Add(Error(row, "MRD の実行にはMPSで保存された値が必要ですが、MPSスタックが空です。"));
                            hasCurrentExpr = false;
                        }
                        else
                        {
                            hasCurrentExpr = mpsStack.Peek();
                        }
                    }
                    else if (cmd.Equals("MPP", StringComparison.OrdinalIgnoreCase))
                    {
                        // MPP: スタックの最上位を取り出す（Pop）
                        if (mpsStack.Count == 0)
                        {
                            issues.Add(Error(row, "MPP の実行にはMPSで保存された値が必要ですが、MPSスタックが空です。"));
                            hasCurrentExpr = false;
                        }
                        else
                        {
                            hasCurrentExpr = mpsStack.Pop();
                            if (mpsStackStartRows.Count > 0)
                            {
                                _ = mpsStackStartRows.Pop(); // 対応するMPS開始位置も削除
                            }
                        }
                    }

                    MarkMeaningful(row);
                    continue;
                }

                // --- FB CALL ---
                if (!string.IsNullOrWhiteSpace(cmd) && cmd.Equals("FBCALL", StringComparison.OrdinalIgnoreCase))
                {
                    if (hasCurrentExpr || exprStack.Count > 0)
                    {
                        issues.Add(Warn(row,
                            "FBCALL 実行時に条件式が残っています。意図せず条件付き呼出になる可能性があるため、段区切り(NOPLF)や無条件LDで明示するのを推奨します。"));
                    }

                    MarkMeaningful(row);
                    continue;
                }

                // --- 出力/実行系 ---
                if (!string.IsNullOrWhiteSpace(cmd) && RequiresExprCommands.Contains(cmd))
                {
                    if (!hasCurrentExpr && exprStack.Count == 0)
                    {
                        issues.Add(Error(row, $"{cmd} の前に条件式(LD/LDI...)がありません。"));
                    }

                    if (string.IsNullOrWhiteSpace(addr))
                    {
                        issues.Add(Error(row, $"{cmd} のアドレスが空です。"));
                    }

                    MarkMeaningful(row);
                    continue;
                }

                // --- その他命令（未定義）---
                if (!string.IsNullOrWhiteSpace(cmd))
                {
                    issues.Add(Warn(row, $"未分類命令 '{cmd}' を検出しました。必要ならバリデータに命令ルールを追加してください。"));
                    MarkMeaningful(row);
                }
            }

            // 末尾でスタックが残ってたら警告
            if (exprStack.Count > 0)
            {
                // スタックの最初の開始位置を取得（スタックの最も古い要素）
                if (exprStackStartRows.Count > 0)
                {
                    // スタックの底（最初にプッシュされた要素）を取得
                    var startRow = exprStackStartRows.ToArray()[exprStackStartRows.Count - 1];
                    issues.Add(new LadderValidationIssue(
                        Key: startRow.Key,
                        StepNo: Unquote(startRow.StepNo) ?? "",
                        Command: Unquote(startRow.Command) ?? "",
                        Address: Unquote(startRow.Address) ?? "",
                        FileName: startRow.FileName,
                        Level: "Warn",
                        Message: $"この位置で開始された条件ブロックが閉じられていません。スタック残数: {exprStack.Count}。ORB/ANB で閉じる必要があります。"
                    ));
                }
                else
                {
                    // 開始位置が不明な場合（通常は発生しないはず）
                    issues.Add(new LadderValidationIssue(
                        Key: lastMeaningfulKey ?? -1,
                        StepNo: "",
                        Command: lastMeaningfulCommand ?? "",
                        Address: "",
                        FileName: prevMeaningfulRow?.FileName,
                        Level: "Warn",
                        Message: $"条件ブロックのスタックが {exprStack.Count} 残っています。LDの重複(枝)が ORB/ANB で閉じられていない可能性があります。"
                    ));
                }
            }

            // MPSスタックが残ってたら警告
            if (mpsStack.Count > 0)
            {
                // MPSスタックの最初の開始位置を取得
                if (mpsStackStartRows.Count > 0)
                {
                    // スタックの底（最初にプッシュされた要素）を取得
                    var startRow = mpsStackStartRows.ToArray()[mpsStackStartRows.Count - 1];
                    issues.Add(new LadderValidationIssue(
                        Key: startRow.Key,
                        StepNo: Unquote(startRow.StepNo) ?? "",
                        Command: Unquote(startRow.Command) ?? "",
                        Address: Unquote(startRow.Address) ?? "",
                        FileName: startRow.FileName,
                        Level: "Warn",
                        Message: $"この位置のMPSに対応するMPPがありません。スタック残数: {mpsStack.Count}。"
                    ));
                }
                else
                {
                    // 開始位置が不明な場合（通常は発生しないはず）
                    issues.Add(new LadderValidationIssue(
                        Key: lastMeaningfulKey ?? -1,
                        StepNo: "",
                        Command: lastMeaningfulCommand ?? "",
                        Address: "",
                        FileName: prevMeaningfulRow?.FileName,
                        Level: "Warn",
                        Message: $"MPSスタックが {mpsStack.Count} 残っています。MPS/MPPの対応が取れていない可能性があります。"
                    ));
                }
            }

            return issues;

            // ---- local helpers ----
            void ResetExpr()
            {
                hasCurrentExpr = false;
                exprStack.Clear();
                exprStackStartRows.Clear();
                mpsStack.Clear();
                mpsStackStartRows.Clear();
            }

            void MarkMeaningful(LadderCsvRow r)
            {
                lastMeaningfulCommand = Unquote(r.Command);
                lastMeaningfulKey = r.Key;
                prevMeaningfulRow = r;
            }
        }

        private static LadderValidationIssue Error(LadderCsvRow row, string msg) =>
            new(row.Key, Unquote(row.StepNo) ?? "", Unquote(row.Command) ?? "", Unquote(row.Address) ?? "", row.FileName, "Error", msg);

        private static LadderValidationIssue Warn(LadderCsvRow row, string msg) =>
            new(row.Key, Unquote(row.StepNo) ?? "", Unquote(row.Command) ?? "", Unquote(row.Address) ?? "", row.FileName, "Warn", msg);

        private static string? Unquote(string? s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            s = s.Trim();
            if (s.Length >= 2 && s.StartsWith("\"") && s.EndsWith("\""))
            {
                return s.Substring(1, s.Length - 2);

            }
            return s;
        }
    }
}
