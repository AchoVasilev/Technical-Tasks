namespace App
{
    public class Tax : ITax
    {
        public decimal Percentage { get; set; } = Constants.TaxPercentage;

        public ITaxCalculator TaxCalculator { get; set; } = new TaxCalculator();
    }
}