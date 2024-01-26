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
    public class FcSbusController : ControllerBase
    {
        private readonly DTCHECKERContext _context;

        public FcSbusController(DTCHECKERContext context)
        {
            _context = context;
        }

        [HttpGet("maintenanceGetSBU")]
        public async Task<ActionResult<IEnumerable<FcSbu>>> maintenanceGetSBU()
        {
            try
            {
                return Ok(_context.FcSbus.ToList());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        // GET: api/FcSbus
        [HttpGet("getSBU")]
        public async Task<ActionResult<IEnumerable<FcSbu>>> getSBU(int _page, int _limit, String? search, String? sortBy)
        {

            if (search == null)
            {
                return await _context.FcSbus.Where(x => x.IsDeleted != 1).Skip(_page * _limit).Take(_limit).ToListAsync();
            }
            return await _context.FcSbus.Where(x => x.Code.Contains(search) || x.Name.Contains(search)&& x.IsDeleted != 1).Skip(_page * _limit).Take(_limit).ToListAsync();
        }

        [HttpGet("getSBULength")]
        public async Task<ActionResult<IEnumerable<FcSbu>>> getSBULength(String? search, String? sortBy)
        {
            if (search == null)
            {

                if (_context.FcSbus.Count() % 5 == 0)
                {
                    return Ok(_context.FcSbus.Where(x => x.IsDeleted != 1).Count() / 5);
                }

                else
                {
                    return Ok((_context.FcSbus.Where(x => x.IsDeleted != 1).Count() / 5) + 1);
                }
            }
            else
            {

                if (_context.FcSbus.Where(x => x.Code.Contains(search) || x.Name.Contains(search)&& x.IsDeleted != 1).Count() % 5 == 0)
                {
                    return Ok(_context.FcSbus.Where(x => x.Code.Contains(search) || x.Name.Contains(search)&& x.IsDeleted != 1).Count() / 5);
                }

                else
                {
                    return Ok((_context.FcSbus.Where(x => x.Code.Contains(search) || x.Name.Contains(search)&& x.IsDeleted != 1).Count() / 5) + 1);
                }
            }


        }

        [HttpPost("PostSBU")]
        public async Task<ActionResult<IEnumerable<FcSbu>>> PostSBU([FromForm]FcSbu fcSbu)
        {


            try
            {
                var checker = _context.FcSbus.Where(x => x.Code.Equals(fcSbu.Code)).ToList().Count;


                if (checker == 0)
                {
                    _context.FcSbus.Add(fcSbu);
                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    return Ok("Already");
                }

               return BadRequest();
                
     
            }catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("editSBU")]
        public async Task<ActionResult<IEnumerable<FcSbu>>> editSBU([FromForm] FcSbu fcSbu)
        {


            try
            {
                var checker = _context.FcSbus.Where(x => x.Code == fcSbu.Code).Select(x => x.Id).FirstOrDefault();
                if (checker != fcSbu.Id && checker != 0)
                {
                    return Ok("Already");
                }
                else
                {

                    var insert = _context.FcSbus.FirstOrDefault(x => x.Id == fcSbu.Id);

                    insert.Code = fcSbu.Code;
                    insert.Name = fcSbu.Name;
                    insert.Status = fcSbu.Status;

                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("deleteSBU")]
        public async Task<ActionResult<FcDepartment>> deleteSBU([FromForm] int id)
        {

            try
            {

                var update = _context.FcSbus.Where(x => x.Id == id).FirstOrDefault();

                update.IsDeleted = 1;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private bool FcSbuExists(int id)
        {
            return _context.FcSbus.Any(e => e.Id == id);
        }
    }
}
