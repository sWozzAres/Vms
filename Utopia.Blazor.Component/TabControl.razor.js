export function tabControlInit(element, objRef) {
    const tabsId = element.getAttribute('aria-labelledby');

    //console.debug(tabsId, ' tabControlInit: ', element, objRef);

    const keyDownHandler = event => {

        const stopEvent = () => {
            event.stopPropagation();
            event.preventDefault();
        };

        const focusTabByIndex = (index) => {
            const id = tabsId + "-tab-" + index;
            document.getElementById(id).focus();
        };

        switch (event.key) {
            case 'ArrowLeft':
                stopEvent();
                objRef.invokeMethodAsync("moveFocusToPreviousTabJS").then((index) => {
                    focusTabByIndex(index);
                });
                break;

            case 'ArrowRight':
                stopEvent();
                objRef.invokeMethodAsync("moveFocusToNextTabJS").then((index) => {
                    focusTabByIndex(index);
                });
                break;

            case 'Home':
                stopEvent();
                objRef.invokeMethodAsync("moveFocusToFirstTabJS").then((index) => {
                    focusTabByIndex(index);
                });
                break;

            case 'End':
                stopEvent();
                objRef.invokeMethodAsync("moveFocusToLastTabJS").then((index) => {
                    focusTabByIndex(index);
                });
                break;

            default:
                break;
        }
    };

    element.addEventListener('keydown', keyDownHandler);

    return {
        stop: () => {
            element.removeEventListener('keydown', keyDownHandler);
        }
    }
}