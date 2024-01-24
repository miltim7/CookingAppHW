const hamburger = document.querySelector(".hamburger");
const navLinks = document.querySelector(".nav-links");
const links = document.querySelectorAll(".nav-links li");

hamburger.addEventListener('click', () => {
    navLinks.classList.toggle("open");
    links.forEach(link => {
        link.classList.toggle("fade");
    });

    hamburger.classList.toggle("toggle");
});

function validateForm() {
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

    return true;
}

function handleEnterKeyPress(event, nextInputId) {
    if (event.key === "Enter") {
        event.preventDefault();
        document.getElementById(nextInputId).focus();
    }
}