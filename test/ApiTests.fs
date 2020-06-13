module ValidatorTest.ApiTests

open Validator.Api
open Xunit

module FT = ValidatorTest.Fixture.Types
module FR = ValidatorTest.Fixture.Rules
module T = Validator.Types

type Int32 = System.Int32
type String = System.String

[<Fact>]
let ``Test withFunction function`` () =
    let invalid : T.Invalid = {
        Message = "Message 1"
        Property = "Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Ok (), 
        withFunction invalid (fun (s: string) -> s.Length > 6) "jamaica")

    Assert.Equal<T.Validation>(
        Error <| Set [invalid],
        withFunction invalid (fun (s: string) -> s.Length > 6) "japan")

[<Fact>]
let ``Test withValidator function`` () =
    let invalid : T.Invalid = {
        Message = "Message 1"
        Property = "Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Ok (),
        withValidator  
            (fun (s: string) -> if s.Length > 6 then Ok () else Error <| Set [invalid]) 
            "Country"
            "jamaica")
            

    let nestedInvalid : T.Invalid = {
        Message = "Message 1"
        Property = "Country.Property1"
        Code = "Code1"
    }
    Assert.Equal<T.Validation>(
        Error <| Set [nestedInvalid],
        withValidator 
            (fun (s: string) -> if s.Length > 6 then Ok () else Error <| Set [invalid])
            "Country"
            "japan")

[<Fact>]
let ``Test withValidatorWhen function`` () =
    let invalid : T.Invalid = {
        Message = "Message 1"
        Property = "Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Ok (),
        withValidatorWhen  
            (fun (s: string) -> s.StartsWith "ja")
            (fun (s: string) -> if s.Length > 6 then Ok () else Error <| Set [invalid])
            "Country"
            "jamaica")

    let error : T.Invalid = {
        Message = "Message 1"
        Property = "Country.Property1"
        Code = "Code1"
    }
    Assert.Equal<T.Validation>(
        Error <| Set [error],
        withValidatorWhen 
            (fun (s: string) -> s.StartsWith "ja")
            (fun (s: string) -> if s.Length > 6 then Ok () else Error <| Set [invalid])
            "Country"
            "japan")

    Assert.Equal<T.Validation>(
        Ok (),
        withValidatorWhen  
            (fun (s: string) -> s.StartsWith "ja")
            (fun (s: string) -> if s.Length > 6 then Ok () else Error <| Set [invalid])
            "Country"
            "spain")

[<Fact>]
let ``Test eachWithValidator function over a list`` () =
    let invalid : T.Invalid = {
        Message = "Message 1"
        Property = "Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Ok (),
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            ["japan"; "jamaica"])

    let error1 : T.Invalid = {
        Message = "Message 1"
        Property = "Country.[1].Property1"
        Code = "Code1"
    }
    let error2 : T.Invalid = {
        Message = "Message 1"
        Property = "Country.[2].Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Error <| Set [error1],
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            ["japan"; "france"; "jamaica"])

    Assert.Equal<T.Validation>(
        Error <| Set [error1; error2],
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            ["japan"; "france"; "spain"])

[<Fact>]
let ``Test eachWithValidator function over an array`` () =
    let invalid : T.Invalid = {
        Message = "Message 1"
        Property = "Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Ok (),
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            [|"japan"; "jamaica"|])

    let error1 : T.Invalid = {
        Message = "Message 1"
        Property = "Country.[1].Property1"
        Code = "Code1"
    }
    let error2 : T.Invalid = {
        Message = "Message 1"
        Property = "Country.[2].Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Error <| Set [error1],
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            [|"japan"; "france"; "jamaica"|])

    Assert.Equal<T.Validation>(
        Error <| Set [error1; error2],
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            [|"japan"; "france"; "spain"|]) 

[<Fact>]
let ``Test eachWithValidator function over a sequence`` () =
    let invalid : T.Invalid = {
        Message = "Message 1"
        Property = "Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Ok (),
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            (seq ["japan"; "jamaica"]))

    let error1 : T.Invalid = {
        Message = "Message 1"
        Property = "Country.[1].Property1"
        Code = "Code1"
    }
    let error2 : T.Invalid = {
        Message = "Message 1"
        Property = "Country.[2].Property1"
        Code = "Code1"
    }

    Assert.Equal<T.Validation>(
        Error <| Set [error1],
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            (seq ["japan"; "france"; "jamaica"]))

    Assert.Equal<T.Validation>(
        Error <| Set [error1; error2],
        eachWithValidator  
            (fun (s: string) -> if s.StartsWith "ja" then Ok () else Error <| Set [invalid]) 
            "Country"
            (seq ["japan"; "france"; "spain"]))

