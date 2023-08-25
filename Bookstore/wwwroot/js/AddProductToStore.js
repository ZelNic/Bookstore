const bdBook = document.


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


//document.getElementById("myForm").addEventListener("submit", function (event) {
//    event.preventDefault();

//    var title = document.getElementById("searchTitle").value;
//    var id = document.getElementById("searchId").value;
//    var result = document.getElementById("result").value;

//    result = findProduct(title, id);
//});

//function findProduct(title, id) {
//    fetch(`/Stockkeeper/Stock/GetProductAsync?nameProduct=${title}&productId=${id}`, { method: 'POST' })
//        .then(response => {
//        })
//        .catch(error => {
//        });
//}




//fetch('/your-server-url')
//    .then(response => response.json())
//    .then(results => {
//        // Ваш код для отображения результатов в HTML
//        const resultsContainer = document.getElementById('results');
//        results.forEach(result => {
//            const resultElement = document.createElement('p');
//            resultElement.textContent = result;
//            resultsContainer.appendChild(resultElement);
//        });
//    })
//    .catch(error => {
//        console.error('Ошибка:', error);
//    });
