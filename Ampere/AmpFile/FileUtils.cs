using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Ampere.AmpFile;

/// <summary>
/// A static class for File utility functions.
/// </summary>
public static class FileUtils
{
    /// <summary>
    /// Appends a string value into the file.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="value">The string value to write</param>
    public static void WriteLine(FileInfo fileInfo, string value)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(value);

        using var file = new StreamWriter(fileInfo.FullName, append: true);
        if (new FileInfo(fileInfo.FullName).Length > 0)
        {
            var lastLine = File.ReadLines(fileInfo.FullName).LastOrDefault();
            if (!string.IsNullOrEmpty(lastLine))
                file.WriteLine();
        }
        file.WriteLine(value);
    }

    /// <summary>
    /// Replaces all instances of a specific value from a file with another replacement value.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="oldValue">The value to replace</param>
    /// <param name="replacementValue">The replacement value</param>
    public static void ReplaceAll(FileInfo fileInfo, string oldValue, string replacementValue)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(oldValue);
        ArgumentNullException.ThrowIfNull(replacementValue);

        var text = File.ReadAllText(fileInfo.FullName);
        text = text.Replace(oldValue, replacementValue);
        File.WriteAllText(fileInfo.FullName, text);
    }

    /// <summary>
    /// Replaces all instances of a specific value from a file with another replacement value if and only if
    /// the old value is solely in one line.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="oldValue">The value to replace</param>
    /// <param name="replacementValue">The replacement value</param>
    public static void ReplaceAllByLine(FileInfo fileInfo, string oldValue, string replacementValue)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        File.WriteAllLines(fileInfo.FullName,
            File.ReadLines(fileInfo.FullName)
                .Select(l => l == oldValue ? replacementValue : l)
                .ToList());
    }

    /// <summary>
    /// Replaces all instances of a specific value from a file with another replacement value from a specified line.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="oldValue">The value to replace</param>
    /// <param name="replacementValue">The replacement value</param>
    /// <param name="line">The line number to replace from</param>
    public static void ReplaceAllInLine(FileInfo fileInfo, string oldValue, string replacementValue, int line)
    {
        ReplaceInLines(fileInfo, new Dictionary<KeyValuePair<string, string>, int>()
        {
            {
                new KeyValuePair<string, string>(oldValue, replacementValue),
                line
            }
        });
    }

    /// <summary>
    /// Replaces all instances of a specific value from a file with another replacement value from a specified line.
    /// This overload facilitates the replacement through a Dictionary where the key is an instance of
    /// <see cref="KeyValuePair{TKey,TValue}"/> and the value is an int. This allows for unique replacements to occur
    /// in more than one line.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="replacementDict">A Dictionary of replacement values and line numbers</param>
    public static void ReplaceInLines(FileInfo fileInfo, Dictionary<KeyValuePair<string, string>, int> replacementDict)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(replacementDict);

        var arrLine = File.ReadAllLines(fileInfo.FullName);
        foreach (var ((key, s), value) in replacementDict)
        {
            arrLine[value - 1] = arrLine[value - 1].Replace(key, s);
        }
        File.WriteAllLines(fileInfo.FullName, arrLine);
    }

    /// <summary>
    /// Replaces an entire line with a replacement value.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="replacementValue">The replacement value</param>
    /// <param name="line">The line number to replace from</param>
    public static void ReplaceLine(FileInfo fileInfo, string replacementValue, int line)
    {
        ReplaceLines(fileInfo, new Dictionary<string, int> { { replacementValue, line } });
    }

    /// <summary>
    /// Replace an entire line with a replacement value. This overload uses a Dictionary of replacement values
    /// and line numbers to replace more than one line.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="replacementValueLine">A Dictionary of replacement values and line number</param>
    public static void ReplaceLines(FileInfo fileInfo, Dictionary<string, int> replacementValueLine)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(replacementValueLine);

        var arrLine = File.ReadAllLines(fileInfo.FullName);
        foreach (var (key, value) in replacementValueLine)
        {
            arrLine[value - 1] = key;
        }
        File.WriteAllLines(fileInfo.FullName, arrLine);
    }

    /// <summary>
    /// Removes all instances of a specific value from a file if and only if the value is solely in one line.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="valToRemove">The value to remove</param>
    public static void RemoveFromEachLine(FileInfo fileInfo, string valToRemove)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(valToRemove);

        File.WriteAllLines(fileInfo.FullName,
            File.ReadLines(fileInfo.FullName)
                .Where(l => l != valToRemove)
                .ToList());
    }

    /// <summary>
    /// Removes a specific line number from a file.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="line">The line number to remove</param>
    public static void RemoveLine(FileInfo fileInfo, int line)
    {
        RemoveLines(fileInfo, line);
    }

    /// <summary>
    /// Removes a variable argument number of lines from a file.
    /// Lines are removed correctly regardless of order by sorting indices in descending order.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write the value to</param>
    /// <param name="lines">The line numbers to remove (1-based)</param>
    public static void RemoveLines(FileInfo fileInfo, params int[] lines)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(lines);

        var fileAsList = File.ReadAllLines(fileInfo.FullName).ToList();
        foreach (var line in lines.OrderByDescending(l => l))
        {
            fileAsList.RemoveAt(line - 1);
        }
        File.WriteAllLines(fileInfo.FullName, fileAsList);
    }

    /// <summary>
    /// Returns the line of the matched predicate in the file. If the predicate is not found,
    /// -1 is returned.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to read from</param>
    /// <param name="predicate">The function predicate to find in the file</param>
    /// <returns>The 1-based line number, or -1 if not found</returns>
    public static int FindInFile(FileInfo fileInfo, Func<string, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(predicate);

        using var file = new StreamReader(fileInfo.FullName, detectEncodingFromByteOrderMarks: true);
        var count = 1;

        while (file.ReadLine() is { } line)
        {
            if (predicate(line))
                return count;
            count++;
        }

        return -1;
    }

    /// <summary>
    /// Returns all 1-based line numbers matching the predicate.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to read from</param>
    /// <param name="predicate">The function predicate to match</param>
    /// <returns>An enumerable of 1-based line numbers</returns>
    public static IEnumerable<int> FindAllInFile(FileInfo fileInfo, Func<string, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(predicate);

        var results = new List<int>();
        using var reader = new StreamReader(fileInfo.FullName, detectEncodingFromByteOrderMarks: true);
        var count = 1;
        while (reader.ReadLine() is { } line)
        {
            if (predicate(line))
                results.Add(count);
            count++;
        }
        return results;
    }

    /// <summary>
    /// Returns the value found at a line number.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to read from</param>
    /// <param name="line">The line number to find (1-based)</param>
    /// <returns>The trimmed content at the specified line</returns>
    public static string GetValueAtLine(FileInfo fileInfo, int line) =>
        File.ReadAllLines(fileInfo.FullName)[line - 1].Trim();

    /// <summary>
    /// Reads a range of lines from the file (1-based, inclusive on both ends).
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to read from</param>
    /// <param name="startLine">The starting line number (1-based, inclusive)</param>
    /// <param name="endLine">The ending line number (1-based, inclusive)</param>
    /// <returns>An array of lines in the specified range</returns>
    public static string[] ReadLineRange(FileInfo fileInfo, int startLine, int endLine)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentOutOfRangeException.ThrowIfLessThan(startLine, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(endLine, startLine);

        return File.ReadLines(fileInfo.FullName)
            .Skip(startLine - 1)
            .Take(endLine - startLine + 1)
            .ToArray();
    }

    /// <summary>
    /// Efficiently counts the number of lines in a file without loading the entire content into memory.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to read from</param>
    /// <returns>The number of lines in the file</returns>
    public static int GetLineCount(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        int count = 0;
        using var reader = new StreamReader(fileInfo.FullName);
        while (reader.ReadLine() is not null)
            count++;
        return count;
    }

    /// <summary>
    /// Computes the SHA-256 hash of a file's contents.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to hash</param>
    /// <returns>The hex-encoded SHA-256 hash string</returns>
    public static string ComputeSha256Hash(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        using var stream = File.OpenRead(fileInfo.FullName);
        var hashBytes = SHA256.HashData(stream);
        return Convert.ToHexString(hashBytes);
    }

    /// <summary>
    /// Writes text to a file atomically by writing to a temporary file first, then replacing
    /// the target file. This prevents data corruption if the process is interrupted mid-write.
    /// </summary>
    /// <param name="fileInfo">The target file</param>
    /// <param name="content">The content to write</param>
    public static void SafeWriteAllText(FileInfo fileInfo, string content)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(content);

        var dir = fileInfo.DirectoryName ?? throw new InvalidOperationException("File has no parent directory.");
        var tempPath = Path.Combine(dir, Path.GetRandomFileName());
        try
        {
            File.WriteAllText(tempPath, content);
            File.Move(tempPath, fileInfo.FullName, overwrite: true);
        }
        catch
        {
            if (File.Exists(tempPath))
                File.Delete(tempPath);
            throw;
        }
    }

    /// <summary>
    /// Reads the entire contents of a file asynchronously.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to read from</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>The file contents as a string</returns>
    public static async Task<string> ReadAllTextAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        return await File.ReadAllTextAsync(fileInfo.FullName, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Writes text to a file asynchronously.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to write to</param>
    /// <param name="content">The content to write</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    public static async Task WriteAllTextAsync(FileInfo fileInfo, string content, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(content);
        await File.WriteAllTextAsync(fileInfo.FullName, content, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads all lines of a file asynchronously.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to read from</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>An array of all lines in the file</returns>
    public static async Task<string[]> ReadAllLinesAsync(FileInfo fileInfo, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        return await File.ReadAllLinesAsync(fileInfo.FullName, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Creates the file if it does not exist, or updates its last-write timestamp if it does.
    /// Similar to the Unix <c>touch</c> command.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance</param>
    public static void TouchFile(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        if (!fileInfo.Exists)
        {
            EnsureDirectoryExists(fileInfo);
            using var _ = File.Create(fileInfo.FullName);
        }
        else
        {
            fileInfo.LastWriteTimeUtc = DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Ensures the parent directory of the specified file exists, creating it if necessary.
    /// </summary>
    /// <param name="fileInfo">The FileInfo whose parent directory should be ensured</param>
    public static void EnsureDirectoryExists(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        var dir = fileInfo.Directory;
        if (dir is not null && !dir.Exists)
            dir.Create();
    }

    /// <summary>
    /// Copies a file to the specified destination, optionally overwriting an existing file.
    /// </summary>
    /// <param name="fileInfo">The source file</param>
    /// <param name="destinationPath">The destination file path</param>
    /// <param name="overwrite">Whether to overwrite an existing file at the destination</param>
    /// <returns>A FileInfo for the new file</returns>
    public static FileInfo CopyTo(FileInfo fileInfo, string destinationPath, bool overwrite = false)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(destinationPath);

        return fileInfo.CopyTo(destinationPath, overwrite);
    }

    /// <summary>
    /// Moves a file to the specified destination.
    /// </summary>
    /// <param name="fileInfo">The source file</param>
    /// <param name="destinationPath">The destination file path</param>
    /// <param name="overwrite">Whether to overwrite an existing file at the destination</param>
    public static void MoveTo(FileInfo fileInfo, string destinationPath, bool overwrite = false)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);
        ArgumentNullException.ThrowIfNull(destinationPath);

        fileInfo.MoveTo(destinationPath, overwrite);
    }

    /// <summary>
    /// Checks whether a file is currently locked by another process.
    /// </summary>
    /// <param name="fileInfo">The FileInfo instance to check</param>
    /// <returns>True if the file is locked, false otherwise</returns>
    public static bool IsFileLocked(FileInfo fileInfo)
    {
        ArgumentNullException.ThrowIfNull(fileInfo);

        if (!fileInfo.Exists) return false;

        try
        {
            using var stream = fileInfo.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
    }

    /// <summary>
    /// Returns the size of a directory in bytes, given an abstract file path.
    /// </summary>
    /// <param name="dirPath">The path to the directory</param>
    /// <returns>The size of the directory in bytes</returns>
    public static long GetDirectorySize(this string dirPath)
    {
        ArgumentNullException.ThrowIfNull(dirPath);

        var dirInfo = new DirectoryInfo(dirPath);
        long length = dirInfo.EnumerateFiles("*", SearchOption.AllDirectories)
            .Sum(fi => fi.Length);
        return length;
    }

    /// <summary>
    /// Returns the size of file in bytes, given an abstract file path.
    /// </summary>
    /// <param name="filePath">The path to the file</param>
    /// <returns>The size of the file in bytes</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetFileSize(this string filePath) => new FileInfo(filePath).Length;

    /// <summary>
    /// Returns a pathname to the root directory of the System.
    /// </summary>
    /// <returns>A pathname to the root directory of the System</returns>
    public static string? GetRootPath() => Path.GetPathRoot(Environment.SystemDirectory);

    /// <summary>
    /// Returns a pathname to the user's profile folder.
    /// </summary>
    /// <returns>A pathname to the user's profile folder</returns>
    public static string GetUserPath() => Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
}
