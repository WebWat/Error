open System

open Error

let array = [|2.998; 2.954; 3.135; 2.775; 3.005; 2.86; 3.058; 2.666; 2.834; 2.735; 2.842;
2.918; 2.875; 2.961; 3.058; 2.791; 2.79; 2.876; 2.965; 2.755; 2.712; 2.912;
2.971; 2.706; 3.149; 2.551; 2.871; 3.062; 2.821; 2.686|]
let average = arithmeticAverage array
let stand = standardDeviation array average
let standerr = standardError array average
let abs = AverageAbsoluteError standerr array.Length uConfidenceLevel.p95

printfn "Массив данных:
%A" array
printfn "Среднее арифметическое:
%f" average
printfn "Среднеквадратическое отклонение (случайная погрешность отдельного измерения):
%f" stand
printfn "Средняя квадратичная погрешность (оценка погрешности всей серии измерений):
%f" standerr
printfn "Средняя абсолютная погрешность:
%f" abs
printfn "Абсолютная погрешность измерений:
x = (%0.3f +- %0.3f), %s\n" average abs (nameof uConfidenceLevel.p95)

DisplayHistorgramAndDataTable array 7

DisplayRatioAccuracyTable array average stand

saveOutput array

Console.ReadKey true |> ignore
