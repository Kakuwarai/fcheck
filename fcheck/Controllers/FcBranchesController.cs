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
    public class FcBranchesController : ControllerBase
    {
        private readonly DTCHECKERContext _context;

        public FcBranchesController(DTCHECKERContext context)
        {
            _context = context;
        }

        // GET: api/FcBranches
        [HttpGet("getBranch")]
        public async Task<ActionResult<IEnumerable<FcBranch>>> GetFcBranches(int _page, int _limit, String? search, String? sortBy)
        {
            try
            {
                if (search == null)
                {
                    return Ok(await _context.FcBranches.Where(x => x.IsDeleted != 1).Skip(_page * _limit).Take(_limit).ToListAsync());
                }
                // var asdasd = await _context.FcBranches.Where(x => x.Code.Contains(search) || x.Name.Contains(search)).Skip(_page * _limit).Take(_limit).ToListAsync();
                return Ok(await _context.FcBranches.Where(x => x.Code.Contains(search) || x.Name.Contains(search) && x.IsDeleted != 1).Skip(_page * _limit).Take(_limit).ToListAsync());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("branchLength")]
        public async Task<ActionResult<IEnumerable<FcBranch>>> branchLength(String? search)
        {
            try
            {

                if (search == null)
                {
                    if (_context.FcBranches.Where(x => x.IsDeleted != 1).ToList().Count % 5 == 0)
                    {
                        return Ok(_context.FcBranches.Where(x => x.IsDeleted != 1).ToList().Count / 5);
                    }
                    return Ok((_context.FcBranches.Where(x => x.IsDeleted != 1).ToList().Count / 5) + 1);
                }
                if (_context.FcBranches.Where(x => x.Code.Contains(search) || x.Name.Contains(search) && x.IsDeleted != 1).ToList().Count % 5 == 0)
                {
                    return Ok(_context.FcBranches.Where(x => x.Code.Contains(search) || x.Name.Contains(search) && x.IsDeleted != 1).ToList().Count / 5);
                }
                return Ok((_context.FcBranches.Where(x => x.Code.Contains(search) || x.Name.Contains(search) && x.IsDeleted != 1 ).ToList().Count / 5) + 1);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("addBranch")]
        public async Task<ActionResult<FcBranch>> addBranch([FromForm] FcBranch fcBranch)
        {
            try
            {
                var checker = _context.FcBranches.Where(x => x.Code == fcBranch.Code).FirstOrDefault();
                if (checker != null)
                {
                    return Ok("Already");
                }else
                {
                    _context.FcBranches.Add(fcBranch);
                    await _context.SaveChangesAsync();
                    return Ok();
                }

               

            }
            catch (Exception e){
                return BadRequest(e.Message);
            }
        
        }


        [HttpPost("editBranch")]
        public async Task<ActionResult<FcBranch>> editBranch([FromForm] FcBranch fcBranch)
        {
            try
            {
                var checker = _context.FcBranches.Where(x => x.Code == fcBranch.Code).Select(x => x.Id).FirstOrDefault();
                if (checker != fcBranch.Id && checker != 0)
                {
                    return Ok("Already");
                }
                else
                {

                    var insert = _context.FcBranches.FirstOrDefault(x => x.Id == fcBranch.Id);

                    insert.Code = fcBranch.Code;
                    insert.Name = fcBranch.Name;
                    insert.Status = fcBranch.Status;

                    await _context.SaveChangesAsync();
                    return Ok();
                }



            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("drpDepartment")]
        public async Task<ActionResult<IEnumerable<FcBranch>>> drpDepartment()
        {
            try
            {

                return Ok(_context.FcBranches.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("deleteBranch")]
        public async Task<ActionResult<FcBranch>> deleteBranch([FromForm] int id)
        {

            try
            {

                var update = _context.FcBranches.Where(x => x.Id == id).FirstOrDefault();

                update.IsDeleted = 1;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        private bool FcBranchExists(int id)
        {
            return _context.FcBranches.Any(e => e.Id == id);
        }
    }
}
