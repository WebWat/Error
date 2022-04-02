open System

open Error

let array = readFile "C:\Users\sereg\Desktop\1.txt"
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
printfn "Средняя квадратичая погрешность (оценка погрешности всей серии измерений):
%f" standerr
printfn "Средняя абсолютная погрешность:
%f" abs
printfn "Абсолютная погрешность измерений:
elem = (%0.3f +- %0.3f), %s" average abs (nameof uConfidenceLevel.p95)

DisplayHistorgramDataTable array

DisplayRatioAccuracyTable array average stand

Console.ReadKey true |> ignore