using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class WebGLImageSaver : MonoBehaviour
{
    // Импортируем функцию из нашего jslib плагина
    [DllImport("__Internal")]
    private static extern void DownloadFileFromWebGL(IntPtr arrayPtr, int size, string fileName);

    [SerializeField]
    private RenderTexture _renderTextureToSave;

    public void SaveRenderTextureToPNG(string fileNameWithoutExtension)
    {
        if (_renderTextureToSave == null)
        {
            Debug.LogError("RenderTexture не назначена!");
            return;
        }

        // 1. Создаем Texture2D нужного размера с поддержкой альфа-канала (RGBA32)
        Texture2D texture2D = new Texture2D(_renderTextureToSave.width, _renderTextureToSave.height, TextureFormat.RGBA32, false);

        // 2. Сохраняем текущую активную RenderTexture, чтобы вернуть её позже
        RenderTexture previousActive = RenderTexture.active;
        RenderTexture.active = _renderTextureToSave;

        // 3. Читаем пиксели из RenderTexture в Texture2D
        texture2D.ReadPixels(new Rect(0, 0, _renderTextureToSave.width, _renderTextureToSave.height), 0, 0);
        texture2D.Apply();

        // 4. Возвращаем активную RenderTexture на место
        RenderTexture.active = previousActive;

        // 5. Кодируем текстуру в PNG (прозрачность сохраняется)
        byte[] pngBytes = texture2D.EncodeToPNG();

        // Очищаем созданную текстуру из памяти RAM
        Destroy(texture2D);

        // 6. Отправляем байты в браузер (работает только в WebGL сборке)
#if UNITY_WEBGL && !UNITY_EDITOR
            GCHandle pinnedArray = GCHandle.Alloc(pngBytes, GCHandleType.Pinned);
            IntPtr pointer = pinnedArray.AddrOfPinnedObject();
            
            DownloadFileFromWebGL(pointer, pngBytes.Length, fileNameWithoutExtension + ".png");
            
            pinnedArray.Free();
#else
        // Альтернатива для тестирования в Редакторе Unity (сохранит в корень проекта)
        System.IO.File.WriteAllBytes(Application.dataPath + "/../" + fileNameWithoutExtension + ".png", pngBytes);
        Debug.Log("Файл сохранен локально (Editor): " + fileNameWithoutExtension + ".png");
#endif
    }
}