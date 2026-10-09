document.addEventListener('DOMContentLoaded', () => {
    const csrfToken = document.querySelector('meta[name="csrf-token"]')?.content;

    document.querySelectorAll('.mark-done').forEach(btn => {
        btn.addEventListener('click', async () => {
            const id = btn.dataset.id;

            const response = await fetch(`/Tasks/Index?handler=Toggle&id=${id}`, {
                method: 'POST',
                headers: {
                    'X-CSRF-TOKEN': csrfToken || ''
                }
            });

            if (response.ok) {
                location.reload();
            } else if (response.status === 400) {
                alert('CSRF-токен отсутствует или неверный');
            } else {
                alert('Ошибка: ' + response.status);
            }
        });
    });
});