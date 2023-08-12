export function init() {
	console.info('PageHeader init');

	var observer = new IntersectionObserver(function (entries) {
		console.info('intersect');

		// no intersection with screen
		if (entries[0].intersectionRatio === 0)
			document.querySelector(".page-header").classList.add("nav-container-sticky");
		// fully intersects with screen
		else if (entries[0].intersectionRatio === 1)
			document.querySelector(".page-header").classList.remove("nav-container-sticky");
	}, { threshold: [0, 1] });

	observer.observe(document.querySelector("#nav-container-top"));

	return {
		stop: () => {
			observer.disconnect();
		}
	}
}