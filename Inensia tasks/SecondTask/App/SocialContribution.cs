namespace App
{
    public class SocialContribution : ITax
    {
        public decimal Percentage { get; set; } = Constants.SocialContributionPercentage;

        public ITaxCalculator TaxCalculator { get; set; } = new TaxCalculator();
    }
}