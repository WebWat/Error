open System
open Spectre.Console
open System.Collections.Generic
open System.IO

type uConfidenceLevel =
    | p80 = 0
    | p90 = 1
    | p95 = 2
    | p99 = 3


let readFile (path:string) : float[] = 
    let list = List<float>()
    use reader = new StreamReader(path)

    let mutable line = ""

    line <- reader.ReadLine()

    while line <> null do
        list.Add(float(line))
        line <- reader.ReadLine()

    list.ToArray()
    
// Trash
let generateRandomMeasures (min: int) (max: int) (decimalPlace: int) (count: int) : float[] =
    let random = Random()
    Array.init count (
        fun _ -> 
        let first = random.Next(min, max)
        let second = random.NextDouble() * pown 10.0 decimalPlace

        float(first) + float(int(second)) / pown 10.0 decimalPlace
    )

let arithmeticAverage (measures: float[]) : float =
    Array.sum measures / float(measures.Length)
 
// Случайную погрешность отдельного измерения характеризует так называемое 
// среднеквадратическое отклонение
let standardDeviation (measures: float[]) (average: float) : float =
    let sum: float = Array.sumBy (fun x -> (x - average) * (x - average)) measures
    sqrt(sum/(float(measures.Length) - 1.0))

// Для оценки погрешности всей серии измерений, вместо отдельного измерения надо 
// найти среднюю квадратичную погрешность среднего арифметического
let standardError (measures: float[]) (average: float) : float =
    let sum: float = Array.sumBy (fun x -> (x - average) * (x - average)) measures
    sqrt(sum/(float(measures.Length) * (float(measures.Length) - 1.0)))

// https://www.chem-astu.ru/science/reference/t-statistic.html
let getStudentCoefficient (measuresLength: int) (confidenceLevel: uConfidenceLevel) : float =
    match confidenceLevel with
    | uConfidenceLevel.p95 -> match measuresLength with
                              | 1 -> 12.7060
                              | 2 -> 4.3020
                              | 3 -> 3.182
                              | 4 -> 2.776
                              | 5 -> 2.570
                              | 6 -> 2.4460
                              | 7 -> 2.3646
                              | 8 -> 2.3060
                              | 9 -> 2.2622
                              | 10 -> 2.2281
                              | 11 -> 2.201
                              | 12 -> 2.1788
                              | 13 -> 2.1604
                              | 14 -> 2.1448
                              | 15 -> 2.1314
                              | 16 -> 2.1190
                              | 17 -> 2.1098
                              | 18 -> 2.1009
                              | 19 -> 2.0930
                              | 20 -> 2.08600
                              | 30 -> 2.0423
                              | measuresLength -> 0

    | confidenceLevel -> 0

// Средняя абсолютная погрешность
let AverageAbsoluteError (stDeviation: float) (measuresLength: int) (confidenceLevel: uConfidenceLevel) =
    stDeviation * getStudentCoefficient measuresLength confidenceLevel

let DisplayHistorgramDataTable (measures: float[]) : unit =
    let table = new Table();

    let del = ((measures |> Array.max) - (measures |> Array.min)) / 7.0
    let mutable first = measures |> Array.min

    table.AddColumn("№") |> ignore
    table.AddColumn("Interval") |> ignore
    table.AddColumn("deln") |> ignore // результатов наблюдений Δn, попавших в
                                      // каждый интервал
    table.AddColumn("deln/(n*delt)") |> ignore // значения плотности вероятности попадания
                                               // случайной величины в интервал

    for i = 1 to 7 do
        let deln = Array.where (fun x -> x > first && x < first + del) measures 
                   |> Array.length

        let density = float(deln)/float(measures.Length)

        table.AddRow(
            string(i), 
            $"({first:f3}; {(first + del):f3})", 
            string(deln),
            string(density)
        ) |> ignore
        
        first <- first + del

    AnsiConsole.Write(table);

let DisplayRatioAccuracyTable (measures: float[]) (average: float) (stDeviation: float) : unit =
    let table = new Table();

    let addRow (number: int) (left: float) (right: float) (a: float) = 
        let deln = Array.where (fun x -> x > left && x < right) measures 
                   |> Array.length

        let density = float(deln)/float(measures.Length)

        table.AddRow(
            string(number), 
            $"({left:f3}; {right:f3})", 
            string(deln),
            string(density),
            string(a)
        ) |> ignore        

    table.AddColumn("№") |> ignore
    table.AddColumn("Interval") |> ignore
    table.AddColumn("deln") |> ignore
    table.AddColumn("deln/n") |> ignore
    table.AddColumn("a") |> ignore

    addRow 1 (average - stDeviation) (average + stDeviation) 0.68
    addRow 2 (average - 2.0 * stDeviation) (average + 2.0 * stDeviation) 0.95
    addRow 3 (average - 3.0 * stDeviation) (average + 3.0 * stDeviation) 0.997

    AnsiConsole.Write(table);



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