[<Fact>]
let ``Test builder's validateWith + withValidator function`` () =
    let vAddress = validator<FT.Address>() {
        validateWith "Street" (fun address -> address.Street) [
            FR.stringStartsWith "Castle"
        ]
    }
    let vUser = validator<FT.User1>() {
        validateWith "Address" (fun user -> user.Address) [
            (withValidator vAddress)
        ]
    }

    let user : FT.User1 = {
        Name = "John"
        Age = 32
        Address = { Street = "Castle Lane"; Number = 54 }
    }
    Assert.Equal<T.Validation>(Ok (), vUser user)

    let user : FT.User1 = {
        Name = "John"
        Age = 32
        Address = { Street = "Lane Castle"; Number = 54 }
    }
    let error : T.Validation = Error <| Set [{
        T.Message = "Must start with 'Castle'"
        T.Property = "Address.Street"
        T.Code = "StringStartsWith"
    }]
    Assert.Equal<T.Validation>(error, vUser user)

[<Fact>]
let ``Test builder's validateBasic member`` () =
    let vNumber = validator<Int32>() {
        validateBasic "Int32" [
            FR.intIsGreaterThan 3
        ]
    }
    Assert.Equal(Ok (), vNumber 6)
    
    let error : T.Validation = Error <| Set [{
        T.Message = "Must be greater than '3'"
        T.Property = "Int32"
        T.Code = "IntIsGreaterThan"
    }]
    Assert.Equal(error, vNumber 3)

[<Fact>]
let ``Test builder's validateWhen function`` () =
    let vUser = validator<FT.User1>() {
        validateWhen (fun u -> u.Age >= 18) "Name" (fun u -> u.Name) [
            FR.stringStartsWith "Jo"
        ]
    }

    let user : FT.User1 = {
        Name = "John"
        Age = 32
        Address = { Street = "Castle Lane"; Number = 54 }
    }
    Assert.Equal<T.Validation>(Ok (), vUser user)

    let user : FT.User1 = {
        Name = "Peter"
        Age = 32
        Address = { Street = "Castle Lane"; Number = 54 }
    }
    let error : T.Validation = Error <| Set [{
        T.Message = "Must start with 'Jo'"
        T.Property = "Name"
        T.Code = "StringStartsWith"
    }]
    Assert.Equal<T.Validation>(error, vUser user)

    let user : FT.User1 = {
        Name = "John"
        Age = 15
        Address = { Street = "Castle Lane"; Number = 54 }
    }
    Assert.Equal<T.Validation>(Ok (), vUser user)

[<Fact>]
let ``Test builder's validateWith + withValidatorWhen function`` () =
    let vAddressHigh = validator<FT.Address>() {
        validateWith "Street" (fun address -> address.Street) [
            FR.stringStartsWith "Castle"
        ]
    }
    let vAddressLow = validator<FT.Address>() {
        validateWith "Street" (fun address -> address.Street) [
            FR.stringStartsWith "Lane"
        ]
    }
    let vUser = validator<FT.User1>() {
        validateWith "Address" (fun user -> user.Address) [
            (withValidatorWhen (fun address -> address.Number > 10) vAddressHigh)
            (withValidatorWhen (fun address -> address.Number <= 10) vAddressLow)
        ]
    }

    let user1 : FT.User1 = {
        Name = "John"
        Age = 32
        Address = { Street = "Castle Lane"; Number = 54 }
    }
    Assert.Equal(Ok (), vUser user1)

    let user2 : FT.User1 = {
        Name = "John"
        Age = 32
        Address = { Street = "Lane Castle"; Number = 54 }
    }
    let error : T.Validation = Error <| Set [{
        T.Message = "Must start with 'Castle'"
        T.Property = "Address.Street"
        T.Code = "StringStartsWith"
    }]
    Assert.Equal<T.Validation>(error, vUser user2)

    let user3 : FT.User1 = {
        Name = "John"
        Age = 32
        Address = { Street = "Lane Castle"; Number = 6 }
    }
    Assert.Equal(Ok (), vUser user3)

    let user4 : FT.User1 = {
        Name = "John"
        Age = 32
        Address = { Street = "Castle Lane"; Number = 6 }
    }
    let error : T.Validation = Error <| Set [{
        T.Message = "Must start with 'Lane'"
        T.Property = "Address.Street"
        T.Code = "StringStartsWith"
    }]
    Assert.Equal(error, vUser user4)

[<Fact>]
let ``Test builder's eachWithValidator function`` () =
    let vNumber = validator<Int32>() {
        validateBasic "Int32" [
            FR.intIsGreaterThan 3
        ]
    }
    let vUser = validator<FT.User2>() {
        validateWith "FavouriteNumbers" (fun user -> user.FavouriteNumbers) [
            (eachWithValidator vNumber)
        ]
    }

    let user1 : FT.User2 = { Name = "John"; FavouriteNumbers = [6; 4; 12; 20; 7] }
    Assert.Equal(Ok (), vUser user1)

    let user2 : FT.User2 = { Name = "John"; FavouriteNumbers = [6; 2; 12; 20; 1] }
    Assert.Equal(
        Error <| Set [
            { 
                T.Message = "Must be greater than '3'"
                T.Property = "FavouriteNumbers.[1].Int32" 
                T.Code = "IntIsGreaterThan"
            }
            { 
                T.Message = "Must be greater than '3'"
                T.Property = "FavouriteNumbers.[4].Int32"
                T.Code = "IntIsGreaterThan"
            }
        ],
        vUser user2)

