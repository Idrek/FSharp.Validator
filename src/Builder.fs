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

    [<CustomOperation("validateWhen")>]
    member this.ValidateWhen
            (
                state: list<T.State<'t>>,
                predicate: 't -> bool,
                property: string,
                getProperty: 't -> 'p,
                rules: list<string -> 'p -> T.Validation>
            ) : list<T.State<'t>> =
        {
            Predicate = predicate
            Rules = List.map (fun rule -> getProperty >> (rule property)) rules
        } :: state

    [<CustomOperation("validateRequired")>]
    member this.ValidateRequired
            (
                state: list<T.State<'t>>,
                property: string,
                getProperty: 't -> Option<'p>,
                rules: list<string -> 'p -> T.Validation>
            ) : list<T.State<'t>> =
        {
            Predicate = fun _ -> true
            Rules = rules |> List.map (fun rule ->
                (fun v ->
                    match getProperty v with
                    | None -> 
                        Error <| Set [{ 
                            T.Message = "Required value in 'Option' type"
                            T.Property = property
                            T.Code = "RequiredOption"
                        }]
                    | Some p -> rule property p))
        } :: state

    [<CustomOperation("validateOptional")>]
    member this.ValidateOptional
            (
                state: list<T.State<'t>>,
                property: string,
                getProperty: 't -> Option<'p>,
                rules: list<string -> 'p -> T.Validation>
            ) : list<T.State<'t>> =
        {
            Predicate = fun _ -> true
            Rules = rules |> List.map (fun rule ->
                (fun v ->
                    match getProperty v with
                    | None -> Ok ()
                    | Some p -> rule property p))
        } :: state

    [<CustomOperation("validateUnion")>]
    member this.ValidateUnion 
            (
                state: list<T.State<'t>>,
                property: string,
                getProperty: 't -> Option<'p>,
                rules: list<string -> 'p -> T.Validation>
            ) : list<T.State<'t>> =
        this.ValidateOptional(state, property, getProperty, rules)

    member this.Run (state: list<T.State<'t>>) : 't -> T.Validation =
        let execute (t: 't) : T.Validation =
            let invalids : Set<T.Invalid> = 
                state
                |> List.filter (fun state -> state.Predicate t)
                |> List.collect (fun state -> List.map (fun v -> v t) state.Rules)
                |> List.map (fun validated -> 
                    match validated with 
                    | Ok () -> Set.empty 
                    | Error failure -> failure)
                |> Set.unionMany 
            match Set.isEmpty invalids with
            | true -> Ok ()
            | false -> Error invalids
        execute
        

 