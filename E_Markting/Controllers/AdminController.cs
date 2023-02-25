using E_Markting.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace E_Markting.Controllers
{
    public class AdminController : Controller
    {
        private MarktingDBEntities db = new MarktingDBEntities();
        // GET: Admin
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult login(Admin avm)
        {
            Admin ad = db.Admins.Where(x => x.ad_UserName == avm.ad_UserName && x.ad_Password == avm.ad_Password).SingleOrDefault();
            if (ad != null)
            {

                Session["ad_id"] = ad.ad_Id.ToString();
                return RedirectToAction("Create");

            }
            else
            {
                ViewBag.error = "Invalid username or password";

            }

            return View();
        }

        public ActionResult Create()
        {
            if (Session["ad_id"] == null)
            {
               return RedirectToAction("login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create(Category cat,HttpPostedFileBase imgFile)
        {
            string path = uploadimgfile(imgFile);
            if(path.Equals("-1"))
            {
                ViewBag.error = "Image canot be uploaded.";
            }
            else
            {
                Category category = new Category();
                category.cat_Name=cat.cat_Name;
                category.catImage=path;
                cat.cat_Status = 1;
                category.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("ViewCategory");
            }
            return View();
        }
        public string uploadimgfile(HttpPostedFileBase file)
        {
            Random r = new Random();
            string path = "-1";
            int random = r.Next();
            if (file != null && file.ContentLength > 0)
            {
                string extension = Path.GetExtension(file.FileName);
                if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                {
                    try
                    {

                        path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                        file.SaveAs(path);
                        path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);

                        //    ViewBag.Message = "File uploaded successfully";
                    }
                    catch (Exception)
                    {
                        path = "-1";
                    }
                }
                else
                {
                    Response.Write("<script>alert('Only jpg ,jpeg or png formats are acceptable....'); </script>");
                }
            }

            else
            {
                Response.Write("<script>alert('Please select a file'); </script>");
                path = "-1";
            }
            return path;
        }

        public ActionResult ViewCategory(int? page)
        {
            int pageSize = 7,pageIndex=1;
            pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list=db.Categories.Where(x=>x.cat_Status == 1).OrderByDescending(x => x.cat_Id).ToList();
            IPagedList<Category> stu = list.ToPagedList(pageIndex, pageSize);
            return View(stu);

        }
    }
}