module Validator.Api

module B = Validator.Builder

let validator<'t>() : B.ValidatorBuilder<'t> = B.ValidatorBuilder<'t>()