[<Fact>]
let ``Test builder's validation functions for Order type`` () =
    let validOrder : FT.Order = {
        Title = "Cans of stuff"
        Customer = { Age = 45 }
        Cost = 50.0
        Items = [
            { Name = "Baked Beans"; Quantity = 10 }
            { Name = "Kidney Beans"; Quantity = 2 }
        ]
    }

    let invalidOrder : FT.Order = {
        Title = String.Empty
        Customer = { Age = 16 }
        Cost = -100.0
        Items = [
            { Name = "Baked Beans"; Quantity = -2 }
            { Name = ""; Quantity = 5 }
        ]
    }

    let vCustomer = validator<FT.Customer>() {
        validateWith "Age" (fun customer -> customer.Age) [
            FR.intIsGreaterOrEqualTo 18
            FR.intIsLessOrEqualTo 65
        ]
    }

    let vOrderItem = validator<FT.OrderItem>() {
        validateWith "Name" (fun orderItem -> orderItem.Name) [
            FR.stringIsNotEmpty
            FR.stringHasMaxLengthOf 128
        ]
        validateWith "Quantity" (fun r -> r.Quantity) [
            FR.intIsGreaterOrEqualTo  1
        ]
    }
    
    let vOrder = validator<FT.Order>() {
        validateWith "Customer.Age" (fun order -> order.Customer.Age) [
            FR.intIsGreaterOrEqualTo 18
        ]
        validateWith "Items" (fun order -> order.Items) [
            FR.listIsNotEmpty
            eachWithValidator vOrderItem
        ]
        validateWith "Cost" (fun order -> order.Cost) [
            FR.doubleIsGreaterThan  0.0
        ]
        validateWith "Title" (fun order -> order.Title) [
            FR.stringIsNotEmpty
            FR.stringHasMaxLengthOf 128
        ]
    }

    Assert.Equal<T.Validation>(Ok (), vOrder validOrder)
    Assert.Equal<T.Validation>(
        Error <| Set [
            { 
                T.Message = "Must not be empty"
                T.Property = "Title"
                T.Code = "StringIsNotEmpty" 
            }
            { 
                T.Message = "Must be greater or equal to '1'"
                T.Property = "Items.[0].Quantity"
                T.Code = "IntIsGreaterOrEqualTo" 
            }
            { 
                T.Message = "Must be greater than '0'"
                T.Property = "Cost"
                T.Code = "DoubleIsGreaterThan" 
            }
            { 
                T.Message = "Must not be empty"
                T.Property = "Items.[1].Name"
                T.Code = "StringIsNotEmpty" 
            }
            { 
                T.Message = "Must be greater or equal to '18'"
                T.Property = "Customer.Age"
                T.Code = "IntIsGreaterOrEqualTo" 
            }
        ],
        vOrder invalidOrder)

[<Fact>]
let ``Test builder's validation functions for TrafficLightColor type`` () =
    let vLight = validator<FT.TrafficLight>() {
        validateWith "Color" (fun trafficLight -> trafficLight.Color) [
            FR.isColor FT.Green
        ]
    }
    Assert.Equal<T.Validation>(
        Error <| Set [
            { 
                T.Message = "Color is not 'Green'"
                T.Property = "Color"
                T.Code = "IsColor" 
            }
        ],
        vLight { Color = FT.Amber })
    Assert.Equal<T.Validation>(Ok (), vLight { Color = FT.Green })

[<Fact>]
let ``Test builder's validateRequired method`` () =
    let vCommunication = validator<FT.Communication>() {
        validateRequired "Message" (fun communication -> communication.Message) [
            FR.stringHasMaxLengthOf 10
        ]
    }
    Assert.Equal<T.Validation>(Ok (), vCommunication { Size = 4; Message = Some "one two" })
    Assert.Equal<T.Validation>(
        Error <| Set [
            { 
                T.Message = "Must has max length of '10'"
                T.Property = "Message"
                T.Code = "StringHasMaxLengthOf" 
            }
        ],
        vCommunication { Size = 4; Message = Some "one two three" })
    Assert.Equal<T.Validation>(
        Error <| Set [
            { 
                T.Message = "Required value in 'Option' type"
                T.Property = "Message"
                T.Code = "RequiredOption" 
            }
        ],
        vCommunication { Size = 4; Message = None })

[<Fact>]
let ``Test builder's validateOptional method`` () =
    let vCommunication = validator<FT.Communication>() {
        validateOptional "Message" (fun communication -> communication.Message) [
            FR.stringHasMaxLengthOf 10
        ]
    }
    Assert.Equal<T.Validation>(Ok (), vCommunication { Size = 4; Message = Some "one two" })
    Assert.Equal<T.Validation>(
        Error <| Set [
            { 
                T.Message = "Must has max length of '10'"
                T.Property = "Message"
                T.Code = "StringHasMaxLengthOf" 
            }
        ],
        vCommunication { Size = 4; Message = Some "one two three" })
    Assert.Equal<T.Validation>(Ok (), vCommunication { Size = 4; Message = None })

    