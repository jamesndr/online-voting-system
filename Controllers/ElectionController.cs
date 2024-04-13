using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using Onlinevotingsystem.Models;

namespace Onlinevotingsystem.Controllers
{
    public class ElectionController : Controller
    {
        private readonly OnlineVotingDB _context;

        public ElectionController(OnlineVotingDB context)
        {
            _context = context;
        }

        // GET: Election
        public async Task<IActionResult> Index()
        {
            TempData["nop"] = null;
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("password");
            if (x == null || y == null)
            {
                TempData["nop"] = "Please Login To Perform this Functionality!";
                return RedirectToAction("Login", "Users", new { area = "" });
            }
            if (x != "admin" && y != "admin")
            {
                TempData["nop"] = "You dont Have Permission to access this Functionality!";
                return RedirectToAction("Login", "Users", new { area = "" });
            }
            return View(await _context.Election.ToListAsync());
        }
        public async Task<IActionResult> UserElection()
        {
            var Elections = await _context.Election.ToListAsync();
            var today = DateTime.Today;
            List<Election> elec = new List<Election>();
            foreach (var e in Elections)
            {
                if (today <= e.EndDate)
                {
                    elec.Add(e);
                }
            }
            return View(elec);
        }
        public async Task<IActionResult> VuScore(int? id)
        {
            var electionResults = _context.ElectionResult.Where(s=>s.ElectionId==id).ToList();
            int numberOfVotes=0 ;
            int winnerCandidateID = 0;
            Winner winnerCandidate= new Winner();

            int count = 0;
            foreach (var x in electionResults) {
                if (count == 0) {
                    numberOfVotes = x.votes;
                    winnerCandidateID = x.CandidateId;
                }
                if (count > 0)
                {
                    if(numberOfVotes < x.votes)
                    {
                        numberOfVotes = x.votes;
                        winnerCandidateID = x.CandidateId;
                    }
                }
                count++;
            }
            //get Election 
            

            var election = await _context.Election
                .FirstOrDefaultAsync(m => m.Electionid == id);
            
            var today = DateTime.Today;
            //get winner Candidate
            

            var candidate = await _context.Candidate
                .FirstOrDefaultAsync(m => m.Candidateid == winnerCandidateID);
            if (election.EndDate>today.Date)
            {
                winnerCandidate.CandidateName = "Nobody";
                winnerCandidate.CandidateImg = "Wait.jfif";
                winnerCandidate.NoofVotes = 0;
                winnerCandidate.ElectionName = election.ElectionName;
                TempData["wait"] = "....Votings Are Still Going On....";
                return View(winnerCandidate);
            }
            winnerCandidate.CandidateName = candidate.FirstName + "" + candidate.LastName;
            winnerCandidate.CandidateImg = candidate.Image;
            winnerCandidate.NoofVotes = numberOfVotes;
            winnerCandidate.ElectionName = election.ElectionName;
            
            return View(winnerCandidate);
        }
            public async Task<IActionResult> VuCandidate( int? id)
        {
            var Elections= _context.Election.ToList();  
            
            if (    Elections == null)
            {
                return NotFound();
            }
            Election elect = null;
            foreach(var x in Elections)
            {
                if (x.Electionid == id)
                {
                    elect = x;

                }
            }
           // var election =  _context.Election.FirstOrDefaultAsync(m => m.Electionid == id);
            if (elect == null)
            {
                return NotFound();
            }
           //ViewBag.xemail = HttpContext.Session.GetString("email");
            HttpContext.Session.SetString("elecname", ""+elect.Electionid);
            ViewBag.yname= id;
            var candidates = _context.Candidate.Where(m => m.TypeElection == id);
               // .SingleOrDefault(m => m.TypeElection == id);
            //if (candidates != null)
            //{
                return View(candidates);
            //    return Json(new { status = true, message = "Vote For eligible candidate you think ",CandidateList = candidates });
           // }
            // return Json(new { status = false, message = "No Candidates Registered " });
            //return RedirectToAction(nameof(Index));
        }
        // GET: Election/Details/5
        public async Task<IActionResult> CastVotes(int? id)
        {
            
            Reply r= new Reply();
            ElectionResult er = new ElectionResult();
            var votes = await _context.ElectionVotes.ToListAsync();
            ElectionVotes ev = new ElectionVotes();
            var x = HttpContext.Session.GetString("email");
            var y = HttpContext.Session.GetString("elecname");
            var z = HttpContext.Session.GetString("password");
            var election = await _context.Election.FirstOrDefaultAsync(m => m.Electionid == Int16.Parse(y));
            var today = DateTime.Today;
            var usr= await _context.Users
                .FirstOrDefaultAsync(m => m.email == x && m.password==z);
            if(usr != null && y != null && id != null)
            {
                ev.UserId = usr.Userid;
                ev.CandidateId = (int)id;
                ev.ElectionId = Int16.Parse(y);
                var loggedin = _context.ElectionResult.Where(s => s.ElectionId == ev.ElectionId && s.CandidateId == ev.CandidateId).SingleOrDefault();
                var alreadyVoted = false;
                if (usr.CanVote=="true")
                {
                    foreach(var i in votes)
                    {
                        if(i.UserId==ev.UserId && i.ElectionId==ev.ElectionId)
                        {
                            alreadyVoted = true;
                      

                        }
                    }
                    if(!alreadyVoted)
                    {
                        if (election.EndDate > today.Date) {
                            _context.Add(ev);
                            await _context.SaveChangesAsync();
                            if (loggedin == null)
                            {
                                er.ElectionId = Int16.Parse(y);
                                er.CandidateId = (int)id;
                                er.votes = 1;
                                _context.Add(er);
                                await _context.SaveChangesAsync();

                            }
                            else
                            {
                                int i = loggedin.votes;
                                er.ElectionResultId = loggedin.ElectionResultId;
                                er.ElectionId = Int16.Parse(y);
                                er.CandidateId = (int)id;
                                er.votes = i + 1;
                                _context.Update(er);
                                await _context.SaveChangesAsync();
                            }
                            r.status = true;
                            r.color = "alert alert-success";
                            r.message = "Your vote have been casted!";
                            return View(r);
                        }
                        r.status = false;
                        r.color = "alert alert-danger";
                        r.message = "..Election is Over..";
                        // return Json(new { status = false, message = "Your vote have been already casted!" });

                        return View(r);

                    }
                    r.status = false;
                    r.color = "alert alert-danger";
                    r.message = "Your vote have been already casted!";
                    // return Json(new { status = false, message = "Your vote have been already casted!" });

                    return View(r);
                }
                r.status = false;
                r.color = "alert alert-warning";
                r.message = "You dont have permission to vote!";
                //return Json(new { status = false, message = "You dont have permission to vote!" });
                return View(r);
            }

            r.status = false;
            r.color = "alert alert-info";
            r.message = "Sorry for the inconveniece please check after sometime";
            //  return Json(new { status = false, message = "You dont have permission to vote!" });
            return View(r);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Election == null)
            {
                return NotFound();
            }

