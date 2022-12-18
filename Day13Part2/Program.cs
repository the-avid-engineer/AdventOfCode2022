using System.Text.Json;

await SolverUtility<Program>.LogAnswer(
    13,

    new DataStream(),

    (fileReader) => {
        var line = fileReader.ReadLine() ?? "";

        if (line == "")
        {
            return new DoNothing();
        }

        return new ParseLine
        {
            Line = line
        };
    }
);

public class ParseLine : IInstruction<DataStream>
{
    public required string Line { get; init; }

    public DataStream Reduce(DataStream dataStream)
    {
        var packet = new Packet();

        packet.Read(Line);

        dataStream.Packets.Add(packet);

        return dataStream;
    }
}

public class DataStream : IState
{
    public List<Packet> Packets { get; } = new();

    public string ToAnswer()
    {
        var dividerPacket1 = new Packet();
        dividerPacket1.Read("[[2]]");
        Packets.Add(dividerPacket1);

        var dividerPacket2 = new Packet();
        dividerPacket2.Read("[[6]]");
        Packets.Add(dividerPacket2);

        Packets.Sort(Packet.ToSort);

        foreach (var packet in Packets)
        {
            Console.WriteLine(packet.Json.GetRawText());
        }

        var dividerPacket1Index = Packets.IndexOf(dividerPacket1) + 1;
        var dividerPacket2Index = Packets.IndexOf(dividerPacket2) + 1;

        return $"{dividerPacket1Index * dividerPacket2Index}";
    }
}


public class Packet
{
    public JsonElement Json { get; set; }

    public void Read(string input)
    {
        Json = JsonSerializer.Deserialize<JsonElement>(input);
    }

    public static int ToSort(Packet left, Packet right)
    {
        return Compare(left, right) switch
        {
            true => -1,
            false => +1,
            null => 0,
        };
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

        //Console.WriteLine($"{GetPadding(depth)}- Compare {left.GetRawText()} vs {right.GetRawText()}");

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
            //Console.WriteLine($"{GetPadding(depth)}- Mixed types; convert left to [{left.GetRawText()}] and retry comparison");

            var newLeft = JsonSerializer.Deserialize<JsonElement>($"[{left.GetRawText()}]");

            return Compare(depth, newLeft, right);
        }
        else if (left.ValueKind == JsonValueKind.Array && right.ValueKind == JsonValueKind.Number)
        {
            //Console.WriteLine($"{GetPadding(depth)}- Mixed types; convert right to [{right.GetRawText()}] and retry comparison");

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
            //Console.WriteLine($"{GetPadding(depth)}- Left side is smaller, so inputs are in the right order");
            return true;
        }

        if (leftInteger > rightInteger)
        {
            //Console.WriteLine($"{GetPadding(depth)}- Right side is smaller, so inputs are not in the right order");
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
                //Console.WriteLine($"{GetPadding(depth)}- Left side ran out of items, so inputs are in the right order");
                return true;
            }

            if (!IsValid(rightItem))
            {
                //Console.WriteLine($"{GetPadding(depth)}- Right side ran out of items, so inputs are not in the right order");
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
