const myElement = document.getElementById("navbar-expand-rg-id");
const myElementWidth = myElement.clientWidth;
const content = document.getElementById("contentId");
content.style.marginRight = myElementWidth + "px";

const buttons = document.querySelectorAll('.btn');
buttons.forEach(button => {
    button.addEventListener('click', function () {
        const targetId = this.dataset.target;
        const targetElement = document.getElementById(targetId);

        if (targetElement) {
            targetElement.classList.toggle('collapsed');
        }
    });
});

document.getElementById('btnAllId').addEventListener('click', function () {
    const buttonLeft = document.getElementById('btnLeftId');
    const buttonRight = document.getElementById('btnRightId');
    buttonLeft.click();
    buttonRight.click();
});
