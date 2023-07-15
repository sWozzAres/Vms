import React, { Component } from 'react';
import { useState, useEffect, useRef } from 'react';

import { NewTodoForm, TodoList } from './NewTodoForm';

function Dialog({ show, toggle }) {

    const ref = useRef(null)
    

    useEffect(() => {
        if (!ref.current) {
            return;
        }

        if (show) {
            ref.current.showModal();
        }
        else {
            ref.current.close();
        }
    }, [ref, show]);

    return <dialog ref={ref}>
        <h1>A dialog</h1>
        <button onClick={toggle}>Close</button>
    </dialog>
}

export default function App() {
    const [show, setShow] = useState(false);
    const [dialogActive, setDialogActive] = useState(false)

    const [todos, setToDos] = useState(() => {
        const localValue = localStorage.getItem("ITEMS")
        if (!localValue) return [];

        return JSON.parse(localValue);
    });


    useEffect(() => {
        localStorage.setItem("ITEMS", JSON.stringify(todos));
    }, [todos])




    function addToDo(title) {
        setToDos(currentToDos => {
            return [
                ...currentToDos,
                { id: crypto.randomUUID(), title, completed: false },
            ]
        })
    }

    function toggleTodo(id, completed) {
        setToDos(currentToDos => {
            return currentToDos.map(todo => {
                if (todo.id === id) {
                    return { ...todo, completed }
                }

                return todo;
            })
        })
    }

    function deleteToDo(id) {
        setToDos(currentToDos => {
            return currentToDos.filter(todo => todo.id !== id)
        })
    }

    const handleDialogToggle = () => {
        setShow(prevShow => !prevShow);
    };
    let dialog = <Dialog show={show} toggle={handleDialogToggle} />
    
    return (
        <>
            {dialog}
            <button onClick={handleDialogToggle}>Dialog</button>
            <NewTodoForm onSubmit={addToDo} />
            <h1 className="header">Todo List {todos.length}</h1>
            <TodoList todos={todos} toggleTodo={toggleTodo} deleteToDo={deleteToDo} />
        </>)
}