module Tests

open Fable.Mocha
open Fable.Core

open TensorFlow

async {
  let backend = Cpu // Wasm backend broken
  Backend.setWasmPaths ("@tensorflow/tfjs-backend-wasm/dist", false)
  do! Async.AwaitPromise(Tf.setBackend backend)
  printfn $"TensorFlow Backend Set to {backend}"
}
|> Async.StartImmediate

let run () = async {
  let model = Tf.sequential ()
  let layer = Dense.props [ Units 1; InputShape [| 1 |] ]
  model.add (Tf.layers.dense layer)

  model.compile (ModelCompileArgs.props [ Loss MeanSquaredError; Optimizer Adam ])

  let xs = Tf.tensor2d ([| -1.0; 0.0; 1.0; 2.0; 3.0; 4.0 |], [| 6; 1 |])
  let ys = Tf.tensor2d ([| -3.0; -1.0; 1.0; 3.0; 5.0; 7.0 |], [| 6; 1 |])

  do! Async.AwaitPromise(model.fit (xs, ys, {| epochs = 250 |}))
  return model
}

let coreTests =
    testList "Core tests" [

        testAsync "Test - 1" {
            let! value = run ()
            value.predict(Tf.tensor2d ([| 6.0 |], [| 1; 1 |], Float32)).print ()
            Expect.passWithMsg $"Tensorflow completed successfully"
        }
    ]

Mocha.runTests coreTests |> ignore