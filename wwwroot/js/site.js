const hamburger = document.querySelector(".hamburger");
const navLinks = document.querySelector(".nav-links");
const links = document.querySelectorAll(".nav-links li");
const goBack = document.querySelector('.go-back');

goBack.addEventListener('click', () => {
    window.history.back();
})

hamburger.addEventListener('click', () => {
    navLinks.classList.toggle("open");
    links.forEach(link => {
        link.classList.toggle("fade");
    });

    hamburger.classList.toggle("toggle");
});

function validateCreateForm() {
    var titleValue = document.querySelector('#title').value;
    var descriptionValue = document.querySelector('#description').value;
    var categoryValue = document.querySelector('#category').value;
    var priceValue = document.querySelector('#price').value;

    if (titleValue === null || titleValue.trim() === '') {
        alert('Fill the "Title" Field!');
        return false;
    }

    if (descriptionValue === null || descriptionValue.trim() === '') {
        alert('Fill the "Description" field');
        return false;
    }

    if (categoryValue === null || categoryValue.trim() === '') {
        alert('Fill the "Category" field');
        return false;
    }

    if (priceValue === null || priceValue.trim() === '') {
        alert('Fill the "Price" field');
        return false;
    }

    if (priceValue < 0) {
        alert('Price can not be negative nubmer!');
        return false;
    }

    return true;
}

function validateLoginForm() {
    var login = document.querySelector('#login').value;
    var password = document.querySelector('#password').value;

    if (login === 'unauthorized') {
        alert('Login can not be "unauthorized"!');
        return false;
    }
    
    if (login === null || login.trim() === '') {
        alert('Foll the "Login" field!');
        return false;
    }

    if (password === null || password.trim() === '') {
        alert('Foll the "Password" field!');
        return false;
    }

    return true;
}