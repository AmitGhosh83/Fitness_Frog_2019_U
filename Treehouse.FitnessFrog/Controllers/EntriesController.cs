using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Treehouse.FitnessFrog.Data;
using Treehouse.FitnessFrog.Models;

namespace Treehouse.FitnessFrog.Controllers
{
    public class EntriesController : Controller
    {
        private EntriesRepository _entriesRepository = null;

        public EntriesController()
        {
            _entriesRepository = new EntriesRepository();
        }

        public ActionResult Index()
        {
            List<Entry> entries = _entriesRepository.GetEntries();

            // Calculate the total activity.
            double totalActivity = entries
                .Where(e => e.Exclude == false)
                .Sum(e => e.Duration);

            // Determine the number of days that have entries.
            int numberOfActiveDays = entries
                .Select(e => e.Date)
                .Distinct()
                .Count();

            ViewBag.TotalActivity = totalActivity;
            ViewBag.AverageDailyActivity = (totalActivity / (double)numberOfActiveDays);

            return View(entries);
        }

        public ActionResult Add()
        {
            var entry = new Entry()
            {
                Date= DateTime.Today
            };

            //populating the Drop Down list
            UpdateDropDownListItems();

            return View(entry);
        }
        [HttpPost]
        public ActionResult Add( Entry entry)
        {
            //if there arnt duration feild validation errors
            //then make sure that duration is greater than 0
            ValidateEntry(entry);

            if (ModelState.IsValid)
            {
                _entriesRepository.AddEntry(entry);
                return RedirectToAction("Index");
            }

            UpdateDropDownListItems();
            return View(entry);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get the requested entry from the repository
            Entry entry = _entriesRepository.GetEntry((int)id);

            //return a status of not found if status is not found
            if(entry is null)
            {
                return HttpNotFound();
            }
            //pass the entry to the view
            UpdateDropDownListItems();
            return View(entry);
        }
        [HttpPost]
        public ActionResult Edit(Entry entry)
        {
       
            ValidateEntry(entry);
            
            if(ModelState.IsValid)
            {
                _entriesRepository.UpdateEntry(entry);
                return RedirectToAction("Index");
            }

            UpdateDropDownListItems();
            return View(entry);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Entry entry = _entriesRepository.GetEntry((int)id);
            if ( entry is null)
            {
                return HttpNotFound();
            }
            return View(entry);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //Delete the entry from the entriesrepository
            _entriesRepository.DeleteEntry(id);
            //redirect to the list page
            return RedirectToAction("Index");
        }

        private void ValidateEntry(Entry entry)
        {
            if (ModelState.IsValidField("Duration") && entry.Duration <= 0)
            {
                ModelState.AddModelError("Duration", "The Duration feild must be greater than 0");
            }
        }
        private void UpdateDropDownListItems()
        {
            ViewBag.ActivitiesSelectListItems = new SelectList(Data.Data.Activities, "Id", "Name");
        }


    }
}