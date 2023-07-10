export function dialogInit(element) {
    console.log('dialogInit: ', element);
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