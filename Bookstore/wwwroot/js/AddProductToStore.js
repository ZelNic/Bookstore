
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