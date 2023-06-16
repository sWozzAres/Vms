window.menuButton = {
    // keeps track of 'onmousedown' event listeners added to the window so they
    // can later be removed.
    listeners: [],
    listenerId: 0,

    installBackgroundHook: function (id, objRef) {
        const element = document.querySelector('#' + id);
        if (!element)
            return -1;

        const buttonNode = element.querySelector('button');

        const listener = {
            listenerId: this.listenerId,
            
            callback: (event) => {
                //console.log('In eventlistener, element: ', element, ' contains: ', element.contains(event.target))
                if (!element.contains(event.target)) {
                    if (buttonNode.getAttribute('aria-expanded') === 'true') {
                        objRef.invokeMethodAsync('Close');
                    }
                }
            }
        };

        this.listeners.push(listener);

        window.addEventListener('mousedown', listener.callback);

        return this.listenerId++;
    },

    removeBackgroundHook: function (index) {
        const idx = this.listeners.findIndex(x => x.listenerId === index);
        if (idx === -1) {
            return false;
        }

        const listener = this.listeners[idx];

        window.removeEventListener('mousedown', listener.callback);

        this.listeners.splice(idx, 1);

        return true;
    },

    installOnMenuKeydown: function (id, objRef) {
        const element = document.querySelector('#' + id + ' ul[role="menu"]');
        console.log('installOnMenuKeydown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        element.addEventListener('keydown', (event) => {
            const key = event.key;

            function isPrintableCharacter(str) {
                return str.length === 1 && str.match(/\S/);
            }

            function stopEvent() {
                event.stopPropagation();
                event.preventDefault();
            }

            if (event.ctrlKey || event.altKey || event.metaKey) {
                return;
            }

            if (event.shiftKey) {
                if (isPrintableCharacter(key)) {
                    stopEvent();
                    objRef.invokeMethodAsync('setFocusByFirstCharacter', key);
                }

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
                        stopEvent();
                        objRef.invokeMethodAsync('Previous');
                        break;

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
                        if (isPrintableCharacter(key)) {
                            stopEvent();
                            objRef.invokeMethodAsync('setFocusByFirstCharacter', key);
                        }
                        break;
                }
            }
        })

        return true;
    },

    installKeydown: function (id, objRef) {
        const element = document.querySelector('#' + id);
        console.log('installKeydown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        if (!this.installOnMenuKeydown(id, objRef)) {
            console.log('installOnMenuKeydown failed');
        }

        element.addEventListener('keydown', (event) => {
            console.log('onButtonKeydown: ', event.key);

            function stopEvent() {
                event.stopPropagation();
                event.preventDefault();
            }

            switch (event.key) {
                case ' ':
                case 'Enter':
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
        });

        return true;
    }
};