﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Mvc;
using FormFactory.Attributes;
using FormFactory.Example.Models;

namespace FormFactory.Example.Controllers
{
    public class UploadedFilesController : Controller
    {
        static readonly MemoryCache Store = new MemoryCache("UploadedFilesStore");
        internal static string UploadFile(HttpPostedFileBase file)
        {
            var filepath = Guid.NewGuid().ToString().Replace("-", "") +
                           "\\" + Path.GetFileName(file.FileName);
            //var storePath = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(),
            //                    "UploadedFiles\\", filepath);
            //Directory.CreateDirectory(Path.GetDirectoryName(storePath));
            //file.SaveAs(storePath);
            Store.Add(filepath, file, DateTimeOffset.Now.AddSeconds(10));
            return "/UploadedFiles?path=" + filepath;
        }

        [HttpGet]
        public ActionResult Index(string path)
        {
            if (Store.Contains(path) && Store[path] is HttpPostedFileBase)
            {
                var file = (HttpPostedFileBase) Store[path];
                // NOTE: this ContentType could be wrong; demo purposes only.
                return File(file.InputStream, "image/" + file.FileName.Split('.').Last(), file.FileName);
            }
            return Content("File has expired from cache");
        }


        [HttpGet]
        public ActionResult UploadTest()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadTest([FormModel] UploadedFilesTestModel model)
        {
            if (ModelState.IsValid)
            {
                var results = new UploadedFilesResultModel
                                  {
                                      Image1Url = model.Image1 != null ? model.Image1.Url : null,
                                      Image2Url = model.Image2 != null ? model.Image2.Url : null
                                  };
                return View(results);
            }
            return View();
        }
    }
}
