# Changelog

All notable changes to the Ampere project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.9.0] - 2026-03-29

### Added
- .NET 10 target framework support (multi-targeting `net8.0` and `net10.0`)
- `AppendIf` extension method for conditional `StringBuilder` operations
- `SubstringRng` method for range-based substring extraction
- `ReplaceRange` method with multiple overloads for string replacement within ranges
- `Fuse` method (renamed from `Concat`) for enumerable operations
- `CheckBufferAndExtend` utility methods for buffer management
- New overloads with conditional execution support across utility methods
- Additional statistics data analysis tools

### Changed
- Upgraded from .NET 5 to .NET 8 and .NET 10
- Renamed `Concat` to `Fuse` in enumerable utilities
- Improved `FileUtils` with additional enhancements
- Optimized various utility methods for performance
- Updated DocFX to newer version for documentation generation
- Updated copyright to 2026

### Removed
- Removed redundant `StringBuilder` wrapper methods
- Removed deprecated `Replacer.cs` class
- Cleaned up legacy code in `StringUtils`

### Fixed
- Fixed broken shuffle implementation in `Shuffler.cs`
- Fixed compile-time errors across the project
- Fixed DocFX generating stale documentation data
- Fixed project icon file reference

## [0.1.0] - 2021-03-01

### Added
- Initial release of the Ampere Library
- `EnumerableUtils` with collection manipulation utilities
- `StringUtils` and `StringBuilderUtils` for string operations
- `FileUtils` for file system operations
- `Matrix` class with mathematical operations
- `Shuffler` for collection shuffling algorithms
- `Range`, `IntRange`, and `ImmutableRange` types
- `EnumerableStats` for statistical operations on collections
- Cron expression utilities
- DocFX-powered documentation site
- MSTest unit test project
