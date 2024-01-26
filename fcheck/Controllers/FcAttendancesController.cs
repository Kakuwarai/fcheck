using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using fcheck.Models;
using System.Text.Json;
using System.Reflection.Metadata;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Cms;
using Twilio.TwiML.Voice;
using Org.BouncyCastle.Utilities;
using System.Threading;
using System.Net;
using Org.BouncyCastle.Asn1.X509;
using System.Globalization;
using System.Text;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using Newtonsoft.Json;
using static fcheck.Controllers.FcAttendancesController;
using System.Timers;
using System.Runtime.Serialization;
using Org.BouncyCastle.Ocsp;
using System.Runtime.Serialization.Json;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
using System.Text.RegularExpressions;

namespace fcheck.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FcAttendancesController : ControllerBase
    {
        private readonly DTCHECKERContext _context;
        private readonly IConfiguration _configuration;

        public FcAttendancesController(DTCHECKERContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("xlxs")]
        public async Task<ActionResult<FcAttendance>> getEmployeeSubordinateRecords([FromForm] String empID)
        {
            try
            {

                //22045668
                DateTime dates = new DateTime(2023, 12, 30);
                DateTime date = new DateTime(2023, 07, 1);

                return Ok(_context.FcAttendances.Where(x => x.EmployeeId == empID && x.Date <= dates && x.Date >= date).Select(x => new{ x.LocationIn, x.TimeIn, x.LocationOut, x.TimeOut, x.Date}).ToList());

            }
            catch (Exception ex)
            {
                return StatusCode(202, ex.Message);
            }
        }

        [HttpPost("getEmployeeRecords1")]
        public async Task<ActionResult<FcAttendance>> getEmployeeRecords([FromForm] String employeeId, [FromForm] DateTime? filteredDate, [FromForm] int page)
        {
            try
            {


                /*    var records = _context.FcAttendances.Where(x => 
                    x.EmployeeId == employeeId &&
                    (filteredDate == null ? x.Date >= DateTime.Now.AddDays(-25) : x.Date == filteredDate.Value.Date)
                    ).OrderByDescending(x => x.Date).ToList();


                    return Ok(records);*/

                /* var records = _context.FcAttendances.Where(x =>
               employeeId.Contains(x.EmployeeId) &&
               (filteredDate == null ? x.Date >= DateTime.Now.AddDays(-25) : x.Date == filteredDate.Value.Date)
               ).Select(x => new { 
                   x.TimeIn,
                   x.TimeOut,
               x.EmployeeId,
               x.Id,
               x.Date
               }).OrderByDescending(x => x.Date).ToList();*/

                /*   List <String> newEmployeeId = new List<String>();

                   if (employeeId.Contains(","))
                   {
                       newEmployeeId = employeeId.Split(",").ToList();
                   }
                   else
                   {
                       newEmployeeId.Add(employeeId);
                   }*/


                if (employeeId.Contains("*"))
                {


                    var temp = _context.FcEmployees.Where(x => x.ImmediateHeadId == employeeId.Trim(',', '*')).Select(x => x.EmployeeId).ToList();

                    employeeId = temp.Aggregate((total, part) => $"{total} {part}");

                }

                var records = _context.FcAttendances.Join(_context.FcEmployees,
                    attendance => attendance.EmployeeId,
                    employee => employee.EmployeeId,
                     (attendance, employee) => new { Attendance = attendance, Employee = employee }).Where(x =>
                     (employeeId.Contains(x.Attendance.EmployeeId)) &&
                      (filteredDate == null ? x.Attendance.Date >= DateTime.Now.AddDays(-25) : x.Attendance.Date == filteredDate.Value.Date)
                      ).AsEnumerable().Reverse().OrderByDescending(x => x.Attendance.Date).Skip(page * 30).Take(30).Select(x => new
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
            catch (Exception ex)
            {
                return StatusCode(202, ex.Message);
            }
        }

        [HttpPost("getEmployeeSubordinateRecords1")]
        public async Task<ActionResult<FcAttendance>> getEmployeeSubordinateRecords([FromForm] String employeeId, [FromForm] String? search, [FromForm] int? page)
        {
            try
            {


                var subordinates = _context.FcEmployees.Where(x => x.ImmediateHeadId == employeeId && x.Status == "A" &&
                (search == null ? x.Fullname.Contains("") : x.Fullname.Contains(search))
                )/*.Skip(page * 20).Take(20)*/.Select(x => new
                {
                    x.Fullname,
                    x.EmployeeId
                }).OrderBy(x => x.Fullname).ToList().DistinctBy(x => x.EmployeeId);



                return Ok(subordinates);

            }
            catch (Exception ex)
            {
                return StatusCode(202, ex.Message);
            }
        }

        [HttpGet("datas")]
        public IActionResult datas(String branch, int _page, int _limit)
        {
            var d = DateTime.Now.ToString("yyyy/MM/dd");

            var records = _context.FcAttendances.
                Where(x => x.Date == DateTime.Parse(d) && x.BranchIn == branch).
                Skip(_page * 5).
                Take(5).
                OrderByDescending(x => x.Id).
                ToList();

            return Ok(records);
        }
        [HttpGet("datasLength")]
        public IActionResult datasLength(String branch)
        {
            var d = DateTime.Now.ToString("yyyy/MM/dd");

            var asd = _context.FcAttendances.Where(x => x.Date == DateTime.Parse(d) && x.BranchIn == branch).FirstOrDefault();

            if (_context.FcAttendances.Where(x => x.Date == DateTime.Parse(d) && x.BranchIn == branch).ToList() != null)
            {
                if (_context.FcAttendances.Where(x => x.Date == DateTime.Parse(d) && x.BranchIn == branch).ToList().Count % 5 == 0)
                {
                    return Ok(_context.FcAttendances.Where(x => x.Date == DateTime.Parse(d) && x.BranchIn == branch).ToList().Count / 5);
                }
                return Ok((_context.FcAttendances.Where(x => x.Date == DateTime.Parse(d) && x.BranchIn == branch).ToList().Count / 5) + 1);
            }
            else
                return Ok();


        }

        [HttpPost("timeInOutWeb")]
        public async Task<ActionResult<FcAttendance>> timeInOutWeb(
            [FromForm] String accountBranch,
            [FromForm] String timeInOut,
            [FromForm] String locationInOut,
            [FromForm] String latLong,
            [FromForm] String enc,
            [FromForm] DateTime date)
        {
            try
            {
                FcAttendance fcAttendance = new FcAttendance();

                String keySTR = "qwertyuiopasdfgh"; //16 byte
                String ivSTR = "qwertyuiopasdfgh"; //16 byte
                String[] decodedString;
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
                    byte[] input = Convert.FromBase64String(enc);
                    byte[] output = rjm.CreateDecryptor().TransformFinalBlock(input, 0, input.Length);
                    decodedString = (Encoding.UTF8.GetString(output)).Split(',');

                }



                fcAttendance.EmployeeId = decodedString[0];
                fcAttendance.Department = decodedString[3];
                fcAttendance.Department = decodedString[4];
                var Logs = decodedString[2];
                fcAttendance.LocationIn = locationInOut;
                fcAttendance.LocationOut = locationInOut;
                fcAttendance.LatLongIn = latLong;
                fcAttendance.LatLongOut = latLong;
                fcAttendance.TimeIn = timeInOut;
                fcAttendance.TimeOut = timeInOut;
                fcAttendance.Date = date;
                fcAttendance.WorkPlace = "Office";


                var TimeOut = _context.FcAttendances.Where(x =>
             x.EmployeeId == fcAttendance.EmployeeId &&
             x.Date == date &&
             x.TimeOut == null &&
             x.TotalTime == null).Select(x => x.TimeIn).FirstOrDefault();
                var tIns = DateTime.Parse(fcAttendance.TimeIn).ToString(@"hh\:mm tt");

                if (TimeOut == null && Logs == "Time: in")
                {
                    var time = "";


                    var splitter = tIns.Split(' ');

                    if (splitter[1] == "pm")
                    {
                        time = splitter[0] + " PM";
                    }
                    else
                    {
                        time = splitter[0] + " AM";
                    }


                    fcAttendance.BranchIn = accountBranch;
                    fcAttendance.TimeIn = time;
                    fcAttendance.TimeOut = null;
                    fcAttendance.TotalTime = null;
                    fcAttendance.LocationOut = null;
                    fcAttendance.LatLongOut = null;
                    _context.FcAttendances.Add(fcAttendance);
                    await _context.SaveChangesAsync();
                }
                else if (Logs == "Time: out" && TimeOut != null)
                {
                    if (Logs == "Time: in")
                    {
                        return Ok();
                    }

                    var newFcEmployees = _context.FcAttendances.
                      FirstOrDefault(s => s.EmployeeId.Equals(fcAttendance.EmployeeId) &&
                      s.TimeOut == null &&
                      s.TotalTime == null);

                    newFcEmployees.LocationOut = fcAttendance.LocationOut;
                    char[] MyChar = { 'A', 'P', 'M', ' ' };
                    var tOut = fcAttendance.TimeOut.TrimEnd(MyChar);
                    var tIn = "";
                    if (TimeOut.Contains("PM"))
                    {
                        var temp = TimeOut.TrimEnd(MyChar);
                        var temp2 = temp.Split(':');
                        tIn = (int.Parse(temp2[0]) + 12).ToString() + ":" + temp2[1];
                    }
                    else
                    {
                        tIn = TimeOut.TrimEnd(MyChar);
                    }


                    var totalTime = Convert.ToInt32(TimeSpan.Parse(tOut).Subtract(TimeSpan.Parse(tIn)).TotalMinutes);

                    var m = 0;


                    var h = (totalTime / 60);
                    if (totalTime > 60)
                    {

                        while (totalTime > 60)
                        {
                            totalTime = totalTime - 60;
                        }
                        m = totalTime;
                    }
                    else
                    {
                        m = totalTime;
                    }



                    if (m >= 10)
                    {
                        newFcEmployees.TotalTime = h + "H:" + m + "M";
                    }
                    else
                    {
                        newFcEmployees.TotalTime = h + "H:0" + m + "M";
                    }

                    var tOuts = DateTime.Parse(fcAttendance.TimeIn).ToString(@"hh\:mm tt");


                    var time = "";


                    var splitter = tOuts.Split(' ');

                    if (splitter[1] == "pm")
                    {
                        time = splitter[0] + " PM";
                    }
                    else
                    {
                        time = splitter[0] + " AM";
                    }

                    newFcEmployees.BranchOut = accountBranch;
                    newFcEmployees.TimeOut = time;
                    newFcEmployees.LatLongOut = latLong;
                    await _context.SaveChangesAsync();
                }


                DateTime morningBreak = DateTime.Parse("11:00");
                DateTime afternoonBreak = DateTime.Parse("13:00");

                if (Logs == "Time: Break Out")
                {
                    var newFcEmployees = _context.FcAttendances.
                 FirstOrDefault(s => s.EmployeeId.Equals(fcAttendance.EmployeeId) &&
                 s.TimeOut == null &&
                 s.TotalTime == null);
                    if (newFcEmployees != null)
                    {



                        if (newFcEmployees.FirstBreakOut != null)
                        {
                            newFcEmployees.FirstBreakOut = timeInOut;
                        }
                        else if (newFcEmployees.SecondBreakOut != null)
                        {
                            newFcEmployees.SecondBreakOut = timeInOut;
                        }
                        else if (newFcEmployees.ThirdBreakOut != null)
                        {
                            newFcEmployees.ThirdBreakOut = timeInOut;
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                else if (Logs == "Time: Break In")
                {

                    var newFcEmployees = _context.FcAttendances.
                FirstOrDefault(s => s.EmployeeId.Equals(fcAttendance.EmployeeId) &&
                s.TimeOut == null &&
                s.TotalTime == null);

                    if (newFcEmployees != null)
                    {

                        if (newFcEmployees.FirstBreakIn != null)
                        {
                            newFcEmployees.FirstBreakIn = timeInOut;
                        }
                        else if (newFcEmployees.SecondBreakIn != null)
                        {
                            newFcEmployees.SecondBreakIn = timeInOut;
                        }
                        else if (newFcEmployees.ThirdBreakIn != null)
                        {
                            newFcEmployees.ThirdBreakIn = timeInOut;
                        }


                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(decodedString);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }
        // POST: api/FcAttendances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<FcAttendance>> PostFcAttendance([FromForm] FcAttendance fcAttendance, [FromForm] String Logs)
        {
            var TimeOut = _context.FcAttendances.Where(x =>
            x.EmployeeId == fcAttendance.EmployeeId &&
            x.TimeOut == null &&
            x.TotalTime == null).Select(x => x.TimeIn).FirstOrDefault();


            if (TimeOut == null && Logs == "Time: in") {
                fcAttendance.TimeOut = null;
                fcAttendance.TotalTime = null;
                fcAttendance.LocationOut = null;
                _context.FcAttendances.Add(fcAttendance);
                await _context.SaveChangesAsync();
            }
            else if (Logs == "Time: out" && TimeOut != null)
            {
                if (Logs == "Time: in")
                {
                    return Ok();
                }

                var newFcEmployees = _context.FcAttendances.
                  FirstOrDefault(s => s.EmployeeId.Equals(fcAttendance.EmployeeId) &&
                  s.TimeOut == null &&
                  s.TotalTime == null);

                newFcEmployees.LocationOut = fcAttendance.LocationOut;
                char[] MyChar = { 'A', 'P', 'M', ' ' };
                var tOut = fcAttendance.TimeOut.TrimEnd(MyChar);
                var tIn = TimeOut.TrimEnd(MyChar);
                var totalTime = Convert.ToInt32(TimeSpan.Parse(tOut).Subtract(TimeSpan.Parse(tIn)).TotalMinutes);

                var m = 0;


                var h = (totalTime / 60);
                if (totalTime > 60)
                {

                    while (totalTime > 60)
                    {
                        totalTime = totalTime - 60;
                    }
                    m = totalTime;
                }
                else
                {
                    m = totalTime;
                }



                if (m >= 10)
                {
                    newFcEmployees.TotalTime = h + "H:" + m + "M";
                }
                else
                {
                    newFcEmployees.TotalTime = h + "H:0" + m + "M";
                }

                newFcEmployees.TimeOut = fcAttendance.TimeOut;

                await _context.SaveChangesAsync();
            }

            /*_context.FcAttendances.Add(fcAttendance);
            await _context.SaveChangesAsync();*/

            return Ok();
        }

        /* [HttpPost("bundleUploadOffline")]
         public async Task<ActionResult<FcAttendance>> bundleUploadOffline(
             [FromForm] String locationIn,
             [FromForm] String locationOut,
             [FromForm] String employeeId,
             [FromForm] String logs,
             [FromForm] String time,
             [FromForm] String department,
             [FromForm] String sbu,
             [FromForm] String dateTime
             )
         {
             FcAttendance fcAttendance = new FcAttendance();
             var employeeIdList = JsonSerializer.Deserialize<List<String>>(employeeId);
             var logsList = JsonSerializer.Deserialize<List<String>>(logs);
             var timeList = JsonSerializer.Deserialize<List<String>>(time);
             var departmentList = JsonSerializer.Deserialize<List<String>>(department);
             var sbuList = JsonSerializer.Deserialize<List<String>>(sbu);
             var dateTimeList = JsonSerializer.Deserialize<List<String>>(dateTime);

             for (int x = 0; x < employeeIdList.Count; x++)
             {
                 try
                 {
                     var TimeOut = _context.FcAttendances.Where(y =>
            y.EmployeeId == employeeIdList[x] &&
            y.TimeOut == null &&
            y.TotalTime == null).Select(x => x.TimeIn).FirstOrDefault();


                     if (TimeOut == null && logsList[x] == "Time: in")
                     {
                         fcAttendance.Id = 0;
                         fcAttendance.EmployeeId = employeeIdList[x];
                         fcAttendance.WorkPlace = "Office";
                         fcAttendance.TimeIn = timeList[x];
                         fcAttendance.LocationIn = locationIn;
                         fcAttendance.Department = departmentList[x];
                         fcAttendance.Sbu = sbuList[x];
                         fcAttendance.Date = DateTime.Parse(dateTimeList[x]);
                         _context.FcAttendances.Add(fcAttendance);
                         _context.SaveChanges();
                     }
                     else if (logsList[x] == "Time: out" && TimeOut != null)
                     {
                         if (logsList[x] == "Time: in")
                         {
                             return Ok();
                         }

                         var newFcEmployees = _context.FcAttendances.
                           FirstOrDefault(s => s.EmployeeId.Equals(int.Parse(employeeIdList[x])) &&
                           s.TimeOut == null &&
                           s.TotalTime == null);

                         char[] MyChar = { 'A', 'P', 'M', ' ' };
                         var tOut = timeList[x].TrimEnd(MyChar);
                         var tIn = TimeOut.TrimEnd(MyChar);
                         var totalTime = Convert.ToInt32(TimeSpan.Parse(tOut).Subtract(TimeSpan.Parse(tIn)).TotalMinutes);

                         var m = 0;


                         var h = (totalTime / 60);
                         if (totalTime > 60)
                         {

                             while (totalTime > 60)
                             {
                                 totalTime = totalTime - 60;
                             }
                             m = totalTime;
                         }
                         else
                         {
                             m = totalTime;
                         }



                         if (m >= 10)
                         {
                             newFcEmployees.TotalTime = h + "H:" + m + "M";
                         }
                         else
                         {
                             newFcEmployees.TotalTime = h + "H:0" + m + "M";
                         }

                         newFcEmployees.TimeOut = timeList[x];
                         newFcEmployees.LocationOut = locationOut;

                         await _context.SaveChangesAsync();
                     }




                 }
                 catch (Exception e)
                 {
                     return BadRequest(e);
                 }


             }

             return Ok();
         }*/
        [HttpPost("gettimeinout")]
        public async Task<ActionResult<FcAttendance>> gettimeinout(
            [FromForm] String? getDate,
            [FromForm] String? employeeId)
        {

            try
            {
                var dateTimeInfo = await _context.FcAttendances.Where(
                         s => s.EmployeeId == employeeId &&
                         s.WorkPlace == "WFH" &&
                         s.Date == DateTime.Parse(getDate)).ToListAsync();
                return Ok(dateTimeInfo);
            }
            catch (Exception e) {
                return BadRequest(e.Message);
            }

        }


        [HttpPost("WFHTimeIn")]
        public async Task<ActionResult<FcAttendance>> WFHTimeIn([FromForm] FcAttendance fcAttendance)
        {
            //2023 - 01 - 17 15:09:51.130

            if (_context.FcAttendances.Where(x =>
            x.TimeOut == null &&
            x.TimeIn == fcAttendance.TimeIn &&
            x.Date == fcAttendance.Date).Count() == 0)
            {
                fcAttendance.TimeOut = null;
                fcAttendance.TotalTime = null;
                //fcAttendance.Date = DateTime.Parse("2023/01/17");
                _context.FcAttendances.Add(fcAttendance);
                await _context.SaveChangesAsync();
                return Ok(fcAttendance.TimeIn);
            }
            return Ok();

        }

        [HttpPost("WFHTimeOut")]
        public async Task<ActionResult<FcAttendance>> WFHTimeOut(
            [FromForm] String employeeId,
            [FromForm] String timeIn,
            [FromForm] String timeOut,
            [FromForm] String locationOut,
            [FromForm] String getdate)
        {

            var fcemployeeLogs = _context.FcAttendances.
              FirstOrDefault(s => s.TimeIn == timeIn &&
              s.TimeOut == null &&
              s.EmployeeId == employeeId &&
              s.Date == DateTime.Parse(getdate));

            fcemployeeLogs.TimeOut = timeOut;
            fcemployeeLogs.LocationOut = locationOut;

            char[] MyChar = { 'A', 'P', 'M', ' ' };
            var tOut = timeOut.TrimEnd(MyChar);
            var tIn = timeIn.TrimEnd(MyChar);
            var outs = "";
            var ins = "";

            if (timeOut.Contains('P'))
            {
                var trimmer = tOut.Split(':');
                outs = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];
            }
            else
            {
                outs = tOut;
            }
            if (timeIn.Contains('P'))
            {
                var trimmer = tIn.Split(':');
                ins = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];
            }
            else
            {
                ins = tIn;
            }

            var totalTime = Convert.ToInt32(TimeSpan.Parse(outs).Subtract(TimeSpan.Parse(ins)).TotalMinutes);
            var m = 0;
            var h = 0;

            if (totalTime > 60)
            {
                h = (totalTime / 60);
                while (totalTime > 60)
                {
                    totalTime = totalTime - 60;
                }
                m = totalTime;
            }
            else
            {
                m = totalTime;
            }

            if (m >= 10)
            {
                fcemployeeLogs.TotalTime = h + "H:" + m + "M";
            }
            else
            {
                fcemployeeLogs.TotalTime = h + "H:0" + m + "M";
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("gettimeinoutSite")]
        public async Task<ActionResult<FcAttendance>> gettimeinoutSite(
            [FromForm] String? getDate,
            [FromForm] String? employeeId)
        {

            try
            {
                var dateTimeInfo = await _context.FcAttendances.Where(
                         s => s.EmployeeId == employeeId &&
                         s.Date == DateTime.Parse(getDate)).ToListAsync();
                return Ok(dateTimeInfo);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpPost("gettimeinoutSiteNew1")]
        public async Task<ActionResult<FcAttendance>> gettimeinoutSiteNew(
           [FromForm] String? getDate,
           [FromForm] String? employeeId)
        {

            try
            {
                var dateTimeInfo = await _context.FcAttendances.Where(
                         s => s.EmployeeId == employeeId &&
                         s.Date == DateTime.Parse(getDate)).ToListAsync();

                var oldTimeInfo = _context.FcAttendances.Where(
                         s => s.EmployeeId == employeeId && s.Date != DateTime.Now.Date).OrderBy(s => s.Date).LastOrDefault();

                if (dateTimeInfo.Count == 0)
                {
                    return StatusCode(202, new { dateTimeInfo, oldTimeInfo });
                }
                else
                {
                    return Ok(new { dateTimeInfo , oldTimeInfo });
                }


            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        /*      [HttpPost("test1")]
              public async Task<IActionResult> test1(
              [FromForm] IFormFile uploadAttachments)
              {
                  try
                  {
                      string fileName = uploadAttachments.FileName + ".jpg";
                      string fileextention = Path.GetExtension(fileName);
                 *//*     string temp = "test" + fileextention;
                      string temp1 = Path.GetFileName(uploadAttachments.FileName);
                      string test = temp1+".jpg";*//*

                      string uploadpath = Path.Combine("C:\\Users\\mjfalvarez\\Pictures\\ontime\\test//");
                      if (!Directory.Exists(uploadpath))
                      {
                          Directory.CreateDirectory(uploadpath);
                      }
                      using (FileStream fs = System.IO.File.Create(uploadpath + fileextention))
                      {
                          await uploadAttachments.CopyToAsync(fs);
                          await fs.FlushAsync();
                      }
                      return Ok();
                  }
                  catch (Exception ex)
                  {
                      return StatusCode(202, ex.Message);
                  }

                  }*/
        


        public class TimezoneData
        {
            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("message")]
            public string Message { get; set; }

            [JsonProperty("countryCode")]
            public string CountryCode { get; set; }

            [JsonProperty("countryName")]
            public string CountryName { get; set; }

            [JsonProperty("regionName")]
            public string RegionName { get; set; }

            [JsonProperty("cityName")]
            public string CityName { get; set; }

            [JsonProperty("zoneName")]
            public string ZoneName { get; set; }

            [JsonProperty("abbreviation")]
            public string Abbreviation { get; set; }

            [JsonProperty("gmtOffset")]
            public int GmtOffset { get; set; }

            [JsonProperty("dst")]
            public string DaylightSavingTime { get; set; }

            [JsonProperty("zoneStart")]
            public long ZoneStart { get; set; }

            [JsonProperty("zoneEnd")]
            public long? ZoneEnd { get; set; }

            [JsonProperty("nextAbbreviation")]
            public string NextAbbreviation { get; set; }

            [JsonProperty("timestamp")]
            public long UnixTimestamp { get; set; }

            [JsonProperty("formatted")]
            public string FormattedTime { get; set; }
        }

        static async Task<TimezoneData> GetTimezoneDataAsync(double latitude, double longitude)
        {
    
            string apiKey = "ZDKYCVWDF37H";

            using (HttpClient client = new HttpClient())
            {
                string apiUrl = $"http://api.timezonedb.com/v2.1/get-time-zone?key={apiKey}&format=json&by=position&lat={latitude}&lng={longitude}";

                HttpResponseMessage response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    TimezoneData timezoneData = JsonConvert.DeserializeObject<TimezoneData>(responseBody);
                    return timezoneData;
                }
                else
                {
                    throw new Exception($"Error: {response.StatusCode}");
                }
            }
        }

        public static string nomi(String latitude, String longitude)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers.Add("User-Agent: Other");

                // Construct the URL for the reverse geocoding request.
                string apiUrl = $"http://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";

                try
                {
                    byte[] jsonData = webClient.DownloadData(apiUrl);

                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RootObject));

                    using (MemoryStream stream = new MemoryStream(jsonData))
                    {
                        RootObject rootObject = (RootObject)serializer.ReadObject(stream);

                        // Access the display_name property from the parsed JSON data.
                        string displayName = rootObject.display_name;

                        return displayName;


                    }
                }
                catch (WebException ex)
                {
                    // Handle any WebException that might occur (e.g., network issues, server errors).
                    throw new Exception($"Error: {ex}");
                }
                catch (Exception ex)
                {
                    // Handle any other exceptions that might occur during processing.
                    throw new Exception($"Error: {ex}");
                }
            }
        }
        [DataContract]
        public class Address
        {
            [DataMember]
            public string road { get; set; }
            [DataMember]
            public string suburb { get; set; }
            [DataMember]
            public string city { get; set; }
            [DataMember]
            public string state_district { get; set; }
            [DataMember]
            public string state { get; set; }
            [DataMember]
            public string postcode { get; set; }
            [DataMember]
            public string country { get; set; }
            [DataMember]
            public string country_code { get; set; }
        }

        [DataContract]
        public class RootObject
        {
            [DataMember]
            public string place_id { get; set; }
            [DataMember]
            public string licence { get; set; }
            [DataMember]
            public string osm_type { get; set; }
            [DataMember]
            public string osm_id { get; set; }
            [DataMember]
            public string lat { get; set; }
            [DataMember]
            public string lon { get; set; }
            [DataMember]
            public string display_name { get; set; }
            [DataMember]
            public Address address { get; set; }
        }
        [HttpPost("test")]
        public async Task<ActionResult<FcAttendance>> test(string latitude, string longitude)
        {

            try
            {

                string responseData = await GetLocationInfo(latitude, longitude) + " " + nomi(latitude, longitude);

                return Ok(responseData);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }

        public class AdministrativeInfo
        {
            public string name { get; set; }
            public string description { get; set; }
            public int order { get; set; }
            public int adminLevel { get; set; }
            public string isoCode { get; set; }
            public string wikidataId { get; set; }
            public int? geonameId { get; set; }
        }

        public class LocalityInfo
        {
            public List<AdministrativeInfo> administrative { get; set; }
        }

        public class LocationInfo
        {
            public string plusCode { get; set; }
            public LocalityInfo localityInfo { get; set; }
        }
        public static async Task<String> GetLocationInfo(string latitude, string longitude)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string apiUrl = $"https://api.bigdatacloud.net/data/reverse-geocode-client?latitude={latitude}&longitude={longitude}&localityLanguage=en";

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();

                        JsonDocument locationInfo = JsonDocument.Parse(responseData);

                        string plusCode = locationInfo.RootElement.GetProperty("plusCode").GetString();
/*
                        JsonElement adminArray = locationInfo.RootElement
                    .GetProperty("localityInfo")
                    .GetProperty("administrative");

                        // Initialize a StringBuilder to build the string.
                        StringBuilder administrativeNames = new StringBuilder();

                        // Loop through the administrative entities and concatenate their names.
                        foreach (JsonElement adminElement in adminArray.EnumerateArray().Reverse())
                        {
                            string adminName = adminElement.GetProperty("name").GetString();
                            administrativeNames.Append(adminName);
                            administrativeNames.Append(", "); // Add a comma and space for separation.
                        }

                        // Remove the trailing comma and space.
                        string result = plusCode + " " + administrativeNames.ToString().TrimEnd(',', ' ');*/

                        return plusCode;
                    }
                    else
                    {
                        throw new Exception($"API request failed with status code {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    throw new Exception($"HTTP Request Error: {ex.Message}");
                }
            }
        }
        public static string RemoveParenthesesAndCommas(string input)
        {
            // Use a regular expression to remove content within parentheses and commas.
            return Regex.Replace(input, @"[\(\),]", string.Empty);
        }

        public static string RemoveParenthesesContent(string input)
        {
            // Use a regular expression to remove content within parentheses.
            return System.Text.RegularExpressions.Regex.Replace(input, @"\([^)]*\)", string.Empty);
        }

        [HttpPost("uploadFile2")]
        public async Task<IActionResult> uploadFile2(
           [FromForm] IFormFile uploadAttachments,
           [FromForm] string folder,
           [FromForm] string fileName,
           [FromForm] FcAttendance fcAttendance,
           [FromForm] double timeZoneGMT)
        {
            try
            {
                var loglat = fcAttendance.LatLongIn.Split('-');
               
               fcAttendance.LocationIn = await GetLocationInfo(loglat[0], loglat[1]) + " " +  nomi(loglat[0], loglat[1]);

                FcAttendance checker = _context.FcAttendances.Where(x =>
                x.EmployeeId == fcAttendance.EmployeeId &&
                x.TimeIn != null &&
                x.TimeOut == null &&
                x.Date == fcAttendance.Date).FirstOrDefault();

                if (checker != null)
                {
                    return Ok();
                }

                /* var longlat = fcAttendance.LatLongIn.Split('-');

                 TimezoneData timezoneData = await GetTimezoneDataAsync(double.Parse(longlat[0]), double.Parse(longlat[1]));

                 fcAttendance.TimeIn = DateTime.Parse(timezoneData.FormattedTime).ToString("h:mm tt");*/

                var timeZone = DateTime.UtcNow.AddHours(timeZoneGMT).ToString();

                fcAttendance.TimeIn = DateTime.Parse(timeZone).ToString("h:mm tt").ToUpper();

               DateTime time12HourParsed = DateTime.ParseExact(fcAttendance.TimeIn, "h:mm tt", null);

                if (fcAttendance.TimeIn[0].ToString() == "0")
                {
                    fcAttendance.TimeIn = fcAttendance.TimeIn.Remove(0, 1);
                }

                string folders = folder + "//";
                //string filename = uploadAttachments.FileName;
                //string filenames = Path.GetFileName(filename);

                string time = fcAttendance.TimeIn.Replace(":", "");
                string fileNewName = uploadAttachments.FileName + ".jpg";
                string fileextention = Path.GetExtension(fileNewName);
                string newfilename = time.Replace(" ", "") + fcAttendance.EmployeeId + fileextention;
                string uploadpath = Path.Combine("E:\\ontimemobile\\" + folders + "\\" + fileName + "//");
                //string uploadpath = Path.Combine(_configuration["FileUploadLocation"] + folders + "\\" + fileName + "//");
                //string uploadpath = Path.Combine("C:\\Users\\mjfalvarez\\Pictures\\ontime\\" + folders + "\\" + fileName + "//");
                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }
                using (FileStream fs = System.IO.File.Create(uploadpath + newfilename))
                {
                    await uploadAttachments.CopyToAsync(fs);
                    await fs.FlushAsync();
                }

                fcAttendance.TimeOut = null;
                fcAttendance.TotalTime = null;
                _context.FcAttendances.Add(fcAttendance);
                await _context.SaveChangesAsync();



                string time24Hour = time12HourParsed.ToString("HH:mm");
                DateTime inclusivedate = DateTime.Now.Date;

                var requestBody = new
                {
                    EMPLOYEE_ID = fcAttendance.EmployeeId,
                    LOG_DATE = inclusivedate,
                    LOG_TIME = time24Hour,
                    LOG_TYPE = "IN1A",
                    INCLUSIVE_DATE = inclusivedate,
                    SOURCE = "ONTIME"
                }; // Example request body data

                using var httpClient = new HttpClient();

                var req = new HttpRequestMessage(HttpMethod.Post, "https://kunzad2.fastlogistics.com.ph/fastlogistics/api/hrisEMPL_DTR")
                {
                    Headers =
    {

        { "Token", "c3lzdGVtQGZhc3Rncm91cC5iaXo6MjhlNmQyYjUtMzgzMy00ZWQ3LWE4MWUtZTk3NzUyYjU2ZmY3dHNhZg==" },

    },
                    Content = JsonContent.Create(requestBody)
                };

                using var response = await httpClient.SendAsync(req);
                var iss = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Content: " + responseContent);
                }
                else
                {
                    // Request failed
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }



                return Ok("Success");

            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("InnerException") || ex.Message.Contains("inner exception"))
                {

                    return StatusCode(202, ex.InnerException);
                }
                else
                {

                    return StatusCode(202, ex.Message);
                }
            }

        }

        //MultipleUpload files into database api.
        [HttpPost("uploadFile1")]
        public async Task<IActionResult> uploadFile(
            [FromForm]IFormFile uploadAttachments,
            [FromForm] string folder,
            [FromForm] string fileName,
            [FromForm] FcAttendance fcAttendance)
        {
            try
            {

                var checker = _context.FcAttendances.Where(x =>
                x.EmployeeId == fcAttendance.EmployeeId &&
                x.TimeIn != null &&
                x.TimeOut == null &&
                x.Date == fcAttendance.Date).FirstOrDefault();

                    if (checker != null)
                {
                    return Ok();
                }

               


                DateTime time12HourParsed = DateTime.ParseExact(fcAttendance.TimeIn, "h:mm tt", null);
                if (fcAttendance.TimeIn[0].ToString() == "0")
                {
                    fcAttendance.TimeIn = fcAttendance.TimeIn.Remove(0, 1);
                }

                string folders = folder + "//";
                //string filename = uploadAttachments.FileName;
                //string filenames = Path.GetFileName(filename);

                string time = fcAttendance.TimeIn.Replace(":", "");
                string fileNewName = uploadAttachments.FileName + ".jpg";
                string fileextention = Path.GetExtension(fileNewName);
                string newfilename = time.Replace(" ", "") + fcAttendance.EmployeeId + fileextention;
               string uploadpath = Path.Combine("E:\\ontimemobile\\" + folders + "\\" + fileName + "//");
                //string uploadpath = Path.Combine(_configuration["FileUploadLocation"] + folders + "\\" + fileName + "//");
                //string uploadpath = Path.Combine("C:\\Users\\mjfalvarez\\Pictures\\ontime\\" + folders + "\\" + fileName + "//");
                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }
                using (FileStream fs = System.IO.File.Create(uploadpath + newfilename))
                {
                    await uploadAttachments.CopyToAsync(fs);
                    await fs.FlushAsync();
                }

                fcAttendance.TimeOut = null;
                fcAttendance.TotalTime = null;
                _context.FcAttendances.Add(fcAttendance);
                await _context.SaveChangesAsync();

                

                string time24Hour = time12HourParsed.ToString("HH:mm");
                DateTime inclusivedate = DateTime.Now.Date;
            
                var requestBody = new { 
                    EMPLOYEE_ID = fcAttendance.EmployeeId, 
                    LOG_DATE = inclusivedate,
                    LOG_TIME = time24Hour,
                    LOG_TYPE = "IN1A",
                    INCLUSIVE_DATE = inclusivedate,
                    SOURCE = "ONTIME"
                }; // Example request body data

                using var httpClient = new HttpClient();

                var req = new HttpRequestMessage(HttpMethod.Post, "https://kunzad2.fastlogistics.com.ph/fastlogistics/api/hrisEMPL_DTR")
                {
                    Headers =
    {

        { "Token", "c3lzdGVtQGZhc3Rncm91cC5iaXo6MjhlNmQyYjUtMzgzMy00ZWQ3LWE4MWUtZTk3NzUyYjU2ZmY3dHNhZg==" },

    },
                    Content = JsonContent.Create(requestBody)
                };

                using var response = await httpClient.SendAsync(req);
               var iss =  response.StatusCode;  
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Content: " + responseContent);
                }
                else
                {
                    // Request failed
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }



                return Ok("Success");
                
            }
            catch (Exception ex)
            {
          
                if (ex.Message.Contains("InnerException") || ex.Message.Contains("inner exception"))
                {

                    return StatusCode( 202 ,ex.InnerException);
                }
                else
                {

                    return StatusCode(202, ex.Message);
                }
            }
           
        }
        /* [HttpPost("uploadFileTimeOut")]
         public async Task<IActionResult> uploadFileTimeOut(
             [FromForm] IFormFile uploadAttachments,
             [FromForm] string folder,
             [FromForm] string? workPlaceOut,
             [FromForm] string fileName,
             [FromForm] string timeOut,
             [FromForm] string LocationOut,
             [FromForm] string latLongOut,
             [FromForm] String employeeId,
             [FromForm] string getdate)
         {
             try
             {
                 if (timeOut[0].ToString() == "0")
                 {
                     timeOut = timeOut.Remove(0, 1);
                 }

                 string folders = folder + "//";     
                 string filename = uploadAttachments.FileName;
                 //string filenames = Path.GetFileName(filename);

                 string time = timeOut.Replace(":", "");


                 string fileNewName = uploadAttachments.FileName + ".jpg";
                 string fileextention = Path.GetExtension(fileNewName);
                 string newfilename = time.Replace(" ", "") + employeeId + fileextention;
                 string uploadpath = Path.Combine("E:\\ontimemobile\\" + folders + "\\" + fileName + "//");

                //string uploadpath = Path.Combine("C:\\Users\\mjfalvarez\\Pictures\\ontime\\" + folders + "\\" + fileName + "//");


                 if (!Directory.Exists(uploadpath))
                 {
                     Directory.CreateDirectory(uploadpath);
                 }
                 using (FileStream fs = System.IO.File.Create(uploadpath + newfilename))
                 {
                     await uploadAttachments.CopyToAsync(fs);
                     await fs.FlushAsync();
                 }

                 var timeIn = await _context.FcAttendances.Where(
                         s => s.EmployeeId == employeeId &&
                         s.Date == DateTime.Parse(getdate)).ToListAsync();

                 DateTime time12HourParsed = DateTime.ParseExact(timeOut, "h:mm tt", null);

                 string time24Hour = time12HourParsed.ToString("HH:mm");

                 if (timeIn.Count() == 0)
                 {
                     FcAttendance fcAttendance = new FcAttendance();

                     DateTime currentDate = DateTime.Now.Date;

                     var user = _context.FcEmployees.FirstOrDefault(x => x.EmployeeId == employeeId.ToString());



                     fcAttendance.EmployeeId = employeeId;
                     fcAttendance.WorkPlace = workPlaceOut;
                     fcAttendance.WorkPlaceOut = workPlaceOut;
                     fcAttendance.TimeOut = timeOut;
                     fcAttendance.LocationOut = LocationOut;
                     fcAttendance.LatLongOut = latLongOut;
                     fcAttendance.Department = user.Department;
                     fcAttendance.Sbu = user.Sbu;
                     fcAttendance.Date = currentDate;
                     fcAttendance.TotalTime = "0H:00M";
                     fcAttendance.TimeIn = "No Data";

                     _context.FcAttendances.Add(fcAttendance);
                     _context.SaveChanges(); 
                 }
                 else
                 {
                     var fcemployeeLogs = _context.FcAttendances.
              FirstOrDefault(s => s.TimeIn == timeIn[timeIn.Count - 1].TimeIn &&
              s.TimeOut == null &&
              s.EmployeeId == employeeId &&
              s.Date == DateTime.Parse(getdate));


                     if (timeOut[0].ToString() == "0")
                     {
                         timeOut = timeOut.Remove(0, 1);
                     }


                     fcemployeeLogs.TimeOut = timeOut;


                     fcemployeeLogs.WorkPlaceOut = workPlaceOut;
                     fcemployeeLogs.LocationOut = LocationOut;
                     fcemployeeLogs.LatLongOut = latLongOut;

                     char[] MyChar = { 'A', 'P', 'M', ' ' };
                     var tOut = timeOut.TrimEnd(MyChar);
                     var tIn = timeIn[timeIn.Count - 1].TimeIn.TrimEnd(MyChar);

                     var outs = "";
                     var ins = "";

                     if (timeOut.Contains('P'))
                     {

                         if (timeIn[timeIn.Count - 1].TimeIn.Contains("12:"))
                         {
                             outs = tOut;
                         }
                         else
                         {
                             var trimmer = tOut.Split(':');
                             outs = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];

                         }

                     }
                     else
                     {
                         outs = tOut;
                     }
                     if (timeIn[timeIn.Count - 1].TimeIn.Contains('P'))
                     {
                         if (timeIn[timeIn.Count - 1].TimeIn.Contains("12:"))
                         {
                             ins = tIn;

                         }
                         else
                         {
                             var trimmer = tIn.Split(':');
                             ins = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];
                         }

                     }
                     else
                     {
                         ins = tIn;
                     }

                     var totalTime = Convert.ToInt32(TimeSpan.Parse(outs).Subtract(TimeSpan.Parse(ins)).TotalMinutes);
                     var m = 0;
                     var h = 0;

                     if (totalTime > 60)
                     {
                         h = (totalTime / 60);
                         while (totalTime > 60)
                         {
                             totalTime = totalTime - 60;
                         }
                         m = totalTime;
                     }
                     else
                     {
                         m = totalTime;
                     }

                     if (m >= 10)
                     {
                         fcemployeeLogs.TotalTime = h + "H:" + m + "M";
                     }
                     else
                     {
                         fcemployeeLogs.TotalTime = h + "H:0" + m + "M";
                     }
                     await _context.SaveChangesAsync();
                 }



                 DateTime inclusivedate = DateTime.Now.Date;

                 var requestBody = new
                 {
                     EMPLOYEE_ID = employeeId,
                     LOG_DATE = inclusivedate,
                     LOG_TIME = time24Hour,
                     LOG_TYPE = "OUT2A",
                     INCLUSIVE_DATE = inclusivedate,
                     SOURCE = "ONTIME"
                 }; // Example request body data

                 using var httpClient = new HttpClient();

                 var req = new HttpRequestMessage(HttpMethod.Post, "https://kunzad2.fastlogistics.com.ph/fastlogistics/api/hrisEMPL_DTR")
                 {
                     Headers =
     {

         { "Token", "c3lzdGVtQGZhc3Rncm91cC5iaXo6MjhlNmQyYjUtMzgzMy00ZWQ3LWE4MWUtZTk3NzUyYjU2ZmY3dHNhZg==" },

     },
                     Content = JsonContent.Create(requestBody)
                 };

                 using var response = await httpClient.SendAsync(req);
                 var iss = response.StatusCode;
                 if (response.IsSuccessStatusCode)
                 {
                     // Request was successful
                     var responseContent = await response.Content.ReadAsStringAsync();
                     Console.WriteLine("Response Content: " + responseContent);
                 }
                 else
                 {
                     // Request failed
                     Console.WriteLine("Request failed with status code: " + response.StatusCode);
                 }


                 return Ok("Success");

             }
             catch (Exception ex)
             {
                 if (ex.Message.Contains("InnerException") || ex.Message.Contains("inner exception"))
                 {

                     return Ok(ex.InnerException);
                 }
                 else
                 {

                     return Ok(ex.Message);
                 }
             }
             return Ok("Not Uploaded");
         }*/


        [HttpPost("getTimeZoneT")]
        public async Task<ActionResult<FcAttendance>> getTimeZoneT([FromForm] double timeZ)
        {

            try
            {


                var timeUtc = DateTime.UtcNow.AddHours(timeZ).ToString("h:mm tt").ToUpper();

                


                return Ok(timeUtc);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

        }


        [HttpPost("uploadFileTimeOut2")]
        public async Task<IActionResult> uploadFileTimeOut2(
         [FromForm] IFormFile uploadAttachments,
         [FromForm] string folder,
         [FromForm] string? workPlaceOut,
         [FromForm] string fileName,
         [FromForm] string timeOut,
         [FromForm] string LocationOut,
         [FromForm] string latLongOut,
         [FromForm] String employeeId,
         [FromForm] string getdate,
         [FromForm] string timeZoneGMT)
        {
            try
            {
                DateTime inclusivedate = DateTime.Now.Date;
                var loglat = latLongOut.Split('-');


                LocationOut = await GetLocationInfo(loglat[0], loglat[1]) + " " + nomi(loglat[0], loglat[1]);
               
               

                /* var longlat = latLongOut.Split('-');

                 var myDateTime = GetLocalDateTime(double.Parse(longlat[0]), double.Parse(longlat[1]), DateTime.UtcNow);


                 DateTime time12HourParsed = DateTime.ParseExact(fcAttendance.TimeIn, "h:mm tt", null);

                 fcAttendance.TimeIn = time12HourParsed.ToString("h:mm tt");*/

                /*var longlat = latLongOut.Split('-');

                TimezoneData timezoneData = await GetTimezoneDataAsync(double.Parse(longlat[0]), double.Parse(longlat[1]));


                timeOut = DateTime.Parse(timezoneData.FormattedTime).ToString("h:mm tt");*/
                timeOut = timeZoneGMT.ToString();

                if (timeOut[0].ToString() == "0")
                {
                    timeOut = timeOut.Remove(0, 1);
                }

                string folders = folder + "//";
                string filename = uploadAttachments.FileName;
                //string filenames = Path.GetFileName(filename);

                string time = timeOut.Replace(":", "");


                string fileNewName = uploadAttachments.FileName + ".jpg";
                string fileextention = Path.GetExtension(fileNewName);
                string newfilename = time.Replace(" ", "") + employeeId + fileextention;
                string uploadpath = Path.Combine("E:\\ontimemobile\\" + folders + "\\" + fileName + "//");

                //string uploadpath = Path.Combine("C:\\Users\\mjfalvarez\\Pictures\\ontime\\" + folders + "\\" + fileName + "//");


                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }
                using (FileStream fs = System.IO.File.Create(uploadpath + newfilename))
                {
                    await uploadAttachments.CopyToAsync(fs);
                    await fs.FlushAsync();
                }

                var timeIn = await _context.FcAttendances.Where(
                        s => s.EmployeeId == employeeId &&
                        s.Date == DateTime.Parse(getdate)).ToListAsync();

                DateTime time12HourParsed = DateTime.ParseExact(timeOut, "h:mm tt", null);

                string time24Hour = time12HourParsed.ToString("HH:mm");

                if (timeIn.Count() == 0)
                {


                    var fcemployeeLogs = await _context.FcAttendances.Where(
                       s => s.EmployeeId == employeeId).OrderBy(x => x.Date).LastOrDefaultAsync();


                    if (fcemployeeLogs.TimeOut == null)
                    {

                        if (timeOut[0].ToString() == "0")
                        {
                            timeOut = timeOut.Remove(0, 1);
                        }



                        fcemployeeLogs.TimeOut = timeOut;


                        fcemployeeLogs.WorkPlaceOut = workPlaceOut;
                        fcemployeeLogs.LocationOut = LocationOut;
                        fcemployeeLogs.LatLongOut = latLongOut;

                        char[] MyChar = { 'A', 'P', 'M', ' ' };
                        var tOut = timeOut.TrimEnd(MyChar);
                        var tIn = fcemployeeLogs.TimeIn.TrimEnd(MyChar);

                        var outs = "";
                        var ins = "";

                        if (timeOut.Contains('P'))
                        {

                            if (fcemployeeLogs.TimeIn.Contains("12:"))
                            {
                                outs = tOut;
                            }
                            else
                            {
                                var trimmer = tOut.Split(':');
                                outs = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];

                            }

                        }
                        else
                        {
                            outs = tOut;
                        }
                        if (fcemployeeLogs.TimeIn.Contains('P'))
                        {
                            if (fcemployeeLogs.TimeIn.Contains("12:"))
                            {
                                ins = tIn;

                            }
                            else
                            {
                                var trimmer = tIn.Split(':');
                                ins = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];
                            }

                        }
                        else
                        {
                            ins = tIn;
                        }
                        /*   DateTime d = DateTime.Parse(fcemployeeLogs.TimeIn);
                           DateTime d2 = DateTime.Parse(fcemployeeLogs.TimeOut);

                           var tempin = DateTime.ParseExact(d.ToString("HH:mm:ss"), "HH:mm:ss", CultureInfo.InvariantCulture);
                           var tempout = DateTime.ParseExact(d2.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);*/

                        DateTime completeTimein = fcemployeeLogs.Date.Value.Date.Add(DateTime.Parse(fcemployeeLogs.TimeIn).TimeOfDay);
                        DateTime completeTimeout = DateTime.Now.Date.Add(DateTime.Parse(fcemployeeLogs.TimeOut).TimeOfDay);

                        double diffTicks = (completeTimeout - completeTimein).TotalMinutes;

                        var totalTime = int.Parse(diffTicks.ToString());
                        var m = 0;
                        var h = 0;

                        if (totalTime > 60)
                        {
                            h = (totalTime / 60);
                            while (totalTime > 60)
                            {
                                totalTime = totalTime - 60;
                            }
                            m = totalTime;
                        }
                        else
                        {
                            m = totalTime;
                        }

                        if (m >= 10)
                        {
                            fcemployeeLogs.TotalTime = h + "H:" + m + "M";
                        }
                        else
                        {
                            fcemployeeLogs.TotalTime = h + "H:0" + m + "M";
                        }

                        inclusivedate = fcemployeeLogs.Date.Value.Date;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        FcAttendance fcAttendance = new FcAttendance();

                        DateTime currentDate = DateTime.Now.Date;

                        var user = _context.FcEmployees.FirstOrDefault(x => x.EmployeeId == employeeId.ToString());



                        fcAttendance.EmployeeId = employeeId;
                        fcAttendance.WorkPlace = workPlaceOut;
                        fcAttendance.WorkPlaceOut = workPlaceOut;
                        fcAttendance.TimeOut = timeOut;
                        fcAttendance.LocationOut = LocationOut;
                        fcAttendance.LatLongOut = latLongOut;
                        fcAttendance.Department = user.Department;
                        fcAttendance.Sbu = user.Sbu;
                        fcAttendance.Date = currentDate;
                        fcAttendance.TotalTime = "0H:00M";
                        fcAttendance.TimeIn = "No Data";

                        _context.FcAttendances.Add(fcAttendance);
                        _context.SaveChanges();
                    }


                }
                else
                {
                    var fcemployeeLogs = _context.FcAttendances.
             FirstOrDefault(s => s.TimeIn == timeIn[timeIn.Count - 1].TimeIn &&
             s.TimeOut == null &&
             s.EmployeeId == employeeId &&
             s.Date == DateTime.Parse(getdate));


                    if (timeOut[0].ToString() == "0")
                    {
                        timeOut = timeOut.Remove(0, 1);
                    }


                    fcemployeeLogs.TimeOut = timeOut;


                    fcemployeeLogs.WorkPlaceOut = workPlaceOut;
                    fcemployeeLogs.LocationOut = LocationOut;
                    fcemployeeLogs.LatLongOut = latLongOut;

                    char[] MyChar = { 'A', 'P', 'M', ' ' };
                    var tOut = timeOut.TrimEnd(MyChar);
                    var tIn = timeIn[timeIn.Count - 1].TimeIn.TrimEnd(MyChar);

                    var outs = "";
                    var ins = "";

                    if (timeOut.Contains('P'))
                    {

                        if (timeIn[timeIn.Count - 1].TimeIn.Contains("12:"))
                        {
                            outs = tOut;
                        }
                        else
                        {
                            var trimmer = tOut.Split(':');
                            outs = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];

                        }

                    }
                    else
                    {
                        outs = tOut;
                    }
                    if (timeIn[timeIn.Count - 1].TimeIn.Contains('P'))
                    {
                        if (timeIn[timeIn.Count - 1].TimeIn.Contains("12:"))
                        {
                            ins = tIn;

                        }
                        else
                        {
                            var trimmer = tIn.Split(':');
                            ins = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];
                        }

                    }
                    else
                    {
                        ins = tIn;
                    }


                    DateTime completeTimein = fcemployeeLogs.Date.Value.Date.Add(DateTime.Parse(fcemployeeLogs.TimeIn).TimeOfDay);
                    DateTime completeTimeout = DateTime.Now.Date.Add(DateTime.Parse(fcemployeeLogs.TimeOut).TimeOfDay);

                    double diffTicks = (completeTimeout - completeTimein).TotalMinutes;

                    var totalTime = int.Parse(diffTicks.ToString());
                    var m = 0;
                    var h = 0;

                    if (totalTime > 60)
                    {
                        h = (totalTime / 60);
                        while (totalTime > 60)
                        {
                            totalTime = totalTime - 60;
                        }
                        m = totalTime;
                    }
                    else
                    {
                        m = totalTime;
                    }

                    if (m >= 10)
                    {
                        fcemployeeLogs.TotalTime = h + "H:" + m + "M";
                    }
                    else
                    {
                        fcemployeeLogs.TotalTime = h + "H:0" + m + "M";
                    }
                    await _context.SaveChangesAsync();
                }





                var requestBody = new
                {
                    EMPLOYEE_ID = employeeId,
                    LOG_DATE = inclusivedate,
                    LOG_TIME = time24Hour,
                    LOG_TYPE = "OUT2A",
                    INCLUSIVE_DATE = inclusivedate,
                    SOURCE = "ONTIME"
                }; // Example request body data

                using var httpClient = new HttpClient();

                var req = new HttpRequestMessage(HttpMethod.Post, "https://kunzad2.fastlogistics.com.ph/fastlogistics/api/hrisEMPL_DTR")
                {
                    Headers =
    {

        { "Token", "c3lzdGVtQGZhc3Rncm91cC5iaXo6MjhlNmQyYjUtMzgzMy00ZWQ3LWE4MWUtZTk3NzUyYjU2ZmY3dHNhZg==" },

    },
                    Content = JsonContent.Create(requestBody)
                };

                using var response = await httpClient.SendAsync(req);
                var iss = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Content: " + responseContent);
                }
                else
                {
                    // Request failed
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }


                return Ok("Success");

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("InnerException") || ex.Message.Contains("inner exception"))
                {

                    return StatusCode(202, ex.InnerException);
                }
                else
                {

                    return StatusCode(202, ex.Message);
                }
            }
            return Ok("Not Uploaded");
        }

        [HttpPost("uploadFileTimeOut1")]
        public async Task<IActionResult> uploadFileTimeOut1(
           [FromForm] IFormFile uploadAttachments,
           [FromForm] string folder,
           [FromForm] string? workPlaceOut,
           [FromForm] string fileName,
           [FromForm] string timeOut,
           [FromForm] string LocationOut,
           [FromForm] string latLongOut,
           [FromForm] String employeeId,
           [FromForm] string getdate)
        {
            try
            {
                DateTime inclusivedate = DateTime.Now.Date;


                if (timeOut[0].ToString() == "0")
                {
                    timeOut = timeOut.Remove(0, 1);
                }

                string folders = folder + "//";
                string filename = uploadAttachments.FileName;
                //string filenames = Path.GetFileName(filename);

                string time = timeOut.Replace(":", "");


                string fileNewName = uploadAttachments.FileName + ".jpg";
                string fileextention = Path.GetExtension(fileNewName);
                string newfilename = time.Replace(" ", "") + employeeId + fileextention;
                string uploadpath = Path.Combine("E:\\ontimemobile\\" + folders + "\\" + fileName + "//");

                //string uploadpath = Path.Combine("C:\\Users\\mjfalvarez\\Pictures\\ontime\\" + folders + "\\" + fileName + "//");


                if (!Directory.Exists(uploadpath))
                {
                    Directory.CreateDirectory(uploadpath);
                }
                using (FileStream fs = System.IO.File.Create(uploadpath + newfilename))
                {
                    await uploadAttachments.CopyToAsync(fs);
                    await fs.FlushAsync();
                }

                var timeIn = await _context.FcAttendances.Where(
                        s => s.EmployeeId == employeeId &&
                        s.Date == DateTime.Parse(getdate)).ToListAsync();

                DateTime time12HourParsed = DateTime.ParseExact(timeOut, "h:mm tt", null);

                string time24Hour = time12HourParsed.ToString("HH:mm");

                if (timeIn.Count() == 0)
                {


                    var fcemployeeLogs = await _context.FcAttendances.Where(
                       s => s.EmployeeId == employeeId).OrderBy(x => x.Date).LastOrDefaultAsync();


                    if (fcemployeeLogs.TimeOut == null)
                    {

                        if (timeOut[0].ToString() == "0")
                        {
                            timeOut = timeOut.Remove(0, 1);
                        }

                     

                        fcemployeeLogs.TimeOut = timeOut;


                        fcemployeeLogs.WorkPlaceOut = workPlaceOut;
                        fcemployeeLogs.LocationOut = LocationOut;
                        fcemployeeLogs.LatLongOut = latLongOut;

                        char[] MyChar = { 'A', 'P', 'M', ' ' };
                        var tOut = timeOut.TrimEnd(MyChar);
                        var tIn = fcemployeeLogs.TimeIn.TrimEnd(MyChar);

                        var outs = "";
                        var ins = "";

                        if (timeOut.Contains('P'))
                        {

                            if (fcemployeeLogs.TimeIn.Contains("12:"))
                            {
                                outs = tOut;
                            }
                            else
                            {
                                var trimmer = tOut.Split(':');
                                outs = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];

                            }

                        }
                        else
                        {
                            outs = tOut;
                        }
                        if (fcemployeeLogs.TimeIn.Contains('P'))
                        {
                            if (fcemployeeLogs.TimeIn.Contains("12:"))
                            {
                                ins = tIn;

                            }
                            else
                            {
                                var trimmer = tIn.Split(':');
                                ins = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];
                            }

                        }
                        else
                        {
                            ins = tIn;
                        }
                     /*   DateTime d = DateTime.Parse(fcemployeeLogs.TimeIn);
                        DateTime d2 = DateTime.Parse(fcemployeeLogs.TimeOut);

                        var tempin = DateTime.ParseExact(d.ToString("HH:mm:ss"), "HH:mm:ss", CultureInfo.InvariantCulture);
                        var tempout = DateTime.ParseExact(d2.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);*/

                        DateTime completeTimein = fcemployeeLogs.Date.Value.Date.Add(DateTime.Parse(fcemployeeLogs.TimeIn).TimeOfDay); 
                        DateTime completeTimeout = DateTime.Now.Date.Add(DateTime.Parse(fcemployeeLogs.TimeOut).TimeOfDay);

                        double diffTicks = (completeTimeout - completeTimein).TotalMinutes;

                        var totalTime = int.Parse(diffTicks.ToString());
                        var m = 0;
                        var h = 0;

                        if (totalTime > 60)
                        {
                            h = (totalTime / 60);
                            while (totalTime > 60)
                            {
                                totalTime = totalTime - 60;
                            }
                            m = totalTime;
                        }
                        else
                        {
                            m = totalTime;
                        }

                        if (m >= 10)
                        {
                            fcemployeeLogs.TotalTime = h + "H:" + m + "M";
                        }
                        else
                        {
                            fcemployeeLogs.TotalTime = h + "H:0" + m + "M";
                        }

                        inclusivedate = fcemployeeLogs.Date.Value.Date;

                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        FcAttendance fcAttendance = new FcAttendance();

                        DateTime currentDate = DateTime.Now.Date;

                        var user = _context.FcEmployees.FirstOrDefault(x => x.EmployeeId == employeeId.ToString());



                        fcAttendance.EmployeeId = employeeId;
                        fcAttendance.WorkPlace = workPlaceOut;
                        fcAttendance.WorkPlaceOut = workPlaceOut;
                        fcAttendance.TimeOut = timeOut;
                        fcAttendance.LocationOut = LocationOut;
                        fcAttendance.LatLongOut = latLongOut;
                        fcAttendance.Department = user.Department;
                        fcAttendance.Sbu = user.Sbu;
                        fcAttendance.Date = currentDate;
                        fcAttendance.TotalTime = "0H:00M";
                        fcAttendance.TimeIn = "No Data";

                        _context.FcAttendances.Add(fcAttendance);
                        _context.SaveChanges();
                    }

                   
                }
                else
                {
                    var fcemployeeLogs = _context.FcAttendances.
             FirstOrDefault(s => s.TimeIn == timeIn[timeIn.Count - 1].TimeIn &&
             s.TimeOut == null &&
             s.EmployeeId == employeeId &&
             s.Date == DateTime.Parse(getdate));


                    if (timeOut[0].ToString() == "0")
                    {
                        timeOut = timeOut.Remove(0, 1);
                    }


                    fcemployeeLogs.TimeOut = timeOut;


                    fcemployeeLogs.WorkPlaceOut = workPlaceOut;
                    fcemployeeLogs.LocationOut = LocationOut;
                    fcemployeeLogs.LatLongOut = latLongOut;

                    char[] MyChar = { 'A', 'P', 'M', ' ' };
                    var tOut = timeOut.TrimEnd(MyChar);
                    var tIn = timeIn[timeIn.Count - 1].TimeIn.TrimEnd(MyChar);

                    var outs = "";
                    var ins = "";

                    if (timeOut.Contains('P'))
                    {

                        if (timeIn[timeIn.Count - 1].TimeIn.Contains("12:"))
                        {
                            outs = tOut;
                        }
                        else
                        {
                            var trimmer = tOut.Split(':');
                            outs = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];

                        }

                    }
                    else
                    {
                        outs = tOut;
                    }
                    if (timeIn[timeIn.Count - 1].TimeIn.Contains('P'))
                    {
                        if (timeIn[timeIn.Count - 1].TimeIn.Contains("12:"))
                        {
                            ins = tIn;

                        }
                        else
                        {
                            var trimmer = tIn.Split(':');
                            ins = (int.Parse(trimmer[0]) + 12).ToString() + ":" + trimmer[1];
                        }

                    }
                    else
                    {
                        ins = tIn;
                    }


                    DateTime completeTimein = fcemployeeLogs.Date.Value.Date.Add(DateTime.Parse(fcemployeeLogs.TimeIn).TimeOfDay);
                    DateTime completeTimeout = DateTime.Now.Date.Add(DateTime.Parse(fcemployeeLogs.TimeOut).TimeOfDay);

                    double diffTicks = (completeTimeout - completeTimein).TotalMinutes;

                    var totalTime = int.Parse(diffTicks.ToString());
                    var m = 0;
                    var h = 0;

                    if (totalTime > 60)
                    {
                        h = (totalTime / 60);
                        while (totalTime > 60)
                        {
                            totalTime = totalTime - 60;
                        }
                        m = totalTime;
                    }
                    else
                    {
                        m = totalTime;
                    }

                    if (m >= 10)
                    {
                        fcemployeeLogs.TotalTime = h + "H:" + m + "M";
                    }
                    else
                    {
                        fcemployeeLogs.TotalTime = h + "H:0" + m + "M";
                    }
                    await _context.SaveChangesAsync();
                }



                

                var requestBody = new
                {
                    EMPLOYEE_ID = employeeId,
                    LOG_DATE = inclusivedate,
                    LOG_TIME = time24Hour,
                    LOG_TYPE = "OUT2A",
                    INCLUSIVE_DATE = inclusivedate,
                    SOURCE = "ONTIME"
                }; // Example request body data

                using var httpClient = new HttpClient();

                var req = new HttpRequestMessage(HttpMethod.Post, "https://kunzad2.fastlogistics.com.ph/fastlogistics/api/hrisEMPL_DTR")
                {
                    Headers =
    {

        { "Token", "c3lzdGVtQGZhc3Rncm91cC5iaXo6MjhlNmQyYjUtMzgzMy00ZWQ3LWE4MWUtZTk3NzUyYjU2ZmY3dHNhZg==" },

    },
                    Content = JsonContent.Create(requestBody)
                };

                using var response = await httpClient.SendAsync(req);
                var iss = response.StatusCode;
                if (response.IsSuccessStatusCode)
                {
                    // Request was successful
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Response Content: " + responseContent);
                }
                else
                {
                    // Request failed
                    Console.WriteLine("Request failed with status code: " + response.StatusCode);
                }


                return Ok("Success");

            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("InnerException") || ex.Message.Contains("inner exception"))
                {

                    return StatusCode(202,ex.InnerException);
                }
                else
                {

                    return StatusCode(202, ex.Message);
                }
            }
            return Ok("Not Uploaded");
        }

        [HttpPost("siteBreak")]
        public async Task<ActionResult<FcAttendance>> siteBreak(
            [FromForm] String employeeId,
            [FromForm] String time
            )
        {

            var newFcEmployees = _context.FcAttendances.
                          FirstOrDefault(s => s.EmployeeId.Equals(int.Parse(employeeId)) &&
                          s.TimeOut == null &&
                          s.TotalTime == null&&
                          s.WorkPlace == "Site");

            if (newFcEmployees != null)
            {

                DateTime morningBreak = DateTime.Parse("11:00");
                DateTime afternoonBreak = DateTime.Parse("13:00");


        

                if (newFcEmployees.FirstBreakOut == null)
                {
                    newFcEmployees.FirstBreakOut = time;

                }
                else if (newFcEmployees.FirstBreakIn == null)
                {
                    newFcEmployees.FirstBreakIn = time;
                }
               else if(newFcEmployees.SecondBreakOut == null)
                {
                    newFcEmployees.SecondBreakOut = time;
                }
                else if (newFcEmployees.SecondBreakIn == null)
                {
                    newFcEmployees.SecondBreakIn = time;
                }
                else if (newFcEmployees.ThirdBreakOut == null)
                {
                    newFcEmployees.ThirdBreakOut = time;
                }
                else if (newFcEmployees.ThirdBreakIn == null)
                {
                    newFcEmployees.ThirdBreakIn = time;
                }


                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        private bool FcAttendanceExists(int id)
        {
            return _context.FcAttendances.Any(e => e.Id == id);
        }
    }
}
public static class ExtensionMethods
{

