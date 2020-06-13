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

let intIsGreaterThan (value: int) (property: string) : int -> T.Validation =
    let invalid : T.Invalid = {
        Message = sprintf "Must be greater than '%O'" value
        Property = property
        Code = "IntIsGreaterThan" 
    }
    withFunction invalid (fun (target: int) -> target > value)

let intIsGreaterOrEqualTo (value: int) (property: string) : int -> T.Validation =
    let invalid : T.Invalid = {
        Message = sprintf "Must be greater or equal to '%O'" value
        Property = property
        Code = "IntIsGreaterOrEqualTo"
    }
    withFunction invalid (fun (target: int) -> target >= value)

let intIsLessOrEqualTo (value: int) (property: string) : int -> T.Validation =
    let invalid : T.Invalid = {
        Message = sprintf "Must be less or equal to '%O'" value
        Property = property
        Code = "IntIsLessOrEqualTo"
    }
    withFunction invalid (fun (target: int) -> target <= value)

let stringIsNotEmpty (property: string) : string -> T.Validation =
    let invalid : T.Invalid = {
        Message = "Must not be empty"
        Property = property
        Code = "StringIsNotEmpty"
    }
    withFunction invalid (String.IsNullOrWhiteSpace >> not)

