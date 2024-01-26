using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fcheck.Models;

namespace fcheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcPreferencesController : ControllerBase
    {
        private readonly DTCHECKERContext _context;

        public FcPreferencesController(DTCHECKERContext context)
        {
            _context = context;
        }

        // GET: api/FcPreferences
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FcPreference>>> GetFcPreferences()
        {
            return await _context.FcPreferences.ToListAsync();
        }

        // GET: api/FcPreferences/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FcPreference>> GetFcPreference(int id)
        {
            var fcPreference = await _context.FcPreferences.FindAsync(id);

            if (fcPreference == null)
            {
                return NotFound();
            }

            return fcPreference;
        }

        // PUT: api/FcPreferences/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFcPreference(int id, FcPreference fcPreference)
        {
            if (id != fcPreference.Id)
            {
                return BadRequest();
            }

            _context.Entry(fcPreference).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FcPreferenceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/FcPreferences
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("PreferenceWith")]
        public async Task<ActionResult<FcPreference>> PostFcPreference([FromForm]FcPreference fcPreference)
        {

            var checker = _context.FcPreferences.FirstOrDefault(x => x.EmployeeId.Equals(fcPreference.EmployeeId));

            if (checker == null){
                _context.FcPreferences.Add(fcPreference);
                await _context.SaveChangesAsync();
            }
            else
            {
                checker.WithCamera = fcPreference.WithCamera;
                checker.ShowBreakTime = fcPreference.ShowBreakTime;
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetFcPreference", new { id = fcPreference.Id }, fcPreference);
        }

        [HttpPost("getPreferenceWith")]
        public async Task<ActionResult<FcPreference>> getPreferenceWith([FromForm] int UserID)
        {
            try
            {
                var checker = _context.FcPreferences.Where(x => x.EmployeeId == UserID).Count();

                if (checker > 0)
                {
                    return Ok(_context.FcPreferences.Where(x => x.EmployeeId == UserID).ToList());
                }
                else
                {
                    return Ok("noData");
                }
            }catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/FcPreferences/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFcPreference(int id)
        {
            var fcPreference = await _context.FcPreferences.FindAsync(id);
            if (fcPreference == null)
            {
                return NotFound();
            }

            _context.FcPreferences.Remove(fcPreference);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FcPreferenceExists(int id)
        {
            return _context.FcPreferences.Any(e => e.Id == id);
        }
    }
}
