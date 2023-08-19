
function SendNotification(url) {
    Swal.fire({
        title: 'Отправить уведомление о прибытие посылки?',        
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Да, отправить!',
        cancelButtonText: 'Отмена'
    }).then((result) => {
        if (result.isConfirmed) 
        {
            $.ajax({
                url: url,
                type: 'POST'
            })

            Swal.fire(
                'Отправлено!',
                'Пользователь получил уведомление',
                'success'
            )
        }
    })
}

