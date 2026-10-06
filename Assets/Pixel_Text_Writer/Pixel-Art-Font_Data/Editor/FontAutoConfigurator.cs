using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FontAutoConfigurator : EditorWindow
{
    private Font targetFont;
    private Texture2D fontTexture;
    private string symbolOrder = "ABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789-!.,':?/%|hx";

    [MenuItem("Tools/Font Auto Configurator")]
    public static void ShowWindow()
    {
        GetWindow<FontAutoConfigurator>("Font Config");
    }

    private void OnGUI()
    {
        GUILayout.Label("Автоматическая настройка кастомного шрифта", EditorStyles.boldLabel);

        targetFont = (Font)EditorGUILayout.ObjectField("Объект Font (шрифт)", targetFont, typeof(Font), false);
        fontTexture = (Texture2D)EditorGUILayout.ObjectField("Текстура шрифта", fontTexture, typeof(Texture2D), false);
        symbolOrder = EditorGUILayout.TextField("Порядок символов в ряд", symbolOrder);

        if (GUILayout.Button("Рассчитать и заполнить символы"))
        {
            if (targetFont == null || fontTexture == null)
            {
                Debug.LogError("Заполните поля шрифта и текстуры!");
                return;
            }
            ProcessFont();
        }
    }

    private void ProcessFont()
    {
        int texWidth = fontTexture.width;
        int texHeight = fontTexture.height;
        Color32[] pixels = fontTexture.GetPixels32();

        List<CharacterInfo> charList = new List<CharacterInfo>();
        int currentSymbolIndex = 0;
        bool inCharacter = false;
        int startX = 0;

        // Попиксельное сканирование текстуры слева направо
        for (int x = 0; x < texWidth; x++)
        {
            bool hasPixel = false;

            // Проверяем всю вертикальную линию x на наличие непрозрачных пикселей
            for (int y = 0; y < texHeight; y++)
            {
                if (pixels[y * texWidth + x].a > 10) // Если пиксель не прозрачный
                {
                    hasPixel = true;
                    break;
                }
            }

            if (!inCharacter && hasPixel)
            {
                // Наткнулись на начало нового символа
                inCharacter = true;
                startX = x;
            }
            else if (inCharacter && (!hasPixel || x == texWidth - 1))
            {
                // Наткнулись на пустоту — символ закончился
                inCharacter = false;
                int endX = x;
                int charWidth = endX - startX;

                if (currentSymbolIndex < symbolOrder.Length)
                {
                    char symbol = symbolOrder[currentSymbolIndex];
                    CharacterInfo info = new CharacterInfo();

                    info.index = (int)symbol;

                    // Расширяем пиксельные границы на 1 пиксель влево и вправо для отступа
                    // Используем Mathf.Clamp, чтобы не выйти за пределы краев самой текстуры
                    int paddedStartX = Mathf.Max(0, startX - 1);
                    int paddedEndX = Mathf.Min(texWidth, endX + 1);
                    int paddedWidth = paddedEndX - paddedStartX;

                    // 1. UV-координаты теперь берутся с учетом защитного отступа в 1 пиксель
                    info.uvBottomLeft = new Vector2((float)paddedStartX / texWidth, 0f);
                    info.uvBottomRight = new Vector2((float)paddedEndX / texWidth, 0f);
                    info.uvTopLeft = new Vector2((float)paddedStartX / texWidth, 1f);
                    info.uvTopRight = new Vector2((float)paddedEndX / texWidth, 1f);

                    // 2. Геометрия Vert (размер полигона на экране)
                    info.minX = 0;
                    info.maxX = paddedWidth;
                    info.minY = -texHeight;
                    info.maxY = 0;

                    // 3. Шаг смещения равен полной ширине с учетом отступов
                    info.advance = paddedWidth;

                    charList.Add(info);
                    currentSymbolIndex++;
                }
            }
        }

        // Записываем полученные данные в выбранный Font
        Undo.RecordObject(targetFont, "Auto Font Setup");
        targetFont.characterInfo = charList.ToArray();
        EditorUtility.SetDirty(targetFont);

        Debug.Log($"Успешно настроено символов: {charList.Count} из {symbolOrder.Length}");
    }
}