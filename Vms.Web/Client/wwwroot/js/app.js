window.myApp = {
    preventDefaultForKeys: function (event, keys) {
        if (keys.includes(event.key)) {
            event.preventDefault();
        }
    },

    preventDefaultAndStopPropagation: function (event) {
        console.log('preventDefaultAndStopPropagation: ', event);

        event.preventDefault();
        event.stopPropagation();
    },

    installOnMenuKeydown: function (id, objRef) {
        const element = document.querySelector('#' + id + ' ul[role="menu"]');
        console.log('installOnMenuKeydown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        element.addEventListener('keydown', (event) => {
            const key = event.key;
            //let flag = false;

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
                    objRef.invokeMethodAsync('setFocusByFirstCharacter', key).then(() => {
                        stopEvent();
                    });
                    //this.setFocusByFirstCharacter(key);
                    //flag = true;
                }

                if (event.key === 'Tab') {
                    objRef.invokeMethodAsync('Close').then(() => {
                        stopEvent();
                    });
                    //this.closePopup();
                    //flag = true;
                }
            } else {
                switch (key) {
                    case ' ':
                    case 'Enter':
                        objRef.invokeMethodAsync('CloseAndPerformMenuAction').then(() => {
                            stopEvent();
                        });
                        //this.closePopup();
                        //this.performMenuAction(this.currentMenuitem);
                        //flag = true;
                        break;

                    case 'Esc':
                    case 'Escape':
                        objRef.invokeMethodAsync('Close').then(() => {
                            stopEvent();
                        });
                        //this.closePopup();
                        //flag = true;
                        break;

                    case 'Up':
                    case 'ArrowUp':
                        objRef.invokeMethodAsync('Previous').then(() => {
                            stopEvent();
                        });
                        //this.setFocusToPreviousMenuitem();
                        //flag = true;
                        break;

                    case 'ArrowDown':
                    case 'Down':
                        objRef.invokeMethodAsync('Next').then(() => {
                            stopEvent();
                        });
                        //this.setFocusToNextMenuitem();
                        //flag = true;
                        break;

                    case 'Home':
                    case 'PageUp':
                        objRef.invokeMethodAsync('First').then(() => {
                            stopEvent();
                        });
                        //this.setFocusToFirstMenuitem();
                        //flag = true;
                        break;

                    case 'End':
                    case 'PageDown':
                        objRef.invokeMethodAsync('Last').then(() => {
                            stopEvent();
                        });
                        //this.setFocusToLastMenuitem();
                        //flag = true;
                        break;

                    case 'Tab':
                        objRef.invokeMethodAsync('Close');
                        //this.closePopup();
                        break;

                    default:
                        if (isPrintableCharacter(key)) {
                            objRef.invokeMethodAsync('setFocusByFirstCharacter', key).then(() => {
                                stopEvent();
                            });
                            //this.setFocusByFirstCharacter(key);
                            //flag = true;
                        }
                        break;
                }
            }

            //if (flag) {
            //    event.stopPropagation();
            //    event.preventDefault();
            //}
        })

        return true;
    },

    installKeydown: function (id, objRef) {
        const element = document.querySelector('#' + id);
        console.log('installKeydown() id: ', id, ' element: ', element, ' objRef: ', objRef);
        if (!element) {
            return false;
        }

        if(!this.installOnMenuKeydown(id, objRef)){
            console.log('installOnMenuKeydown failed');
        }

        element.addEventListener('keydown', (event) => {
            console.log('onButtonKeydown: ', event.key);

            //const key = event.key,
            let flag = false;
            let method = '';

            switch (event.key) {
                case ' ':
                case 'Enter':
                case 'ArrowDown':
                case 'Down':
                    method = 'OpenFirst';
                    //objRef.invokeMethodAsync('OpenFirst');
                    //this.openPopup();
                    //this.setFocusToFirstMenuitem();
                    flag = true;
                    break;

                case 'Esc':
                case 'Escape':
                    /*this.closePopup();*/
                    //objRef.invokeMethodAsync('Close');
                    method = 'Close';
                    flag = true;
                    break;

                case 'Up':
                case 'ArrowUp':
                    //objRef.invokeMethodAsync('OpenLast');
                    //this.openPopup();
                    //this.setFocusToLastMenuitem();
                    method = 'OpenLast';
                    flag = true;
                    break;

                default:
                    break;
            }

            if (flag) {
                console.log('calling method ', method);
                objRef.invokeMethodAsync(method).then(() => {
                    console.log('call complete');
                    event.stopPropagation();
                    event.preventDefault();
                });
            }
        });

        return true;
    }
};