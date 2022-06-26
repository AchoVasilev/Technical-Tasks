namespace App
{
    public interface ITaxCalculator
    {
        decimal CalculateTax(decimal inputSum, decimal percentage);
    }
}