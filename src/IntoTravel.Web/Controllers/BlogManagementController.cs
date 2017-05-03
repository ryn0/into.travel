using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IntoTravel.Web.Models;
using IntoTravel.Data.Repositories.Interfaces;
using IntoTravel.Data.Models;
using System;
using System.Collections.Generic;
using IntoTravel.Core.Utilities;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace IntoTravel.Web.Controllers
{
    [Authorize]
    public class BlogManagementController : Controller
    {
        const int AmountPerPage = 10;
        private readonly IBlogEntryPhotoRepository _blogEntryPhotoRepository;
        private readonly IBlogEntryRepository _blogEntryRepository;
        private readonly ISiteFilesRepository _siteFilesRepository;

        public BlogManagementController(
            IBlogEntryPhotoRepository blogEntryPhotoRepository,
            IBlogEntryRepository blogEntryRepository, 
            ISiteFilesRepository siteFilesRepository)
        {
            _blogEntryPhotoRepository = blogEntryPhotoRepository;
            _blogEntryRepository = blogEntryRepository;
            _siteFilesRepository = siteFilesRepository;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            int total;

            var blogEntries = _blogEntryRepository.GetPage(pageNumber, AmountPerPage, out total);

            var model = ConvertToListModel(blogEntries);

            return View(model);
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new BlogManagementCreateModel();

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(BlogManagementCreateModel model)
        {
            var entry = _blogEntryRepository.Create(new BlogEntry()
            {
                Title = model.Title,
                Key = model.Title.UrlKey(),
                BlogPublishDateTimeUtc = DateTime.UtcNow
            });

            if (entry.BlogEntryId > 0)
            {
                return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });
            }
            else
            {
                return View(entry);
            }
        }

        [HttpPost]
        public IActionResult SetDefaultPhoto(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);

            _blogEntryPhotoRepository.SetDefaultPhoto(blogEntryPhotoId);

            return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteBlogPhotoAsync(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);

            await _siteFilesRepository.DeleteFileAsync(entry.PhotoUrl);
            _blogEntryPhotoRepository.Delete(blogEntryPhotoId);

            return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });
        }

        [Route("blogmanagement/uploadphotos/{blogEntryId}")]
        [HttpGet]
        public IActionResult UploadPhotos(int blogEntryId)
        {
            var model = new BlogPhotoUploadModel()
            {
                BlogEntryId = blogEntryId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> UploadPhotosAsync(IEnumerable<IFormFile> files, int blogEntryId)
        {
            try
            {
                var folderPath = string.Format("/blogphotos/{0}/", blogEntryId);

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var photoUrl = await _siteFilesRepository.UploadAsync(file, folderPath);

                        var allBlogPhotos = _blogEntryPhotoRepository.GetBlogPhotos(blogEntryId);

                        if (allBlogPhotos.FirstOrDefault(x => x.PhotoUrl == photoUrl.ToString()) == null)
                        {
                            _blogEntryPhotoRepository.Create(new BlogEntryPhoto()
                            {
                                BlogEntryId = blogEntryId,
                                PhotoUrl = photoUrl.ToString()
                            });
                        }
                    }
                }

                return RedirectToAction("Edit", new { blogEntryId = blogEntryId });
            }
            catch
            {
                return RedirectToAction("Upload");
            }
        }

        [HttpGet]
        public IActionResult Edit(int blogEntryId)
        {
            var dbModel = _blogEntryRepository.Get(blogEntryId);

            var model = ToUiEditModel(dbModel);

            return View(model);
        }
       

        [HttpPost]
        public IActionResult Delete(int blogEntryId)
        {
            _blogEntryRepository.Delete(blogEntryId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Edit(BlogManagementEditModel model)
        {
            var dbModel = ConvertToDbModel(model);

            if (_blogEntryRepository.Update(dbModel))
            {
                return RedirectToAction("Index");
            }

            return View(model);
        }

        [Route("blogmanagement/preview/{key}")]
        [HttpGet]
        public IActionResult Preview(string key)
        {
            var model = _blogEntryRepository.Get(key);

            return View(IntoTravel.Web.Helpers.ModelConverter.Convert(model));
        }

 
        private BlogManagementListModel ConvertToListModel(List<BlogEntry> blogEntries)
        {
            var model = new BlogManagementListModel();

            foreach(var entry in blogEntries)
            {
                model.Items.Add(new BlogManagementEntryItemModel()
                {
                    BlogEntryId = entry.BlogEntryId,
                    CreateDate = entry.CreateDate,
                    Title = entry.Title,
                    IsLive = entry.IsLive,
                    Key = entry.Key
                });
            }

            return model;
        }


        private BlogEntry ConvertToDbModel(BlogManagementEditModel model)
        {
            var dbModel = _blogEntryRepository.Get(model.BlogEntryId);
            
            dbModel.BlogPublishDateTimeUtc = model.BlogPublishDateTimeUtc;
            dbModel.Content = model.Content;
            dbModel.Title = model.Title;
            dbModel.IsLive = model.IsLive;
            dbModel.Key = model.Title.UrlKey();

            return dbModel;
        }

        private BlogManagementEditModel ToUiEditModel(BlogEntry dbModel)
        {
            var  model = new BlogManagementEditModel()
            {
                Content = dbModel.Content,
                Title = dbModel.Title,
                BlogEntryId = dbModel.BlogEntryId,
                BlogPublishDateTimeUtc = dbModel.BlogPublishDateTimeUtc,
                IsLive = dbModel.IsLive
            };

            var photos = _blogEntryPhotoRepository.GetBlogPhotos(dbModel.BlogEntryId);

            foreach(var photo in photos)
            {
                model.BlogPhotos.Add(new BlogPhotoModel
                {
                    BlogEntryPhotoId = photo.BlogEntryPhotoId,
                    IsDefault = photo.IsDefault,
                    PhotoUrl = photo.PhotoUrl
                });
            }

            return model;
        }

    }
}
