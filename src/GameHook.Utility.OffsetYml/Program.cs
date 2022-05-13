using System.Text.RegularExpressions;

var existingFilepath = @"";
var newFilepath = @"";

var fileContents = File.ReadAllLines(existingFilepath);
var newFileContents = new List<string>();

foreach (var line in fileContents)
{
    if (line.Contains("address:"))
    {
        var regexExpression = @"address: 0x(.+?)[,\s]";

        var match = Regex.Match(line, regexExpression);
        var matchString = match.Groups[1].Value.Trim();
        var number = Convert.ToInt32(matchString, 16) - 2;

        var newValue = number.ToString("X");
        var newValueString = $"address: 0x{newValue}{match.ToString()[match.ToString().Length - 1]}";

        var newLine = Regex.Replace(line, regexExpression, newValueString);
        newFileContents.Add(newLine);

        Console.WriteLine(newLine);
    }
    else
    {
        newFileContents.Add(line);
    }
}

File.WriteAllLines(newFilepath, newFileContents);