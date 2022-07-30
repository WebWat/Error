module Error

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
        list.AddRange(Array.map (fun x -> float(x)) (line.Split(" ")))
        line <- reader.ReadLine()

    list.ToArray()


let arithmeticAverage (measures: float[]) : float =
    Array.sum measures / float(measures.Length)
 

let standardDeviation (measures: float[]) (average: float) : float =
    let sum: float = Array.sumBy (fun x -> (x - average) * (x - average)) measures
    sqrt(sum/(float(measures.Length) - 1.0))


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
                              | 40 -> 2.0211
                              | 50 -> 2.0086
                              | 100 -> 1.9840
                              | 120 -> 1.9719
                              | 270 -> 1.9695
                              | 500 -> 1.9640
                              | 900 -> 1.9600
                              | measuresLength -> 0

    | confidenceLevel -> 0


let AverageAbsoluteError (stDeviation: float) (measuresLength: int) (confidenceLevel: uConfidenceLevel) =
    stDeviation * getStudentCoefficient measuresLength confidenceLevel


let DisplayHistorgramAndDataTable (measures: float[]) (rows: int) : unit =
    let table = Table();

    let del = ((measures |> Array.max) - (measures |> Array.min)) / float(rows)
    let mutable first = measures |> Array.min

    table.AddColumn("№") |> ignore
    table.AddColumn("Границы интервалов") |> ignore
    table.AddColumn("deln") |> ignore // результатов наблюдений Δn, попавших в каждый интервал
    table.AddColumn("deln/(n delt)") |> ignore // значения плотности вероятности попадания
                                               // случайной величины в интервал
    for i = 1 to rows do
        let deln = Array.where (fun x -> x > first && x < first + del) measures 
                   |> Array.length

        let density = float(deln)/float(measures.Length)

        printfn $"({first:f3}; {(first + del):f3}): {String('=', int(density * 100.0))}"

        table.AddRow(
            string(i), 
            $"({first:f3}; {(first + del):f3})", 
            string(deln),
            $"{density:f3}"
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
            $"{density:f3}",
            string(a)
        ) |> ignore        

    table.AddColumn("№") |> ignore
    table.AddColumn("Границы интервалов") |> ignore
    table.AddColumn("deln") |> ignore
    table.AddColumn("deln/n") |> ignore
    table.AddColumn("a") |> ignore

    addRow 1 (average - stDeviation) (average + stDeviation) 0.68
    addRow 2 (average - 2.0 * stDeviation) (average + 2.0 * stDeviation) 0.95
    addRow 3 (average - 3.0 * stDeviation) (average + 3.0 * stDeviation) 0.997

    AnsiConsole.Write(table);


let saveOutput (measures: float[]) : unit =
    let random = Random();
    use writer = new StreamWriter(File.Create($"C:\\Users\\sereg\\Desktop\\report_{random.Next(100, 999)}.txt"))

    let average = arithmeticAverage measures
    let stand = standardDeviation measures average
    let standerr = standardError measures average
    let abs = AverageAbsoluteError standerr measures.Length uConfidenceLevel.p95

    writer.WriteLine($"Число данных:\n{measures.Length}")
    writer.WriteLine("Массив:")
    for i = 0 to 15 do
        writer.Write($"{measures[i]}, ")
    writer.WriteLine("...")
    writer.WriteLine($"Среднее арифметическое:\n{average:f3}")
    writer.WriteLine($"Среднеквадратическое отклонение (случайная погрешность отдельного измерения):\n{stand:f3}")
    writer.WriteLine($"Средняя квадратичая погрешность (оценка погрешности всей серии измерений):\n{standerr:f3}")
    writer.WriteLine($"Средняя абсолютная погрешность:\n{abs:f3}")
    writer.WriteLine($"Абсолютная погрешность измерений:\nx = ({average:f3} +- {abs:f3}), {(nameof uConfidenceLevel.p95)}\n")

    writer.Flush()