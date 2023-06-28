export function dialogInit(element) {

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