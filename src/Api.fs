module Validator.Api

module B = Validator.Builder
module T = Validator.Types

let validator<'t>() : B.ValidatorBuilder<'t> = B.ValidatorBuilder<'t>()

let withValidator  
        (validate: 't -> T.Validation)
        (property: string)
        (elem: 't) 
        : T.Validation =
    match validate elem with
    | Ok () -> Ok ()
    | Error invalids -> 
        invalids 
        |> Set.map (fun (invalid: T.Invalid) -> 
            { invalid with Property = sprintf "%s.%s" property invalid.Property })
        |> Error

