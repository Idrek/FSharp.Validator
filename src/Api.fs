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

let eachWithValidator  
        (validate: 't -> T.Validation)
        (property: string)
        (elems: seq<'t>) 
        : T.Validation =
    let validatedElems : seq<T.Validation * int> = 
        Seq.mapi (fun index item -> (validate item, index)) elems
    let invalids : Set<T.Invalid> = 
        validatedElems 
        |> Seq.map (fun validated ->
            match validated with
            | (Ok (), _) -> Set.empty
            | (Error invalids, index) -> 
                invalids |> Set.map (fun invalid -> 
                    { invalid with Property = sprintf "%s.[%d].%s" property index invalid.Property }))
        |> Set.unionMany
    match Set.count invalids with
    | 0 -> Ok ()
    | _ -> Error invalids

