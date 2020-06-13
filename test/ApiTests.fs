module ValidatorTest.ApiTests

open Validator.Api
open Xunit

module T = Validator.Types

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

