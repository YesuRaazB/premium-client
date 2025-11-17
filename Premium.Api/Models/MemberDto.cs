public class MemberDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int AgeNextBirthday { get; set; }
    public string DateOfBirthMMYYYY { get; set; } = null!;
    public string OccupationCode { get; set; } = null!;
    public decimal DeathSumInsured { get; set; }
    public decimal MonthlyPremium { get; set; }
}
