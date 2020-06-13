module Validator.Types

type Invalid = {
    Message: string
    Property: string
    Code: string
}

type Validation = Result<unit, Set<Invalid>>

type State<'a> = {
    Predicate: 'a -> bool
    Rules: list<'a -> Validation>
}