    public static DateTimeOffset OnTodayInTimeZone(this TimeOnly time, TimeZoneInfo tz)
    {
        return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTime.UtcNow, tz))
            .ToDateTime(time)
            .ToDateTimeOffset(tz);
    }

 
    public static DateTimeOffset ToDateTimeOffset(this DateTime dt, TimeZoneInfo tz)
    {
        if (dt.Kind != DateTimeKind.Unspecified)
        {
            // Handle UTC or Local kinds (regular and hidden 4th kind)
            DateTimeOffset dto = new DateTimeOffset(dt.ToUniversalTime(), TimeSpan.Zero);
            return TimeZoneInfo.ConvertTime(dto, tz);
        }

        if (tz.IsAmbiguousTime(dt))
        {
            // Prefer the daylight offset, because it comes first sequentially (1:30 ET becomes 1:30 EDT)
            TimeSpan[] offsets = tz.GetAmbiguousTimeOffsets(dt);
            TimeSpan offset = offsets[0] > offsets[1] ? offsets[0] : offsets[1];
            return new DateTimeOffset(dt, offset);
        }

        if (tz.IsInvalidTime(dt))
        {
            // Advance by the gap, and return with the daylight offset  (2:30 ET becomes 3:30 EDT)
            TimeSpan[] offsets = { tz.GetUtcOffset(dt.AddDays(-1)), tz.GetUtcOffset(dt.AddDays(1)) };
            TimeSpan gap = offsets[1] - offsets[0];
            return new DateTimeOffset(dt.Add(gap), offsets[1]);
        }

        // Simple case
        return new DateTimeOffset(dt, tz.GetUtcOffset(dt));
    }
}

/*
   */