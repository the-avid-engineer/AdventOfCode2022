using System.Diagnostics;

await SolverUtility<Program>.LogAnswer(
    7,

    new Terminal(),

    (FileNodeReader) => {
        var line = FileNodeReader.ReadLine() ?? "";

        var parts = line.Split(' ', 3);

        if (parts[0] == "$")
        {
            switch (parts[1])
            {
                case "cd":
                    return new ChangeDirectoryNode
                    {
                        Path = parts[2]
                    };

                case "ls":
                    return new DoNothing();

                default:
                    throw new UnreachableException();
            }
        }

        switch (parts[0])
        {
            case "dir":
                return new ListDirectoryNode
                {
                    Name = parts[1]
                };

            default:
                return new ListFileNode
                {
                    Size = int.Parse(parts[0]),
                    Name = parts[1],
                };
        }
    }
);

public class ListDirectoryNode : IInstruction
{
    public ListDirectoryNode()
    {
    }

    public required string Name { get; set; }

    public IState Reduce(IState previousState)
    {
        if (previousState is not Terminal terminal)
        {
            throw new UnreachableException();
        }

        terminal.CurrentDirectoryNode.ChildDirectories.Add(Name, new DirectoryNode
        {
            ParentDirectoryNode = terminal.CurrentDirectoryNode,
            Name = Name
        });

        return terminal;
    }
}

public class ListFileNode : IInstruction
{
    public required int Size { get; set; }
    public required string Name { get; set; }

    public IState Reduce(IState previousState)
    {
        if (previousState is not Terminal terminal)
        {
            throw new UnreachableException();
        }

        terminal.CurrentDirectoryNode.FileNodes.Add(new FileNode
        {
            Size = Size,
            Name = Name
        });

        return terminal;
    }
}

public class Drive
{
    public DirectoryNode RootDirectoryNode { get; init; } = new()
    {
        ParentDirectoryNode = null,
        Name = "",
    };
}

public class ChangeDirectoryNode : IInstruction
{
    public required string Path { get; set; }

    public IState Reduce(IState previousState)
    {
        if (previousState is not Terminal terminal)
        {
            throw new UnreachableException();
        }

        switch (Path)
        {
            case "/":
                terminal.CurrentDirectoryNode = terminal.Drive.RootDirectoryNode;
                break;

            case ".":
                break;

            case "..":
                if (terminal.CurrentDirectoryNode.ParentDirectoryNode == null)
                {
                    break;
                }
                terminal.CurrentDirectoryNode = terminal.CurrentDirectoryNode.ParentDirectoryNode;
                break;

            default:
                if (!terminal.CurrentDirectoryNode.ChildDirectories.TryGetValue(Path, out var childDirectoryNode))
                {
                    childDirectoryNode = new DirectoryNode
                    {
                        ParentDirectoryNode = terminal.CurrentDirectoryNode,
                        Name = Path,
                    };

                    terminal.CurrentDirectoryNode.ChildDirectories.Add(Path, childDirectoryNode);
                }

                terminal.CurrentDirectoryNode = childDirectoryNode;
                break;
        }

        return terminal;
    }
}

public class DoNothing : IInstruction
{
    public IState Reduce(IState previousState)
    {
        return previousState;
    }
}

public class Terminal : IState
{
    public Drive Drive { get; private init; }
    public DirectoryNode CurrentDirectoryNode { get; set; }

    public Terminal()
    {
        Drive = new();
        CurrentDirectoryNode = Drive.RootDirectoryNode;
    }

    private static void Traverse(DirectoryNode directoryNode, List<DirectoryNode> matchingDirectoryNodes, Func<DirectoryNode, bool> matcher)
    {
        if (matcher.Invoke(directoryNode))
        {
            matchingDirectoryNodes.Add(directoryNode);
        }

        foreach (var (_, childDirectoryNode) in directoryNode.ChildDirectories)
        {
            Traverse(childDirectoryNode, matchingDirectoryNodes, matcher);
        }
    }

    public string ToAnswer()
    {
        const int MaxSize = 70000000;
        const int NeededForUpdate = 30000000;

        var currentSize = Drive.RootDirectoryNode.GetTotalFileSize();
        var unused = MaxSize - currentSize;

        var needToFree = NeededForUpdate - unused;

        var matchingDirectoryNodes = new List<DirectoryNode>();

        Traverse(Drive.RootDirectoryNode, matchingDirectoryNodes, (directoryNode) =>
        {
            return directoryNode.GetTotalFileSize() >= needToFree;
        });

        var smallest = matchingDirectoryNodes
            .MinBy(directoryNode => directoryNode.GetTotalFileSize())!;

        return smallest
            .GetTotalFileSize()
            .ToString();
    }
}

public class DirectoryNode
{
    public required DirectoryNode? ParentDirectoryNode { get; init; }
    public required string Name { get; init; }
    public Dictionary<string, DirectoryNode> ChildDirectories { get; private init; } = new();
    public List<FileNode> FileNodes { get; private init; } = new();

    public int GetTotalFileSize()
    {
        var totalFileSize = FileNodes.Sum(fileNode => fileNode.Size);

        foreach (var (_, directoryNode) in ChildDirectories)
        {
            totalFileSize += directoryNode.GetTotalFileSize();
        }

        return totalFileSize;
    }
}

public class FileNode
{
    public required int Size { get; init; }
    public required string Name { get; init; }
}