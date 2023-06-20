window.jsInterop = {
    setFocus: function (id) {
        const element = document.getElementById(id);
        if (!element)
            return false;

        element.focus();
        return true;
    }
}

window.searchBox = {
    installOnKeydownHandler: function (id, objRef) {
        const element = document.getElementById(id);
        console.log('searchBox.installOnKeydownHandler() id: ', id, ' element: ', element, ' objRef: ', objRef);

        if (!element)
            return false;

        element.addEventListener('keydown', (event) => {

            function stopEvent() {
                event.stopPropagation();
                event.preventDefault();
            }

            switch (event.key) {

                case 'ArrowDown':
                    stopEvent();
                    objRef.invokeMethodAsync('Next');
                    break;
                case 'ArrowUp':
                    stopEvent();
                    objRef.invokeMethodAsync('Prev');
                    break;
            }
        });

        return true;
    }
}
window.dropdownButton = {
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

    installOnDropdownItemKeydown: function (id, objRef) {
        const element = document.querySelector('#' + id + ' ul[role="menu"]');
        console.log('installOnDropdownItemKeydown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        element.addEventListener('keydown', (event) => {
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
        })

        return true;
    },

    installOnDropdownButtonKeydown: function (id, objRef) {
        const element = document.getElementById(id);
        console.log('installOnDropdownButtonKeydown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        if (!this.installOnDropdownItemKeydown(id, objRef)) {
            console.log('installOnDropdownItemKeydown failed');
            return false;
        }

        element.addEventListener('keydown', (event) => {
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
        });

        return true;
    }
};

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

    maintainScrollVisibility: function (activeElement, scrollParent) {
        const { offsetHeight, offsetTop } = activeElement;
        const { offsetHeight: parentOffsetHeight, scrollTop } = scrollParent;

        const isAbove = offsetTop < scrollTop;
        const isBelow = offsetTop + offsetHeight > scrollTop + parentOffsetHeight;

        if (isAbove) {
            scrollParent.scrollTo(0, offsetTop);
        } else if (isBelow) {
            scrollParent.scrollTo(0, offsetTop - parentOffsetHeight + offsetHeight);
        }
    },

    installOnComboKeyDown: function (id, objRef) {
        const element = document.querySelector('#' + id);
        console.log('installOnComboKeyDown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        const SelectActions = {
            Close: 0,
            CloseSelect: 1,
            First: 2,
            Last: 3,
            Next: 4,
            Open: 5,
            PageDown: 6,
            PageUp: 7,
            Previous: 8,
            Select: 9,
            Type: 10,
        };

        element.addEventListener('keydown', (event) => {

            const { key } = event;

            var aad = element.getAttribute('aria-activedescendant');

            const action = getActionFromKey(event, aad.length > 0);

            console.log('keydown ', event.key, ' action ', action, ' aad ', aad);

            switch (action) {
                case SelectActions.Last:
                case SelectActions.First:
                    objRef.invokeMethodAsync("updateMenuStateJS", true);
                //this.updateMenuState(true);
                // intentional fallthrough
                case SelectActions.Next:
                case SelectActions.Previous:
                case SelectActions.PageUp:
                case SelectActions.PageDown:
                    event.preventDefault();
                    objRef.invokeMethodAsync("onOptionChangeJS", action);

                    const activeElement = element.getAttribute('aria-activedescendant');
                    const el = document.getElementById(activeElement);
                    el.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                    return;
                //return this.onOptionChange(
                //    getUpdatedIndex(this.activeIndex, max, action)
                //);
                case SelectActions.CloseSelect:
                    event.preventDefault();
                    objRef.invokeMethodAsync("selectOptionJS");

                //this.selectOption(this.activeIndex);
                // intentional fallthrough
                case SelectActions.Close:
                    event.preventDefault();
                    //return this.updateMenuState(false);
                    objRef.invokeMethodAsync("updateMenuStateJS", false);
                    return;
                case SelectActions.Type:
                    objRef.invokeMethodAsync("onComboTypeJS", key);
                    return;
                //return this.onComboType(key);
                case SelectActions.Open:
                    event.preventDefault();
                    //return this.updateMenuState(true);
                    objRef.invokeMethodAsync("updateMenuStateJS", true);
                    return;
            }


            function getActionFromKey(event, menuOpen) {
                const { key, altKey, ctrlKey, metaKey } = event;
                const openKeys = ['ArrowDown', 'ArrowUp', 'Enter', ' ']; // all keys that will do the default open action
                // handle opening when closed
                if (!menuOpen && openKeys.includes(key)) {
                    return SelectActions.Open;
                }

                // home and end move the selected option when open or closed
                if (key === 'Home') {
                    return SelectActions.First;
                }
                if (key === 'End') {
                    return SelectActions.Last;
                }

                // handle typing characters when open or closed
                if (
                    key === 'Backspace' ||
                    key === 'Clear' ||
                    (key.length === 1 && key !== ' ' && !altKey && !ctrlKey && !metaKey)
                ) {
                    return SelectActions.Type;
                }

                // handle keys when open
                if (menuOpen) {
                    if (key === 'ArrowUp' && altKey) {
                        return SelectActions.CloseSelect;
                    } else if (key === 'ArrowDown' && !altKey) {
                        return SelectActions.Next;
                    } else if (key === 'ArrowUp') {
                        return SelectActions.Previous;
                    } else if (key === 'PageUp') {
                        return SelectActions.PageUp;
                    } else if (key === 'PageDown') {
                        return SelectActions.PageDown;
                    } else if (key === 'Escape') {
                        return SelectActions.Close;
                    } else if (key === 'Enter' || key === ' ') {
                        return SelectActions.CloseSelect;
                    }
                }
            }
        });

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

    installOnMenuButtonKeydown: function (id, objRef) {
        const element = document.querySelector('#' + id);
        console.log('installOnMenuButtonKeydown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        if (!this.installOnMenuKeydown(id, objRef)) {
            console.log('installOnMenuKeydown failed');
            return false;
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