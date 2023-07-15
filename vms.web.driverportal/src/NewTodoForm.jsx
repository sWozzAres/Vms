import { useState } from 'react';
export function NewTodoForm({ onSubmit }) {

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

export function TodoList({ todos, toggleTodo, deleteToDo }) {
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