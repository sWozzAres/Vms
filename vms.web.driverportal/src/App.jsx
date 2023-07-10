import React, { Component } from 'react';
import { useState, useEffect } from 'react';

function NewTodoForm({ onSubmit }) {
    
    const [newItem, setNewItem] = useState("")

    function handleSubmit(e) {
        e.preventDefault()

        if (newItem === "") return;

        onSubmit(newItem);
        setNewItem("");
    }

    return (
        <form onSubmit={handleSubmit} className="new-item-form">
            <div className="form-row">
                <label htmlFor="item">New Item</label>
                <input
                    value={newItem}
                    onChange={e => setNewItem(e.target.value)}
                    type="text"
                    id="item" />
            </div>
            <button className="button">Add</button>
        </form>
    )
}

function TodoList({ todos, toggleTodo, deleteToDo }) {
    return <ul className="list">
        {todos.length === 0 && "No todos"}
        {todos.map(todo => {
            return <TodoItem {...todo} key={todo.id} toggleTodo={toggleTodo} deleteToDo={deleteToDo} />
        })}
    </ul>
}

function TodoItem({ completed, id, title, toggleTodo, deleteToDo }) {
    return <li>
        <label>
            <input type="checkbox"
                checked={completed}
                onChange={e => toggleTodo(id, e.target.checked)} />
            {title}
            <button onClick={() => deleteToDo(id)}
                className="btn btn-danger">Delete</button>
        </label>
    </li>
}

function Dialog() {
    return <dialog open>
    <h1>A dialog</h1>
    </dialog>
}

export default function App() {

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

    let content;
    if (dialogActive) {
        console.log('setting dialog');
        content = <Dialog/>
    }

    return (
        <>
            {content}
            <button onClick={() => setDialogActive(!dialogActive)}>Dialog</button>
            <NewTodoForm onSubmit={addToDo} />
            <h1 className="header">Todo List {todos.length}</h1>
            <TodoList todos={todos} toggleTodo={toggleTodo} deleteToDo={deleteToDo} />
        </>)
}