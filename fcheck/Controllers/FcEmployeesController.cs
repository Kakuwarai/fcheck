
using fcheck.Models;
using Google.Apis.Auth.OAuth2;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json.Linq;
using RestSharp.Authenticators;
using RestSharp;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace fcheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcEmployeesController : ControllerBase
    {
        private readonly DTCHECKERContext _context;
        private readonly IConfiguration _configuration;
        public FcEmployeesController(DTCHECKERContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }


        [HttpPost("test")]
        public async Task<ActionResult> test()
        {
            try
            {

                var employee = _context.FcEmployees.Where(x => x.DeviceId != null && x.EmployeeId.StartsWith("0")).Select(x => x.EmployeeId).ToList();

                var todayUse = _context.FcAttendances.Where(x => employee.Contains(x.EmployeeId)  && x.Date >= DateTime.Now.AddDays(-9)).ToList();

               
                List <string> empId = new List<string>();

                foreach(string id in employee)
                {
                    empId.Add(id.Remove(0, 1));
                   
                } ;

                var errorChecker = _context.FcAttendances.Where(x => empId.Contains(x.EmployeeId) && x.Date >= DateTime.Now.AddDays(-9)).ToList();

                List <dynamic> counts = new List<dynamic> { employee.Count(), todayUse.Count(), errorChecker };
                /*x.Date >= DateTime.Now.AddDays(-9)*/
                return Ok(counts);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpGet]
        public IActionResult Get(String employee)
        {
            try
            {
                Byte[] b = System.IO.File.ReadAllBytes(@"C:\Users\mjfalvarez\Documents\BackupDatas\Kakuwarai\Factcheck\fcheck\fcheck\Images\" + employee + "//" + employee + ".jpg");   // You can use your own method over here.         
                return File(b, "image/jpeg");
            }catch (Exception e)
            {
                return BadRequest(e.Message);
            }
           
        }


        [HttpGet("datas")]
        public IActionResult datas(int branch, int _page, int _limit, String? search, String? sortBy)
        {    
            if(search == null)
            {
                if (sortBy == "EmployeeId")
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && x.Status != "I").OrderBy(x => x.EmployeeId).Skip(_page * _limit).Take(_limit).ToList());
                }
                else if (sortBy == "Fullname")
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && x.Status != "I").OrderBy(x => x.Fullname).Skip(_page * _limit).Take(_limit).ToList());
                }
                else if (sortBy == "Department")
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && x.Status != "I").OrderBy(x => x.Department).Skip(_page * _limit).Take(_limit).ToList());
                }
                else
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && x.Status != "I").OrderBy(x => x.Sbu).Skip(_page * _limit).Take(_limit).ToList());
                }
              
            }
            else
            {
                if (sortBy == "EmployeeId")
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && (x.Fullname + x.EmployeeId).Contains(search) && x.Status != "I").OrderBy(x => x.EmployeeId).Skip(_page * _limit).Take(_limit).ToList());
                }
                else if (sortBy == "Fullname")
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && (x.Fullname + x.EmployeeId).Contains(search) && x.Status != "I").OrderBy(x => x.Fullname).Skip(_page * _limit).Take(_limit).ToList());
                }
                else if (sortBy == "Department")
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && (x.Fullname + x.EmployeeId).Contains(search) && x.Status != "I").OrderBy(x => x.Department).Skip(_page * _limit).Take(_limit).ToList());
                }
                else
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && (x.Fullname + x.EmployeeId).Contains(search) && x.Status != "I").OrderBy(x => x.Sbu).Skip(_page * _limit).Take(_limit).ToList());
                }
            }
        }

        [HttpGet("datasLength")]
        public IActionResult datasLength(int branch, String? search)
        {
            if (search == null)
            {
                if (_context.FcEmployees.Where(x => x.Branch == branch && x.Status != "I").ToList().Count % 6 == 0)
                {
                    return Ok(_context.FcEmployees.Where(x => x.Branch == branch && x.Status != "I").ToList().Count / 6);
                }
                return Ok((_context.FcEmployees.Where(x => x.Branch == branch && x.Status != "I").ToList().Count / 6) + 1);
            }

            if (_context.FcEmployees.Where(x => x.Branch == branch && (x.Fullname + x.EmployeeId).Contains(search) && x.Status != "I").ToList().Count % 6 == 0)
            {
                return Ok(_context.FcEmployees.Where(x => x.Branch == branch && (x.Fullname + x.EmployeeId).Contains(search) && x.Status != "I").ToList().Count / 6);
            }
            return Ok((_context.FcEmployees.Where(x => x.Branch == branch && (x.Fullname + x.EmployeeId).Contains(search) && x.Status != "I").ToList().Count / 6) + 1);
        }



        [HttpPost("addEmployee")]
        public async Task<ActionResult<FcEmployee>> addEmployee([FromForm] FcEmployee fcEmployee)
        {
            try
            {

                if(fcEmployee.Middlename != null)
                {
                    fcEmployee.Fullname = fcEmployee.Firstname + " " + fcEmployee.Middlename + " " + fcEmployee.Lastname;
                }
                else
                {
                    fcEmployee.Fullname = fcEmployee.Firstname + " " + fcEmployee.Lastname;
                }
                _context.FcEmployees.Add(fcEmployee);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost("deleteEmployee")]
        public async Task<ActionResult<FcEmployee>> deleteEmployee([FromForm] String id)
        {
            try
            {

               var update = _context.FcEmployees.Where(x => x.EmployeeId == id).FirstOrDefault();

                update.Status = "I";
                
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        [HttpPost]
        public async Task<ActionResult<FcEmployee>> PostCbUser([FromForm] String fcEmployees, [FromForm] String deviceInfo)
        {
           
            try
            {
                var dball = _context.FcEmployees.FirstOrDefault(s => s.EmployeeId.Equals(fcEmployees));
                var checker = await _context.FcEmployees.Where(x => x.EmployeeId == fcEmployees).Select(x => x.EmployeeId).FirstOrDefaultAsync();
                if (checker == null)
                {
                    return Ok("isNull");
                }
                else
                {
                    if (dball.DeviceId != null)
                    {
                        if(dball.DeviceId == deviceInfo)
                        {
                            return Ok(checker);
                        }
                        return Ok("AlreadyLogIn");
                    }
                    dball.DeviceId = deviceInfo;
                    await _context.SaveChangesAsync();
                    return Ok(checker);
                }

            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }

           

        }
        [HttpPost("editEmployeeDetails")]
        public async Task<ActionResult<FcEmployee>> editEmployeeDetails([FromForm] FcEmployee fcEmployee)
        {

            try
            {
                var selected = _context.FcEmployees.Where(x => x.EmployeeId == fcEmployee.EmployeeId).FirstOrDefault();
                
                selected.Lastname  = fcEmployee.Lastname;
                selected.Firstname = fcEmployee.Firstname;
                selected.Department = fcEmployee.Department;
                selected.Sbu = fcEmployee.Sbu;

                selected.Status = fcEmployee.Status;

                if (fcEmployee.Middlename != "")
                {
                    selected.Fullname = fcEmployee.Firstname + " " + fcEmployee.Middlename + " " + fcEmployee.Lastname;
                    selected.Middlename = fcEmployee.Middlename;
                }
                else
                {
                    selected.Fullname = fcEmployee.Firstname + " " + fcEmployee.Lastname;
                    selected.Middlename = null;
                }

                await _context.SaveChangesAsync();

                return Ok();
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }



        }


        [HttpPost("email1")]
    public async Task<ActionResult<FcEmployee>> email(
        [FromForm] String fcEmployees,
        [FromForm] String emailAddress,
        [FromForm] String deviceId,
        [FromForm] String device)
      {
            FcAccountVerification fcAccountVerification = new FcAccountVerification();
            Random generator = new Random();
            String code = generator.Next(0, 1000000).ToString("D6");

            void DatabaseAdd()
            {
                fcAccountVerification.EmployeeId = fcEmployees;
                fcAccountVerification.VerificationCode = code;
                _context.FcAccountVerifications.Add(fcAccountVerification);
            }


            try {

                var checkingIfSep = _context.VhrisEmployees.Where(x => x.EmplId == fcEmployees).FirstOrDefault();

                if (fcEmployees != "987654321")
                {
                   
                    if (checkingIfSep != null && checkingIfSep.Type == "SEP")
                    {
                        var editor = _context.FcEmployees.
                        FirstOrDefault(s => s.EmployeeId.Equals(fcEmployees));

                        if (editor != null)
                        {
                            editor.Status = "I";
                            _context.SaveChanges();
                            return Ok("isNull");
                        }
                    }
                }

            var newFcEmployees = _context.FcEmployees.
                    FirstOrDefault(s => s.EmployeeId.Equals(fcEmployees));

            var employeeChecker = await _context.FcEmployees.
                    Where(x => x.EmployeeId == fcEmployees).FirstOrDefaultAsync();

           

                var spliter = "";

                if (deviceId.Contains("-"))
                {
                    spliter = deviceId.Split('-')[0];
                }
               


                if (employeeChecker == null){

                    if(checkingIfSep != null)
                    {
                        _context.FcEmployees.Add( new FcEmployee
                        {
                            EmployeeId = checkingIfSep.EmplId,
                            Lastname = checkingIfSep.Lname,
                            Middlename = checkingIfSep.Mname,
                            Firstname = checkingIfSep.Fname,
                            Fullname = checkingIfSep.EmployeeName2,
                            Branch = 255,
                            Sbu = checkingIfSep.CorporateName,
                            Department = checkingIfSep.Departmentname,
                            Status = "A",
                            ImmediateHeadId = checkingIfSep.ImmediateId,
                            ImmediateHeadName = checkingIfSep.ImmediateName,
                            ImmediateEmail = checkingIfSep.ImmediateEmail,
                            CreatedBy = 1,
                            CreatedDate = DateTime.Now


                        });
                        _context.SaveChanges();
                    }
                    else
                    {
                        return Ok("isNull");
                    }


                
                }
               else if (spliter != "Web" && newFcEmployees.DeviceId != null && newFcEmployees.DeviceId != deviceId)
                {
                    return Ok("Account is already logged in to other device");
                }

                var verificationCodeChecker = await _context.FcAccountVerifications.
                   Where(x => x.EmployeeId == fcEmployees && x.Verified == 0).
                   Select(x => x.VerificationCode).FirstOrDefaultAsync();

                var verificationCodeSelected = await _context.FcAccountVerifications.
                        Where(x => x.EmployeeId == fcEmployees && x.Verified == 0).FirstOrDefaultAsync();

                if (verificationCodeChecker != null)
                {
                    if (Convert.ToInt32(DateTime.Now.Subtract(verificationCodeSelected.CreatedDate).TotalMinutes) < 5)
                    {
                        code = verificationCodeChecker;
                    }
                    else
                    {
                        verificationCodeSelected.Verified = 1;
                        await _context.SaveChangesAsync();
                        DatabaseAdd();
                    }                 
                }
                else
                {
                    DatabaseAdd();
                }                                 

            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["smtpCredentials:MailAddress"]));
            email.To.Add(MailboxAddress.Parse(emailAddress));
            email.Subject = "Fact Check Verification Code";

            email.Body = new TextPart(TextFormat.Html)
            {
                Text = "<p style=\"font-size:20px;\"> This email is sent from " + device+"</p>"+
                "<p style=\"font-size:50px;\">"+code+"</p>"             
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_configuration["smtpCredentials:SmtpServer"], int.Parse(_configuration["smtpCredentials:Port"]) , SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["smtpCredentials:Username"], _configuration["smtpCredentials:Password"]);
            await _context.SaveChangesAsync();
            smtp.Send(email);
            smtp.Disconnect(true);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
            return Ok("Email sent");

        }

        [HttpPost("verification1")]
        public async Task<ActionResult<FcAccountVerification>> verify(
            [FromForm] String fcEmployees,
            [FromForm] String verificationCode,
            [FromForm] String deviceInfo)
        {
            try { 
            var verificationCodeChecker = await _context.FcAccountVerifications.
                 Where(x => x.EmployeeId == fcEmployees && x.Verified == 0).
                 FirstOrDefaultAsync();

            var dball = _context.FcEmployees.FirstOrDefault(s => s.EmployeeId.Equals(fcEmployees));

             

            if (verificationCode == verificationCodeChecker.VerificationCode)
            {
                    if (Convert.ToInt32(DateTime.Now.Subtract(verificationCodeChecker.CreatedDate).TotalMinutes) < 5)
                    {
                        verificationCodeChecker.Verified = 1;
                        dball.DeviceId = deviceInfo;
                        await _context.SaveChangesAsync();
                        String[] list = {
                            "verified",
                            dball.Fullname, 
                            dball.Sbu.ToString(), 
                            dball.Department.ToString()};
                        return Ok(list.ToList());
                    }
                    else
                    {
                        return StatusCode(202,"code is already expired");
                    }
                   
            }
            
            return StatusCode(202, "Invalid verification code");
            }catch(Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("changePassword")]
        public async Task<ActionResult<FcAccountVerification>> changePassword(
          [FromForm] int id,
          [FromForm] String currentPassword,
          [FromForm] String newPassword)
        {
            try
            {
               
                var thisUser = _context.FcStationAccounts.Where(x => x.Id == id).FirstOrDefault();

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
                    byte[] input = Convert.FromBase64String(thisUser.Password);
                    byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                   

                    if (Encoding.UTF8.GetString(output) == currentPassword)
                    {
                    

                        using (System.Security.Cryptography.RijndaelManaged rjm2 =
                                               new System.Security.Cryptography.RijndaelManaged
                                               {
                                                   KeySize = 128,
                                                   BlockSize = 128,
                                                   Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                                   IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                               }
                                   )
                        {
                            Byte[] input2 = Encoding.UTF8.GetBytes(newPassword);
                            Byte[] output2 = rjm2.CreateEncryptor().TransformFinalBlock(input2, 0, input2.Length);
                            thisUser.Password = Convert.ToBase64String(output2);
                        }

                    }
                    else
                    {
                        return Ok("failed");

                    }


                }
                await _context.SaveChangesAsync();

                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("versionChecker")]
        public async Task<ActionResult> versionChecker([FromForm] String appVersion)
        {
            try
            {

                    var currentV = _configuration["appVersion"].Split('.');
                    var userV = appVersion.Split('.');

                if (int.Parse(userV[0]) >= int.Parse(currentV[0]))
                {   
                    if (int.Parse(userV[1])  ==  int.Parse(currentV[1]))
                    {
                        if (int.Parse(userV[2]) == int.Parse(currentV[2]))
                        {
                            return StatusCode(202,"Application is up to date");

                        }else if(int.Parse(userV[2]) > int.Parse(currentV[2])) {
                            return StatusCode(202, "Application is up to date");
                        }
                       
                    }
                    else if(int.Parse(userV[1]) > int.Parse(currentV[1])) {
                        return StatusCode(202, "Application is up to date");
                    }
                  
                }

                List<string> datas = new List<string>()
                {
                    _configuration["appVersion"],
                    _configuration["appLink"]
                };

                return Ok(datas.ToList());

            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }


        [HttpPost("maintenanceLogin")]
        public async Task<ActionResult> maintenanceLogin(String username,String typePassword)
        {
            try
            {


               
                String keySTR = "qwertyuiopasdfgh"; //16 byte
                String ivSTR = "qwertyuiopasdfgh"; //16 byte
                String decodedString;
                using (System.Security.Cryptography.RijndaelManaged rjm2 =
                                             new System.Security.Cryptography.RijndaelManaged
                                             {
                                                 KeySize = 128,
                                                 BlockSize = 128,
                                                 Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                                 IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                             }
                                 )
                {
                    Byte[] input2 = Encoding.UTF8.GetBytes(typePassword);
                    Byte[] output2 = rjm2.CreateEncryptor().TransformFinalBlock(input2, 0, input2.Length);
                    decodedString = Convert.ToBase64String(output2);
                }
                var checker = _context.Users.Where(x => (x.Username == username || x.EmailAddress == username) && x.Password == decodedString).Select(x => new { 
                x.Firstname,
                x.Middlename,
                x.Lastname,
                x.Id,
                x.EmailAddress,
                x.Type,
                x.Username,
                x.Password,
                x.EmployeeId
                }).FirstOrDefault();

              

                if (checker != null)
                {
                    var menu = _context.UserMenus.Where(x => x.UserId == checker.Id).Select(x => new { x.MenuId, x.Status });

                    var returns = new { checker, menu };

                    return Ok(returns);
                }
                return StatusCode(202,"Invalid login credentials");

                    /*  String keySTR = "qwertyuiopasdfgh"; //16 byte
                    String ivSTR = "qwertyuiopasdfgh"; //16 byte
                    String decodedString;
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
                        byte[] input = Convert.FromBase64String(typePassword);
                        byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                        decodedString = Encoding.UTF8.GetString(output);

                    }*/


                }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }


        [HttpPost("googleAuth2")]
        public async Task<ActionResult> googleAuth2()
        {
            try
            {

                // Replace with the path to your JSON key file.
                /* string jsonKeyFilePath = "C:\\Users\\mjfalvarez\\Downloads\\ontime-d9a38-firebase-adminsdk-lkmvc-1087e1262d.json";*/

                // Load the JSON key file and create credentials.

                JObject jsonObject = new JObject(
         new JProperty("type", "service_account"),
         new JProperty("project_id", "ontime-d9a38"),
         new JProperty("private_key_id", "1087e1262d026cc274868e21529ccb810755f6a0"),
         new JProperty("private_key", "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQCQZG45ZKwH2foD\n9F7SwAHdV3Xfa9vFB0uEpFyad8vShXoFwDOM7rqvq7Lbfsw8Vy4/+sNO89gMHwRs\nMhr8nXNmAkOlghgf3imBu2VoPZIVIco0HlrzrT3Vrw0zszfOEekkL8sUbRZRdgFZ\nBd32dv1QJ6AU/bmqfCvGM2BH1iikyOY3AASHr25AlQwwckOKuEmIdnVZggxW+vFZ\nA3Vw3y298E5QwuUobhfyx11wIUb39CaxorB/U4M2pxzr1x3vLMayk+87bdo2g3rh\nawM2RU+RQZuf2qiL+HuHlfCTNQoFmU10ZNQcPOYkSrUU8lBaMosQGSO7Ac0Fuyez\nCvkt7lSXAgMBAAECggEAHQWao/2ImFFnL6lL79c46Li+D+iZdSoosyHoHQIMHSXH\n52UA14haTJnZqsV7Hnu1Z5Wq8PERESqg1HBjvTeWbL/ywc76s1kPhaxZTxDlDktK\n3ZvmUwkUnyD+qdARXOF7t1LF4jak4D20OqTzvqie9J/qTJ3M1if8EmvcI8Q3Shtr\nx2K/muCBdDZ2rQ20pnRKmK+u/p7D/sfdJJ9C0dPA6o0pBOb9HkJESZfCLl/Av/Xm\nTkXbhXlS1ycWxRVl9KHFi7BhzfNPl0014/VCfpPpdl1zmEvvBIuQIyhbQteSe+Wj\nsg4cYnjBvpJ99DZxX6s+FZ6ETh/LW7Z2nFmtqdzNwQKBgQDLAeXaVX0qQ30/uKua\nJz+fiC7L6LJPEm9jggG9tLL+uDtR7YoWOeNWrG7ZvoVYfXSiX1BX6x928YcRK7Lt\n8KtbNCDqenRSjOzsCi4nn1WEYmx/W+W/bWFnpgqyhXoleAUUXzXlmYF6lhrI6Wgx\nB7UdnA1b67ULynie+Un7OaNd1wKBgQC2FYpIP71wVXGHlQvsCSugAKTMQGUwFeBH\nqZSiJsJgv8LP5qWmg1qJiiHUkUz3kdYBLzXPgF+dInwUQm0us4Dwt7vR75/0sna6\n+RvoWfkDuNaHp8B9Z5MXDeS12nIiOBWlDoBr4WxxszSHKlYsJZMhgZl89JA7QpcT\nyFVzip5nQQKBgG1IiEQQ4WBvFG3JLBfAKS/oT8jVa6fFdNFMm0NUk0csRkLEbSGp\n6AgwqzvspDwOzUrbpJSvrjCJtPw5WgldFeHzHgUcBqKp2qD2mgIadB7gHSgygGuc\npmL1r6yKzkZ9Zb6hwpHnIRys0Z52h/BdBdL6vvZM3RmL1YM/D+R+EsPhAoGAI12R\nqzC+Iahm0ZewZhrt3zjgyU8kmC9XjXOxi8ENde8o7mp0+B+BIT+0DG5gTaGEuFws\nDm53o7SD9wdj33M78wUstLzfC47yIqSpt/pptEBdYqHBcFoCprKrHmscSesswQ9N\nACcbggX1iN8/N3ng1twuLKfvr6LiZxQ9DKD63UECgYEAi6axwz6he/o3VWS8i+sV\nABDD9Rx/FjBvQuCUMw3QB2fjz4J2Ojy9EynSJVVZ3WZWYQ2NwY5bclcT3k6cQhUN\n7ck+rbe63/PpQ+y3Afn+/ecLEjt8/WoNBG2SHsIsD5G+80ElJeFQ/A40GcwLL6sn\nzeWvcq0O90tzTsJMx0NBfzc=\n-----END PRIVATE KEY-----\n"),
         new JProperty("client_email", "firebase-adminsdk-lkmvc@ontime-d9a38.iam.gserviceaccount.com"),
         new JProperty("client_id", "107993299512943381972"),
         new JProperty("auth_uri", "https://accounts.google.com/o/oauth2/auth"),
         new JProperty("token_uri", "https://oauth2.googleapis.com/token"),
         new JProperty("auth_provider_x509_cert_url", "https://www.googleapis.com/oauth2/v1/certs"),
         new JProperty("client_x509_cert_url", "https://www.googleapis.com/robot/v1/metadata/x509/firebase-adminsdk-lkmvc%40ontime-d9a38.iam.gserviceaccount.com"),
         new JProperty("universe_domain", "googleapis.com")
     );


                GoogleCredential googleCredential;
                /* using (var jsonStream = new FileStream(jsonKeyFilePath, FileMode.Open, FileAccess.Read))
                 {
                     googleCredential = GoogleCredential.FromJson(jsonKeyFilePath);
                 }*/
                googleCredential = GoogleCredential.FromJson(jsonObject.ToString());
                // Use the credentials to obtain an access token.
                googleCredential = googleCredential.CreateScoped("https://www.googleapis.com/auth/cloud-platform");

                // Get the access token.
                string accessToken = googleCredential.UnderlyingCredential.GetAccessTokenForRequestAsync().Result;
               
                return Ok(accessToken);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("googleAuth2test")]
        public async Task<ActionResult> googleAuth2test([FromForm] String title, [FromForm] String message)
        {
            try
            {
                string ApiKey = "MDUyMzMzZjQtNGFmOS00ZjVkLTgyMTctYjI2M2Y3MTYyNGFj";
                string AppId = "6087a792-7c1a-4bb1-9970-09e520fbcd58";

                using (var httpClient = new HttpClient())
                {
                    string apiUrl = "https://onesignal.com/api/v1/notifications";

                    httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + ApiKey);

                    var payload = new
                    {
                        app_id = AppId,
                        headings = new { en = title },
                        contents = new { en = message },
                        included_segments = new[] { "All" }
                    };

                    var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);

                    var content = new StringContent(jsonPayload, System.Text.Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return Ok("Notification sent successfully!");                     }
                    else
                    {
                        return Ok($"Error sending notification: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }


               
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("dashBoard")]
        public async Task<ActionResult> dashBoard()
        {
            try
            {
                var allNotRegistereUser = /*_context.FcEmployees.Where(x => x.DeviceId == null).Count()*/0;

                var allRegisteredUser = _context.FcEmployees.Where(x => x.DeviceId != null).Count();

              /*  var today = _context.FcAttendances.Where(x => x.Date == DateTime.Now.Date).Count();*/

                var workPlace = _context.FcAttendances.Where(x => x.Date == DateTime.Now.Date).ToList();

                var office = workPlace.Where(x => x.WorkPlace == "Office" || x.WorkPlace == "OFFICE").Count();

                var wfh = workPlace.Where(x => x.WorkPlace == "wfh" || x.WorkPlace == "WFH").Count();

                var obt = workPlace.Where(x => x.WorkPlace == "obt" || x.WorkPlace == "OBT").Count();
/*
                var officeOut = workPlace.Where(x => x.WorkPlaceOut == "office" || x.WorkPlaceOut == "OFFICE").Count();

                var wfhOut = workPlace.Where(x => x.WorkPlaceOut == "wfh" || x.WorkPlaceOut == "WFH").Count();

                var obtOut = workPlace.Where(x => x.WorkPlaceOut == "obt" || x.WorkPlaceOut == "OBT").Count();*/

                var sevenDays = _context.FcAttendances.Where(x => x.Date >= DateTime.Now.AddDays(-9)).Select(x => new {
                    x.WorkPlace,
                    x.Date
                }).ToList();

                List <dynamic> returns = new List<dynamic>() { allNotRegistereUser, allRegisteredUser, workPlace.Count(), office ,wfh , obt, sevenDays };


                return Ok(returns);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }


        [HttpPost("employeesData")]
        public async Task<ActionResult> employeesData(int page, String? Search)
        {
            try
            {
                var returns = _context.FcEmployees.
                    Where(x => x.Status == "A" && ( Search == null? x.Fullname.Contains(""): x.Fullname.Contains(Search)||x.EmployeeId.Contains(Search) )).
                    Select(x => new {
                    x.EmployeeId,
                    x.Fullname,
                    x.Department,
                    x.ImmediateHeadName
                    }).
                    OrderBy(x => x.Fullname).
                    Skip(page * 10).
                    Take(10).
                    ToList();

                return Ok(returns);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("employeesDataCount")]
        public async Task<ActionResult> employeesDataCount(String? Search)
        {
            try
            {
                var returns = _context.FcEmployees.
                    Where(x => x.Status == "A" && (Search == null ? x.Fullname.Contains("") : x.Fullname.Contains(Search) || x.EmployeeId.Contains(Search))).
                    Count();

                if ((returns % 10) > 0)
                {
                    return Ok((returns / 10) + 1);
                }

                return Ok(returns/10);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("menuAccess")]
        public async Task<ActionResult> menuAccess(int page, String? Search, String? empId)
        {
            try
            {
                var users = _context.Users.
                    Where(x => x.Type != "Super" && x.EmployeeId != empId && x.Status != 0 && (Search == null ? (x.Firstname.Contains("") || x.Middlename.Contains("") || x.Lastname.Contains("")) : ((x.Firstname +" "+ x.Middlename + " " + x.Lastname).Contains(Search) || x.EmailAddress.Contains(Search)))).
                    Select(x => new {
                        x.Id,
                        x.Firstname,
                        x.Middlename,
                        x.Lastname,
                        x.EmailAddress,
                    }).
                    OrderBy(x => x.Firstname).
                    Skip(page * 10).
                    Take(10).
                    ToList();

                List <dynamic> returns = new List<dynamic>();

                foreach (var user in users)
                {
                    var temp = _context.UserMenus.Where(x => x.UserId == user.Id && x.Status == 1).Select(x => new
                    {
                        x.MenuId,
                        x.UserId

                    });

                    returns.Add(new{
                        user,
                        temp
                    });
                }

                return Ok(returns);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("menuAccessCount")]
        public async Task<ActionResult> menuAccessCount(String? Search, String? empId)
        {
            try
            {
                var returns = _context.Users.
                    Where(x => x.Type != "Super" && x.EmployeeId != empId && x.Status != 0 && (Search == null ? (x.Firstname.Contains("") || x.Middlename.Contains("") || x.Lastname.Contains("")) : (x.Firstname.Contains(Search) || x.Middlename.Contains(Search) || x.Lastname.Contains(Search) || x.EmailAddress.Contains(Search)))).Count();

                if ((returns % 10) > 0)
                {
                    return Ok((returns / 10) + 1);
                }

                return Ok(returns / 10);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

   

            [HttpPost("menuAccessSave")]
        public async Task<ActionResult> menuAccessSave(long id,long userId, byte user, byte account, byte menu, byte employee, byte messaging)
        {
            try
            {
                
                    var temp = _context.UserMenus.Where(x => x.UserId == userId && x.MenuId == 2).FirstOrDefault();

                    if(temp != null)
                    {
                        temp.Status = user;
                    }
                    else
                    {
                        _context.UserMenus.Add(new UserMenu
                        {
                            UserId = userId,
                            MenuId = 2,
                            Status = user,
                            BranchCode = "ALA",
                            CreatedByUserId = id,
                            CreatedDate = DateTime.Now,
                            Add = 1,
                            Edit = 1,
                            Delete = 1
                        });
                    }

                
                
                    var temp1 = _context.UserMenus.Where(x => x.UserId == userId && x.MenuId == 3).FirstOrDefault();

                    if (temp1 != null)
                    {
                        temp1.Status = account;
                    }
                    else
                    {
                        _context.UserMenus.Add(new UserMenu
                        {
                            UserId = userId,
                            MenuId = 3,
                            Status = account,
                            BranchCode = "ALA",
                            CreatedByUserId = id,
                            CreatedDate = DateTime.Now,
                            Add = 1,
                            Edit = 1,
                            Delete = 1
                        });
                    }
                
                
                    var temp2 = _context.UserMenus.Where(x => x.UserId == userId && x.MenuId == 6).FirstOrDefault();

                    if (temp2 != null)
                    {
                        temp2.Status = menu;
                    }
                    else
                    {
                        _context.UserMenus.Add(new UserMenu
                        {
                            UserId = userId,
                            MenuId = 6,
                            Status = menu,
                            BranchCode = "ALA",
                            CreatedByUserId = id,
                            CreatedDate = DateTime.Now,
                            Add = 1,
                            Edit = 1,
                            Delete = 1
                        });
                    }
                
               
                    var temp3 = _context.UserMenus.Where(x => x.UserId == userId && x.MenuId == 5).FirstOrDefault();

                    if (temp3 != null)
                    {
                        temp3.Status = employee;
                    }
                    else
                    {
                        _context.UserMenus.Add(new UserMenu
                        {
                            UserId = userId,
                            MenuId = 5,
                            Status = employee,
                            BranchCode = "ALA",
                            CreatedByUserId = id,
                            CreatedDate = DateTime.Now,
                            Add = 1,
                            Edit = 1,
                            Delete = 1
                        });
                    }

                var temp4 = _context.UserMenus.Where(x => x.UserId == userId && x.MenuId == 7).FirstOrDefault();

                if (temp4 != null)
                {
                    temp4.Status = messaging;
                }
                else
                {
                    _context.UserMenus.Add(new UserMenu
                    {
                        UserId = userId,
                        MenuId = 7,
                        Status = messaging,
                        BranchCode = "ALA",
                        CreatedByUserId = id,
                        CreatedDate = DateTime.Now,
                        Add = 1,
                        Edit = 1,
                        Delete = 1
                    });
                }

                _context.SaveChanges();

                return Ok();

            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("accounts")]
        public async Task<ActionResult> accounts(int page, String? Search, String? empId)
        {
            try
            {
                var returns = _context.Users.
                    Where(x => x.Type != "Super" && x.EmployeeId  != empId && (Search == null ? (x.Firstname.Contains("") || x.Middlename.Contains("") || x.Lastname.Contains("")) : (x.Firstname.Contains(Search) || x.Middlename.Contains(Search) || x.Lastname.Contains(Search) || x.EmailAddress.Contains(Search)))).
                    Select(x => new {
                        x.Firstname,
                        x.Middlename,
                        x.Lastname,
                        x.EmailAddress,
                        x.Username,
                        x.Type,
                        x.Id,
                        x.Status
                    }).
                    OrderBy(x => x.Firstname).
                    Skip(page * 10).
                    Take(10).
                    ToList();

                return Ok(returns);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }
        [HttpPost("accountsDataCount")]
        public async Task<ActionResult> accountsDataCount(String? Search, String? empId)
        {
            try
            {
                var returns = _context.Users.
                    Where(x => x.Type != "Super" && x.EmployeeId != empId && (Search == null ? (x.Firstname.Contains("") || x.Middlename.Contains("") || x.Lastname.Contains("")) : (x.Firstname.Contains(Search) || x.Middlename.Contains(Search) || x.Lastname.Contains(Search) || x.EmailAddress.Contains(Search)))).
                    Count();

                if ((returns % 10) > 0)
                {
                    return Ok((returns / 10) + 1);
                }

                return Ok(returns / 10);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("accountsAdd")]
        public async Task<ActionResult> accountsAdd(User user)
        {
            try
            {

                String keySTR = "qwertyuiopasdfgh"; //16 byte
                String ivSTR = "qwertyuiopasdfgh"; //16 byte
                String decodedString;
                using (System.Security.Cryptography.RijndaelManaged rjm2 =
                                             new System.Security.Cryptography.RijndaelManaged
                                             {
                                                 KeySize = 128,
                                                 BlockSize = 128,
                                                 Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                                 IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                             }
                                 )
                {
                    Byte[] input2 = Encoding.UTF8.GetBytes(user.Password);
                    Byte[] output2 = rjm2.CreateEncryptor().TransformFinalBlock(input2, 0, input2.Length);
                    user.Password = Convert.ToBase64String(output2);
                }
                user.CreatedDate = DateTime.Now;
                user.Status = 1;
                _context.Users.Add(user);
                _context.SaveChanges();

                return Ok("Success!");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }


        [HttpPost("accountsEdit")]
        public async Task<ActionResult> accountsEdit(User user)
        {
            try
            {

                var editUser = _context.Users.Where(x => x.Id == user.Id).FirstOrDefault();

                editUser.Firstname = user.Firstname;
                editUser.Middlename = user.Middlename;
                editUser.Lastname = user.Lastname;
                editUser.Username = user.Username;
                editUser.EmailAddress = user.EmailAddress;
                editUser.Type = user.Type;
                editUser.Status = user.Status;
                editUser.ModifiedByUserId = user.ModifiedByUserId;
                editUser.ModifiedDate = DateTime.Now;

                _context.SaveChanges();

                return Ok("Success!");
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedEmployee")]
        public async Task<ActionResult> selectedEmployee(String employeeId)
        {
            try
            {
                var returns = _context.FcEmployees.Select( x => new
                {
                    x.EmployeeId,
                    x.Fullname,
                    x.Sbu,
                    x.Department,
                    x.ImmediateHeadName,
                    x.DeviceId
                }).FirstOrDefault(x => x.EmployeeId == employeeId);

                return Ok(returns);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedEmployeeResetDeviceId")]
        public async Task<ActionResult> selectedEmployeeResetDeviceId(String employeeId)
        {
            try
            {
                var returns = _context.FcEmployees.FirstOrDefault(x => x.EmployeeId == employeeId);


                if (returns != null)
                {
                    returns.DeviceId = null;
                    _context.SaveChanges();
                }



                return Ok(returns);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedEmployeeSubordinate")]
        public async Task<ActionResult> selectedEmployeeSubordinate(String employeeId)
        {
            try
            {
                var returns = _context.FcEmployees.Where(x => x.ImmediateHeadId == employeeId && x.Status == "A").Select(x => new
                {
                    x.EmployeeId,
                    x.Fullname,
                    x.Sbu,
                    x.Department,
                    x.DeviceId
                }).OrderBy(x => x.Fullname).ToList();

                return Ok(returns);
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedEmployeeSyncSubordinate")]
        public async Task<ActionResult> selectedEmployeeSyncSubordinate(String employeeId)
        {
            try
            {
                var employeeIds = _context.VhrisEmployees.Where(x => x.ImmediateId == employeeId && x.Type != "SEP").Select(x => new
                {
                    x.EmplId,
                    x.EmployeeName2,
                    x.ImmediateId,
                    x.ImmediateName,
                    x.ImmediateEmail
                }).ToList();


                if (employeeIds != null)
                {
                    for (int index = 0; index < employeeIds.Count; index++)
                    {


                        var toChange = _context.FcEmployees.FirstOrDefault(x => x.EmployeeId == employeeIds[index].EmplId);

                        if (toChange != null)
                        {
                            toChange.ImmediateHeadId = employeeIds[index].ImmediateId;
                            toChange.ImmediateHeadName = employeeIds[index].ImmediateName;
                            toChange.ImmediateEmail = employeeIds[index].ImmediateEmail;


                        }


                    }
                    _context.SaveChanges();
                }

                return Ok();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedEmployeeActivity")]
        public async Task<ActionResult<FcEmployee>> selectedEmployeeActivity(String employeeId, DateTime? filteredDate, int page)
        {
            try
            {
                var returns = _context.FcAttendances.Where(x => x.EmployeeId == employeeId &&
                (filteredDate == null ? x.Date <= DateTime.Now : x.Date == filteredDate.Value.Date)).OrderByDescending(x => x.Date).
                    Skip(page * 10).
                    Take(10).ToList();

                return Ok(returns);

            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedEmployeeActivityCount")]
        public async Task<ActionResult<FcEmployee>> selectedEmployeeActivityCount(String employeeId, DateTime? filteredDate)
        {
            try
            {
                var returns = _context.FcAttendances.Where(x => x.EmployeeId == employeeId &&
                (filteredDate == null ? x.Date <= DateTime.Now : x.Date == filteredDate.Value.Date)).OrderByDescending(x => x.Date).Count();

                if ((returns % 10) > 0)
                {
                    return Ok((returns / 10) + 1);
                }

                return Ok(returns / 10);

            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedSubordinateActivity")]
        public async Task<ActionResult<FcEmployee>> selectedSubordinateActivity(String employeeId, DateTime? filteredDate, int page)
        {
            try
            {
                if (employeeId.Contains("*"))
                {
                    var temp = _context.FcEmployees.Where(x => x.ImmediateHeadId == employeeId.Trim(',', '*')).Select(x => x.EmployeeId).ToList();

                    if (temp.Count == 0)
                    {
                        return Ok();
                    }
                    employeeId = temp.Aggregate((total, part) => $"{total} {part}");
                }

               

                var records = _context.FcAttendances.Join(_context.FcEmployees,
                    attendance => attendance.EmployeeId,
                    employee => employee.EmployeeId,
                     (attendance, employee) => new { Attendance = attendance, Employee = employee }).Where(x =>
                     (employeeId.Contains(x.Attendance.EmployeeId)) &&
                      (filteredDate == null ? x.Attendance.Date <= DateTime.Now : x.Attendance.Date == filteredDate.Value.Date)
                      ).AsEnumerable().Reverse().OrderByDescending(x => x.Attendance.Date).Skip(page * 10).Take(10).Select(x => new
                      {
                          x.Attendance.Date,
                          x.Employee.EmployeeId,
                          x.Employee.Fullname,
                          x.Attendance.TimeIn,
                          x.Attendance.TimeOut,
                          x.Attendance.TotalTime,
                          x.Attendance.WorkPlace,
                          x.Attendance.WorkPlaceOut,
                          x.Attendance.LocationIn,
                          x.Attendance.LocationOut,
                      }).Distinct().ToList();


                return Ok(records);

            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }


        [HttpPost("selectedSubordinateActivityCount")]
        public async Task<ActionResult<FcEmployee>> selectedSubordinateActivityCount(String employeeId, DateTime? filteredDate)
        {
            try
            {

                if (employeeId.Contains("*"))
                {
                    var temp = _context.FcEmployees.Where(x => x.ImmediateHeadId == employeeId.Trim(',', '*')).Select(x => x.EmployeeId).ToList();

                    if (temp.Count == 0)
                    {
                        return Ok();
                    }
                    employeeId = temp.Aggregate((total, part) => $"{total} {part}");
                }

                var records = _context.FcAttendances.Join(_context.FcEmployees,
                    attendance => attendance.EmployeeId,
                    employee => employee.EmployeeId,
                     (attendance, employee) => new { Attendance = attendance, Employee = employee }).Where(x =>
                     (employeeId.Contains(x.Attendance.EmployeeId)) &&
                      (filteredDate == null ? x.Attendance.Date <= DateTime.Now : x.Attendance.Date == filteredDate.Value.Date)
                      ).Distinct().Count();


                if ((records % 10) > 0)
                {
                    return Ok((records / 10) + 1);
                }

                return Ok(records / 10);

            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        [HttpPost("selectedSubordinateActivitySubordinates")]
        public async Task<ActionResult<FcEmployee>> selectedSubordinateActivitySubordinates(String employeeId)
        {
            try
            {

                var records = _context.FcEmployees.Where(x => x.ImmediateHeadId == employeeId && x.Status =="A").Select(x => new {
                    x.EmployeeId,
                    x.Fullname
                }).ToList();

                

                return Ok(records);

            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        public class valueEmployee
        {
            public long employeeId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string Lastname { get; set; }
            public string Firstname { get; set; }
            public string Middlename { get; set; }
        }

        [HttpPost("profileSave")]
        public async Task<ActionResult<FcEmployee>> profileSave(valueEmployee values)
        {
            try
            {


                String keySTR = "qwertyuiopasdfgh"; //16 byte
                String ivSTR = "qwertyuiopasdfgh"; //16 byte
                String decodedString;
                using (System.Security.Cryptography.RijndaelManaged rjm2 =
                                             new System.Security.Cryptography.RijndaelManaged
                                             {
                                                 KeySize = 128,
                                                 BlockSize = 128,
                                                 Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                                 IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                             }
                                 )
                {
                    Byte[] input2 = Encoding.UTF8.GetBytes(values.Password);
                    Byte[] output2 = rjm2.CreateEncryptor().TransformFinalBlock(input2, 0, input2.Length);
                    decodedString = Convert.ToBase64String(output2);
                }

                User userDetails = _context.Users.Where(x => x.Id == values.employeeId && x.Password == decodedString).FirstOrDefault();

                if (userDetails != null)
                {
                    userDetails.Username = values.Username.ToUpper();
                    userDetails.Firstname = values.Firstname.ToUpper();
                    userDetails.Middlename = values.Middlename.ToUpper();
                    userDetails.Lastname = values.Lastname.ToUpper();

                    _context.SaveChanges();

                    return Ok(userDetails);
                }
                else
                {
                    return StatusCode(202, "Wrong password");
                }




            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }


        [HttpPost("profileSavePassword")]
        public async Task<ActionResult<FcEmployee>> profileSavePassword(long userId, String oldPassword, String newPassword)
        {
            try
            {

                oldPassword = encrypt(oldPassword).Result;
                newPassword = encrypt(newPassword).Result;

                User userDetails = _context.Users.Where(x => x.Id == userId && x.Password == oldPassword).FirstOrDefault();

                if (userDetails != null)
                {
                    userDetails.Password = newPassword;


                    _context.SaveChanges();

                    return Ok("Saved successfully");
                }
                else
                {
                    return StatusCode(202, "Wrong password");
                }




            }
            catch (Exception e)
            {
                if (e.Message.Contains("InnerException") || e.Message.Contains("inner exception"))
                {

                    return StatusCode(202, "InnerExeption: " + e.InnerException);
                }
                else
                {

                    return StatusCode(202, "Error Message: " + e.Message);
                }
            }
        }

        public static async Task<String> encrypt(string password)
        {
            try
            {

                String keySTR = "qwertyuiopasdfgh"; //16 byte
                String ivSTR = "qwertyuiopasdfgh"; //16 byte
                String decodedString;
                using (System.Security.Cryptography.RijndaelManaged rjm2 =
                                             new System.Security.Cryptography.RijndaelManaged
                                             {
                                                 KeySize = 128,
                                                 BlockSize = 128,
                                                 Key = ASCIIEncoding.ASCII.GetBytes(keySTR),
                                                 IV = ASCIIEncoding.ASCII.GetBytes(ivSTR)
                                             }
                                 )
                {
                    Byte[] input2 = Encoding.UTF8.GetBytes(password);
                    Byte[] output2 = rjm2.CreateEncryptor().TransformFinalBlock(input2, 0, input2.Length);
                    decodedString = Convert.ToBase64String(output2);
                    return decodedString;
                }
            }
            catch (HttpRequestException ex)
            {
                throw new Exception($"HTTP Request Error: {ex.Message}");
            }
        }
        /*    [HttpPost("Twillo")]
            public async Task<ActionResult<FcEmployee>> Twillo()
            {
                string accountSid = "ACad0ec9f883e72875e95562a5e7c36627";
                string authToken = "769fbb598bb29d8a57d7fe05acc647f5";

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: "Hi there",
                    from: "+639972138187",
                    to: "+639205191039"
                );

                return Ok(message.Sid);

            }*/

        // POST: api/FcEmployees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /* [HttpPost]
         public async Task<ActionResult<FcEmployee>> PostFcEmployee(FcEmployee fcEmployee)
         {

             String message = HttpUtility.UrlEncode("This is your message");
             using (var wb = new WebClient())
             {
                 byte[] response = wb.UploadValues("https://api.txtlocal.com/send/", new NameValueCollection()
                 {
                 {"apikey" , "MzI0NTY3Njg3NjVhNTY3MTM4NDQzNDZjNDk2MjcxNTM="},
                 {"numbers" , "09972138187"},
                 {"message" , message},
                 {"sender" , "FastGroup"}
                 });
                 string result = System.Text.Encoding.UTF8.GetString(response);
                 return Ok(result);
             }

             *//* String result;
              string apiKey = "MzI0NTY3Njg3NjVhNTY3MTM4NDQzNDZjNDk2MjcxNTM=";
              string numbers = "09972138187"; // in a comma seperated list
              string message = "This is your message";
              string sender = "textApi";

              String url = "https://api.txtlocal.com/send/?apikey=" + apiKey + "&numbers=" + numbers + "&message=" + message + "&sender=" + sender;
              //refer to parameters to complete correct url string

              StreamWriter myWriter = null;
              HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);

              objRequest.Method = "POST";
              objRequest.ContentLength = Encoding.UTF8.GetByteCount(url);
              objRequest.ContentType = "application/x-www-form-urlencoded";
              try
              {
                  myWriter = new StreamWriter(objRequest.GetRequestStream());
                  myWriter.Write(url);
              }
              catch (Exception e)
              {
                  return Ok(e.Message);
              }
              finally
              {
                  myWriter.Close();
              }

              HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
              using (StreamReader sr = new StreamReader(objResponse.GetResponseStream()))
              {
                  result = sr.ReadToEnd();
                  // Close and clean up the StreamReader
                  sr.Close();
              }
              return Ok(result);*/

        /*_context.FcEmployees.Add(fcEmployee);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetFcEmployee", new { id = fcEmployee.Id }, fcEmployee);*//*
    }*/

        /*    [HttpPost]
            public async Task<ActionResult<FcEmployee>> PostFcEmployee(FcEmployee fcEmployee)
            {
                *//*string accountSid = Environment.GetEnvironmentVariable("ACad0ec9f883e72875e95562a5e7c36627");
                string authToken = Environment.GetEnvironmentVariable("769fbb598bb29d8a57d7fe05acc647f5");

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: "Hi there",
                    from: "Fast Services",
                    to: new Twilio.Types.PhoneNumber("+639972138187")
                );

                return Ok(message.Sid);*//*
                return Ok();
            }*/




        private bool FcEmployeeExists(int id)
        {
            return _context.FcEmployees.Any(e => e.Id == id);
        }
    }
}
