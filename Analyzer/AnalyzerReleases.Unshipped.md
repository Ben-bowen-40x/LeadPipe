; Unshipped analyzer release
; https://github.com/dotnet/roslyn/blob/main/src/RoslynAnalyzers/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|------
FT0001  | Usage    | Error    | Do not use Unix Time converters for DateTimeOffset. Use ToUnixTime() or FromUnixTime() instead.
FT0002  | Usage    | Error    | Do not use ToHashSet. Use ToHashSetFast() instead.
FT0003  | Usage    | Error    | Do not use ToDictionary. Use ToDictionaryFast() instead.
FT0004  | Usage    | Warning  | Unreachable code detected
FT0005  | Usage    | Warning  | Unused local variable