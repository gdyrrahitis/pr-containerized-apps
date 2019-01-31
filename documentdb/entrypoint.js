const orders = [
    {
        "total": 90.98,
        "items": 2,
        "orderItems": [
            {
                "item": "Amazon Echo",
                "price": 79.99
            },
            {
                "item": "keyboard",
                "price": 10.99
            }
        ]
    }
];

db.createCollection("ordersCollection");
db.ordersCollection.insertMany(orders, function (err, result) {
    if (err) {
        throw err;
    }

    print(`Number of orders inserted ${result.insertedCount}`);
});