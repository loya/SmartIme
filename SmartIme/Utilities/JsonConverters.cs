using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartIme.Utilities
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string colorString = reader.GetString();

                // 尝试解析十六进制颜色值 (#AARRGGBB 或 #RRGGBB)
                if (colorString.StartsWith("#"))
                {
                    try
                    {
                        // 确保是8位十六进制格式 (#AARRGGBB 或 #RRGGBB)
                        if (colorString.Length == 7) // #RRGGBB
                        {
                            colorString = "#FF" + colorString.Substring(1); // 添加不透明度
                        }

                        if (colorString.Length == 9) // #AARRGGBB
                        {
                            int argbValue = Convert.ToInt32(colorString.Substring(1), 16);
                            return Color.FromArgb(argbValue);
                        }
                    }
                    catch
                    {
                        // 如果解析失败，返回黑色
                        return Color.Black;
                    }
                }

                // 尝试解析已知的颜色名称
                try
                {
                    return ColorTranslator.FromHtml(colorString);
                }
                catch
                {
                    return Color.Black;
                }
            }

            // 如果不是字符串类型，返回默认颜色
            return Color.Black;
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            // 将颜色值写为十六进制字符串格式
            // string hexColor = $"#{value.A:X2}{value.R:X2}{value.G:X2}{value.B:X2}";
            // writer.WriteStringValue(hexColor);

            writer.WriteStringValue(ColorTranslator.ToHtml(value));
        }
    }

    public class FontJsonConverter : JsonConverter<Font>
    {
        public override Font Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string fontString = reader.GetString();

                try
                {
                    // 使用FontConverter将字符串转换为Font对象
                    var converter = new FontConverter();
                    var font = converter.ConvertFromString(fontString);
                    if (font != null)
                    {
                        return (Font)font;
                    }
                    else
                    {
                        return new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
                    }
                }
                catch
                {
                    // 如果转换失败，返回默认字体
                }
            }

            // 如果不是字符串类型或转换失败，返回默认字体
            return new Font("微软雅黑", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
        }

        public override void Write(Utf8JsonWriter writer, Font value, JsonSerializerOptions options)
        {
            if (value != null)
            {
                // 使用FontConverter将Font对象转换为字符串
                var converter = new FontConverter();
                // var fontString = converter.ConvertToString(value);
                var fontString = (string)converter.ConvertTo(value, typeof(string));

                if (!string.IsNullOrEmpty(fontString))
                {
                    writer.WriteStringValue(fontString);
                }
                else
                {
                    writer.WriteStringValue("Microsoft YaHei, 9pt");
                }
            }
            else
            {
                writer.WriteStringValue("Microsoft YaHei, 9pt");
            }
        }
    }

    public class EnumJsonConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                // 解析为字符串
                string enumString = reader.GetString();
                if (Enum.TryParse<T>(enumString, true, out T result))
                {
                    return result;
                }

                // 如果字符串解析失败，尝试解析为数字
                if (int.TryParse(enumString, out int numericValue))
                {
                    if (Enum.IsDefined(typeof(T), numericValue))
                    {
                        return (T)(object)numericValue;
                    }
                }
            }
            else if (reader.TokenType == JsonTokenType.Number)
            {
                int numericValue = reader.GetInt32();
                if (Enum.IsDefined(typeof(T), numericValue))
                {
                    return (T)(object)numericValue;
                }
            }

            // 如果解析失败，返回默认值（通常是枚举的第一个值）
            return default(T);
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            // 将枚举值写为字符串格式
            writer.WriteStringValue(value.ToString());
        }
    }

}