export function menuButtonInit(buttonNode, menuNode, objRef) {

    const callback = (event) => {
        if (!buttonNode.contains(event.target)) {
            if (buttonNode.getAttribute('aria-expanded') === 'true') {
                objRef.invokeMethodAsync('Close');
            }
        }
    };

    const buttonKeydown = (event) => {
        console.log('keydown: ', event.key);

        function stopEvent() {
            event.stopPropagation();
            event.preventDefault();
        }

        switch (event.key) {
            case ' ':
            case 'Enter':
                stopEvent();
                objRef.invokeMethodAsync('Open');
                break;
            case 'ArrowDown':
            case 'Down':
                stopEvent();
                objRef.invokeMethodAsync('OpenFirst');
                break;

            case 'Esc':
            case 'Escape':
                stopEvent();
                objRef.invokeMethodAsync('Close');
                break;

            case 'Up':
            case 'ArrowUp':
                stopEvent();
                objRef.invokeMethodAsync('OpenLast');
                break;

            default:
                break;
        }
    };
    const itemKeydown = (event) => {
        const key = event.key;

        function stopEvent() {
            event.stopPropagation();
            event.preventDefault();
        }

        if (event.ctrlKey || event.altKey || event.metaKey) {
            return;
        }

        if (event.shiftKey) {
            if (event.key === 'Tab') {
                stopEvent();
                objRef.invokeMethodAsync('Close');
            }
        } else {
            switch (key) {
                case ' ':
                case 'Enter':
                    stopEvent();
                    objRef.invokeMethodAsync('CloseAndPerformMenuAction');
                    break;

                case 'Esc':
                case 'Escape':
                    stopEvent();
                    objRef.invokeMethodAsync('Close');
                    break;

                case 'Up':
                case 'ArrowUp':
                case 'ArrowLeft':
                    stopEvent();
                    objRef.invokeMethodAsync('Previous');
                    break;

                case 'ArrowRight':
                case 'ArrowDown':
                case 'Down':
                    stopEvent();
                    objRef.invokeMethodAsync('Next');
                    break;

                case 'Home':
                case 'PageUp':
                    stopEvent();
                    objRef.invokeMethodAsync('First');
                    break;

                case 'End':
                case 'PageDown':
                    stopEvent();
                    objRef.invokeMethodAsync('Last');
                    break;

                case 'Tab':
                    objRef.invokeMethodAsync('Close');
                    break;

                default:
                    break;
            }
        }
    };

    window.addEventListener('mousedown', callback);
    buttonNode.addEventListener('keydown', buttonKeydown);
    menuNode.addEventListener('keydown', itemKeydown);

    return {
        install: () => {
            console.info('installing');
            window.addEventListener('mousedown', callback);
        },
        uninstall: () => {
            console.info('uninstalling');
            window.removeEventListener('mousedown', callback);
        },
        stop: () => {
            console.info('stopping');
            menuNode.removeEventListener('keydown', itemKeydown);
            buttonNode.removeEventListener('keydown', buttonKeydown);
            window.removeEventListener('mousedown', callback);
        }
    }
}