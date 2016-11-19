﻿namespace ProjectCheckerTask.Test

open NUnit.Framework
open ProjectCheckerTask
open Foq
open FSharp.Data
open System.Xml.Linq
open System.IO
open System.Reflection

open RuleBase
open ProjectTypes

type MSBuildApiIncludesTests() = 
    let assemblyRunningPath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString()
    
    let cppProjectFile = """<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003" DefaultTargets="Build" ToolsVersion="4.0">
  <ItemGroup Label="ProjectConfigurations">
    <ProjectConfiguration Include="Debug|Win32">
      <Configuration>Debug</Configuration>
      <Platform>Win32</Platform>
    </ProjectConfiguration>
    <ProjectConfiguration Include="Debug|x64">
      <Configuration>Debug</Configuration>
      <Platform>x64</Platform>
    </ProjectConfiguration>
    <ProjectConfiguration Include="Release|Win32">
      <Configuration>Release</Configuration>
      <Platform>Win32</Platform>
    </ProjectConfiguration>
    <ProjectConfiguration Include="Release|x64">
      <Configuration>Release</Configuration>
      <Platform>x64</Platform>
    </ProjectConfiguration>
  </ItemGroup>
  <PropertyGroup Label="Globals">
    <ProjectGuid>{DA54BEC9-D5E5-4BD5-B232-586076AB0278}</ProjectGuid>
    <RootNamespace>libdia_units</RootNamespace>
    <SccProjectName>SurroundSCMScci</SccProjectName>
    <SccLocalPath>..</SccLocalPath>
    <SccProvider>MSSCCI:Surround SCM</SccProvider>
    <Keyword>Win32Proj</Keyword>
    <SccAuxPath />
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.Default.props" />
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|Win32'" Label="Configuration">
    <ConfigurationType>StaticLibrary</ConfigurationType>
    <CharacterSet>MultiByte</CharacterSet>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|Win32'" Label="Configuration">
    <ConfigurationType>StaticLibrary</ConfigurationType>
    <CharacterSet>MultiByte</CharacterSet>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Debug|x64'" Label="Configuration">
    <ConfigurationType>StaticLibrary</ConfigurationType>
    <CharacterSet>MultiByte</CharacterSet>
  </PropertyGroup>
  <PropertyGroup Condition="'$(Configuration)|$(Platform)'=='Release|x64'" Label="Configuration">
    <ConfigurationType>StaticLibrary</ConfigurationType>
    <CharacterSet>MultiByte</CharacterSet>
  </PropertyGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.props" />
  <ImportGroup Label="ExtensionSettings">
  </ImportGroup>
  <ImportGroup Condition="'$(Configuration)|$(Platform)'=='Debug|Win32'" Label="PropertySheets">
    <Import Project="$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props" Condition="exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')" Label="LocalAppDataPlatform" />
  </ImportGroup>
  <ImportGroup Condition="'$(Configuration)|$(Platform)'=='Release|Win32'" Label="PropertySheets">
    <Import Project="$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props" Condition="exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')" Label="LocalAppDataPlatform" />
  </ImportGroup>
  <ImportGroup Condition="'$(Configuration)|$(Platform)'=='Debug|x64'" Label="PropertySheets">
    <Import Project="$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props" Condition="exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')" Label="LocalAppDataPlatform" />
  </ImportGroup>
  <ImportGroup Condition="'$(Configuration)|$(Platform)'=='Release|x64'" Label="PropertySheets">
    <Import Project="$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props" Condition="exists('$(UserRootDir)\Microsoft.Cpp.$(Platform).user.props')" Label="LocalAppDataPlatform" />
  </ImportGroup>
  <ItemDefinitionGroup>
    <Link>
      <SubSystem>Console</SubSystem>
    </Link>
    <ClCompile>
      <AdditionalIncludeDirectories>c:\Windows</AdditionalIncludeDirectories>
    </ClCompile>
  </ItemDefinitionGroup>
  <PropertyGroup Label="UserMacros" />
  <ItemGroup>
    <ClCompile Include="file1.cpp" />
    <ClCompile Include="file2.cpp" />
  </ItemGroup>
  <ItemGroup>
    <ClInclude Include="header1.hpp" />
    <ClInclude Include="header2.hpp" />
    <Compile Include="Strings.cs" />
    <Compile Include="Strings.cs" />
  </ItemGroup>
  <Import Project="$(VCTargetsPath)\Microsoft.Cpp.targets" />
  <ImportGroup Label="ExtensionTargets">
  </ImportGroup>
</Project>"""

    let projectPath = Path.Combine(assemblyRunningPath, "project.vcxproj")

    [<SetUp>]
    member test.``Setup`` () = 
        File.WriteAllText(projectPath, cppProjectFile)
        
    [<TearDown>]
    member test.``TearUp`` () = 
        File.Delete(projectPath)

    [<Test>]
    member test.``Include Paths`` () = 
        let Rule = new IncludeFolderNotUsedDuringInCompilation()
        let msbuildproject = new Microsoft.Build.Evaluation.Project(projectPath)
        (Rule :> RuleBase).ExecuteCheckMsbuild(msbuildproject, projectPath, Array.empty, "", "", List.Empty)
        Assert.That((Rule :> RuleBase).GetIssues().Length, Is.EqualTo(6))
