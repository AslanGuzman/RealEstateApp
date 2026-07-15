window.addEventListener('load', function () {
    var toasts = document.querySelectorAll('.app-toast');
    toasts.forEach(function (toast) {
        setTimeout(function () {
            toast.style.transition = 'opacity 0.4s ease';
            toast.style.opacity = '0';
            setTimeout(function () { toast.remove(); }, 400);
        }, 4000);
    });

    document.querySelectorAll('[data-chat-scroll]').forEach(function (box) {
        box.scrollTop = box.scrollHeight;
    });

    var photoInput = document.querySelector('[data-photo-input]');
    var photoPreview = document.querySelector('[data-photo-preview]');
    var photoPlaceholder = document.querySelector('[data-photo-placeholder]');
    var photoName = document.querySelector('[data-photo-name]');

    if (photoInput && photoPreview && photoPlaceholder) {
        var defaultName = photoName ? photoName.textContent : '';

        photoInput.addEventListener('change', function () {
            var file = photoInput.files && photoInput.files[0];

            if (!file || !file.type.startsWith('image/')) {
                photoPreview.classList.add('d-none');
                photoPlaceholder.classList.remove('d-none');
                if (photoName) {
                    photoName.textContent = defaultName;
                    photoName.title = '';
                }
                return;
            }

            if (photoName) {
                photoName.textContent = file.name;
                photoName.title = file.name;
            }

            var reader = new FileReader();
            reader.onload = function (event) {
                photoPreview.src = event.target.result;
                photoPreview.classList.remove('d-none');
                photoPlaceholder.classList.add('d-none');
            };
            reader.readAsDataURL(file);
        });
    }
});
