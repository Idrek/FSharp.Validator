module Validator.Types

type Invalid = {
    Message: string
    Property: string
    Code: string
}

type Validation = Result<unit, Set<Invalid>>

