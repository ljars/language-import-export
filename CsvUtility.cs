using System.Text;

public class CsvReader
{
    private string csv = string.Empty;
    private int index = 0;
    private bool inQuotes = false;
    
    public CsvReader(string csvToRead)
    {
        this.index = 0;
        this.csv = csvToRead;
        this.inQuotes = false;
    }

    public bool CanRead()
    {
        return (csv != null && index >= 0 && index < csv.Length);
    }

    public string Read()
    {
        if (!CanRead())
            return null;

        var builder = new StringBuilder();
        while (CanRead() && !AtTerminalCharacter())
        {
            ProcessCharacter(builder);
            index++;
        }

        index++;
        EndQuotes();
        return builder.ToString();
    }

    private bool InQuotes => inQuotes;

    private void StartQuotes() => inQuotes = true;

    private void EndQuotes() => inQuotes = false;

    private void ProcessCharacter(StringBuilder builder)
    {
        var current = csv[index];
        if (current != '\"')
        {
            builder.Append(current);
            return;
        }

        if (InQuotes && !IsQuoteEscaped())
        {
            EndQuotes();
        }
        else if (InQuotes)
        {
            index++; // quotes are escaped as pairs of double quotes
            builder.Append('\"');
        }
        else if (!InQuotes)
        {
            StartQuotes();
        }
    }

    private bool AtTerminalCharacter()
    {
        var current = csv[index];

        if (current != ',' && current != '\"' && current != '\n')
            return false;

        if (current == ',' && !InQuotes)
        {
            return true;
        }

        if (current == '\n' && !InQuotes)
        {
            return true;
        }

        return false;
    }

    private bool IsQuoteEscaped()
    {
        if (index + 1 >= csv.Length)
            return false;
        return csv[index + 1] == '\"';
    }
}
