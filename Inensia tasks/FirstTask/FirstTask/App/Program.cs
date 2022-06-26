
namespace App
{
    using System.Globalization;
    using System.Text;
    using Newtonsoft.Json;

    public class Program
    {
        // The reading from the file and the writing on the console could be extracted to separate interfaces so we can have Dependency Inversion
        public static void Main(string[] args)
        {
            var movieStarsJson = File.ReadAllText("input.txt");

            var movieStars = JsonConvert.DeserializeObject<MovieStar[]>(movieStarsJson);

            var result = GetMovieStarsText(movieStars);

            Console.WriteLine(result);
        }

        private static int CalculateAge(DateTime dateOfBirth)
        {
            var age = DateTime.Now.Year - dateOfBirth.Year;

            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
            {
                age--;
            }

            return age;
        }

        private static string GetMovieStarsText(MovieStar[] movieStars)
        {
            var stringBuilder = new StringBuilder();

            foreach (var movieStar in movieStars)
            {
                var date = DateTime.ParseExact(movieStar.DateOfBirth, "MMMM dd, yyyy", CultureInfo.InvariantCulture);

                var age = CalculateAge(date);
                stringBuilder.AppendLine($"{movieStar.Name} {movieStar.Surname}")
                            .AppendLine($"{movieStar.Sex}")
                            .AppendLine($"{movieStar.Nationality}")
                            .AppendLine($"{age} years old");
            }

            return stringBuilder.ToString().TrimEnd();
        }
    }
}