export function init(element, objRef) {
    console.log('init: ', element, objRef);

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

    const mouseDownHandler = event => {
        // if element is disabled, prevent 'mousedown' setting focus 
        if (element.getAttribute('tabindex') === '-1') {
            event.preventDefault();
        }
    };

    const keyDownHandler = event => {
        const { key } = event;

        // 'aria-activedescendant' only set when the menu is open
        const action = getActionFromKey(event, element.getAttribute('aria-activedescendant').length > 0);
        
        switch (action) {
            case SelectActions.Last:
            case SelectActions.First:
                objRef.invokeMethodAsync("UpdateMenuStateJS", true);

            // intentional fallthrough
            case SelectActions.Next:
            case SelectActions.Previous:
            case SelectActions.PageUp:
            case SelectActions.PageDown:
                event.preventDefault();
                objRef.invokeMethodAsync("OnOptionChangeJS", action).then(() => {
                    scrollActiveItemIntoView(element.getAttribute('aria-activedescendant'));
                });
                return;

            case SelectActions.CloseSelect:
                event.preventDefault();
                objRef.invokeMethodAsync("SelectOptionJS");

            // intentional fallthrough
            case SelectActions.Close:
                event.preventDefault();
                objRef.invokeMethodAsync("UpdateMenuStateJS", false);
                return;

            case SelectActions.Type:
                objRef.invokeMethodAsync("OnComboTypeJS", key).then(activeId => {
                    scrollActiveItemIntoView(activeId);
                });
                return;

            case SelectActions.Open:
                event.preventDefault();
                objRef.invokeMethodAsync("UpdateMenuStateJS", true).then(() => {
                    scrollActiveItemIntoView(element.getAttribute('aria-activedescendant'));
                });
                return;
        }

        function scrollActiveItemIntoView(activeId) {
            if (activeId) {
                const el = document.getElementById(activeId);
                if (el)
                    el.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
            }
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
    };

    element.addEventListener('mousedown', mouseDownHandler);
    element.addEventListener('keydown', keyDownHandler);

    return {
        stop: () => {
            element.removeEventListener('mousedown', mouseDownHandler);
            element.removeEventListener('keydown', keyDownHandler);
        }
    }
}