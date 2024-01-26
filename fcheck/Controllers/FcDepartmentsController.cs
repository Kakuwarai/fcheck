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
    public class FcDepartmentsController : ControllerBase
    {
        private readonly DTCHECKERContext _context;

        public FcDepartmentsController(DTCHECKERContext context)
        {
            _context = context;
        }
        [HttpGet("maintenanceGetDepartment")]
        public async Task<ActionResult<IEnumerable<FcDepartment>>> maintenanceGetDepartment()
        {
            try
            {
                return Ok(_context.FcDepartments.ToList());
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        // GET: api/FcDepartments
        [HttpGet("getDepartment")]
        public async Task<ActionResult<IEnumerable<FcDepartment>>> getDepartment(int _page, int _limit, String? search, String? sortBy)
        {
            if(search == null)
            {
                return await _context.FcDepartments.Where(x => x.IsDeleted != 1).Skip(_page * _limit).Take(_limit).ToListAsync();
            }
            return await _context.FcDepartments.Where(x => x.Departments.Contains(search) && x.IsDeleted != 1).Skip(_page * _limit).Take(_limit).ToListAsync();
        }

        [HttpGet("getDepartmentLength")]
        public async Task<ActionResult<IEnumerable<FcDepartment>>> getDepartmentLength(String? search)
        {
            try
            {

                if (search == null)
                {
                    if (_context.FcDepartments.Where(x => x.IsDeleted != 1).ToList().Count % 5 == 0)
                    {
                        return Ok(_context.FcDepartments.Where(x => x.IsDeleted != 1).ToList().Count / 5);
                    }
                    else if(_context.FcDepartments.Where(x => x.Status != "I").ToList().Count < 5 && _context.FcDepartments.Where(x => x.IsDeleted != 1).ToList().Count >= 1){
                        return Ok(1);
                    }
                    return Ok((_context.FcDepartments.ToList().Count / 5) + 1);
                }
                if (_context.FcDepartments.Where(x => x.Departments.Contains(search) && x.IsDeleted != 1).ToList().Count % 5 == 0)
                {
                    return Ok(_context.FcDepartments.Where(x => x.Departments.Contains(search) && x.IsDeleted != 1).ToList().Count / 5);
                }
                return Ok((_context.FcDepartments.Where(x => x.Departments.Contains(search) && x.IsDeleted != 1).ToList().Count / 5) + 1);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("addDepartment")]
        public async Task<ActionResult<FcDepartment>> addDepartment([FromForm] FcDepartment fcDepartment)
        {
            try
            {
                var checker = _context.FcDepartments.Where(x => x.Departments == fcDepartment.Departments).FirstOrDefault();

                if (checker != null)
                {
                    return Ok("Already");
                }
                else
                {
                    _context.FcDepartments.Add(fcDepartment);
                    await _context.SaveChangesAsync();
                    return Ok();
                }



            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("editDepartment")]
        public async Task<ActionResult<FcDepartment>> editBranch([FromForm] FcDepartment fcDepartment)
        {
            try
            {
                var checker = _context.FcDepartments.Where(x => x.Departments == fcDepartment.Departments).Select(x => x.Id).FirstOrDefault();
                if (checker != fcDepartment.Id && checker != 0)
                {
                    return Ok("Already");
                }
                else
                {

                    var insert = _context.FcDepartments.FirstOrDefault(x => x.Id == fcDepartment.Id);

                    insert.Departments = fcDepartment.Departments;
                    insert.Status = fcDepartment.Status;

                    await _context.SaveChangesAsync();
                    return Ok();
                }



            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("deleteDepartment")]
        public async Task<ActionResult<FcDepartment>> deleteBranch([FromForm] int id)
        {

            try
            {

                var update = _context.FcDepartments.Where(x => x.Id == id).FirstOrDefault();

                update.IsDeleted = 1;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        private bool FcDepartmentExists(int id)
        {
            return _context.FcDepartments.Any(e => e.Id == id);
        }
    }
}
