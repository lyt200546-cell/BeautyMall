using MZWlyt.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MZWlyt.Controllers
{
    public class CartController : Controller
    {
        private CosmeticsEntities db = new CosmeticsEntities();

        // GET: Cart
        public ActionResult Index()
        {
            if (Session["IdInfo"] == null)
            {
                return Content("<script>alert('用户登录已过期或未登录,请重新登录!');window.location.href='/User/Login';</script>");
            }
            var user = Session["IdInfo"] as tb_user;
            var tb_cart = db.tb_carts.Include(c => c.tb_product).Where(a => a.uname == user.uid.ToString());
            return View(tb_cart.ToList());
        }

        // GET: Cart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_cart tb_cart = db.tb_carts.Find(id);
            if (tb_cart == null)
            {
                return HttpNotFound();
            }
            return View(tb_cart);
        }

        public ActionResult JoinCart(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // 检查是否为管理员
            if (Session["Role"] != null && Session["Role"].ToString() == "admin")
            {
                return Content("<script>alert('管理员账户不支持此功能，请切换为用户登录！');window.location.href='/Product/Index';</script>");
            }

            var user = Session["IdInfo"] as tb_user;
            if (user == null)
            {
                return Content("<script>alert('用户登录已过期或未登录,请重新登录!');window.location.href='/User/Login';</script>");
            }

            var cart = db.tb_carts.Where(a => a.uname == user.uid.ToString()).FirstOrDefault();
            if (cart == null)
            {
                cart = new tb_cart();
                cart.uname = user.uid.ToString();
                tb_product tb_product = db.tb_products.Find(id);
                if (tb_product == null)
                {
                    return HttpNotFound();
                }
                cart.pid = id;
                cart.pname = tb_product.pname;
                cart.price = tb_product.price;
                cart.nums = 1;
                cart.photo = tb_product.photo.Replace("../", "/");
                db.tb_carts.Add(cart);
                db.SaveChanges();
                return Content("<script>alert('添加购物车成功!');window.location.href='/Cart/Index';</script>");
            }
            else
            {
                tb_product tb_product = db.tb_products.Find(id);
                if (tb_product == null)
                {
                    return HttpNotFound();
                }
                var myCart = db.tb_carts.Where(a => a.uname == user.uid.ToString()).Where(p => p.pid == id).FirstOrDefault();
                if (myCart == null)
                {
                    myCart = new tb_cart();
                    myCart.uname = user.uid.ToString();
                    myCart.pid = id;
                    myCart.pname = tb_product.pname;
                    myCart.price = tb_product.price;
                    myCart.nums = 1;
                    myCart.photo = tb_product.photo.Replace("../", "/");
                    db.tb_carts.Add(myCart);
                    db.SaveChanges();
                }
                else
                {
                    myCart.nums += 1;
                    db.Entry(myCart).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return Content("<script>alert('添加购物车成功!');window.location.href='/Cart/Index';</script>");
            }
        }



        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.pid = new SelectList(db.tb_products, "pid", "pname");
            return View();
        }

        // POST: Cart/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "cid,uname,pid,pname,price,nums,photo")] tb_cart tb_cart)
        {
            if (ModelState.IsValid)
            {
                db.tb_carts.Add(tb_cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.pid = new SelectList(db.tb_products, "pid", "pname", tb_cart.pid);
            return View(tb_cart);
        }

        // GET: Cart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_cart tb_cart = db.tb_carts.Find(id);
            if (tb_cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.pid = new SelectList(db.tb_products, "pid", "pname", tb_cart.pid);
            return View(tb_cart);
        }

        // POST: Cart/Edit/5
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            int cid = Convert.ToInt32(form["cid"]);
            int nums = Convert.ToInt32(form["nums"]);

            var cart = db.tb_carts.Find(cid);
            if (cart == null)
            {
                return Content("<script>alert('未找到购物车记录！');window.location.href='/Cart/Index';</script>");
            }

            cart.nums = nums;
            db.SaveChanges();

            return Content("<script>alert('保存成功!');window.location.href='/Cart/Index';</script>");
        }

        // GET: Cart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_cart tb_cart = db.tb_carts.Find(id);
            if (tb_cart == null)
            {
                return HttpNotFound();
            }
            return View(tb_cart);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_cart tb_cart = db.tb_carts.Find(id);
            db.tb_carts.Remove(tb_cart);
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
