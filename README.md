# FSharp.Validator

Library to validate .NET and F# data types (it's strictly based in [AccidentalFish.FSharp.Validation](https://github.com/JamesRandall/AccidentalFish.FSharp.Validation)).

## Installation

It's not uploaded to Nuget yet, so follow the process to create a local package from this repository:

```
$ git clone https://github.com/Idrek/FSharp.Validator FSharp.Validator && cd $_
$ dotnet pack src/Validator.fsproj
$ mkdir /tmp/nuget
$ nuget add src/bin/Debug/FSharp.Validator.0.0.1.nupkg -Source $_
$ ## Create new `Demo` project and add the local installed library.
$ cd /tmp && dotnet new console --language F# --name Demo && cd $_
$ dotnet add Demo.fsproj package FSharp.Validator --source /tmp/nuget/ --version 0.0.1
```

## Usage

This library doesn't include any rules. You can create your own `Rules` module with your custom ones and use them from the computation expression validators. They are higher-order functions defined with following conditions:

- Their last argument must be a `string` type (as `property` name in examples) that will be populated from the validators.
- Return argument is a function that accepts the target value to validate and returns 
    a `Result` with a set of failures, if any.
- If any extra values are needed to execute validation, they will be normal arguments just before the `property`.

Here are some examples (and even more in source code tests):

```
module Rules

module V = Validator.Api
module T = Validator.Types

type DateTimeOffset = System.DateTimeOffset

let isSleepTime (property: string) : DateTimeOffset -> T.Validation =
    let failure : T.Failure = {
        Message = "Must be sleep time"
        Property = property
        Code = "IsSleepTime" 
    }
    V.withFunction failure (fun (target: DateTimeOffset) -> target.Hour >= 23 || target.Hour <= 8)

let intIsBetweenInclusive (valueFrom: int, valueTo: int) (property: string) : int -> T.Validation =
    let failure : T.Failure = {
        Message = sprintf "Must be between '%O' and '%O' (inclusive)" valueFrom valueTo
        Property = property
        Code = "IntIsBetweenInclusive" 
    }
    V.withFunction failure (fun (target: int) -> target >= valueFrom && target <= valueTo)

let listIsNotEmpty (property: string) : list<'a> -> T.Validation =
    let failure : T.Failure = {
        Message = "Must not be empty list"
        Property = property
        Code = "ListIsNotEmpty"
    }
    V.withFunction failure (List.isEmpty >> not)

let isSuperset (value: Set<'a>) (property: string) : Set<'a> -> T.Validation =
    let failure : T.Failure = {
        Message = sprintf "Set must contain '%O'" value
        Property = property
        Code = "IsSuperset"
    }
    V.withFunction failure (fun (target: Set<'a>) -> Set.isSuperset target value)
```

```
module Program

module R = Rules
module V = Validator.Api
module T = Validator.Types

[<EntryPoint>]
let main args = 

    (* validateBasic: Validate that number is larger than 3 *)
    let vNumber = validator<Int32>() {
        validateBasic "Int32" [
            R.intIsGreaterThan 3
        ]
    }
    let r : T.Validation = vNumber 6

    (* validateWhen: `User` is a record. Validate that his name starts with `Jo` if he is 
       at least 18 years old.
    *)
    let vUser = validator<User>() {
        validateWhen (fun u -> u.Age >= 18) "Name" (fun u -> u.Name) [
            R.stringStartsWith "Jo"
        ]
    }
    let r : T.Validation = vUser { Name = "John"; Age = 40 }

    (* validateRequired: `Communication` is a record. Validate that its message is a `Some`
       shorter than 10 characters.
    *)
    let vCommunication = validator<Communication>() {
        validateRequired "Message" (fun communication -> communication.Message) [
            R.stringHasMaxLengthOf 10
        ]
    }
    let r : T.Validation = vCommunication { Size = 4; Message = Some "one two" }

    (* validateOptional: `Communication` is a record. Validate that its message is a `Some`
       shorter than 10 characters OR a `None`.
    *)
    let vCommunication = validator<Communication>() {
        validateOptional "Message" (fun communication -> communication.Message) [
            R.stringHasMaxLengthOf 10
        ]
    }
    let r : T.Validation = vCommunication { Size = 4; Message = Some "one two" }
    let r : T.Validation = vCommunication { Size = 4; Message = None }

    (* validateUnion: `Job` is a record with a single-union. Validate its nested value. *)
    let vJob = validator<Job>() {
        validateUnion "Id.JobId" (fun job -> let (JobId id) = job.Id in Some id) [
            R.stringIsNotEmpty
            R.stringHasMaxLengthOf 16
        ]
    }
    let r : T.Validation = vJob { Id = JobId "123" }

    (* validateWith: `Address` is a record. Validate its `Street` field. *)
    let vAddress = validator<Address>() {
        validateWith "Street" (fun address -> address.Street) [
            FR.stringStartsWith "Castle"
        ]
    }

    0
```

## Tests

Run tests of the project:

```
$ git clone https://github.com/Idrek/FSharp.Validator FSharp.Validator && cd $_
$ dotnet test test/ValidatorTest.fsproj

Test Run Successful.
Total tests: 20
     Passed: 20
 Total time: 1.7146 Seconds
```

## API

```
module B = Validator.Builder
module T = Validator.Types

// Computation expression to apply validation rules.
let validator<'t>() : B.ValidatorBuilder<'t>
    with:
        validateBasic: Apply rules for any type without any kind of mapping.
        validateWhen: Match a precondition, map type and apply rules to it.
        validateRequired: Apply rules for a required `Option` type.
        validateOptional: Apply rules for an optional `Option` type.
        validateUnion: Apply rules for an union type.
        validateWith: Map type and apply rules to it. 

// Apply another validator from a rule.
let withValidator (validate: 't -> T.Validation) (property: string) (elem: 't) : T.Validation

// Apply another validator to a enumeration container type (like `seq`, `array` or `list`).
let eachWithValidator (validate: 't -> T.Validation) (property: string) (elems: list<'t>) : T.Validation

// Apply another validator after a predicate condition is met.
let withValidatorWhen (predicate: 't -> bool) (validate: 't -> T.Validation) (property: string) (elem: 't) : T.Validation

// Create rule.
let withFunction (failure: T.Failure) (f: 't -> bool) (elem: 't) : T.Validation
```