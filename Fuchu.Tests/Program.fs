﻿open System
open Fuchu
open NUnit.Framework

let tests = 
    "" ->> [
        "basic" --> 
            fun () -> Assert.AreEqual(4, 2+2)
        "sumTestResults" ->> [
            let sumTestResultsTests = 
                [
                    { TestRunResult.Name = ""; Result = Passed; Time = TimeSpan.FromMinutes 2. }
                    { TestRunResult.Name = ""; Result = Exception (ArgumentException()); Time = TimeSpan.FromMinutes 3. }
                    { TestRunResult.Name = ""; Result = Failed ""; Time = TimeSpan.FromMinutes 4. }
                    { TestRunResult.Name = ""; Result = Passed; Time = TimeSpan.FromMinutes 5. }
                    { TestRunResult.Name = ""; Result = Failed ""; Time = TimeSpan.FromMinutes 6. }
                    { TestRunResult.Name = ""; Result = Passed; Time = TimeSpan.FromMinutes 7. }
                ]
            let r = sumTestResults sumTestResultsTests
            yield "Passed" -->
                fun () -> Assert.AreEqual(3, r.Passed)
            yield "Failed" -->
                fun () -> Assert.AreEqual(2, r.Failed)
            yield "Exception" -->
                fun () -> Assert.AreEqual(1, r.Errored)
            yield "Time" -->
                fun () -> Assert.AreEqual(TimeSpan.FromMinutes 27., r.Time)
        ]
        "TestResultCounts" ->> [
            let c1 = { Passed = 1; Ignored = 5; Failed = 2; Errored = 3; Time = TimeSpan.FromSeconds 20. }
            yield "plus" ->> [
                let c2 = { Passed = 2; Ignored = 6; Failed = 3; Errored = 4; Time = TimeSpan.FromSeconds 25. }
                let r = c1 + c2
                yield "Passed" --> fun () -> Assert.AreEqual(3, r.Passed)
                yield "Ignored" --> fun () -> Assert.AreEqual(11, r.Ignored)
                yield "Failed" --> fun () -> Assert.AreEqual(5, r.Failed)
                yield "Errored" --> fun () -> Assert.AreEqual(7, r.Errored)
                yield "Time" --> fun () -> Assert.AreEqual(TimeSpan.FromSeconds 45., r.Time)
            ]
            yield "ToString" --> 
                fun () -> Assert.AreEqual("6 tests run: 1 passed, 5 ignored, 2 failed, 3 errored (00:00:20)\n", c1.ToString())
        ]
        "Exception handling" ->> [
            "NUnit ignore" --> 
                fun () ->
                    let ignore2 _ _ = ()
                    let ignore3 _ _ _ = ()
                    let eval = eval ignore ignore2 ignore2 ignore3 ignore3 Seq.map
                    let test() = Assert.Ignore "a"
                    let test = TestCase test
                    match eval  test with
                    | [{ Result = Ignored "a" }] -> ()
                    | x -> Assert.Fail "Wrong test evaluation"
        ]
    ]

[<EntryPoint>]
let main args =
    run tests