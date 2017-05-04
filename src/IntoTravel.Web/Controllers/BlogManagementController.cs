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
using Microsoft.AspNetCore.Http.Internal;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using IntoTravel.Web.Helpers;

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
            if (!ModelState.IsValid)
                throw new Exception();

            var title = model.Title.Trim();
            var entry = _blogEntryRepository.Create(new BlogEntry()
            {
                Title = title,
                Key = title.UrlKey(),
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
                var folderPath = GetBlogPhotoFolder(blogEntryId);

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

        private string GetBlogPhotoFolder(int blogEntryId)
        {
            return string.Format("/blogphotos/{0}/", blogEntryId);
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
        public async Task<IActionResult> Rotate90DegreesAsync(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);
            var folderPath = GetBlogPhotoFolder(entry.BlogEntryId);
            var stream = await ToStreamAsync(entry.PhotoUrl);
            var imageHelper = new ImageUtilities();
            const float angle = 90;
            var rotatedBitmap = imageHelper.RotateImage(Image.FromStream(stream), angle);
 
            Image fullPhoto = rotatedBitmap;

            var streamRotated = ToAStream(fullPhoto, ImageFormat.Jpeg);

            await _siteFilesRepository.UploadAsync(
                                        streamRotated, 
                                        entry.PhotoUrl.GetFileNameFromUrl(), 
                                        folderPath);


            fullPhoto.Dispose();
            streamRotated.Dispose();
            rotatedBitmap.Dispose();

            return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });
        }

        [HttpPost]
        public IActionResult Edit(BlogManagementEditModel model)
        {
            if (!ModelState.IsValid)
                throw new Exception();

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

            return View("DisplayBlog", ModelConverter.Convert(model));
        }


        private async Task<MemoryStream> ToStreamAsync(string imageUrl)
        {
            var request = System.Net.WebRequest.Create(imageUrl);
            var response = await request.GetResponseAsync();
            var responseStream = response.GetResponseStream();
            var ms = new MemoryStream();

            await responseStream.CopyToAsync(ms);
            
            ms.Seek(0, SeekOrigin.Begin);

            return ms;
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
                    Key = entry.Key,
                    LiveUrlPath = UrlBuilder.BlogUrlPath(entry.Key, entry.BlogPublishDateTimeUtc),
                    PreviewUrlPath = UrlBuilder.BlogPreviewUrlPath(entry.Key)
                });
            }

            return model;
        }


        private BlogEntry ConvertToDbModel(BlogManagementEditModel model)
        {
            var title = model.Title.Trim();

            var dbModel = _blogEntryRepository.Get(model.BlogEntryId);
            
            dbModel.BlogPublishDateTimeUtc = model.BlogPublishDateTimeUtc;
            dbModel.Content = model.Content;
            dbModel.Title = title;
            dbModel.IsLive = model.IsLive;
            dbModel.Key = title.UrlKey();

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
                IsLive = dbModel.IsLive,
            };
        
            foreach(var photo in dbModel.Photos)
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

        public Stream ToAStream(Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }
    }
}
