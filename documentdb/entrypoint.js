const orders = [
    {
        "orderId": "5513eeb8-52c6-4367-a934-2dc810e17f74",
        "total": 90.98,
        "items": 2,
        "orderStatus": "submitted",
        "buyerName": "George Dyrrachitis",
        "orderItems": [
            {
                "productId": "2447a589-86b6-4895-a416-bbdddf398a53",
                "item": "Amazon Echo",
                "price": 79.99,
                "units": 1
            },
            {
                "productId": "d3b9d9ca-e05b-4984-bae2-a668e06eb6ae",
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