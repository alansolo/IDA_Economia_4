document.getElementById('menuTrigger').addEventListener('click',
    () => {
        document.querySelector('section').classList.add('animate');
        document.getElementById('menu').classList.toggle('_menu-list-in')
    })

document.getElementById('closemenu').addEventListener('click',
    () => {
        document.querySelector('section').classList.toggle('animate');
        document.getElementById('menu').classList.toggle('_menu-list-in')
    })