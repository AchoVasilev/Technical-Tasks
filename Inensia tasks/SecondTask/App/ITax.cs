namespace App
{
    public interface ITax
    {
        decimal Percentage { get; set; }

        ITaxCalculator TaxCalculator { get; set; }
    }
}