const orders = [
    {
        "total": 90.98,
        "items": 2,
        "orderStatus": "submitted",
        "buyerName": "George Dyrrachitis",
        "orderItems": [
            {
                "item": "Amazon Echo",
                "price": 79.99,
                "units": 1
            },
            {
                "item": "keyboard",
                "price": 10.99,
                "units": 1
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