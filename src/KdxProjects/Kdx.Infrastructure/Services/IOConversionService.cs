using OfficeOpenXml;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Services
{
    /// <summary>
    /// Provides functionality to convert Excel files containing I/O configuration data into a list of IO objects using
    /// specified conversion settings.
    /// </summary>
    /// <remarks>Use this service to extract and transform I/O information from Excel worksheets, supporting
    /// options such as including circle mark data and applying new naming rules. The conversion process is controlled
    /// via the ConversionSettings class, which specifies the source file path, target sheet names, and other options.
    /// This class is intended for scenarios where structured I/O data needs to be programmatically imported from Excel
    /// for further processing or integration.</remarks>
    public class IOConversionService
    {
        public class ConversionSettings
        {
            public string SourceFilePath { get; set; } = string.Empty;
            public string SheetNames { get; set; } = string.Empty; // カンマ区切り
            public bool IncludeCircle { get; set; } // ●○情報を含むか
            public bool UseNewNaming { get; set; } // 新名称ルールを使用するか
            public int PlcId { get; set; }
        }

        private const int MAX_LINE = 10000;

        /// <summary>
        /// Converts data from specified Excel worksheets into a list of IO objects based on the provided conversion
        /// settings.
        /// </summary>
        /// <remarks>Each IO object in the returned list is populated according to the rules defined in
        /// the conversion settings, including PLC identifier and station number. Only worksheets listed in the settings
        /// are processed; missing or invalid sheets are skipped. The method does not modify the source Excel
        /// file.</remarks>
        /// <param name="settings">The conversion settings that specify the source Excel file path, worksheet names to process, PLC identifier,
        /// and additional options for data extraction and naming. Cannot be null.</param>
        /// <returns>A list of IO objects extracted from the specified Excel worksheets. The list will be empty if no valid IO
        /// data is found.</returns>
        public List<IO> ConvertExcelToIOList(ConversionSettings settings)
        {
            // EPPlus 8 ライセンス設定
            ExcelPackage.License.SetNonCommercialPersonal("Personal");

            var result = new List<IO>();

            using var package = new ExcelPackage(new FileInfo(settings.SourceFilePath));
            var sheetNames = settings.SheetNames.Split(',').Select(s => s.Trim()).ToArray();
            var adCounter = 0;

            foreach (var sheetName in sheetNames)
            {
                var worksheet = package.Workbook.Worksheets[sheetName];
                if (worksheet == null) continue;

                var blankCount = 0;

                for (int row = 1; row <= MAX_LINE; row++)
                {
                    // F列で"Ｉ／Ｏ番号"を検索
                    var cellValue = worksheet.Cells[row, 6].Text; // F列

                    if (cellValue == "Ｉ／Ｏ番号")
                    {
                        // 1行下のアドレスを取得
                        var address = worksheet.Cells[row + 1, 6].Text
                            .Replace(" ", "")
                            .Replace("　", "");

                        var nColumnValue = worksheet.Cells[row + 1, 14].Text; // N列

                        if (nColumnValue.EndsWith("64AD"))
                        {
                            // A/Dユニットの処理
                            adCounter++;
                            var addressHex = address.Substring(1, 3);
                            var addressDec = Convert.ToInt32(addressHex, 16) * 16;
                            var addressPrefix = address.Substring(0, 1);

                            var adIOs = ProcessADData(worksheet, row + 2, addressDec, adCounter, addressPrefix);
                            foreach (var io in adIOs)
                            {
                                io.PlcId = settings.PlcId;
                                io.StationNumber = sheetName;
                                io.IOSpot = worksheet.Cells[row - 1, 6].Text; // 制御盤情報
                                io.System = worksheet.Cells[row, 11].Text.Replace("局番:", "").Replace("局番：", ""); // 局番
                            }
                            result.AddRange(adIOs);
                        }
                        else if (!string.IsNullOrEmpty(address) && address != "CH" && !address.StartsWith("B"))
                        {
                            // 通常のI/Oデータ処理
                            var addressHex = address.Substring(1, 3);
                            var addressDec = Convert.ToInt32(addressHex, 16) * 16;

                            var ios = ProcessIOData(
                                worksheet,
                                row + 2,
                                addressDec,
                                settings.IncludeCircle,
                                settings.UseNewNaming,
                                sheetName);

                            foreach (var io in ios)
                            {
                                io.PlcId = settings.PlcId;
                            }
                            result.AddRange(ios);
                        }

                        row += 17;
                        blankCount = 0;
                    }
                    else
                    {
                        blankCount++;
                    }

                    if (blankCount > 100) break;
                }
            }

            return result;
        }

        private List<IO> ProcessIOData(
            ExcelWorksheet worksheet,
            int startRow,
            int addressDec,
            bool includeCircle,
            bool useNewNaming,
            string sheetName)
        {
            var result = new List<IO>();

            var controlBoxInfo = worksheet.Cells[startRow - 3, 6].Text;      // 制御盤情報
            var addressPrefix = worksheet.Cells[startRow, 6].Text;           // アドレスプレフィックス
            var stationNumber = worksheet.Cells[startRow - 2, 11].Text       // 局番
                .Replace("局番:", "")
                .Replace("局番:", "");

            for (int i = 0; i <= 15; i++)
            {
                var row = startRow + i;
                var io = new IO
                {
                    Address = addressPrefix + worksheet.Cells[row, 7].Text,  // G列:アドレス下1桁
                    IOSpot = controlBoxInfo,
                    System = stationNumber,
                    StationNumber = sheetName
                };

                // PLCコメント (B,C,D,E列)
                var comment1 = worksheet.Cells[row, 2].Text;
                var comment2 = worksheet.Cells[row, 3].Text;
                var comment3 = worksheet.Cells[row, 4].Text;
                var comment4 = worksheet.Cells[row, 5].Text;
                var fullComment = comment1 + comment2 + comment3 + comment4;

                // X/Y/Fコメントの振り分け
                if (addressPrefix.StartsWith("X"))
                {
                    io.XComment = fullComment;
                }
                else if (addressPrefix.StartsWith("Y"))
                {
                    io.YComment = fullComment;
                }
                else if (addressPrefix.StartsWith("F"))
                {
                    io.FComment = fullComment;
                }

                // ●○情報
                var circle = "";
                if (includeCircle)
                {
                    var circleValue = worksheet.Cells[row, 8].Text; // H列
                    circle = string.IsNullOrEmpty(circleValue) ? circleValue + "　 " : circleValue + " ";
                }

                var rb = worksheet.Cells[row, 9].Text;           // I列: RB
                var deviceNum = worksheet.Cells[row, 11].Text;   // K列: 440
                var explanation = worksheet.Cells[row, 12].Text; // L列: 説明

                io.IOName = circle + rb + "-" + deviceNum;
                io.IOExplanation = explanation;

                // ユニット名称の処理
                var unit17 = worksheet.Cells[startRow + 17, 12].Text; // L列
                var unit18 = worksheet.Cells[startRow + 18, 12].Text;

                if (!string.IsNullOrEmpty(unit18) && !string.IsNullOrEmpty(unit17))
                {
                    io.UnitName = i <= 7 ? unit17 : unit18;
                }
                else if (string.IsNullOrEmpty(unit18) && !string.IsNullOrEmpty(unit17))
                {
                    io.UnitName = unit17;
                }
                else
                {
                    io.UnitName = worksheet.Cells[startRow + 40, 12].Text;
                }

                // 新名称ルールの処理
                if (useNewNaming && (rb == "PXS" || rb == "LS" || rb == "MS"))
                {
                    var (newDeviceNum, seigyo, matsuN) = ProcessNewNamingRule(deviceNum, explanation, circle);
                    io.IONameNaked = rb + "-" + newDeviceNum + seigyo + matsuN;
                    io.LinkDevice = "True";
                }

                result.Add(io);
            }

            return result;
        }

        private (string deviceNum, string seigyo, string matsuN) ProcessNewNamingRule(
            string deviceNum,
            string explanation,
            string circle)
        {
            var seigyo = "";
            var matsuN = "";

            // プッシャー&ホールド or サンドカットの処理
            if (explanation.Contains("プッシャー") || explanation.Contains("ホールド") || explanation.Contains("カッター"))
            {
                if (explanation.Contains("伸び"))
                {
                    deviceNum = deviceNum.Replace("B", "Y").Replace("G", "Y");
                }
                else if (explanation.Contains("縮み"))
                {
                    deviceNum = deviceNum.Replace("B", "V").Replace("G", "V");
                }
                else
                {
                    deviceNum = deviceNum.Replace("B", "!").Replace("G", "!");
                }
            }

            // トラバーサの処理
            if (explanation.Contains("トラバーサ"))
            {
                deviceNum = deviceNum.Replace("B", "R").Replace("G", "F");
            }

            // 端の場合
            if (explanation.Contains("端"))
            {
                seigyo = (circle == "● " || circle == "▼" || circle == "▲") ? "B" : "G";
            }

            // 減速、低速の場合
            if (explanation.Contains("減速") || explanation.Contains("低速"))
            {
                seigyo = "L";
            }

            // 高速の場合
            if (explanation.Contains("高速"))
            {
                seigyo = "H";
            }

            // クランプの場合
            if (explanation.Contains("開き") || explanation.Contains("閉じ"))
            {
                seigyo = (circle == "● ") ? "B" : "G";
            }

            // 先頭3文字が異なる場合
            if (deviceNum.Length >= 3 && explanation.Length >= 3 &&
                deviceNum.Substring(0, 3) != explanation.Substring(0, 3))
            {
                deviceNum = explanation.Substring(0, 3) + deviceNum.Substring(1, 1);
            }

            // センサ末尾に数字がある場合
            if (deviceNum.Length > 0)
            {
                var lastChar = deviceNum[deviceNum.Length - 1];
                if (lastChar >= '1' && lastChar <= '4')
                {
                    matsuN = lastChar.ToString();
                    deviceNum = deviceNum.Substring(0, deviceNum.Length - 1);
                }
            }

            return (deviceNum, seigyo, matsuN);
        }

        private List<IO> ProcessADData(ExcelWorksheet worksheet, int startRow, int addressDec, int adCounter, string addressPrefix)
        {
            var result = new List<IO>();
            var isOdd = adCounter % 2 == 1;
            var unitNumber = isOdd ? (adCounter / 2) + 1 : adCounter / 2;

            var xComments = new string?[16];
            var yComments = new string?[16];

            if (isOdd)
            {
                xComments[0] = $"AD{unitNumber}-CH.1変換完了ﾌﾗｸﾞ";
                xComments[1] = $"AD{unitNumber}-CH.2変換完了ﾌﾗｸﾞ";
                xComments[2] = $"AD{unitNumber}-CH.3変換完了ﾌﾗｸﾞ";
                xComments[3] = $"AD{unitNumber}-CH.4変換完了ﾌﾗｸﾞ";
                xComments[4] = $"AD{unitNumber}-CH.1ﾚﾝｼﾞｴﾗｰ ﾌﾗｸﾞ";
                xComments[5] = $"AD{unitNumber}-CH.2ﾚﾝｼﾞｴﾗｰ ﾌﾗｸﾞ";
                xComments[6] = $"AD{unitNumber}-CH.3ﾚﾝｼﾞｴﾗｰ ﾌﾗｸﾞ";
                xComments[7] = $"AD{unitNumber}-CH.4ﾚﾝｼﾞｴﾗｰ ﾌﾗｸﾞ";
                xComments[12] = $"A/Dﾕﾆｯﾄ{unitNumber}EPRO書込ｴﾗｰﾌﾗｸﾞ";

                yComments[0] = $"AD{unitNumber}-CH.1移動平均処理指定ﾌﾗｸﾞ";
                yComments[1] = $"AD{unitNumber}-CH.2移動平均処理指定ﾌﾗｸﾞ";
                yComments[2] = $"AD{unitNumber}-CH.3移動平均処理指定ﾌﾗｸﾞ";
                yComments[3] = $"AD{unitNumber}-CH.4移動平均処理指定ﾌﾗｸﾞ";
            }
            else
            {
                xComments[8] = $"A/Dﾕﾆｯﾄ{unitNumber}ｲﾆｼｬﾙ 　ﾃﾞｰﾀ処理要求ﾌﾗｸﾞ";
                xComments[9] = $"A/Dﾕﾆｯﾄ{unitNumber}ｲﾆｼｬﾙ 　ﾃﾞｰﾀ設定完了ﾌﾗｸﾞ";
                xComments[10] = $"A/Dﾕﾆｯﾄ{unitNumber}ｴﾗｰ状態 ﾌﾗｸﾞ";
                xComments[11] = $"A/Dﾕﾆｯﾄ{unitNumber}ﾘﾓｰﾄﾚﾃﾞｨ";

                yComments[8] = $"A/Dﾕﾆｯﾄ{unitNumber}ｲﾆｼｬﾙ   ﾃﾞｰﾀ処理完了ﾌﾗｸﾞ";
                yComments[9] = $"A/Dﾕﾆｯﾄ{unitNumber}ｲﾆｼｬﾙ   ﾃﾞｰﾀ設定要求ﾌﾗｸﾞ";
                yComments[10] = $"A/Dﾕﾆｯﾄ{unitNumber}ｴﾗｰｸﾘｱ　要求";
            }

            for (int i = 0; i <= 15; i++)
            {
                var io = new IO
                {
                    Address = addressPrefix + (addressDec + i).ToString("X"),
                    XComment = xComments[i],
                    YComment = yComments[i],

                };
                result.Add(io);

            }

            return result;
        }
    }
}
