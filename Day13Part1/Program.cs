using System.Text.Json;

await SolverUtility<Program>.LogAnswer(
    13,

    new DataStream(),

    (fileReader) => {
        return new ParseLine
        {
            Line = fileReader.ReadLine() ?? ""
        };
    }
);

public class ParseLine : IInstruction<DataStream>
{
    public required string Line { get; init; }

    public DataStream Reduce(DataStream dataStream)
    {
        switch (dataStream.NextAction)
        {
            case Action.ReadLeft:
                dataStream.Left.Read(Line);

                dataStream.NextAction = Action.ReadRight;
                break;

            case Action.ReadRight:
                dataStream.Right.Read(Line);


                dataStream.NextAction = Action.Skip;
                dataStream.PacketPairs.Add((dataStream.Left, dataStream.Right));
                dataStream.Left = new();
                dataStream.Right = new();
                break;

            default:
                dataStream.NextAction = Action.ReadLeft;
                break;
        }

        return dataStream;
    }
}

public class DataStream : IState
{
    public Packet Left { get; set; } = new();
    public Packet Right { get; set; } = new();

    public Action NextAction { get; set; } = Action.ReadLeft;

    public List<(Packet, Packet)> PacketPairs { get; } = new();

    public string ToAnswer()
    {
        var sumOfIndices = 0;

        for (var i = 1; i <= PacketPairs.Count; i++)
        {
            var (leftPacket, rightPacket) = PacketPairs[i - 1];

            Console.WriteLine($"== Pair {i} ==");

            if (Packet.Compare(leftPacket, rightPacket) == true)
            {
                sumOfIndices += i;
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        return sumOfIndices.ToString();
    }
}

public enum Action
{
    ReadLeft,
    ReadRight,
    Skip,
}


public class Packet
{
    private JsonElement Json { get; set; }

    public void Read(string input)
    {
        Json = JsonSerializer.Deserialize<JsonElement>(input);
    }

    public static bool? Compare(Packet left, Packet right)
    {
        return Compare(0, left.Json, right.Json);
    }

    private static bool IsValid(JsonElement json)
    {
        return json.ValueKind == JsonValueKind.Number || json.ValueKind == JsonValueKind.Array;
    }

    private static string GetPadding(int depth)
    {
        return string.Join("", Enumerable.Repeat("  ", depth));
    }

    private static bool? Compare(int depth, JsonElement left, JsonElement right)
    {
        if (!IsValid(left) || !IsValid(right))
        {
            throw new NotSupportedException();
        }

        Console.WriteLine($"{GetPadding(depth)}- Compare {left.GetRawText()} vs {right.GetRawText()}");

        if (left.ValueKind == right.ValueKind)
        {
            if (left.ValueKind == JsonValueKind.Number)
            {
                return CompareInteger(depth + 1, left.GetInt32(), right.GetInt32());
            }

            return CompareList(depth + 1, left.EnumerateArray().ToArray(), right.EnumerateArray().ToArray());
        }

        if (left.ValueKind == JsonValueKind.Number && right.ValueKind == JsonValueKind.Array)
        {
            Console.WriteLine($"{GetPadding(depth)}- Mixed types; convert left to [{left.GetRawText()}] and retry comparison");

            var newLeft = JsonSerializer.Deserialize<JsonElement>($"[{left.GetRawText()}]");

            return Compare(depth, newLeft, right);
        }
        else if (left.ValueKind == JsonValueKind.Array && right.ValueKind == JsonValueKind.Number)
        {
            Console.WriteLine($"{GetPadding(depth)}- Mixed types; convert right to [{right.GetRawText()}] and retry comparison");

            var newRight = JsonSerializer.Deserialize<JsonElement>($"[{right.GetRawText()}]");

            return Compare(depth, left, newRight);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    private static bool? CompareInteger(int depth, int leftInteger, int rightInteger)
    {
        if (leftInteger < rightInteger)
        {
            Console.WriteLine($"{GetPadding(depth)}- Left side is smaller, so inputs are in the right order");
            return true;
        }

        if (leftInteger > rightInteger)
        {
            Console.WriteLine($"{GetPadding(depth)}- Right side is smaller, so inputs are not in the right order");
            return false;
        }

        return null;
    }

    private static bool? CompareList(int depth, JsonElement[] leftList, JsonElement[] rightList)
    {
        for (var i = 0; i < Math.Max(leftList.Length, rightList.Length); i++)
        {
            var leftItem = leftList.ElementAtOrDefault(i);
            var rightItem = rightList.ElementAtOrDefault(i);

            if (!IsValid(leftItem))
            {
                Console.WriteLine($"{GetPadding(depth)}- Left side ran out of items, so inputs are in the right order");
                return true;
            }

            if (!IsValid(rightItem))
            {
                Console.WriteLine($"{GetPadding(depth)}- Right side ran out of items, so inputs are not in the right order");
                return false;
            }

            var comparison = Compare(depth, leftItem, rightItem);

            if (comparison.HasValue)
            {
                return comparison;
            }
        }

        return null;
    }
}
