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

Target.create "All" ignore

let deps = [
    "Clean" ==> "Build" ==> "All"
]

[<EntryPoint>]
let main args =
    Target.runOrDefault "All"
    0