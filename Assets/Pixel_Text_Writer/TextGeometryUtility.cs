using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public static class TextGeometryUtility
{
    public static Vector2 GetExactPixelSize(Text textComponent)
    {
        if (textComponent == null || string.IsNullOrEmpty(textComponent.text))
            return Vector2.zero;

        // Берем геометрию, сгенерированную для отрисовки на Canvas
        TextGenerator tg = textComponent.cachedTextGenerator;
        var vertices = tg.verts;

        if (vertices.Count == 0)
            return Vector2.zero;

        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;

        // Проходим по всем вершинам меша текста и находим крайние точки
        for (int i = 0; i < vertices.Count; i++)
        {
            Vector3 pos = vertices[i].position;
            if (pos.x < minX) minX = pos.x;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.y > maxY) maxY = pos.y;
        }

        float exactWidth = maxX - minX;
        float exactHeight = maxY - minY;

        // Округляем до целых пикселей, так как это пиксель-арт
        return new Vector2(Mathf.Round(exactWidth), Mathf.Round(exactHeight));
    }
}