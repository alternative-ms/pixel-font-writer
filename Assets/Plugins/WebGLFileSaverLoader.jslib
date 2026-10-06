mergeInto(LibraryManager.library, {
    DownloadFileFromWebGL: function (arrayPtr, size, fileNamePtr) {
        var fileName = UTF8ToString(fileNamePtr);
        var bytes = new Uint8Array(HEAPU8.buffer, arrayPtr, size);
        
        var blob = new Blob([bytes], { type: "image/png" });
        var link = document.createElement('a');
        link.href = window.URL.createObjectURL(blob);
        link.download = fileName;
        
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    },

    UploadFileToWebGL: function (objectNamePtr, callbackNamePtr) {
        var objectName = UTF8ToString(objectNamePtr);
        var callbackName = UTF8ToString(callbackNamePtr);

        // Ищем или создаем невидимый input для выбора файла
        var fileInput = document.getElementById('UnityWebGLFileInput');
        if (!fileInput) {
            fileInput = document.createElement('input');
            fileInput.id = 'UnityWebGLFileInput';
            fileInput.type = 'file';
            fileInput.accept = 'image/png'; // принимаем только PNG
            fileInput.style.display = 'none';
            document.body.appendChild(fileInput);
        }

        fileInput.onchange = function (event) {
            var file = event.target.files[0];
            if (!file) return;

            var reader = new FileReader();
            reader.onload = function (e) {
                // Передаем base64 строку обратно в Unity
                SendMessage(objectName, callbackName, e.target.result);
                // Очищаем инпут для возможности повторного выбора того же файла
                fileInput.value = ''; 
            };
            reader.readAsDataURL(file);
        };

        fileInput.click();
    }
});