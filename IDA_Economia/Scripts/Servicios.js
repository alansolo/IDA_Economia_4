window.addEventListener('load', () => {
    document.getElementById('intro1').classList.add('animate1')
    document.getElementById('intro2').classList.add('animate2')
})

const getScrollBarWidth = () => innerWidth - document.documentElement.clientWidth

document.documentElement.style.setProperty('--scroll-bar', getScrollBarWidth())

let services = Array.from(document.querySelectorAll('div._appear'));
let servicesScrollTop = services.map(service =>
    service.getBoundingClientRect().top - (innerHeight / 2)
)

console.log(servicesScrollTop)

window.addEventListener('scroll', () => {
    servicesScrollTop.forEach((el, i) => {
        if (scrollY >= el) {
            services[i].classList.add('animate')
        }
    })
});