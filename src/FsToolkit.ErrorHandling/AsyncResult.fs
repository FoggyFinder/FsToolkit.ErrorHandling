namespace FsToolkit.ErrorHandling

[<RequireQualifiedAccess>]
module AsyncResult = 

  let map f ar =
    Async.map (Result.map f) ar

  let mapError f ar =
    Async.map (Result.mapError f) ar    

  let bind f ar = async {
    let! result = ar
    let t = 
      match result with 
      | Ok x -> f x
      | Error e -> async { return Error e }
    return! t      
  }

  let foldResult onSuccess onError ar =
    Async.map (Result.fold onSuccess onError) ar

  let ofTask aTask = 
    aTask
    |> Async.AwaitTask 
    |> Async.Catch 
    |> Async.map Result.ofChoice
  
  let retn x =
    Ok x
    |> Async.singleton
  
  let returnError x =
    Error x
    |> Async.singleton

  let map2 f xR yR =
    Async.map2 (Result.map2 f) xR yR

  let map3 f xR yR zR =
    Async.map3 (Result.map3 f) xR yR zR

  let apply fAR xAR =
    map2 (fun f x -> f x) fAR xAR

  /// Returns the specified error if the async-wrapped value is false.
  let requireTrue error value = 
    value |> Async.map (Result.requireTrue error)

  /// Returns the specified error if the async-wrapped value is true.
  let requireFalse error value =
    value |> Async.map (Result.requireFalse error) 

  // Converts an async-wrapped Option to a Result, using the given error if None.
  let requireSome error option =
    option |> Async.map (Result.requireSome error)

  // Converts an async-wrapped Option to a Result, using the given error if Some.
  let requireNone error option =
    option |> Async.map (Result.requireNone error)

  /// Returns Ok if the async-wrapped value and the provided value are equal, or the specified error if not.
  let requireEquals other error this =
    this |> Async.map (Result.requireEquals other error)


  /// Returns Ok if the async-wrapped sequence is empty, or the specified error if not.
  let requireEmpty error xs =
    xs |> Async.map (Result.requireEmpty error)

  /// Returns Ok if the async-wrapped sequence is not-empty, or the specified error if not.
  let requireNotEmpty error xs =
    xs |> Async.map (Result.requireNotEmpty error)

  /// Returns the first item of the async-wrapped sequence if it exists, or the specified
  /// error if the sequence is empty
  let requireHead error xs =
    xs |> Async.map (Result.requireHead error)

  /// Replaces an error value of an async-wrapped result with a custom error
  /// value.
  let setError error asyncResult =
    asyncResult |> Async.map (Result.setError error)

  /// Replaces a unit error value of an async-wrapped result with a custom
  /// error value. Safer than setError since you're not losing any information.
  let withError error asyncResult =
    asyncResult |> Async.map (Result.withError error)

  /// Extracts the contained value of an async-wrapped result if Ok, otherwise
  /// uses ifError.
  let defaultValue ifError asyncResult =
    asyncResult |> Async.map (Result.defaultValue ifError)

  /// Extracts the contained value of an async-wrapped result if Ok, otherwise
  /// evaluates ifErrorThunk and uses the result.
  let defaultWith ifErrorThunk asyncResult =
    asyncResult |> Async.map (Result.defaultWith ifErrorThunk)

  /// Same as defaultValue for a result where the Ok value is unit. The name
  /// describes better what is actually happening in this case.
  let ignoreError result =
    defaultValue () result

  /// If the async-wrapped result is Ok, executes the function on the Ok value.
  /// Passes through the input value.
  let tee f asyncResult =
    asyncResult |> Async.map (Result.tee f)


  /// If the async-wrapped result is Ok and the predicate returns true, executes
  /// the function on the Ok value. Passes through the input value.
  let teeIf predicate f asyncResult =
    asyncResult |> Async.map (Result.teeIf predicate f)


  /// If the async-wrapped result is Error, executes the function on the Error
  /// value. Passes through the input value.
  let teeError f asyncResult =
    asyncResult |> Async.map (Result.teeError f)

  /// If the async-wrapped result is Error and the predicate returns true,
  /// executes the function on the Error value. Passes through the input value.
  let teeErrorIf predicate f asyncResult =
    asyncResult |> Async.map (Result.teeErrorIf predicate f)

module AsyncResultOperators =

  let inline (<!>) f x = AsyncResult.map f x
  let inline (<*>) f x = AsyncResult.apply f x
  let inline (>>=) x f = AsyncResult.bind f x

