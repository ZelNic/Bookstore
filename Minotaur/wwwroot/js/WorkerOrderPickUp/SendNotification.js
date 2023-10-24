
function SendNotification(url) {
    Swal.fire({
        title: 'Подтвердить прибытие посылки в пункт выдачи?',    
        text: 'Клиент получит уведомление о прибытие посылки.',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Да, подтвердить!',
        cancelButtonText: 'Отмена'
    }).then((result) => {
        if (result.isConfirmed) 
        {
            $.ajax({
                url: "/",
                type: 'POST'
            })

            Swal.fire(
                'Подтверждено!',
                'Пользователь получил уведомление',
                'success'
            )
        }
    })
}

