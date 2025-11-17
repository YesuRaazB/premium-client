//using premium.Api.Data;
//using premium.Api.Models;
//using premium.Api.Services;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace premium.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class MembersController : ControllerBase
//    {
//        private readonly premiumDbContext _db;
//        private readonly IPremiumCalculator _calculator;
//        private readonly ILogger<MembersController> _logger;

//        public MembersController(premiumDbContext db, IPremiumCalculator calculator, ILogger<MembersController> logger)
//        {
//            _db = db;
//            _calculator = calculator;
//            _logger = logger;
//        }

//        // GET: api/members
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var members = await _db.Members.Include(m => m.Occupation).ToListAsync();

//            var result = members.Select(m => new MemberDto
//            {
//                Id = m.Id,
//                Name = m.Name,
//                AgeNextBirthday = m.AgeNextBirthday,
//                DateOfBirthMMYYYY = m.DateOfBirthMMYYYY,
//                OccupationCode = m.OccupationCode,
//                DeathSumInsured = m.DeathSumInsured,
//                MonthlyPremium = m.MonthlyPremium
//            });

//            return Ok(result);
//        }

//        // GET: api/members/{id}
//        [HttpGet("{id}")]
//        public async Task<IActionResult> Get(int id)
//        {
//            var member = await _db.Members.Include(m => m.Occupation).FirstOrDefaultAsync(m => m.Id == id);
//            if (member == null) return NotFound();

//            var dto = new MemberDto
//            {
//                Id = member.Id,
//                Name = member.Name,
//                AgeNextBirthday = member.AgeNextBirthday,
//                DateOfBirthMMYYYY = member.DateOfBirthMMYYYY,
//                OccupationCode = member.OccupationCode,
//                DeathSumInsured = member.DeathSumInsured,
//                MonthlyPremium = member.MonthlyPremium
//            };

//            return Ok(dto);
//        }

//        // POST: api/members
//        [HttpPost]
//        public async Task<IActionResult> Create([FromBody] MemberDto dto)
//        {
//            if (dto == null) return BadRequest("payload required");
//            if (!ModelState.IsValid) return BadRequest(ModelState);

//            if (string.IsNullOrWhiteSpace(dto.Name)
//                || string.IsNullOrWhiteSpace(dto.DateOfBirthMMYYYY)
//                || string.IsNullOrWhiteSpace(dto.OccupationCode))
//            {
//                return BadRequest("All fields are mandatory.");
//            }

//            var occupation = await _db.Occupations.FindAsync(dto.OccupationCode);
//            if (occupation == null) return BadRequest("Invalid occupation code.");

//            var monthly = _calculator.CalculateMonthlyPremium(dto.DeathSumInsured, occupation.Factor, dto.AgeNextBirthday);

//            var member = new Member
//            {
//                Name = dto.Name,
//                AgeNextBirthday = dto.AgeNextBirthday,
//                DateOfBirthMMYYYY = dto.DateOfBirthMMYYYY,
//                OccupationCode = dto.OccupationCode,
//                DeathSumInsured = dto.DeathSumInsured,
//                MonthlyPremium = monthly
//            };

//            _db.Members.Add(member);
//            await _db.SaveChangesAsync();

//            var outDto = new MemberDto
//            {
//                Id = member.Id,
//                Name = member.Name,
//                AgeNextBirthday = member.AgeNextBirthday,
//                DateOfBirthMMYYYY = member.DateOfBirthMMYYYY,
//                OccupationCode = member.OccupationCode,
//                DeathSumInsured = member.DeathSumInsured,
//                MonthlyPremium = member.MonthlyPremium
//            };

//            return CreatedAtAction(nameof(Get), new { id = member.Id }, outDto);
//        }

//        // PUT: api/members/{id}
//        [HttpPut("{id}")]
//        public async Task<IActionResult> Update(int id, [FromBody] MemberDto dto)
//        {
//            var member = await _db.Members.FindAsync(id);
//            if (member == null) return NotFound();

//            var occupation = await _db.Occupations.FindAsync(dto.OccupationCode);
//            if (occupation == null) return BadRequest("Invalid occupation code.");

//            member.Name = dto.Name;
//            member.AgeNextBirthday = dto.AgeNextBirthday;
//            member.DateOfBirthMMYYYY = dto.DateOfBirthMMYYYY;
//            member.OccupationCode = dto.OccupationCode;
//            member.DeathSumInsured = dto.DeathSumInsured;
//            member.MonthlyPremium = _calculator.CalculateMonthlyPremium(dto.DeathSumInsured, occupation.Factor, dto.AgeNextBirthday);

//            await _db.SaveChangesAsync();
//            return NoContent();
//        }

//        // DELETE: api/members/{id}
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var member = await _db.Members.FindAsync(id);
//            if (member == null) return NotFound();
//            _db.Members.Remove(member);
//            await _db.SaveChangesAsync();
//            return NoContent();
//        }

//        // Calculation endpoint stays here
//        // GET: api/members/calc?occupationCode=Doctor&death=100000&age=30
//        [HttpGet("calc")]
//        public async Task<IActionResult> Calculate([FromQuery] string occupationCode, [FromQuery] decimal death, [FromQuery] int age)
//        {
//            var occupation = await _db.Occupations.FindAsync(occupationCode);
//            if (occupation == null) return BadRequest("Invalid occupation code");
//            var monthly = _calculator.CalculateMonthlyPremium(death, occupation.Factor, age);
//            return Ok(new { MonthlyPremium = monthly });
//        }
//    }
//}

