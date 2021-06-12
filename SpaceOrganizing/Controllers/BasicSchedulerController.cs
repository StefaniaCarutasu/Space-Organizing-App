using DHTMLX.Common;
using DHTMLX.Scheduler;
using DHTMLX.Scheduler.Data;
using SpaceOrganizing.Data;
using SpaceOrganizing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SpaceOrganizing.Controllers
{
    public class BasicSchedulerController : Controller
    {
        private static int groupId = 0;
        private ApplicationDbContext db = new ApplicationDbContext();

        private void getTasks()
        {
            List<Tasks> tasks = (from task in db.Tasks
                                 where task.GroupId == groupId
                                 select task).ToList();

            var entities = new DefaultConnection();

            if (entities.Events != null)
            {
                foreach (Event e in entities.Events)
                {
                    entities.Events.Remove(e);
                }
            }

            foreach (Tasks t in tasks)
            {
                Event ev = new Event();
                ev.id = t.TaskId;
                ev.text = t.Title;
                ev.start_date = t.Deadline;
                ev.end_date = t.Deadline.AddDays(1);
                entities.Events.Add(ev);
            }

            entities.SaveChanges();
        }

        public ActionResult Index(int id)
        {
            groupId = id;
            var sched = new DHXScheduler(this);
            sched.Skin = DHXScheduler.Skins.Terrace;
            sched.LoadData = true;
            sched.EnableDataprocessor = true;
            sched.InitialDate = DateTime.Now;
            return View(sched);
        }

        public ContentResult Data()
        {
            getTasks();
            return (new SchedulerAjaxData(
                new DefaultConnection().Events
                .Select(e => new { e.id, e.text, e.start_date, e.end_date })
                )
                );
        }

        public ContentResult Save(int? id, FormCollection actionValues)
        {
            var action = new DataAction(actionValues);
            var changedEvent = DHXEventsHelper.Bind<Event>(actionValues);
            var entities = new DefaultConnection();
            try
            {
                switch (action.Type)
                {
                    case DataActionTypes.Insert:
                        entities.Events.Add(changedEvent);
                        break;
                    case DataActionTypes.Delete:
                        changedEvent = entities.Events.FirstOrDefault(ev => ev.id == action.SourceId);
                        entities.Events.Remove(changedEvent);
                        break;
                    default:// "update"
                        var target = entities.Events.Single(e => e.id == changedEvent.id);
                        DHXEventsHelper.Update(target, changedEvent, new List<string> { "id" });
                        break;
                }
                entities.SaveChanges();
                action.TargetId = changedEvent.id;
            }
            catch (Exception a)
            {
                action.Type = DataActionTypes.Error;
            }

            return (new AjaxSaveResponse(action));
        }
    }
}
