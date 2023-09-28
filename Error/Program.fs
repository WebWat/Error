open System
open Error

let array = [|2.998; 2.954; 3.135; 2.775; 3.005; 2.86; 3.058; 2.666; 2.834; 2.735; 2.842;
2.918; 2.875; 2.961; 3.058; 2.791; 2.79; 2.876; 2.965; 2.755; 2.712; 2.912;
2.971; 2.706; 3.149; 2.551; 2.871; 3.062; 2.821; 2.686|]

let average    = arithmeticAverage array
let stdDev     = standardDeviation array average
let stdDevMean = standardDeviationOfMean array average
let abs        = averageAbsoluteError stdDevMean array.Length uConfidenceLevel.p95

printfn "Data array:
%A" array
printfn "Arithmetic mean:
%f" average
printfn "Standard deviation:
%f" stdDev
printfn "Standard deviation of the mean:
%f" stdDevMean
printfn "Average absolute error:
%f" abs
printfn "Absolute error of measurement:
x = (%0.3f +- %0.3f), %s\n" average abs (nameof uConfidenceLevel.p95)

displayHistorgramAndDataTable array 7

displayRatioAccuracyTable array average stdDev

Console.ReadKey true |> ignore
