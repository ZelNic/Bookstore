
function showConfirmation() {

    var userInput = prompt("Введите код получения", "");

    userInput = userInput.replace(/\D/g, "");

    if (userInput === "") {
        alert("Вы не ввели значение!");
    } else {
        if (code == userInput) {
            alert("Код верный");
        }
        else {
            alert("Неверный код");
        }
    }

}

