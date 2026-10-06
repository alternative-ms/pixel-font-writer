using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI; // если нужно вывести на UI Canvas

public class WebGLImageLoader : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void UploadFileToWebGL(string objectName, string callbackName);

    [Header("Куда применить текстуру?")]
    public Renderer targetRenderer; // Для 3D объекта (например, куба)
    public RawImage targetRawImage;   // Для UI интерфейса

    // Метод, который нужно повесить на кнопку "Загрузить PNG" в Unity
    public void RequestImageFromBrowser()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
            // Передаем имя ЭТОГО объекта и имя метода-колбэка
            UploadFileToWebGL(gameObject.name, "OnImageLoadedFromBrowser");
#else
        Debug.LogWarning("Загрузка файлов через jslib работает только в WebGL билде!");
        // Тут можно сделать костыль для редактора через UnityEditor.EditorUtility.OpenFilePanel, если очень нужно
#endif
    }

    // Этот метод автоматически вызовется из JavaScript, когда пользователь выберет файл
    public void OnImageLoadedFromBrowser(string base64DataUri)
    {
        if (string.IsNullOrEmpty(base64DataUri)) return;

        try
        {
            // Строка base64 от браузера идет с префиксом "data:image/png;base64,", его нужно отрезать
            string base64Data = base64DataUri.Split(',')[1];
            byte[] imageBytes = Convert.FromBase64String(base64Data);

            // Создаем пустую текстуру (размер 2х2 изменится автоматически при загрузке данных)
            Texture2D loadedTexture = new Texture2D(2, 2, TextureFormat.RGBA32, false);

            // ImageConversion загружает PNG/JPG байты и сохраняет альфа-канал
            if (ImageConversion.LoadImage(loadedTexture, imageBytes))
            {
                ApplyTexture(loadedTexture);
            }
            else
            {
                Debug.LogError("Не удалось сконвертировать байты в текстуру.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка при обработке изображения: {e.Message}");
        }
    }

    private void ApplyTexture(Texture2D texture)
    {
        // Применяем на 3D объект (например, на MeshRenderer)
        if (targetRenderer != null)
        {
            targetRenderer.material.mainTexture = texture;
        }

        // Или на UI элемент RawImage
        if (targetRawImage != null)
        {
            targetRawImage.texture = texture;
        }
    }
}