            var election = await _context.Election
                .FirstOrDefaultAsync(m => m.Electionid == id);
            if (election == null)
            {
                return NotFound();
            }

            return View(election);
        }

        // GET: Election/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Election/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Electionid,ElectionName,EndDate")] Election election)
        {
            TempData["eles"] = null;
            if (ModelState.IsValid)
            {
                _context.Add(election);
                await _context.SaveChangesAsync();
                TempData["eles"] = "..Election Created Successfully..";
                return RedirectToAction(nameof(Create));
            }
            TempData["elef"] = "..Election Creation Failed..";
            return View(election);
        }

        // GET: Election/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Election == null)
            {
                return NotFound();
            }

            var election = await _context.Election.FindAsync(id);
            if (election == null)
            {
                return NotFound();
            }
            return View(election);
        }

        // POST: Election/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Electionid,ElectionName,EndDate")] Election election)
        {
            if (id != election.Electionid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(election);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ElectionExists(election.Electionid))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(election);
        }

        // GET: Election/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Election == null)
            {
                return NotFound();
            }

            var election = await _context.Election
                .FirstOrDefaultAsync(m => m.Electionid == id);
            if (election == null)
            {
                return NotFound();
            }

            return View(election);
        }

        // POST: Election/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Election == null)
            {
                return Problem("Entity set 'OnlineVotingDB.Election'  is null.");
            }
            var election = await _context.Election.FindAsync(id);
            if (election != null)
            {
                _context.Election.Remove(election);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ElectionExists(int id)
        {
          return _context.Election.Any(e => e.Electionid == id);
        }
    }
}
