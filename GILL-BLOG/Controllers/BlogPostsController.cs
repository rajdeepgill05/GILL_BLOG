using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GILL_BLOG.Helpers;
using GILL_BLOG.Models;

namespace GILL_BLOG.Controllers
{
    public class BlogPostsController : Controller
    {
       
            private ApplicationDbContext db = new ApplicationDbContext();

            // GET: BlogPosts
            public ActionResult Index()
            {
                return View(db.Posts.ToList());
            }

            // GET: BlogPosts/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlogPost blogPost = db.Posts.Find(id);
                if (blogPost == null)
                {
                    return HttpNotFound();
                }
                return View(blogPost);
            }
            // GET: BlogPosts/Details/5
            public ActionResult DetailsSlug(string slug)
            {
                if (slug == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlogPost blogPost = db.Posts.Where(p => p.Slug == slug).FirstOrDefault();
                if (blogPost == null)
                {
                    return HttpNotFound();
                }
                return View("Details", blogPost);
            }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
            {
                return View();
            }

            // POST: BlogPosts/Create
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
            {
                if (ModelState.IsValid)
                {
                    var Slug = StringUtilities.URLFriendly(blogPost.Title);
                    if (String.IsNullOrWhiteSpace(Slug))
                    {
                        ModelState.AddModelError("Title", "Invalid title");
                        return View(blogPost);
                    }
                    if (db.Posts.Any(p => p.Slug == Slug))
                    {
                        ModelState.AddModelError("Title", "The title must be unique");
                        return View(blogPost);
                    }

                    blogPost.Slug = Slug;
                    blogPost.Created = DateTimeOffset.Now;
                    db.Posts.Add(blogPost);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            if (ImageUploadValidator.IsWebFriendlyImage(image))
            {
                var fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                blogPost.MediaURL = "/Uploads/" + fileName;
            }

            return View(blogPost);
            }


            // GET: BlogPosts/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlogPost blogPost = db.Posts.Find(id);
                if (blogPost == null)
                {
                    return HttpNotFound();
                }
                return View(blogPost);
            }

            // POST: BlogPosts/Edit/5
            // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Slug,Body,MediaUrl,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
                {
                    var blog = db.Posts.Where(p => p.Id == blogPost.Id).FirstOrDefault();

                    blog.Body = blogPost.Body;
                    blog.MediaURL = blogPost.MediaURL;
                    blog.Published = blogPost.Published;
                    blog.Slug = blogPost.Slug;
                    blog.Title = blogPost.Title;
                    blog.Updated = DateTime.Now;

                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            if (ImageUploadValidator.IsWebFriendlyImage(image))
            {
                var fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                blogPost.MediaURL = "/Uploads/" + fileName;
            }
            return View(blogPost);
            }


            // GET: BlogPosts/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BlogPost blogPost = db.Posts.Find(id);
                if (blogPost == null)
                {
                    return HttpNotFound();
                }
                return View(blogPost);
            }

            // POST: BlogPosts/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                BlogPost blogPost = db.Posts.Find(id);
                db.Posts.Remove(blogPost);
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
