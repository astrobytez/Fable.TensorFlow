module TensorFlow

open Fable.Core
open Fable.Core.JsInterop

type Tensor =
  abstract print: unit -> unit

type Model =
  abstract add: obj -> unit
  abstract compile: obj -> unit
  abstract fit: Tensor * Tensor * obj -> JS.Promise<unit>
  abstract predict: obj -> Tensor

type Layers =
  abstract dense: obj -> obj

type JsArray = obj array

module JsArray =
  // https://github.com/fable-compiler/Fable/issues/433
  // https://github.com/fable-compiler/Fable/issues/1356
  let ofArray arr : JsArray = arr |> Array.ofSeq |> Array.map box

[<StringEnum>]
type BackEnd =
  | Wasm // Wasm backend breaks when set.
  | Cpu

type private TensorFlow =
  abstract setBackend: BackEnd -> JS.Promise<unit>

  abstract layers: Layers
  abstract tensor2d: JsArray * JsArray -> Tensor
  abstract sequential: unit -> Model

[<ImportAll("@tensorflow/tfjs")>]
let private tf: TensorFlow = jsNative

[<Import("setWasmPaths", "@tensorflow/tfjs-backend-wasm")>]
let private setWasmPaths (paths: string, usePlatformFetch: bool) : unit = jsNative

[<StringEnum>]
type Activation =
  | Relu
  | Softmax

type Dense =
  | Units of int
  | InputShape of JsArray
  | Activation of Activation

  static member props(ps: Dense list) = ps |> keyValueList CaseRules.LowerFirst

[<StringEnum>]
type LossOrMetricFn = | MeanSquaredError

[<StringEnum>]
type Optimizer =
  | [<CompiledName("sgd")>] StocasticGradientDecent
  | [<CompiledName("adagrad")>] AdaGrad
  | [<CompiledName("adadelta")>] AdaDelta
  | [<CompiledName("adamax")>] AdaMax
  | [<CompiledName("rmsprop")>] RmsProp
  | Momentum
  | Adam

type ModelCompileArgs =
  | Loss of LossOrMetricFn
  | Optimizer of Optimizer
  | Metrics of string

  static member props(ps: ModelCompileArgs list) = ps |> keyValueList CaseRules.LowerFirst

// Main public API
module Tf =
  let setWasmPaths = setWasmPaths
  let sequential = tf.sequential
  let setBackend = tf.setBackend
  let layers = tf.layers

  let tensor2d (vals: #seq<'a>, shape: #seq<'b>) =
    tf.tensor2d (JsArray.ofArray vals, JsArray.ofArray shape)
