module Validator.Builder

module T = Validator.Types

type ValidatorBuilder<'t> () =

    member this.Yield (_: unit) : list<T.State<'t>> = List.empty


 