const x = [1, 2, 3, 4, 5];
const [y, z] = x;

const obj = {
    a: 1, b: 2, c: { d: 3 }
};

const { a, b } = obj;
const { a, c: { d } } = obj;


const numbers = [];
// assignment pattern
({ a: numbers[0], b: numbers[1] } = obj);

const [ad = 1] = [];
const { bd = 2 } = { bd: undefined };
const { cd = 2 } = { cd: null };

const { bl = console.log('hey!') } = { bl: 2 };

const { ar, ...others } = { a: 1, b: 2, c: 3 };
const [first, ...others2] = [1, 2, 3];

const foo = ['one', 'two'];
const [blue, yellow, green] = foo;

let a = 1;
let b = 2;
[a, b] = [b, a];

const arr = [1, 2, 3];
[arr[2], arr[1]] = [arr[1], arr[2]];

function f() { return [1, 2, 3]; }
const [a, , b] = f(); // 1,3
const [c] = f(); // 1
[, ,] = f();

const [a, b, ...{ length }] = [1, 2, 3];
const [a, b, ...[c, d]] = [1, 2, 3, 4];
const [a, b, ...[c, d, ...[e, f]]] = [1, 2, 3, 4, 5, 6];

// iterable -> array destructure
const [a, b] = new Map([1, 2], [3, 4]);

const obj = {
    *[Symbol.iterator]() {
        for (const v of [0, 1, 2, 3]) {
            console.log(v);
            yield v;
        }
    },
};
const [a, b] = obj; // Only logs 0 and 1

const obj = {
    *[Symbol.iterator]() {
        for (const v of [0, 1, 2, 3]) {
            console.log(v);
            yield v;
        }
    },
};
const [a, b, ...rest] = obj; // Logs 0 1 2 3
console.log(rest); // [2, 3] (an array)
