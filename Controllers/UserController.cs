using MZWlyt.Models;
using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace MZWlyt.Controllers
{
    public class UserController : Controller
    {
        private CosmeticsEntities db = new CosmeticsEntities();

        // GET: User
        public ActionResult Login()
        {
            return View();
        }


        // GET: User/Details/5
        [HttpPost]
        public ActionResult Login([Bind(Include = "uname,password")] tb_user tb_user)
        {
            var user = db.tb_users.Where(a => a.uname == tb_user.uname).FirstOrDefault();
            if (user == null)
            {
                return Content("<script>alert('用户名不存在');history.go(-1);</script>");
            }
            if (user.password != tb_user.password)
            {
                return Content("<script>alert('用户名或密码输入错误');history.go(-1);</script>");
            }
            Session["Role"] = "user";
            Session["IdInfo"] = user;
            return RedirectToAction("Index", "user");
        }

        public ActionResult Index()
        {
            if (Session["Role"] == null)
            {
                return Content("<script>alert('用户登录已过期或未登录,请重新登录!');window.location.href='/User/Login';</script>");
            }
            if (Session["Role"].ToString() == "admin")
            {
                return View(db.tb_users.ToList());
            }
            else
            {
                int uid = ((tb_user)Session["IdInfo"]).uid;
                return View(db.tb_users.Where(a => a.uid == uid).ToList());
            }
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_user tb_user = db.tb_users.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        // GET: User/Create
        public ActionResult Register()
        {
            return View();
        }

        // POST: 注册用户信息,注册成功后自动登录
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "uid, uname, password,confirmPassword, address,tel,email")] tb_user tb_user)
        {
            if (ModelState.IsValid)
            {
                db.tb_users.Add(tb_user);
                db.SaveChanges();
                return Login(tb_user);
            }
            return View();
        }



        // POST: User/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "uid,uname,password,confirmPassword,address,tel,email")] tb_user tb_user)
        {
            if (ModelState.IsValid)
            {
                db.tb_users.Add(tb_user);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tb_user);
        }

        // GET: User/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_user tb_user = db.tb_users.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        // POST: User/Edit/5
        // 为了防止"过多发布"攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection form)
        {
            try
            {
                int uid = Convert.ToInt32(form["uid"]);

                // 使用ADO.NET直接执行SQL，绕过Entity Framework验证
                string connString = System.Configuration.ConfigurationManager.ConnectionStrings["CosmeticsEntities"].ConnectionString;
                using (var conn = new System.Data.SqlClient.SqlConnection(connString))
                {
                    conn.Open();
                    string sql = @"UPDATE tb_user SET uname=@uname, password=@password, address=@address, tel=@tel, email=@email WHERE uid=@uid";
                    using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@uname", form["uname"]);
                        cmd.Parameters.AddWithValue("@password", form["password"]);
                        cmd.Parameters.AddWithValue("@address", form["address"]);
                        cmd.Parameters.AddWithValue("@tel", form["tel"]);
                        cmd.Parameters.AddWithValue("@email", form["email"]);
                        cmd.Parameters.AddWithValue("@uid", uid);
                        cmd.ExecuteNonQuery();
                    }
                }

                return Content("<script>alert('保存成功!');window.location.href='/Product/Index';</script>");
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMsg += " | " + ex.InnerException.Message;
                }
                return Content("<script>alert('保存失败: " + errorMsg.Replace("'", "\\'") + "');window.history.back();</script>");
            }
        }

        // GET: User/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_user tb_user = db.tb_users.Find(id);
            if (tb_user == null)
            {
                return HttpNotFound();
            }
            return View(tb_user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_user tb_user = db.tb_users.Find(id);
            db.tb_users.Remove(tb_user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult LogOut()
        {
            Session["Role"] = null;
            Session["IdInfo"] = null;
            return RedirectToAction("Index", "Product");
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
