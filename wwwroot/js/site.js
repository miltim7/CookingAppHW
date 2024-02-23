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

function toggleChangePasswordForm() {
    var changePasswordForm = document.querySelector("#changePasswordForm");
    changePasswordForm.style.display = "flex";

    var profileForm = document.querySelector("#profileForm");
    profileForm.style.display = "none";

    var changePasswordButton = document.querySelector('#changePasswordButton');
    changePasswordButton.style.display = "none";

    var goBackButton = document.querySelector('.go-back');
    goBackButton.style.display = "none";

    var cancelChangePasswordButton = document.querySelector('.cancel-change-password');
    cancelChangePasswordButton.style.display = "inline-block";

    cancelChangePasswordButton.addEventListener('click', () => {
        profileForm.style.display = "flex";
        changePasswordForm.style.display = 'none';

        goBackButton.style.display = "inline-block";
        cancelChangePasswordButton.style.display = "none";

        changePasswordButton.style.display = "flex";
    })
}

function changePassword(password) {
    var currentPassword = document.querySelector("#currentPassword").value;
    var newPassword = document.querySelector("#newPassword").value;
    var confirmNewPassword = document.querySelector("#confirmNewPassword").value;

    if (currentPassword.trim() === "" || currentPassword === null ||
        newPassword.trim() === "" || newPassword === null ||
        confirmNewPassword.trim() === "" || confirmNewPassword === null) {
        alert("Fields can not be empty!");
        return false;
    }

    if (currentPassword !== password) {
        alert("Old Password is incorrect!");
        return false;
    }

    if (newPassword !== confirmNewPassword) {
        alert("New passwords don't match");
        return false;
    }

    if (newPassword === currentPassword) {
        alert("New password is the same as old");
        return false;
    }

    return true;
}