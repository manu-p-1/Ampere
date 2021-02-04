# Ampere Contribution Guidelines

Ampere is only as good as its contributors and we are excited to help you make positive change. When contributing to this repository, please first discuss the change you wish to make via issue,
email, or any other method with the owners of this repository before making a change.

# Pull Request Process

1. Ensure any install or build dependencies are removed before the end of the layer when doing a 
   build.
2. Update the README.md with details of changes to the interface, this includes new environment 
   variables, exposed ports, useful file locations and container parameters.
3. Follow the Pull Request template carefully. Certain changes may *not* require you to answer all of the questions, however, an in-depth discussion about the changes goes a long way.
4. You may merge the Pull Request in once you have the sign-off of two other developers, or if you 
   do not have permission to do that, you may request the second reviewer to merge it for you.

## Do's and Don'ts

**Do** follow .NET coding style guidelines and conventions  
**Do** document all of your changes (screenshots are helpful)  
**Do** be engaged  
**Do** test your changes (integration and regression)  
**Do** respond to feedback  

**Don't** push large changes - no one likes to read a 5000 line diff  
**Don't** duplicate or repeat yourself  
**Don't** discourage others  

# Code of Conduct
Contributors must strictly adhere to the Ampere code of conduct mentioned here: [Code of Conduct](https://github.com/manu-p-1/Ampere/blob/master/CODE_OF_CONDUCT.md)

# Contribution
There are a variety of ways that you can contribute to Ampere for the betterment of developers. There are four main components to contribute to:

1. Utilities
2. Ampere DocFX
3. GitHub Repo
4. Other (Scripting, CI/CD, Unit Testing, etc...)

We'll discuss how to contribute to each one in detail below.

## Utilities
Customized utility classes and methods form the foundation of Ampere. The code is divided up by category within the Ampere project. If you find that a utility does not exist or various classes need to be written to accomplish the task, consider finding the right category to place your code. Once you decide on the directory structure and it's been approved by Ampere maintainers, you're free to type away!

### Environment Information
It is important to note that Ampere only supports the [.NET 5 runtime](https://dotnet.microsoft.com/download/dotnet/5.0). After you setup .NET 5 with Visual Studio, you'll be able to fork and clone the repository. The project can be built using `dotnet build` or Visual Studio and the output will display the `AssemblyPath`. The AssemblyPath is a `.dll` file is created within the `bin` folder of the repository; this file can be imported into your Visual Studio Proejcts. If a debug executable has not been setup already, follow these steps:

### Utility Documentation
Make sure to document all information using the XML format on top of each class, property and method. An example is provided below:

```csharp
/// <summary>
/// Inserts the specified element at the specified index in the enumerable (modifying the original enumerable).
/// If element at that position exits, If shifts that element and any subsequent elements to the right,
/// adding one to their indices. The method also allows for inserting more than one element into
/// the enumerable at one time given that they are specified. This Insert method is functionally similar
/// to the Insert method of the List class. <see cref="System.Collections.IList.Insert(int, object)"/>
/// for information about the add method of the List class.
/// </summary>
/// <typeparam name="T">The type of the enumerable</typeparam>
/// <param name="src">The IEnumerable to be used</param>
/// <param name="startIdx">The index to start insertion</param>
/// <param name="amtToIns">The amount of elements to insert into the enumerable</param>
/// <param name="valuesToIns">Optionally, the values to insert into the empty indices of the new enumerable</param>
/// <returns>An enumerable of the elements inserted into the enumerable, if any</returns>
/// <exception cref="IndexOutOfRangeException">Thrown when the valuesToIns enumerable does not match the amount to insert (if it is greater than 0)</exception>
/// <exception cref="IndexOutOfRangeException">Thrown when the amtToIns or the startIdx is less than 0</exception>
/// <example>This sample shows how to call the <see cref="Insert{T}"/> method.</example>
/// <seealso cref="System.Collections.IList.Insert(int, object)"/>
/// <code>
///
/// using static Utilities.EnumerableUtils;
///
/// class TestClass
/// {
///     static void Main(string[] args)
///     {
///         var w = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10}.AsEnumerable();
///         Insert(ref w, 1, 3);
///         //Printing out 'w' results in: 2, 0, 0, 0, 3, 4, 5, 6, 7, 8, 9, 10
///
///         var y = new int[9] {2, 3, 4, 5, 6, 7, 8, 9, 10}.AsEnumerable();
///         Insert(ref y, 1, 3, 250, 350, 450);
///         //Printing out 'y' results in: 2, 250, 350, 450, 3, 4, 5, 6, 7, 8, 9, 10
///     }
/// }
/// </code>
public static IEnumerable<T> Insert<T>(ref IEnumerable<T> src, int startIdx, int amtToIns, params T[] valuesToIns)
{ ... }
```

## DocFX
Since all Ampere methods are documented using .NET XML documentation. This is compiled using [DocFX](https://dotnet.github.io/docfx/) with the `docfx.json` file under the [DocFX folder](https://github.com/manu-p-1/Ampere/tree/master/DocFx). DocFX creates static HTML pages which are used by <https://powerplug.me>. If you would like to change documentation on this website after understanding the DocFX file structure, you can contribute it at the top-level DocFX folder.

### Building DocFX
Download the DocFX executable from their website. For convenience, it is recommended that the executable be added to the System environment variable PATH. [Here's a tutorial](https://www.c-sharpcorner.com/article/add-a-directory-to-path-environment-variable-in-windows-10/) on how to do that. To run the executable, go to the top level solution directory and run:

```powershell
docfx.exe .\DocFx\docfx.json
```

This allows you to build the documentation, then to view it locally on your localhost:

```powershell
docfx.exe .\DocFx\docfx.json --serve
```

For more command options when needed, view the DocFX website provided above.

## Repository Contributions
The repository is the first thing a developer see's about the project and we like to keep it tidy. If you see ways to contribute, such as improving this document, changing README's, or improving the logo artwork, don't hesitate to create and issue and get it resolved. Keep in mind that aformentioned guidelines to good pull requests still apply.

## Other Contributions
This is a broad contribution component which encompasses a massive role in the success of Ampere. Although no guidelines apply, contribution in this area signfies a level of content expertise and understanding. This section includes adding build scripts to improve productivity, creating CI/CD workflows, security compliance, adding unit testing assemblies to test PSCmdlets, optimizing the codebase with respect to performance, design, among other things. Keep in mind that aformentioned guidelines to good pull requests still apply.

# State
Ampere is a very fluid project and you may encounter issues during execution, especially for preleases. To report an issue visit, [Ampere Issues](https://github.com/manu-p-1/Ampere/issues). If you are able to fix the isssue yourself by building the project, show this repo some love and fork it, would ya?

# Licensing
Ampere is licensed under the [**GNU General Public License v3.0**](https://www.gnu.org/licenses/gpl-3.0.en.html). The GNU General Public License is a free, copyleft license for software and other kinds of works.

# Attribution

This Code of Conduct is adapted from the [Contributor Covenant][homepage], version 1.4,
available at [http://contributor-covenant.org/version/1/4][version]

[homepage]: http://contributor-covenant.org
[version]: http://contributor-covenant.org/version/1/4/
