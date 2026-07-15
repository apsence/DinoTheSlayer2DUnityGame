using System.Globalization;
using System.Text;
using UnityEngine;

/// <summary>
/// Конвертирует упрощённую markdown-подобную разметку в TextMeshPro rich text теги.
/// Цвета и параметры инлайн-кода берутся из HintSettings, а не захардкожены.
///
/// Поддерживаемый синтаксис:
///   !!текст!!   — warning
///   ++текст++   — success
///   --текст--   — danger
///   **текст**   — bold
///   `код`       — инлайн-код: фон + моноширинный (как code-span у AI в чате)
///
/// Пример:
///   "Нажмите !!E!!, чтобы поднять `Item.Pickup()`"
/// </summary>
public static class HintTextFormatter
{
    public static string Format(string raw, HintSettings settings)
    {
        if (string.IsNullOrEmpty(raw)) return raw;

        if (settings == null)
        {
            Debug.LogError("[HintTextFormatter] HintSettings не задан — форматирование пропущено.");
            return raw;
        }

        string warning = ToHex(settings.warningColor);
        string success = ToHex(settings.successColor);
        string danger  = ToHex(settings.dangerColor);
        string codeBg   = ToHex(settings.codeBackground);
        string codeText = ToHex(settings.codeTextColor);
        string codeEm   = settings.codeMonospaceEm.ToString(CultureInfo.InvariantCulture) + "em";

        string result = raw;
        result = ReplaceWrapped(result, "!!", $"<color=#{warning}>", "</color>");
        result = ReplaceWrapped(result, "++", $"<color=#{success}>", "</color>");
        result = ReplaceWrapped(result, "--", $"<color=#{danger}>", "</color>");
        result = ReplaceWrapped(result, "**", "<b>", "</b>");
        result = ReplaceWrapped(result, "`",
            $"<mark=#{codeBg}><color=#{codeText}><mspace={codeEm}>",
            "</mspace></color></mark>");

        return result;
    }

    private static string ToHex(Color color) => ColorUtility.ToHtmlStringRGBA(color);

    /// <summary>
    /// Заменяет каждую пару вхождений marker на openTag/closeTag по очереди
    /// (первое вхождение — открывающее, второе — закрывающее, и так далее).
    /// </summary>
    private static string ReplaceWrapped(string input, string marker, string openTag, string closeTag)
    {
        var sb = new StringBuilder(input.Length);
        int i = 0;
        bool isOpen = false;

        while (i < input.Length)
        {
            if (i + marker.Length <= input.Length && input.Substring(i, marker.Length) == marker)
            {
                sb.Append(isOpen ? closeTag : openTag);
                isOpen = !isOpen;
                i += marker.Length;
            }
            else
            {
                sb.Append(input[i]);
                i++;
            }
        }

        if (isOpen) sb.Append(closeTag);

        return sb.ToString();
    }
}