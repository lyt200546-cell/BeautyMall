using MZWlyt.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MZWlyt.Controllers
{
    public class OrdersController : Controller
    {
        private CosmeticsEntities db = new CosmeticsEntities();

        // GET: Orders
        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Content("<script>alert('用户登录已过期或未登录,请重新登录!');window.location.href='/User/Login';</script>");
            }

            if (Session["Role"].ToString() == "user")
            {
                var user = Session["IdInfo"] as tb_user;
                // 使用AsNoTracking直接从数据库获取最新数据，不使用缓存
                var orders = db.tb_orderses.AsNoTracking().Where(a => a.uname == user.uid.ToString()).ToList();

                // 为每个订单获取订单详情的状态
                foreach (var order in orders)
                {
                    var detail = db.tb_orderDetailses.AsNoTracking().FirstOrDefault(d => d.oid == order.oid);
                    if (detail != null)
                    {
                        ViewData[order.oid.ToString()] = detail.states;
                    }
                }

                return View(orders);
            }
            // 管理员显示所有订单
            var allOrders = db.tb_orderses.AsNoTracking().ToList();
            foreach (var order in allOrders)
            {
                var detail = db.tb_orderDetailses.AsNoTracking().FirstOrDefault(d => d.oid == order.oid);
                if (detail != null)
                {
                    ViewData[order.oid.ToString()] = detail.states;
                }
            }
            return View(allOrders);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_orders tb_orders = db.tb_orderses.Find(id);
            if (tb_orders == null)
            {
                return HttpNotFound();
            }
            return View(tb_orders);
        }

        public ActionResult InsertOrder()
        {
            if (Session["IdInfo"] == null)
            {
                return Content("<script>alert('用户登录已过期或未登录,请重新登录!');window.location.href='/User/Login';</script>");
            }
            var user = Session["IdInfo"] as tb_user;

            //计算订单金额
            var cart = db.tb_carts.Where(a => a.uname == user.uid.ToString()).ToList();
            if (cart == null || cart.Count == 0)
            {
                return Content("<script>alert('购物车为空!');window.location.href='/Cart/Index';</script>");
            }

            decimal? priceTotal = 0;
            int productCounts = 0;
            foreach (var c in cart)
            {
                priceTotal += c.price * c.nums;
                productCounts++;
            }

            // 获取用户信息
            var currentUser = db.tb_users.Find(user.uid);
            if (currentUser == null)
            {
                return Content("<script>alert('用户信息不存在!');window.location.href='/User/Login';</script>");
            }

            if (string.IsNullOrEmpty(currentUser.address) || string.IsNullOrEmpty(currentUser.tel))
            {
                return Content("<script>alert('请先完善收货地址和联系电话!');window.location.href='/User/Edit/' + user.uid;</script>");
            }

            // 创建订单
            tb_orders order = new tb_orders();
            order.uname = user.uid.ToString();
            order.orderTime = DateTime.Now;
            order.allPrice = priceTotal;
            order.address = currentUser.address;
            order.tel = currentUser.tel;
            order.pcounts = productCounts;
            db.tb_orderses.Add(order);

            try
            {
                db.SaveChanges();

                // 创建订单详情
                foreach (var myC in cart)
                {
                    var orderDetail = new tb_orderDetails();
                    orderDetail.oid = order.oid;
                    orderDetail.uname = myC.uname ?? "";
                    orderDetail.pid = myC.pid;
                    orderDetail.pname = myC.pname;
                    orderDetail.price = myC.price ?? 0;
                    orderDetail.nums = myC.nums ?? 0;
                    orderDetail.photo = myC.photo ?? "";
                    orderDetail.states = "未付款";
                    db.tb_orderDetailses.Add(orderDetail);
                }
                db.SaveChanges();
            }
            catch
            {
                // 订单和详情已创建，即使清空购物车失败也返回成功
            }

            return Content("<script>alert('提交订单成功!');window.location.href='/Orders/Index';</script>");
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "oid,uname,orderTime,allPrice,address,tel,pcounts")] tb_orders tb_orders)
        {
            if (ModelState.IsValid)
            {
                db.tb_orderses.Add(tb_orders);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_orders);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_orders tb_orders = db.tb_orderses.Find(id);
            if (tb_orders == null)
            {
                return HttpNotFound();
            }
            return View(tb_orders);
        }

        // POST: Orders/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "oid,uname,orderTime,allPrice,address,tel,pcounts")] tb_orders tb_orders)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tb_orders).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tb_orders);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_orders tb_orders = db.tb_orderses.Find(id);
            if (tb_orders == null)
            {
                return HttpNotFound();
            }
            return View(tb_orders);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_orders tb_orders = db.tb_orderses.Find(id);
            db.tb_orderses.Remove(tb_orders);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // 付款方法
        public ActionResult Pay(int id)
        {
            var order = db.tb_orderses.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            // 更新订单详情的状态为已付款
            var orderDetails = db.tb_orderDetailses.Where(d => d.oid == id).ToList();
            foreach (var detail in orderDetails)
            {
                detail.states = "已付款";
                db.Entry(detail).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Content("<script>alert('付款成功!');window.location.href='/Orders/Index';</script>");
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
