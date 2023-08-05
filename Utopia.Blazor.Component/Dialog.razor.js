export function dialogInit(element) {
    //console.debug('dialogInit: ', element);
    return {
        show: () => {
            element.show();
        },
        showModal: () => {
            element.showModal();
        },
        close: () => {
            element.close();
        }
    };
}