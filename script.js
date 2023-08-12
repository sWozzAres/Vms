var observer = new IntersectionObserver(function (entries) {
	// no intersection with screen
	if (entries[0].intersectionRatio === 0)
		document.querySelector("#nav-container").classList.add("nav-container-sticky");
	// fully intersects with screen
	else if (entries[0].intersectionRatio === 1)
		document.querySelector("#nav-container").classList.remove("nav-container-sticky");
}, { threshold: [0, 1] });

observer.observe(document.querySelector("#nav-container-top"));
