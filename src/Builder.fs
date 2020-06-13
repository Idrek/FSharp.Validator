module Validator.Builder

module T = Validator.Types

type ValidatorBuilder<'t> () =

    member this.Yield (_: unit) : list<T.State<'t>> = List.empty

    [<CustomOperation("validateWith")>]
    member this.ValidateWith 
            (
                state: list<T.State<'t>>,
                property: string,
                getProperty: 't -> 'p,
                rules: list<string -> 'p -> T.Validation>
            ) : list<T.State<'t>> =
        {
            Predicate = fun _ -> true
            Rules = List.map (fun rule -> getProperty >> (rule property)) rules
        } :: state

    [<CustomOperation("validateBasic")>]
    member this.ValidateBasic
            (
                state: list<T.State<'t>>,
                property: string,
                rules: list<string -> 't -> T.Validation>
            ) : list<T.State<'t>> =
        {
            Predicate = fun _ -> true
            Rules = List.map (fun rule -> rule property) rules
        } :: state
        

 