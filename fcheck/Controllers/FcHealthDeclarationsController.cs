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
    public class FcHealthDeclarationsController : ControllerBase
    {
        private readonly DTCHECKERContext _context;

        public FcHealthDeclarationsController(DTCHECKERContext context)
        {
            _context = context;
        }


        // GET: api/FcHealthDeclarations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FcHealthDeclaration>>> GetFcHealthDeclarations()
        {
            return await _context.FcHealthDeclarations.ToListAsync();
        }

        // GET: api/FcHealthDeclarations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FcHealthDeclaration>> GetFcHealthDeclaration(int id)
        {
            var fcHealthDeclaration = await _context.FcHealthDeclarations.FindAsync(id);

            if (fcHealthDeclaration == null)
            {
                return NotFound();
            }

            return fcHealthDeclaration;
        }

        // PUT: api/FcHealthDeclarations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFcHealthDeclaration(int id, FcHealthDeclaration fcHealthDeclaration)
        {
            if (id != fcHealthDeclaration.Id)
            {
                return BadRequest();
            }

            _context.Entry(fcHealthDeclaration).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FcHealthDeclarationExists(id))
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

        // POST: api/FcHealthDeclarations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FcHealthDeclaration>> PostFcHealthDeclaration([FromForm]FcHealthDeclaration fcHealthDeclaration, [FromForm] String? sickString)
        {
            if(sickString == null)
            {
            
                return Ok();
            }
           var sickList = sickString.Split(',');

            for (int x = 0; x < sickList.Length-1; x++)
            {
                fcHealthDeclaration.Id = 0;
                fcHealthDeclaration.HealthDeclaration = sickList[x];
                _context.FcHealthDeclarations.Add(fcHealthDeclaration);
                await _context.SaveChangesAsync();
            }

          

            return Ok();
        }
        
     
        // DELETE: api/FcHealthDeclarations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFcHealthDeclaration(int id)
        {
            var fcHealthDeclaration = await _context.FcHealthDeclarations.FindAsync(id);
            if (fcHealthDeclaration == null)
            {
                return NotFound();
            }

            _context.FcHealthDeclarations.Remove(fcHealthDeclaration);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FcHealthDeclarationExists(int id)
        {
            return _context.FcHealthDeclarations.Any(e => e.Id == id);
        }
    }
}
