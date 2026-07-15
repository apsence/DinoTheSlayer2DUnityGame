using UnityEngine;

[CreateAssetMenu(fileName = "HintSettings", menuName = "GUI/HintSettings")]
public class HintSettings : ScriptableObject
{
    [Header("Анимация")]
    public float fadeSpeed = 8f;

    [Header("Цвета форматирования текста (!! ++ --)")]
    public Color warningColor = new Color32(0xFF, 0xB0, 0x20, 0xFF);
    public Color successColor = new Color32(0x4C, 0xD9, 0x7B, 0xFF);
    public Color dangerColor  = new Color32(0xFF, 0x5C, 0x5C, 0xFF);

    [Header("Инлайн-код (`код`)")]
    public Color codeBackground = new Color32(0x33, 0x33, 0x33, 0x99); // альфа обязательна для <mark>
    public Color codeTextColor  = new Color32(0xF5, 0xF5, 0xF5, 0xFF);
    [Range(0.1f, 2f)] public float codeMonospaceEm = 0.55f;
}
