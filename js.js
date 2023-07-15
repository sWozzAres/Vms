const x = [1, 2, 3, 4, 5];
const [y, z] = x;

const obj = {
    a: 1, b: 2, c: { d: 3 }
};

const { a, b } = obj;
const { a, c: { d } } = obj;


const numbers = [];
({ a: numbers[0], b: numbers[1] } = obj);

const [ad = 1] = [];
const { bd = 2 } = { bd: undefined };
const { cd = 2 } = { cd: null };

const { bl = console.log('hey!') } = { bl: 2 };

