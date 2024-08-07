using CandidateProject.EntityModels;
using CandidateProject.ViewModels;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace CandidateProject.Controllers
{
    public class CartonController : Controller
    {
        private CartonContext db = new CartonContext();

        // GET: Carton
        // public ActionResult Index()
        // {
        //     var cartons = db.Cartons
        //         .Select(c => new CartonViewModel()
        //         {
        //             Id = c.Id,
        //             CartonNumber = c.CartonNumber
        //         })
        //         .ToList();

        //     return View(cartons);
        // }
        //ENhancement1
        public ActionResult Index()
        {
            var cartons = db.Cartons
                .Include(c => c.CartonDetails)
                .Select(c => new
                {
                    Carton = c,
                    EquipmentCount = c.CartonDetails.Count
                })
                .ToList();

            return View(cartons);
        }

        // GET: Carton/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonViewModel()
                {
                    Id = c.Id,
                    CartonNumber = c.CartonNumber
                })
                .SingleOrDefault();
            if (carton == null)
            {
                return HttpNotFound();
            }
            return View(carton);
        }

        // GET: Carton/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Carton/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CartonNumber")] Carton carton)
        {
            if (ModelState.IsValid)
            {
                db.Cartons.Add(carton);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(carton);
        }

        // GET: Carton/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonViewModel()
                {
                    Id = c.Id,
                    CartonNumber = c.CartonNumber
                })
                .SingleOrDefault();
            if (carton == null)
            {
                return HttpNotFound();
            }
            return View(carton);
        }

        // POST: Carton/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CartonNumber")] CartonViewModel cartonViewModel)
        {
            if (ModelState.IsValid)
            {
                var carton = db.Cartons.Find(cartonViewModel.Id);
                carton.CartonNumber = cartonViewModel.CartonNumber;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cartonViewModel);
        }

        // GET: Carton/Delete/5
        // public ActionResult Delete(int? id)
        // {
        //     if (id == null)
        //     {
        //         return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //     }
        //     Carton carton = db.Cartons.Find(id);
        //     if (carton == null)
        //     {
        //         return HttpNotFound();
        //     }
        //     return View(carton);
        // }


       //Bug 1
        public ActionResult Delete(int? id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid carton ID.");
            }

            var carton = _context.Cartons.Include(c => c.CartonDetails)
                                        .FirstOrDefault(c => c.Id == id);

            if (carton == null)
            {
                return HttpNotFound("Carton not found.");
            }

            // Remove associated CartonDetails if any
            var cartonDetails = _context.CartonDetails.Where(cd => cd.CartonId == id).ToList();
            _context.CartonDetails.RemoveRange(cartonDetails);

            // Remove the carton itself
            _context.Cartons.Remove(carton);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        // POST: Carton/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Carton carton = db.Cartons.Find(id);
            db.Cartons.Remove(carton);
            db.SaveChanges();
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

        public ActionResult AddEquipment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonDetailsViewModel()
                {
                    CartonNumber = c.CartonNumber,
                    CartonId = c.Id
                })
                .SingleOrDefault();

            if (carton == null)
            {
                return HttpNotFound();
            }

            var equipment = db.Equipments
                .Where(e => !db.CartonDetails.Where(cd => cd.CartonId == id).Select(cd => cd.EquipmentId).Contains(e.Id) )
                .Select(e => new EquipmentViewModel()
                {
                    Id = e.Id,
                    ModelType = e.ModelType.TypeName,
                    SerialNumber = e.SerialNumber
                })
                .ToList();
            
            carton.Equipment = equipment;
            return View(carton);
        }

        // public ActionResult AddEquipmentToCarton([Bind(Include = "CartonId,EquipmentId")] AddEquipmentViewModel addEquipmentViewModel)
        // {
        //     if (ModelState.IsValid)
        //     {
        //         var carton = db.Cartons
        //             .Include(c => c.CartonDetails)
        //             .Where(c => c.Id == addEquipmentViewModel.CartonId)
        //             .SingleOrDefault();
        //         if (carton == null)
        //         {
        //             return HttpNotFound();
        //         }
        //         var equipment = db.Equipments
        //             .Where(e => e.Id == addEquipmentViewModel.EquipmentId)
        //             .SingleOrDefault();
        //         if (equipment == null)
        //         {
        //             return HttpNotFound();
        //         }
        //         var detail = new CartonDetail()
        //         {
        //             Carton = carton,
        //             Equipment = equipment
        //         };
        //         carton.CartonDetails.Add(detail);
        //         db.SaveChanges();
        //     }
        //     return RedirectToAction("AddEquipment", new { id = addEquipmentViewModel.CartonId });
        // }

        //Bug2

        public ActionResult AddEquipmentToCarton([Bind(Include = "CartonId,EquipmentId")] AddEquipmentViewModel addEquipmentViewModel)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the carton and equipment from the database
                var carton = db.Cartons
                    .Include(c => c.CartonDetails)
                    .Where(c => c.Id == addEquipmentViewModel.CartonId)
                    .SingleOrDefault();
                
                if (carton == null)
                {
                    return HttpNotFound();
                }

                var equipment = db.Equipments
                    .Where(e => e.Id == addEquipmentViewModel.EquipmentId)
                    .SingleOrDefault();
                
                if (equipment == null)
                {
                    return HttpNotFound();
                }

                // Check if the equipment is already assigned to the same carton
                var existingCartonDetail = db.CartonDetails
                    .Where(cd => cd.EquipmentId == addEquipmentViewModel.EquipmentId)
                    .SingleOrDefault();

                if (existingCartonDetail != null && existingCartonDetail.CartonId == addEquipmentViewModel.CartonId)
                {
                    // Equipment is already assigned to this carton
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Equipment is already assigned to this carton.");
                }

                // Check if the equipment is already assigned to another carton
                if (existingCartonDetail != null && existingCartonDetail.CartonId != addEquipmentViewModel.CartonId)
                {
                    // Equipment is already assigned to another carton
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Equipment is already assigned to another carton.");
                }

                // Check if the carton is not exceeding the limit of 10 pieces
                if (carton.CartonDetails.Count >= 10)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Carton can only hold up to 10 pieces of equipment.");
                }

                // Add the equipment to the carton
                var detail = new CartonDetail()
                {
                    Carton = carton,
                    Equipment = equipment
                };

                carton.CartonDetails.Add(detail);
                db.SaveChanges();
            }
            
            return RedirectToAction("AddEquipment", new { id = addEquipmentViewModel.CartonId });
        }


        public ActionResult ViewCartonEquipment(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var carton = db.Cartons
                .Where(c => c.Id == id)
                .Select(c => new CartonDetailsViewModel()
                {
                    CartonNumber = c.CartonNumber,
                    CartonId = c.Id,
                    Equipment = c.CartonDetails
                        .Select(cd => new EquipmentViewModel()
                        {
                            Id = cd.EquipmentId,
                            ModelType = cd.Equipment.ModelType.TypeName,
                            SerialNumber = cd.Equipment.SerialNumber
                        })
                })
                .SingleOrDefault();
            if (carton == null)
            {
                return HttpNotFound();
            }
            return View(carton);
        }

        // public ActionResult RemoveEquipmentOnCarton([Bind(Include = "CartonId,EquipmentId")] RemoveEquipmentViewModel removeEquipmentViewModel)
        // {
        //     return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //     if (ModelState.IsValid)
        //     {
        //         //Remove code here
        //     }
        //     return RedirectToAction("ViewCartonEquipment", new { id = removeEquipmentViewModel.CartonId });
        // }

        public ActionResult RemoveEquipmentOnCarton([Bind(Include = "CartonId,EquipmentId")] RemoveEquipmentViewModel removeEquipmentViewModel)
        {
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Retrieve the carton including its details
                var carton = _context.Cartons
                    .Include(c => c.CartonDetails)
                    .FirstOrDefault(c => c.Id == removeEquipmentViewModel.CartonId);
                
                if (carton == null)
                {
                    // Return NotFound result if carton is not found
                    return HttpNotFound("Carton not found.");
                }

                // Retrieve the specific equipment from the carton
                var equipment = carton.CartonDetails
                    .FirstOrDefault(cd => cd.EquipmentId == removeEquipmentViewModel.EquipmentId);
                
                if (equipment == null)
                {
                    // Return NotFound result if equipment is not found in the carton
                    return HttpNotFound("Equipment not found in carton.");
                }

                // Remove the equipment from the carton details
                carton.CartonDetails.Remove(equipment);

                // Optionally, remove the equipment from the CartonDetails table
                _context.CartonDetails.Remove(equipment);

                // Save changes to the database
                _context.SaveChanges();

                // Redirect to the view of the carton equipment
                return RedirectToAction("ViewCartonEquipment", new { id = removeEquipmentViewModel.CartonId });
            }

            // Return the view with validation errors if the model state is not valid
            return View(removeEquipmentViewModel);
        }

        //Enhancement2

        
        public ActionResult RemoveAllItemsFromCarton(int? id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid carton ID.");
            }

            var carton = db.Cartons
                .Include(c => c.CartonDetails)
                .SingleOrDefault(c => c.Id == id);

            if (carton == null)
            {
                return HttpNotFound("Carton not found.");
            }

            // Remove all CartonDetails associated with the carton
            db.CartonDetails.RemoveRange(carton.CartonDetails);

            // Save changes to the database
            db.SaveChanges();

            return RedirectToAction("Index");
        }


    }
}
