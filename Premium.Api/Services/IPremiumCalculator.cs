using premium.Api.Models;

namespace premium.Api.Services  
{
    public interface IPremiumCalculator
    {
        decimal CalculateMonthlyPremium(decimal deathCover, decimal occupationFactor, int ageNextBirthday);

    }
}