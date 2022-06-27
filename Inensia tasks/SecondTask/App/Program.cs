namespace App
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var grossValue = decimal.Parse(Console.ReadLine());

            var salary = 0m;

            if (grossValue <= Constants.NonTaxableSum)
            {
                salary = grossValue;
            }
            else
            {
                var tax = new Tax();
                salary -= tax.TaxCalculator.CalculateTax(grossValue, tax.Percentage);
            }

            // Social contributions of 15% are expected to be made as well. 
            // As for the previous case, the taxable income is whatever is above 1000 IDR but social contributions never apply to amounts higher than 3000. 
            // Example 2 : Irina has a salary of 3400 IDR. She owns income tax : 10% out of 2400 => 240. 
            // Her Social contributions are 15% out of 2000 => 300. In total her tax is 540 and she gets to bring home 2860 IDR
            if (grossValue > Constants.NonTaxableSum)
            {
                var socialContribution = new SocialContribution();

                if (grossValue <= Constants.SocialContributionMaxSumThreshold)
                {
                    salary -= socialContribution.TaxCalculator.CalculateTax(grossValue, socialContribution.Percentage);
                }
                else
                {
                    salary -= socialContribution.TaxCalculator.CalculateTax(Constants.SocialContributionMaxSumThreshold, socialContribution.Percentage);
                }
            }

            Console.WriteLine(salary);

            // To add new taxes we create a new class that implements the ITax interface and we add a condition in which we should additionally tax the tax payer.
        }
    }
}