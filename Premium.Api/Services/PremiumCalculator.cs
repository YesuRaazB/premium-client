using Microsoft.AspNetCore.Mvc;
using premium.Api.Services;

namespace premium.Api.Services
{
    public class PremiumCalculator : IPremiumCalculator
    {
        // Formula from spec: Death Premium = (Death Cover amount * Occupation Rating Factor * Age) /1000 * 12
        //public decimal CalculateMonthlyPremium(decimal deathCover, decimal occupationFactor, int ageNextBirthday)
        //{
        //    // Ensure decimals with correct precision
        //    var yearlyPremium = (deathCover * occupationFactor * ageNextBirthday) / 1000m;
        //    var monthlyPremium = yearlyPremium / 12m;
        //    return decimal.Round(monthlyPremium, 2);
        //}

        public decimal CalculateMonthlyPremium(decimal deathCover, decimal occupationFactor, int ageNextBirthday)
        {
            if (deathCover <= 0 || occupationFactor <= 0 || ageNextBirthday <= 0)
                return 0;

            decimal yearlyPremium = (deathCover * occupationFactor * ageNextBirthday) / 1000m;
            decimal monthlyPremium = yearlyPremium / 12m;
            return Math.Round(monthlyPremium, 2); // round to 2 decimal places
        }


        //[HttpPost]
        //public IActionResult SaveMember([FromBody] dynamic payload)
        //{
        //    // In a real app, save to database
        //    return Ok(new { monthlyPremium = payload.deathSumInsured * 1.5 / 12 });
        //}
    }
}



