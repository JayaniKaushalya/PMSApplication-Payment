using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using PMSWebApplication.Models;
using PMSWebApplication.Models.DomainModels;





namespace PMSWebApplication.Controllers
{
    public class PaymentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        // GET: Payments
        public async Task<ActionResult> Index()
        {
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");
            var payments = db.Payments.Include(p => p.Project).Include(p => p.Task);
            return View(await payments.ToListAsync());

            
        }
        [HttpPost]
        public async Task<ActionResult> Index(int ProjectId)

        {
           
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");
            var payments = db.Payments.Include(p => p.Project).Include(p => p.Task).Where(p=>p.ProjectId==ProjectId);
            return View(await payments.ToListAsync());

        }
       


        // GET: Payments/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // GET: Payments/Create
        public ActionResult Create()
        {
           
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "TaskName", "TaskStages");
            return View();
        }


        



        // POST: Payments/Create
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ProjectId,TaskStages,TaskId,PayDate,PaymentAmount,PayMethod,PayDiscription")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Payments.Add(payment);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", payment.ProjectId);
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "TaskName", "TaskStages", payment.TaskId);
           
           
            return View(payment);
        }

        


        // GET: Payments/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", payment.ProjectId);
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "TaskName", "TaskStages", payment.TaskId);
            return View(payment);
        }

        // POST: Payments/Edit/5
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ProjectId,TaskStages,TaskId,PayDate,PaymentAmount,PayMethod,PayDiscription")] Payment payment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(payment).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName", payment.ProjectId);
            ViewBag.TaskId = new SelectList(db.Tasks, "Id", "TaskName", "TaskStages", payment.TaskId);
            return View(payment);
        }

        // GET: Payments/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Payment payment = await db.Payments.FindAsync(id);
            if (payment == null)
            {
                return HttpNotFound();
            }
            return View(payment);
        }

        // POST: Payments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Payment payment = await db.Payments.FindAsync(id);
            db.Payments.Remove(payment);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        
        
        
        // GET: Upcomming Payements

        public async Task<ActionResult> UpCommingPayments(int? id )
        {

            
            List<UpcommingPayment> ProjectId = new List<UpcommingPayment>();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");


            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today ).ToListAsync();
            List<UpcommingPayment> upcommingPayemets = new List<UpcommingPayment>();
           
            foreach (var task in tasks)
            {
                UpcommingPayment payment = new UpcommingPayment();
                Project project;

                if (id == null) {
                    project = await db.Projects.FindAsync(task.ProjectId);
                } else
                {
                    if (id == task.ProjectId)
                    {
                        project = await db.Projects.FindAsync(task.ProjectId);
                    }
                    else continue;
                }
                
          
                var count = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id && x.TaskStages == task.TaskStages).Select(x => x.PaymentAmount).Sum();



                payment.Id = task.Id;
                payment.Payment = (task.TaskWisePayment - count);
                payment.TaskName = task.TaskName;
                payment.ProjectName = project.ProjectName;
                payment.Deadline = task.Deadline;
                payment.TaskStages = task.TaskStages;

                upcommingPayemets.Add(payment);
            }

           return View(upcommingPayemets);


     }





        // GET: Duepayment
        public async Task<ActionResult> Duepayment(int? id)
        {

            List<Duepayment> ProjectId = new List<Duepayment>();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");


            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<Duepayment> duepayment = new List<Duepayment>();

            foreach (var task in tasks)

            {
                Duepayment paid = new Duepayment();
                Project project;

                if (id == null)
                {
                    project = await db.Projects.FindAsync(task.ProjectId);
                }
                else
                {
                    if (id == task.ProjectId)
                    {
                        project = await db.Projects.FindAsync(task.ProjectId);
                    }
                    else continue;
                }

                var total = db.Payments.Where(x => x.ProjectId == project.Id && x.TaskId == task.Id && x.TaskStages == task.TaskStages).Select(x => x.PaymentAmount).Sum();

                paid.Id = task.Id;
                paid.PaidAmount = total;
                paid.TaskName = task.TaskName;
                paid.ProjectName = project.ProjectName;
                paid.Deadline = task.Deadline;
                paid.TaskStages = task.TaskStages;

                duepayment.Add(paid);
            }
            return View(duepayment);

        }




        // GET: Task wise payment
        public async Task<ActionResult> Taskwisepayment(int? id)
        {

            List<Taskwisepayment> ProjectId = new List<Taskwisepayment>();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");

            var tasks = await db.Tasks.Where(x => x.Deadline > DateTime.Today).ToListAsync();
            List<Taskwisepayment> taskwisepayment = new List<Taskwisepayment>();

            foreach (var task in tasks)
            {
                Taskwisepayment paid = new Taskwisepayment();
                Project project;

                if (id == null)
                {
                    project = await db.Projects.FindAsync(task.ProjectId);
                }
                else
                {
                    if (id == task.ProjectId)
                    {
                        project = await db.Projects.FindAsync(task.ProjectId);
                    }
                    else continue;
                }



                

                paid.Id = task.Id;
                paid.Payment = task.TaskWisePayment;
                paid.TaskName = task.TaskName;
                paid.ProjectName = project.ProjectName;
            
                paid.TaskStages = task.TaskStages;

                taskwisepayment.Add(paid);
            }
            return View(taskwisepayment);

        }

        
        
        
        // GET: TotalPayment

        [HttpGet, ActionName("TotalPayment")]
        public async Task<ActionResult> TotalPayment()

        {

            List<UpcommingPayment> ProjectId = new List<UpcommingPayment>();
            ViewBag.ProjectId = new SelectList(db.Projects, "Id", "ProjectName");


            var query = from Tasks in db.Tasks
                        group new { Tasks, Tasks.Project } by new
                        {
                            Tasks.ProjectId,
                            Tasks.Project.ProjectName,
                            Tasks.TaskStages
                        } into g
                        select new Totalpayment
                        {
                           Id = g.Key.ProjectId,
                            TaskStages = g.Key.TaskStages,
                            ProjectName = g.Key.ProjectName,
                            Payment = (decimal?)g.Sum(p => p.Tasks.TaskWisePayment)
                        };
            var tasks = await query.ToListAsync();
            return View(tasks);
        }

       




    }

}

