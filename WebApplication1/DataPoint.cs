namespace WebApplication1;

public class DataPoint
{
    public DateOnly Date { get; set; }
    public double Value { get; set; }

    public DataPoint(DateOnly date, double value)
    {
        Date = date;
        Value = value;
    }

    public override string ToString()
    {
        return $"DataPoint {{ Date = {Date}, Value = {Value} }}";
    }
}