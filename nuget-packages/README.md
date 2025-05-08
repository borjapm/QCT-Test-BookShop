# Bookstore.Domain NuGet Packages

This directory contains NuGet packages for the Bookstore.Domain project targeting different frameworks:

## Available Packages

1. **Bookstore.Domain.1.0.0.nupkg**
   - Targets .NET Framework 4.8
   - Use for .NET Framework projects

2. **Bookstore.Domain.8.0.0.nupkg**
   - Targets .NET 8.0
   - Use for .NET 8 projects

3. **Bookstore.Domain.8.1.0.nupkg**
   - Multi-targeted package supporting both .NET Framework 4.8 and .NET 8.0
   - Use when you need to support both frameworks

## Usage

To use these packages in your project, you can add them as a local NuGet source:

```
nuget sources add -name "BobsBookstoreLocal" -source "[path-to-this-directory]"
```

Then install the appropriate package:

```
dotnet add package Bookstore.Domain --version [version]
```

Or add a reference in your project file:

```xml
<PackageReference Include="Bookstore.Domain" Version="[version]" />
```

## Building the Packages

To rebuild these packages, use the following commands from the Bookstore.Domain directory:

```
dotnet pack Bookstore.Domain.NetFramework.csproj -c Release
dotnet pack Bookstore.Domain.Net8.csproj -c Release
dotnet pack Bookstore.Domain.MultiTarget.csproj -c Release
```