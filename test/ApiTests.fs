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


