using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fcheck.Models;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace fcheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcStationAccountsController : ControllerBase
    {
        private readonly DTCHECKERContext _context;

        public FcStationAccountsController(DTCHECKERContext context)
        {
            _context = context;
        }

        [HttpPost("LoginPost")]
        public async Task<ActionResult<FcStationAccount>> GetFcStationAccount([FromForm]String username, [FromForm] String password)
        {

            String keySTR = "qwertyuiopasdfgh"; //16 byte
            String ivSTR = "qwertyuiopasdfgh"; //16 byte

            using (System.Security.Cryptography.RijndaelManaged rjm =
                                   new System.Security.Cryptography.RijndaelManaged
                                   {
                                       KeySize = 128,
                                       BlockSize = 128,
                                       Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                       IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                   }
                       )
            {
                Byte[] input = Encoding.UTF8.GetBytes(password);
                Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
                password = Convert.ToBase64String(output);
            }

            var confirmation = _context.FcStationAccounts.Where(
                x => x.Username == username &&
                x.Password == password).FirstOrDefault();
            var all = "";
            
            if (confirmation == null)
            {
                all = "Invalid";
                return Ok(all);
            }
            else
            {
                return Ok(confirmation);
            }

            
            
        }

        // GET: api/FcStationAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FcStationAccount>>> GetFcStationAccounts()
        {
            return await _context.FcStationAccounts.Where(x => x.Status != "S" && x.IsDeleted != 1).ToListAsync();
        }

/*
        [HttpPost("checkbox")]
        public async Task<ActionResult<FcStationAccount>> checkbox([FromForm]int id, [FromForm] bool permissionBool)
        {
            var selectedContent = _context.FcStationAccounts.FirstOrDefault(x => x.Id.Equals(id));

            if (selectedContent != null)
            {
                selectedContent.Permission = (permissionBool == true ? 1 : 0);
            }

            await _context.SaveChangesAsync();  

            return Ok();
        }*/


        // POST: api/FcStationAccounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       

        [HttpPost("loginDropDown")]
        public async Task<ActionResult<FcStationAccount>> loginDropDown([FromForm]String text)
        {

            try
            {
                var id = _context.FcStationAccounts.Where(x => x.Username == text).FirstOrDefault();

                if (id!= null)
                {
                    if (id.Id != 0)
                    {
                        if (id.Status == "S")
                        {
                            return Ok("Admin");
                        }
                        var branches = _context.FcAccountBranches.Where(x => x.AccountId == id.Id).ToList();
                        return Ok(branches);
                    }
                }

                return Ok("noData");
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }

          
        }



        [HttpPost("deleteStationAccount")]
        public async Task<ActionResult<FcStationAccount>> deleteStationAccount([FromForm] int id)
        {

            try
            {

                var update = _context.FcStationAccounts.Where(x => x.Id == id).FirstOrDefault();

                update.IsDeleted = 1;

                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("addAccount")]
public async Task<ActionResult<FcStationAccount>> addAccount(
      [FromForm] FcStationAccount fcStationAccount,
      [FromForm] String data)
        {

            try{
                List<FcAccountBranch> fcAccountBranches = JsonConvert.DeserializeObject<List<FcAccountBranch>>(data)!;

                String keySTR = "qwertyuiopasdfgh"; //16 byte
                String ivSTR = "qwertyuiopasdfgh"; //16 byte

                var checker = _context.FcStationAccounts.Where(x =>
                x.Firstname == fcStationAccount.Firstname &&
                x.Middlename == fcStationAccount.Middlename &&
                x.Lastname == fcStationAccount.Lastname).Count();

                if (checker > 0 )
                {
                    return Ok("Already"); 
                }

                using (System.Security.Cryptography.RijndaelManaged rjm =
                                       new System.Security.Cryptography.RijndaelManaged
                                       {
                                           KeySize = 128,
                                           BlockSize = 128,
                                           Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                           IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                       }
                           )
                {
                    Byte[] input = Encoding.UTF8.GetBytes(fcStationAccount.Firstname.Trim()!);
                    Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
                    fcStationAccount.Password = Convert.ToBase64String(output);
                }

                if (fcStationAccount.Middlename == null)
                {
                    fcStationAccount.Fullname = fcStationAccount.Firstname + " " + fcStationAccount.Lastname;
                }
                else
                {
                    fcStationAccount.Fullname = fcStationAccount.Firstname + " " + fcStationAccount.Middlename + " " + fcStationAccount.Lastname;
                }

                _context.FcStationAccounts.Add(fcStationAccount);

                await _context.SaveChangesAsync();
                for (int x = 0; x < fcAccountBranches.Count; x++)
                {

                    fcAccountBranches[x].AccountId = fcStationAccount.Id;
                    _context.FcAccountBranches.Add(fcAccountBranches[x]);
                }
                await _context.SaveChangesAsync();



                return Ok();
            }catch(Exception e)
            {
                return BadRequest(e.Message);   
            }

}


        [HttpPost("editAccount")]
        public async Task<ActionResult<FcStationAccount>> editAccount(
    [FromForm] FcStationAccount fcStationAccount,
    [FromForm] String data)
        {

            try
            {
                List<FcAccountBranch> fcAccountBranches = JsonConvert.DeserializeObject<List<FcAccountBranch>>(data)!;

              

                var checker = _context.FcStationAccounts.Where(x =>
                x.Firstname == fcStationAccount.Firstname &&
                x.Middlename == fcStationAccount.Middlename &&
                x.Lastname == fcStationAccount.Lastname).FirstOrDefault();

                if (checker != null)
                {
                    if (checker.Id != fcStationAccount.Id)
                    {
                        return Ok("Already");
                    }
                }

                var newData = _context.FcStationAccounts.Where(x =>
                x.Id.Equals(fcStationAccount.Id)).FirstOrDefault();

                if (fcStationAccount.Middlename == null)
                {
                    fcStationAccount.Fullname = fcStationAccount.Firstname + " " + fcStationAccount.Lastname;
                }
                else
                {
                    fcStationAccount.Fullname = fcStationAccount.Firstname + " " + fcStationAccount.Middlename + " " + fcStationAccount.Lastname;
                }

                newData.Fullname = fcStationAccount.Fullname;
                newData.Firstname = fcStationAccount.Firstname;
                newData.Middlename = fcStationAccount.Middlename;
                newData.Lastname = fcStationAccount.Lastname;
                newData.Username = fcStationAccount.Username;

                await _context.SaveChangesAsync();


                var toRemove = _context.FcAccountBranches.Where(x => x.AccountId == fcStationAccount.Id).ToList();

                for (int x = 0; x < toRemove.Count; x++)
                {
                    _context.FcAccountBranches.Remove(toRemove[x]);
                }

                for (int x = 0; x < fcAccountBranches.Count; x++)
                {

                    fcAccountBranches[x].AccountId = fcStationAccount.Id;
                    _context.FcAccountBranches.Add(fcAccountBranches[x]);
                }
                await _context.SaveChangesAsync();



                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpGet("accountBranch")]
        public async Task<ActionResult<FcAccountBranch>> accountBranch(int accountId)
        {

         return Ok(_context.FcAccountBranches.Where(x => x.AccountId == accountId).ToList());   


        }

        [HttpPost("permissions")]
        public async Task<ActionResult<FcAccountBranch>> permissions([FromForm] int accountId, [FromForm] int? branchId)
        {
            try
            {
                var asd = _context.FcStationAccounts.Where(x => x.Id.Equals(accountId) && x.Status == "S").Count();

            if (_context.FcStationAccounts.Where(x => x.Id.Equals(accountId) && x.Status == "S").Count() != 0)
            {
                    return Ok("admin");
            }

            return Ok(_context.FcAccountBranches.Where(x => x.AccountId.Equals(accountId) && x.BranchId.Equals(branchId)).Select(x => x.Permission).FirstOrDefault());

            }catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        [HttpPost("adminpermission")]
        public async Task<ActionResult<FcAccountBranch>> adminpermission([FromForm] String username, [FromForm] String password)
        {
            try
            {

                String keySTR = "qwertyuiopasdfgh"; //16 byte
                String ivSTR = "qwertyuiopasdfgh"; //16 byte

                using (System.Security.Cryptography.RijndaelManaged rjm =
                                       new System.Security.Cryptography.RijndaelManaged
                                       {
                                           KeySize = 128,
                                           BlockSize = 128,
                                           Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                           IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                       }
                           )
                {
                    Byte[] input = Encoding.UTF8.GetBytes(password);
                    Byte[] output = rjm.CreateEncryptor().TransformFinalBlock(input, 0, input.Length);
                     

                    if(_context.FcStationAccounts.Where(x => 
                    x.Username.Equals(username)&&
                    x.Password.Equals(Convert.ToBase64String(output)) &&
                    x.Status == "S"
                    ).Count() != 0)
                    {
                        return Ok("granted");
                    }
                    else
                    {
                        return Ok("failed");
                    }
                }


            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }

        }

        private bool FcStationAccountExists(int id)
        {
            return _context.FcStationAccounts.Any(e => e.Id == id);
        }
    }
}
