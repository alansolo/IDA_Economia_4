const getScrollBarWidth = () => innerWidth - document.documentElement.clientWidth

document.documentElement.style.setProperty('--scroll-bar', getScrollBarWidth())

let services = Array.from(document.querySelectorAll('section'));
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