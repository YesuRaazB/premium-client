using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using premium.Api.Data;
using premium.Api.Models;
using premium.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace premium.Api.Controllers
{
        [ApiController]
        [Route("api/members")]
    public class premiumController : ControllerBase
    {
        private readonly premiumDbContext _db;
        private readonly IPremiumCalculator _calculator;
        private readonly ILogger<premiumController> _logger;

        public premiumController(premiumDbContext db, IPremiumCalculator calculator, ILogger<premiumController> logger)
        {
            _db = db;
            _calculator = calculator;
            _logger = logger;
        }

        // ----------------- Occupations -----------------
       
        [HttpGet("occupations")]
        public async Task<IActionResult> GetOccupations()
        {
            var list = await _db.Occupations
                .Select(o => new
                {
                    code = o.Code,
                    displayName = o.DisplayName,
                    rating = o.Rating,
                    factor = o.Factor
                })
                .ToListAsync();
            return Ok(list);
        }

        // GET: api/occupations/{code}
        [HttpGet("occupations/{code}")]
        public async Task<IActionResult> GetOccupation(string code)
        {
            var occ = await _db.Occupations
                .Where(o => o.Code == code)
                .Select(o => new
                {
                    code = o.Code,
                    displayName = o.DisplayName,
                    rating = o.Rating,
                    factor = o.Factor
                })
                .FirstOrDefaultAsync();
            if (occ == null) return NotFound();
            return Ok(occ);
        }

        // ----------------- Members -----------------
        // GET: api/members

        [HttpGet("members")]
        public async Task<IActionResult> GetMembers()
        {
            var members = await _db.Members.Include(m => m.Occupation).ToListAsync();
            var result = members.Select(m => new MemberDto
            {
                Id = m.Id,
                Name = m.Name,
                AgeNextBirthday = m.AgeNextBirthday,
                DateOfBirthMMYYYY = m.DateOfBirthMMYYYY,
                OccupationCode = m.OccupationCode,
                DeathSumInsured = m.DeathSumInsured,
                MonthlyPremium = m.MonthlyPremium
            });
            return Ok(result);
        }

        // GET: api/members/{id}
        [HttpGet("members/{id}")]
        public async Task<IActionResult> GetMember(int id)
        {
            var member = await _db.Members.Include(m => m.Occupation).FirstOrDefaultAsync(m => m.Id == id);
            if (member == null) return NotFound();

            return Ok(new MemberDto
            {
                Id = member.Id,
                Name = member.Name,
                AgeNextBirthday = member.AgeNextBirthday,
                DateOfBirthMMYYYY = member.DateOfBirthMMYYYY,
                OccupationCode = member.OccupationCode,
                DeathSumInsured = member.DeathSumInsured,
                MonthlyPremium = member.MonthlyPremium
            });
        }

        // POST: api/members
        [HttpPost("members")]
        public async Task<IActionResult> CreateMember([FromBody] MemberDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name)
                || string.IsNullOrWhiteSpace(dto.DateOfBirthMMYYYY)
                || string.IsNullOrWhiteSpace(dto.OccupationCode))
            {
                return BadRequest("All fields are mandatory.");
            }

            var occupation = await _db.Occupations.FindAsync(dto.OccupationCode);
            if (occupation == null) return BadRequest("Invalid occupation code.");

            var monthly = _calculator.CalculateMonthlyPremium(dto.DeathSumInsured, occupation.Factor, dto.AgeNextBirthday);

            var member = new Member
            {
                Name = dto.Name,
                AgeNextBirthday = dto.AgeNextBirthday,
                DateOfBirthMMYYYY = dto.DateOfBirthMMYYYY,
                OccupationCode = dto.OccupationCode,
                DeathSumInsured = dto.DeathSumInsured,
                MonthlyPremium = monthly
            };

            _db.Members.Add(member);
            await _db.SaveChangesAsync();

            dto.Id = member.Id;
            dto.MonthlyPremium = member.MonthlyPremium;
            return CreatedAtAction(nameof(GetMember), new { id = member.Id }, dto);
        }

        // PUT: api/members/{id}
        [HttpPut("members/{id}")]
        public async Task<IActionResult> UpdateMember(int id, [FromBody] MemberDto dto)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null) return NotFound();

            var occupation = await _db.Occupations.FindAsync(dto.OccupationCode);
            if (occupation == null) return BadRequest("Invalid occupation code.");

            member.Name = dto.Name;
            member.AgeNextBirthday = dto.AgeNextBirthday;
            member.DateOfBirthMMYYYY = dto.DateOfBirthMMYYYY;
            member.OccupationCode = dto.OccupationCode;
            member.DeathSumInsured = dto.DeathSumInsured;
            member.MonthlyPremium = _calculator.CalculateMonthlyPremium(dto.DeathSumInsured, occupation.Factor, dto.AgeNextBirthday);

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/members/{id}
        [HttpDelete("members/{id}")]
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null) return NotFound();
            _db.Members.Remove(member);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // ----------------- Premium Calculation -----------------
        //[HttpGet("members/calc")]
        //public async Task<IActionResult> CalculatePremium([FromQuery] string occupationCode, [FromQuery] decimal death, [FromQuery] int age)
        //{
        //    var occupation = await _db.Occupations.FindAsync(occupationCode);
        //    if (occupation == null) return BadRequest("Invalid occupation code");

        //    var monthly = _calculator.CalculateMonthlyPremium(death, occupation.Factor, age);
        //    return Ok(new { MonthlyPremium = monthly });
        //}

        //[HttpGet("calc")]
        //public IActionResult Calc([FromQuery] string occupationCode, [FromQuery] int age, [FromQuery] decimal death)
        //{
        //    // Example logic for monthly premium
        //    decimal factor = 1.5m; // Example factor
        //    decimal yearly = (death * factor * age) / 1000;
        //    decimal monthly = yearly / 12;
        //    return Ok(monthly);
        //}


        [HttpGet("members/calc")]
        public async Task<IActionResult> Calculate([FromQuery] string occupationCode, [FromQuery] decimal death, [FromQuery] int age)
        {
            if (string.IsNullOrWhiteSpace(occupationCode)) return BadRequest("occupationCode is required");
            var occupation = await _db.Occupations.FindAsync(occupationCode);
            if (occupation == null) return BadRequest("Invalid occupation code");
            var monthly = _calculator.CalculateMonthlyPremium(death, occupation.Factor, age);
            return Ok(new { monthlyPremium = monthly });
        }

    }
}
