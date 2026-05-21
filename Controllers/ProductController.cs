using MZWlyt.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MZWlyt.Controllers
{
    public class ProductController : Controller
    {
        private CosmeticsEntities db = new CosmeticsEntities();

        // GET: Product/Index
        public ActionResult Index(int? page, string search, bool? ai = false)
        {
            var productList = from s in db.tb_products select s;

            // 搜索功能
            if (!string.IsNullOrEmpty(search))
            {
                productList = productList.Where(p => p.pname.Contains(search));
            }

            productList = productList.OrderByDescending(a => a.salenums);
            int pageNumber = page ?? 1;
            int pageSize = 9;
            IPagedList<tb_product> productPagedList = productList.ToPagedList(pageNumber, pageSize);

            ViewBag.IsAI = ai == true;
            return View(productPagedList);
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id, int? page)
        {
            // 如果没有指定id或id不存在，重定向到首页
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            tb_product tb_product = db.tb_products.Find(id);
            if (tb_product == null)
            {
                return RedirectToAction("Index");
            }

            // get prev and next product
            var allProducts = db.tb_products.OrderBy(p => p.pid).ToList();
            int detailPageSize = 1;
            int totalCount = allProducts.Count;
            int totalPages = totalCount;
            int currentIndex = allProducts.FindIndex(p => p.pid == id);
            int prevId = 0, nextId = 0;

            if (currentIndex > 0)
                prevId = allProducts[currentIndex - 1].pid;
            if (currentIndex < allProducts.Count - 1)
                nextId = allProducts[currentIndex + 1].pid;

            int currentPage = currentIndex + 1;

            // get product IDs for each page
            List<int> pageProductIds = new List<int>();
            for (int i = 0; i < totalPages; i++)
            {
                int productIndex = i * detailPageSize;
                if (productIndex < allProducts.Count)
                    pageProductIds.Add(allProducts[productIndex].pid);
            }

            ViewBag.PrevId = prevId;
            ViewBag.NextId = nextId;
            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalCount = totalCount;
            ViewBag.PageProductIds = pageProductIds;
            ViewBag.TotalItems = allProducts.Count;

            return View(tb_product);
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pid,pname,photo,price,pnums,salenums,mess,state")] tb_product tb_product)
        {
            HttpPostedFile hpf = System.Web.HttpContext.Current.Request.Files[0];
            string name = hpf.FileName;
            string originalFileName = "";        //原始文件名
            if (name.Contains("."))
            {
                originalFileName =Path.GetFileNameWithoutExtension(hpf.FileName.ToString());
            }
            string allowExtension = ".jpg|.jpeg|.png|.doc|.docx|.pdf|.xls|.xlsx|.mp4 |.bmp";
            string fileExtension = Path.GetExtension(name);        //文件扩展名
            if (!allowExtension.Contains(fileExtension.ToLower()))
            {
                return Json(new { code = false, message = "文件格式不合法" });
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + "image/";
            if (!System.IO.Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fullpath = path + originalFileName.Replace(" ", "") + fileExtension;
            if (System.IO.File.Exists(fullpath))
            {
                System.IO.File.Delete(fullpath);
            }
            hpf.SaveAs(fullpath);
            tb_product.photo = "/image/" + originalFileName.Replace(" ", "") + fileExtension;
            if (ModelState.IsValid)
            {
                db.tb_products.Add(tb_product);
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            return View(tb_product);
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_product tb_product = db.tb_products.Find(id);
            if (tb_product == null)
            {
                return HttpNotFound();
            }
            return View(tb_product);
        }

        // POST: Product/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性。有关
        // 详细信息，请参阅 https://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "pid,pname,photo,price,pnums,salenums,mess,state")] tb_product tb_product)
        {
            HttpPostedFile hpf = null;
            try
            {
                hpf = System.Web.HttpContext.Current.Request.Files[0];
            }
            catch (Exception)
            {
            }
            if (hpf.FileName != "")
            {
                string name = hpf.FileName;
                string originalFileName = "";        //原始文件名
                if (name.Contains("."))
                {
                    originalFileName =Path.GetFileNameWithoutExtension(hpf.FileName.ToString());
                }
                string allowExtension = ".jpg|.jpeg|.png|.doc|.docx|.pdf|.xls|.xlsx|.mp4";
                string fileExtension = Path.GetExtension(name);        //文件扩展名
                if (!allowExtension.Contains(fileExtension.ToLower()))
                {
                    return Json(new { code = false, message = "文件格式不合法" });
                }
                string path = AppDomain.CurrentDomain.BaseDirectory + "image/";
                if (!System.IO.Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fullpath = path + originalFileName.Replace(" ", "") + fileExtension;
                if (System.IO.File.Exists(fullpath))
                {
                    System.IO.File.Delete(fullpath);
                }
                hpf.SaveAs(fullpath);
                tb_product.photo = "/image/" + originalFileName.Replace(" ", "") + fileExtension;
            }
            else
            {
                var productObj = db.tb_products.AsNoTracking().FirstOrDefault(a => a.
pid == tb_product.pid);
                tb_product.photo = productObj.photo;
            }
            if (ModelState.IsValid)
            {
                db.Entry(tb_product).State = EntityState.Modified;
                db.SaveChanges();
                return Content("<script>alert('保存成功!');window.location.href='/Product/Index';</script>");
            }
            return View(tb_product);
        }

        // GET: 按id显示某一待删除商品信息
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tb_product tb_product = db.tb_products.Find(id);
            if (tb_product == null)
            {
                return HttpNotFound();
            }
            return View(tb_product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tb_product tb_product = db.tb_products.Find(id);
            db.tb_products.Remove(tb_product);
            db.SaveChanges();
            return RedirectToAction("Details");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        public ActionResult SaveImage(string filePath)
        {
            HttpPostedFile hpf = System.Web.HttpContext.Current.Request.Files[0];
            string name = hpf.FileName;
            string originalFileName = "";        //原始文件名
            if (name.Contains("."))
            {
                originalFileName =Path.GetFileNameWithoutExtension(hpf.FileName.ToString());
            }
            string currentFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            //新文件名
            string allowExtension = ".jpg|.jpeg|.png|.doc|.docx|.pdf|.xls|.xlsx|.mp4";
            string fileExtension = Path.GetExtension(name);        //文件扩展名
            if (!allowExtension.Contains(fileExtension.ToLower()))
            {
                return Json(new { code = false, message = "文件格式不合法" });
            }
            string path = AppDomain.CurrentDomain.BaseDirectory + filePath;
            if (!System.IO.Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fullpath = path + currentFileName + fileExtension;
            if (System.IO.File.Exists(fullpath))
            {
                System.IO.File.Delete(fullpath);
            }
            hpf.SaveAs(fullpath);
            return Json(new { code = true, message = "上传成功" });
        }

        // AI推荐功能
        [HttpPost]
        public async Task<ActionResult> AIRecommend(string userInput)
        {
            try
            {
                // 获取所有商品
                var products = db.tb_products.ToList();

                if (products == null || products.Count == 0)
                {
                    return Json(new { success = false, message = "暂无商品数据" });
                }

                // 获取商品名称列表，用于推荐
                var productNames = string.Join("、", products.Select(p => p.pname));

                // 构建Prompt - 真正使用用户输入
                // 要求输出格式：回答问题 + 推荐商品（必须是数据库中存在的）+ 搜索关键词
                var prompt = $@"你是一个化妆品店客服顾问。

用户问题：""{userInput}""

店里有以下商品：{productNames}

请按以下格式回复（每行前面不要加序号或符号）：
您感觉{userInput}，说明皮肤需要补水保湿。
为您推荐：XXX（商品全名，必须是上面列表中存在的）。
使用后效果描述（一句话）。
建议搜索关键词：YYY（商品名称中的2-4个字，用于搜索）

注意：
1. 只推荐列表中有的商品，不要编造！
2. 搜索关键词必须是商品名称中的2-4个字，比如商品是'玻尿酸补水精华液'，关键词可以是'精华液'或'补水'";

                // DeepSeek API配置
                var apiKey = ""; // 请在这里填入你的DeepSeek API Key，或使用环境变量
                var model = "deepseek-chat";

                var requestBody = new
                {
                    model = model,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    max_tokens = 256
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(60);
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                    // DeepSeek API 端点
                    var response = await client.PostAsync("https://api.deepseek.com/v1/chat/completions", httpContent);

                    var responseContent = await response.Content.ReadAsStringAsync();

                    // 检查API响应状态
                    if (!response.IsSuccessStatusCode)
                    {
                        return Json(new { success = false, message = "API请求失败: " + response.StatusCode, debug = responseContent });
                    }

                    // 解析API响应
                    var apiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    string aiReply = "";

                    // 从 choices[0].message.content 获取回复
                    if (apiResponse.choices != null && apiResponse.choices.Count > 0)
                    {
                        aiReply = apiResponse.choices[0].message.content.ToString();
                    }

                    if (string.IsNullOrEmpty(aiReply))
                    {
                        return Json(new { success = false, message = "AI返回内容为空", debug = responseContent });
                    }

                    // 提取推荐商品和关键词
                    string recommendProduct = "";
                    string keyword = "";
                    var lines = aiReply.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();

                    foreach (var line in lines)
                    {
                        // 提取推荐商品
                        if (line.Contains("为您推荐："))
                        {
                            recommendProduct = line.Replace("为您推荐：", "").Replace("为您推荐:", "").Trim();
                        }
                        // 提取搜索关键词
                        if (line.Contains("建议搜索关键词：") || line.Contains("建议搜索关键词:"))
                        {
                            keyword = line.Replace("建议搜索关键词：", "").Replace("建议搜索关键词:", "").Trim();
                        }
                    }

                    // 验证推荐商品：确保是数据库中存在的商品
                    string validProduct = "";
                    string validKeyword = "";
                    if (!string.IsNullOrEmpty(recommendProduct))
                    {
                        foreach (var p in products)
                        {
                            // 完全匹配或包含关系
                            if (p.pname == recommendProduct || recommendProduct.Contains(p.pname) || p.pname.Contains(recommendProduct))
                            {
                                validProduct = p.pname;
                                // 从商品名称中提取2-4个字作为搜索关键词
                                if (!string.IsNullOrEmpty(keyword))
                                {
                                    validKeyword = keyword;
                                }
                                else
                                {
                                    // 如果没有提取到关键词，从商品名称中取
                                    if (p.pname.Length >= 4)
                                        validKeyword = p.pname.Substring(2, 2);
                                    else
                                        validKeyword = p.pname;
                                }
                                break;
                            }
                        }
                    }

                    // 如果推荐商品验证失败，尝试从关键词反向匹配
                    if (string.IsNullOrEmpty(validProduct) && !string.IsNullOrEmpty(keyword))
                    {
                        foreach (var p in products)
                        {
                            if (p.pname.Contains(keyword))
                            {
                                validProduct = p.pname;
                                validKeyword = keyword;
                                break;
                            }
                        }
                    }

                    // 如果还是没找到，使用关键词搜索
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        validKeyword = keyword;
                    }

                    return Json(new { success = true, reply = aiReply.Trim(), keyword = validKeyword });
                }
            }
            catch (Exception ex)
            {
                // 返回详细错误信息
                string errorDetail = ex.Message;
                if (ex.InnerException != null)
                {
                    errorDetail += " | " + ex.InnerException.Message;
                }
                return Json(new { success = false, message = "AI推荐出错: " + errorDetail });
            }
        }
    }
}
