namespace App
{
    public class TaxCalculator : ITaxCalculator
    {
        public decimal CalculateTax(decimal inputSum, decimal percentage)
        {
            return percentage / 100 * inputSum;
        }
    }
}