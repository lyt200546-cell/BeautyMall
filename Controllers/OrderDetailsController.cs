using MZWlyt.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MZWlyt.Controllers
{
    public class OrderDetailsController : Controller
    {
        private CosmeticsEntities db = new CosmeticsEntities();

        // GET: OrderDetails
        public ActionResult Index(int? oid)
        {
            if (Session["IdInfo"] == null)
            {
                return Content("<script>alert('用户登录已过期或未登录,请重新登录!');window.location.href='/User/Login';</script>");
            }

            tb_user user = Session["IdInfo"] as tb_user;

            // 如果传入了oid参数，则显示该订单的详情
            if (oid.HasValue)
            {
                var result = db.tb_orderDetailses.Include(d => d.tb_product).Where(d => d.oid == oid.Value).ToList();
                return View(result);
            }

            if (Session["Role"] != null && Session["Role"].ToString() == "user")
            {
                // 普通用户：显示自己的订单详情
                if (user == null || user.uid == 0)
                {
                    return View(new List<tb_orderDetails>());
                }
                string userId = user.uid.ToString();
                var result = db.tb_orderDetailses.Include(d => d.tb_product).Where(d => d.uname == userId).ToList();
                return View(result);
            }

            // 管理员显示所有订单详情
            return View(db.tb_orderDetailses.Include(d => d.tb_product).ToList());
        }

        // GET: OrderDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_orderDetails tb_orderDetails = db.tb_orderDetailses.Find(id);
            if (tb_orderDetails == null)
            {
                return HttpNotFound();
            }
            return View(tb_orderDetails);
        }

        // GET: OrderDetails/Create
        public ActionResult Create()
        {
            ViewBag.pid = new SelectList(db.tb_products, "pid", "pname");
            return View();
        }

        // POST: OrderDetails/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,oid,uname,pid,pname,price,nums,photo,states")] tb_orderDetails tb_orderDetails)
        {
            if (ModelState.IsValid)
            {
                db.tb_orderDetailses.Add(tb_orderDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.pid = new SelectList(db.tb_products, "pid", "pname", tb_orderDetails.pid);
            return View(tb_orderDetails);
        }

        // GET: OrderDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_orderDetails tb_orderDetails = db.tb_orderDetailses.Find(id);
            if (tb_orderDetails == null)
            {
                return HttpNotFound();
            }
            ViewBag.pid = new SelectList(db.tb_products, "pid", "pname", tb_orderDetails.pid);
            return View(tb_orderDetails);
        }

        // POST: OrderDetails/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,oid,uname,pid,pname,price,nums,photo,states")] tb_orderDetails tb_orderDetails)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_orderDetails).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.pid = new SelectList(db.tb_products, "pid", "pname", tb_orderDetails.pid);
            return View(tb_orderDetails);
        }

        // GET: OrderDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_orderDetails tb_orderDetails = db.tb_orderDetailses.Find(id);
            if (tb_orderDetails == null)
            {
                return HttpNotFound();
            }
            return View(tb_orderDetails);
        }

        // POST: OrderDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_orderDetails tb_orderDetails = db.tb_orderDetailses.Find(id);
            db.tb_orderDetailses.Remove(tb_orderDetails);
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
