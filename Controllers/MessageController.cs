using MZWlyt.Models;
using PagedList;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MZWlyt.Controllers
{
    public class MessageController : Controller
    {
        private CosmeticsEntities db = new CosmeticsEntities();

        // GET: Message
        public ActionResult Index(int? page)
        {
            var messageList = from s in db.tb_messages select s;
            messageList = messageList.OrderByDescending(a => a.messDate);
            int pageNumber = page ?? 1;
            int pageSize = 10;
            IPagedList<tb_message> messagePagedList = messageList.ToPagedList(pageNumber, pageSize);
            return View(messagePagedList);
        }

        // GET: Message/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_message tb_message = db.tb_messages.Find(id);
            if (tb_message == null)
            {
                return HttpNotFound();
            }
            return View(tb_message);
        }

        // GET: Message/Create
        public ActionResult Create()
        {
            // 检查用户是否登录（普通用户或管理员）
            if (Session["IdInfo"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            // 普通用户获取用户名
            var user = Session["IdInfo"] as tb_user;
            if (user != null)
            {
                ViewBag.CurrentUserName = user.uname;
            }
            // 管理员也可以留言
            else if (Session["Role"] != null && Session["Role"].ToString() == "admin")
            {
                var admin = Session["IdInfo"] as tb_admin;
                if (admin != null)
                {
                    ViewBag.CurrentUserName = admin.aname;
                }
            }

            return View();
        }

        // POST: Message/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "mid,title,mess")] tb_message tb_message)
        {
            // 检查用户是否登录（普通用户或管理员）
            if (Session["IdInfo"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            // 普通用户
            var user = Session["IdInfo"] as tb_user;
            if (user != null)
            {
                tb_message.uname = user.uname;
            }
            // 管理员
            else if (Session["Role"] != null && Session["Role"].ToString() == "admin")
            {
                var admin = Session["IdInfo"] as tb_admin;
                if (admin != null)
                {
                    tb_message.uname = admin.aname;
                }
            }
            tb_message.messDate = DateTime.Now;

            // 手动清除uname和messDate的验证错误，因为这些由控制器自动设置
            ModelState.Remove("uname");
            ModelState.Remove("messDate");

            if (ModelState.IsValid)
            {
                db.tb_messages.Add(tb_message);
                db.SaveChanges();
                return Content("<script>alert('留言成功！');window.location.href='/Message/Index';</script>");
            }

            return View(tb_message);
        }

        // GET: Message/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_message tb_message = db.tb_messages.Find(id);
            if (tb_message == null)
            {
                return HttpNotFound();
            }
            return View(tb_message);
        }

        // POST: Message/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "mid,title,mess,uname,messDate")] tb_message tb_message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_message);
        }

        // GET: Message/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_message tb_message = db.tb_messages.Find(id);
            if (tb_message == null)
            {
                return HttpNotFound();
            }
            return View(tb_message);
        }

        // POST: Message/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_message tb_message = db.tb_messages.Find(id);
            db.tb_messages.Remove(tb_message);
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
    }
}
