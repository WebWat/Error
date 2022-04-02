open System
open Spectre.Console
open System.Collections.Generic
open System.IO
open Error


let array = readFile "C:\Users\sereg\Desktop\1.txt"
let average = arithmeticAverage array
let stand = standardDeviation array average
let standerr = standardError array average
let abs = AverageAbsoluteError standerr array.Length uConfidenceLevel.p95

printfn "Array: %A" array
printfn "Average: %f" average
printfn "StandardDeviation: %f" stand
printfn "StandardError: %f" standerr
printfn "AverageAbsoluteError: %f" abs
printfn "ErrorElem: elem = (%f +- %f) %s" average abs (nameof uConfidenceLevel.p95)

DisplayHistorgramDataTable array

DisplayRatioAccuracyTable array average stand

Console.ReadKey true |> ignore