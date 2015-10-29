﻿namespace ProjectCheckerTask.Test

open NUnit.Framework
open Foq
open FSharp.Data
open System.Xml.Linq
open System.IO
open ProjectCheckerTask
open VSSolutionUtils
open System.Reflection

type TestAllCheck() = 
    let assemblyRunningPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString()
    [<Test>]
    member test.``Should Not Run for Know supported Project Type`` () = 
        let checker = new ProjectCheckerTask()
        checker.ExecuteAnalysisOnProjectFile(assemblyRunningPath +  "/project.csproj", "")
        let issues = checker.GetAllIssues()
        Assert.That(issues.Length, Is.EqualTo(1))

