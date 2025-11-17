//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using premium.Api.Data;

//namespace premium.Api.Controllers
//{
//    [ApiController]
//    [Route("api/[controller]")]
//    public class OccupationsController : ControllerBase
//    {
//        private readonly premiumDbContext _db;
//        public OccupationsController(premiumDbContext db) => _db = db;

//        // GET: api/occupations
//        [HttpGet]
//        public async Task<IActionResult> GetAll()
//        {
//            var list = await _db.Occupations
//                .Select(o => new
//                {
//                    code = o.Code,
//                    displayName = o.DisplayName,
//                    rating = o.Rating,
//                    factor = o.Factor
//                })
//                .ToListAsync();

//            return Ok(list);
//        }

//        // GET: api/occupations/{code}
//        [HttpGet("{code}")]
//        public async Task<IActionResult> GetByCode(string code)
//        {
//            var occ = await _db.Occupations
//                .Where(o => o.Code == code)
//                .Select(o => new
//                {
//                    code = o.Code,
//                    displayName = o.DisplayName,
//                    rating = o.Rating,
//                    factor = o.Factor
//                })
//                .FirstOrDefaultAsync();

//            if (occ == null) return NotFound();
//            return Ok(occ);
//        }
//    }
//}
