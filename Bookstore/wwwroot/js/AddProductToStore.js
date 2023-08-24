
const openSearch = document.getElementById('openSearch');
const searchContainer = document.querySelector('.hidden');
const hide = document.getElementById('hide');

openSearch.addEventListener('click', function () {
    searchContainer.classList.remove('hidden');
    searchContainer.classList.add('visible');
    hide.classList.add('visible');
});

hide.addEventListener('click', function () {
    searchContainer.classList.remove('visible');
    searchContainer.classList.add('hidden');
    hide.classList.remove('visible');
});


document.getElementById("myForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Предотвращение обычного поведения отправки формы

    // Получение значений полей формы
    var title = document.getElementById("searchTitle").value;
    var id = document.getElementById("searchId").value;
    var result = document.getElementById("result").value;
    
    // Вызов функции для отправки запроса на сервер и обработки результата
    result = findProduct(title, id);
});

function findProduct(title, id) {
    fetch(`/Stockkeeper/Stock/GetProductAsync?nameProduct=${title}&productId=${id}`)
        .then(response => {
            // Обработка ответа от сервера
        })
        .catch(error => {
            // Обработка ошибки
        });
}

fetch('/your-server-url')
    .then(response => response.json())
    .then(results => {
        // Ваш код для отображения результатов в HTML
        const resultsContainer = document.getElementById('results');
        results.forEach(result => {
            const resultElement = document.createElement('p');
            resultElement.textContent = result;
            resultsContainer.appendChild(resultElement);
        });
    })
    .catch(error => {
        console.error('Ошибка:', error);
    });
