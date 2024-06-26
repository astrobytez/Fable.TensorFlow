open Fake.Core

open Fake.IO
open Fake.Core
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators

[<Literal>]
let here = __SOURCE_DIRECTORY__

let initializeContext () =
    let execContext = Context.FakeExecutionContext.Create false "build.fsx" []
    Context.setExecutionContext (Context.RuntimeContext.Fake execContext)

let runProc cmd path args =
    CreateProcess.fromRawCommand cmd args
    |> CreateProcess.withWorkingDirectory path
    |> Proc.run // start with the above configuration
    |> ignore // ignore exit code

let dotnet args path = runProc "dotnet" path args

initializeContext ()

Target.create "Clean" (fun _ -> !! "src/**/bin" ++ "src/**/obj" |> Shell.cleanDirs)

Target.create "Build" (fun _ ->
    dotnet [ "build" ]  (here </> "fable")
)
Target.create "Test" (fun _ ->
    dotnet [ "fable"; "watch"; "-o"; "output"; "-s"; "--run"; "npx"; "vite" ] "fable/tests")

Target.create "Format" (fun _ -> dotnet [ "fantomas"; "." ] ".")

let deps = [
    "Clean" ==> "Build" ==> "Test"
]

[<EntryPoint>]
let main args =
    Target.runOrDefault "Test"
    0