module ValidatorTest.Fixture.Types

type Address = {
    Street: string
    Number: int
}

type User1 = {
    Name: string
    Age: int
    Address: Address
}

type User2 = {
    Name: string
    FavouriteNumbers: list<int>
}

type Customer = {
    Age: int
}

type OrderItem = {
    Name: string
    Quantity: int
}

type Order = {
    Title: string
    Customer: Customer
    Cost: double
    Items: list<OrderItem>
}

type TrafficLightColor = Red | Green | Amber

type TrafficLight = {
    Color: TrafficLightColor
}

type Communication = {
    Size: int
    Message: Option<string>
}

type JobId = JobId of string

type Job = {
    Id: JobId
}

type Ingredient =
    | Juice of double
    | Fruit of string

type Recipe = {
    Value: Ingredient
}

type Color = Black | Blue | Yellow