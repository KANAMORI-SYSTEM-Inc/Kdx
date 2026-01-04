using Kdx.Infrastructure.Supabase.Repositories;

namespace KdxDesigner.Services.ErrorMessageGenerator
{
    /// <summary>
    /// エラーメッセージ生成サービスの基底クラス
    /// 共通のロジックを提供
    /// </summary>
    public abstract class ErrorMessageGeneratorBase
    {
        protected readonly ISupabaseRepository _repository;

        // UI表示用の装飾記号（選択状態を示す●○マーク）
        // これらはエラーメッセージには不要なため置換時に削除
        protected const string FILLED_CIRCLE = "●";
        protected const string HOLLOW_CIRCLE = "○";

        // 全角・半角空白（エラーメッセージの整形用）
        protected const string HALF_WIDTH_SPACE = " ";
        protected const string FULL_WIDTH_SPACE = "　";

        protected ErrorMessageGeneratorBase(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// プレースホルダーを置換
        /// UI表示用の装飾記号（●○）や空白文字を削除してエラーメッセージを整形します
        /// これらの記号はUIでの選択状態表示に使われるため、エラーメッセージには不要です
        /// </summary>
        /// <param name="template">テンプレート文字列（例: "{CylinderName}のインターロック異常"）</param>
        /// <param name="values">プレースホルダー値のディクショナリ</param>
        /// <returns>置換後の文字列</returns>
        protected string ReplacePlaceholders(string? template, Dictionary<string, string?> values)
        {
            if (string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            var result = template;
            foreach (var kvp in values)
            {
                if (kvp.Value == null)
                {
                    continue;
                }

                // UI表示用の装飾記号と空白を削除してエラーメッセージを整形
                var value = kvp.Value?.Replace(FILLED_CIRCLE, "")
                                       .Replace(HOLLOW_CIRCLE, "")
                                       .Replace(HALF_WIDTH_SPACE, "")
                                       .Replace(FULL_WIDTH_SPACE, "");
                Console.WriteLine($"Replacing {{{kvp.Key}}} with '{value}'");
                result = result.Replace($"{{{kvp.Key}}}", value ?? "");
            }
            return result;
        }
    }
}
