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

    installKeydown: function (id) {
        var element = document.querySelector('#' + id);
        console.log('id: ', id, ' element: ', element);

        element.addEventListener('keydown', (event) => {
            console.log('onButtonKeydown: ', event.key);

            var key = event.key,
                flag = false;

            switch (key) {
                case ' ':
                case 'Enter':
                case 'ArrowDown':
                case 'Down':
                    //this.openPopup();
                    //this.setFocusToFirstMenuitem();
                    flag = true;
                    break;

                case 'Esc':
                case 'Escape':
                    /*this.closePopup();*/
                    flag = true;
                    break;

                case 'Up':
                case 'ArrowUp':
                    //this.openPopup();
                    //this.setFocusToLastMenuitem();
                    flag = true;
                    break;

                default:
                    break;
            }

            if (flag) {
                event.stopPropagation();
                event.preventDefault();
            }
        });
    }
};