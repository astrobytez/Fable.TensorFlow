module TensorFlow

open Fable.Core
open Fable.Core.JsInterop

type Backend =
    [<Import("setWasmPaths", "@tensorflow/tfjs-backend-wasm")>]
    static member setWasmPaths(paths: string, ?usePlatformFetch: bool) : unit = jsNative

type Tensor =
    abstract print: unit -> unit

type Model =
    abstract add: obj -> unit
    abstract compile: obj -> unit
    abstract fit: Tensor * Tensor * obj -> JS.Promise<unit>
    abstract predict: obj -> Tensor

// https://github.com/fable-compiler/Fable/issues/433
// https://github.com/fable-compiler/Fable/issues/1356
type JsArray = obj array

[<StringEnum>]
type BackEnd =
    | Wasm // Wasm backend breaks when set.
    | Cpu


[<StringEnum>]
type DataType =
    | Float32
    | Int32
    | Bool
    | Complex64
    | String

[<StringEnum>]
type Activation =
    | Elu
    | HardSigmoid
    | Linear
    | Relu
    | Relu6
    | Selu
    | Sigmoid
    | Softmax
    | Softplus
    | Softsign
    | Tanh
    | Swish
    | Mish
    | Gelu
    | Gelu_New

type Dense =
    | Units of int
    | InputShape of JsArray
    | Activation of Activation

    static member props(ps: Dense list) = ps |> keyValueList CaseRules.LowerFirst

type Layers =
    abstract dense: obj -> Dense

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
type TensorFlow =
    // Backends
    abstract setBackend: BackEnd -> JS.Promise<unit>

    // Layers API
    abstract layers: Layers

    // Tensors
    abstract tensor: values: JsArray * ?shape: JsArray * ?dtype: DataType -> Tensor
    abstract tensor2d: values: JsArray * ?shape: JsArray * ?dtype: DataType -> Tensor

    // Models
    abstract sequential: unit -> Model

[<ImportAll("@tensorflow/tfjs")>]
let Tf: TensorFlow = jsNative
