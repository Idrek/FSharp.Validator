module ValidatorTest.Fixture.Rules

open Validator.Api

module T = Validator.Types

type String = System.String

let stringStartsWith (value: string) (property: string) : string -> T.Validation =
    let invalid : T.Invalid = {
        Message = sprintf "Must start with '%O'" value
        Property = property
        Code = "StringStartsWith" 
    }
    withFunction invalid (fun (target: string) -> target.StartsWith value)

