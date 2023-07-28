const navbarExpandLeft = document.getElementById("navbar-expand-lg-id");
const navbarExpandRight = document.getElementById("navbar-expand-rg-id");
const navbarExpandLeftWidth = navbarExpandLeft.clientWidth;
const navbarExpandRightWidth = navbarExpandRight.clientWidth;
const contentElement = document.getElementById("contentId");
contentElement.style.marginLeft = navbarExpandLeftWidth + "px";
contentElement.style.marginRight = navbarExpandRightWidth + "px";

const btnLeft = document.getElementById('btnLeftId');
btnLeft.addEventListener('click', function () {
    if (contentElement) {
        navbarExpandLeft.classList.toggle('collapsed');
        contentElement.classList.toggle('collapsed-left');
    }
});

const btnRight = document.getElementById('btnRightId');
btnRight.addEventListener('click', function () {
    if (contentElement) {
        navbarExpandRight.classList.toggle('collapsed');
        contentElement.classList.toggle('collapsed-right');
    }
});

document.getElementById('btnAllId').addEventListener('click', function () {
    const buttonLeft = document.getElementById('btnLeftId');
    const buttonRight = document.getElementById('btnRightId');
    //const contentFullScreen = document.querySelector('.content');
    buttonLeft.click();
    buttonRight.click();
/*    contentFullScreen.classList.toggle('collapsed');*/
});