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
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using IntoTravel.Web.Helpers;
using System.Collections;

namespace IntoTravel.Web.Controllers
{
    [Authorize]
    public class BlogManagementController : Controller
    {
        const int AmountPerPage = 10;
        private readonly IBlogEntryPhotoRepository _blogEntryPhotoRepository;
        private readonly IBlogEntryTagRepository _blogEntryTagRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IBlogEntryRepository _blogEntryRepository;
        private readonly ISiteFilesRepository _siteFilesRepository;

        public BlogManagementController(
            IBlogEntryPhotoRepository blogEntryPhotoRepository,
            IBlogEntryTagRepository blogEntryTagRepository,
            ITagRepository tagRepository,
            IBlogEntryRepository blogEntryRepository, 
            ISiteFilesRepository siteFilesRepository)
        {
            _blogEntryPhotoRepository = blogEntryPhotoRepository;
            _blogEntryTagRepository = blogEntryTagRepository;
            _tagRepository = tagRepository;
            _blogEntryRepository = blogEntryRepository;
            _siteFilesRepository = siteFilesRepository;
        }

        public IActionResult Index(int pageNumber = 1)
        {
            int total;

            var blogEntries = _blogEntryRepository.GetPage(pageNumber, AmountPerPage, out total);

            var model = ConvertToListModel(blogEntries);
            model.Total = total;
            model.CurrentPageNumber = pageNumber;
            model.QuantityPerPage = AmountPerPage;
            var pageCount = (double)model.Total / model.QuantityPerPage;
            model.PageCount = (int)Math.Ceiling(pageCount);

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

        [HttpGet]
        public IActionResult SetDefaultPhoto(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);

            _blogEntryPhotoRepository.SetDefaultPhoto(blogEntryPhotoId);

            return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });
        }


        [HttpGet]
        public async Task<IActionResult> DeleteBlogPhotoAsync(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);

            var allBlogPhotos = _blogEntryPhotoRepository.GetBlogPhotos(entry.BlogEntryId)
                                                         .Where(x => x.BlogEntryPhotoId != blogEntryPhotoId)
                                                         .OrderBy(x => x.Rank);

            await DeleteBlogPhoto(blogEntryPhotoId);
            int newRank = 1;

            foreach(var photo in allBlogPhotos)
            {
                photo.Rank = newRank;
                _blogEntryPhotoRepository.Update(photo);

                newRank++;
            }

            return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });
        }

        [HttpGet]
        public  IActionResult RankPhotoUp(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);

            if (entry.Rank == 1)
                return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId});

            var allBlogPhotos = _blogEntryPhotoRepository.GetBlogPhotos(entry.BlogEntryId);

            var rankedHigher = allBlogPhotos.First(x => x.Rank == entry.Rank - 1);
            var higherRankValue = rankedHigher.Rank;
            rankedHigher.Rank = higherRankValue + 1;
            _blogEntryPhotoRepository.Update(rankedHigher);

            entry.Rank = higherRankValue;
            _blogEntryPhotoRepository.Update(entry);

            return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });
        }

        [HttpGet]
        public IActionResult RankPhotoDown(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);
            var allBlogPhotos = _blogEntryPhotoRepository.GetBlogPhotos(entry.BlogEntryId);

            if (entry.Rank == allBlogPhotos.Count())
                return RedirectToAction("Edit", new { blogEntryId = entry.BlogEntryId });

            var rankedLower = allBlogPhotos.First(x => x.Rank == entry.Rank + 1);
            var lowerRankValue = rankedLower.Rank;
            rankedLower.Rank = lowerRankValue - 1;
            _blogEntryPhotoRepository.Update(rankedLower);

            entry.Rank = lowerRankValue;
            _blogEntryPhotoRepository.Update(entry);

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
            var allBlogPhotos = _blogEntryPhotoRepository.GetBlogPhotos(blogEntryId);
            var highestRank = allBlogPhotos.Count();
            int currentRank = highestRank;

            try
            {
                var folderPath = GetBlogPhotoFolder(blogEntryId);

                foreach (var file in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var photoUrl = await _siteFilesRepository.UploadAsync(file, folderPath);

                        if (allBlogPhotos.FirstOrDefault(x => x.PhotoUrl == photoUrl.ToString()) == null)
                        {
                            _blogEntryPhotoRepository.Create(new BlogEntryPhoto()
                            {
                                BlogEntryId = blogEntryId,
                                PhotoUrl = photoUrl.ToString(),
                                Rank = currentRank + 1
                            });

                            currentRank++;
                        }
                    }
                }

                return RedirectToAction("Edit", new { blogEntryId = blogEntryId });
            }
            catch (Exception ex)
            {
                throw new Exception("Upload failed", ex.InnerException);
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
        public async Task<IActionResult> DeleteAsync(int blogEntryId)
        {
            var blogEntry = _blogEntryRepository.Get(blogEntryId);

            foreach (var photo in blogEntry.Photos)
            {
                await DeleteBlogPhoto(photo.BlogEntryPhotoId);
            }

            var task = Task.Run(() => _blogEntryRepository.Delete(blogEntryId));
            var myOutput = await task; 

            return RedirectToAction("Index");
        }



        [HttpGet]
        public async Task<IActionResult> Rotate90DegreesAsync(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);
            var folderPath = GetBlogPhotoFolder(entry.BlogEntryId);
            var stream = await ToStreamAsync(entry.PhotoUrl);
            var imageHelper = new ImageUtilities();
            const float angle = 90;
            var rotatedBitmap = imageHelper.RotateImage(Image.FromStream(stream), angle);
 
            Image fullPhoto = rotatedBitmap;

            var streamRotated = ToAStream(fullPhoto, SetImageFormat(entry.PhotoUrl));

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
                return View(model);

            var dbModel = ConvertToDbModel(model);

            if (_blogEntryRepository.Update(dbModel))
            {
                var allPhotos = _blogEntryPhotoRepository.GetBlogPhotos(model.BlogEntryId);

                foreach (var photo in allPhotos)
                {
                    photo.Title = Request.Form["PhotoTitle_" + photo.BlogEntryPhotoId];
                    photo.Description = Request.Form["PhotoDescription_" + photo.BlogEntryPhotoId];

                    _blogEntryPhotoRepository.Update(photo);
                }

                SetBlogTags(model, dbModel);

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

        private async Task<BlogEntryPhoto> DeleteBlogPhoto(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);

            await _siteFilesRepository.DeleteFileAsync(entry.PhotoUrl);
            _blogEntryPhotoRepository.Delete(blogEntryPhotoId);

            return entry;
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
            dbModel.MetaDescription = (model.MetaDescription != null) ? model.MetaDescription.Trim() : string.Empty;

            return dbModel;
        }

        private BlogManagementEditModel ToUiEditModel(BlogEntry dbModel)
        {
            var model = new BlogManagementEditModel()
            {
                Content = dbModel.Content,
                Title = dbModel.Title,
                BlogEntryId = dbModel.BlogEntryId,
                BlogPublishDateTimeUtc = dbModel.BlogPublishDateTimeUtc,
                IsLive = dbModel.IsLive,
                LiveUrlPath = UrlBuilder.BlogUrlPath(dbModel.Key, dbModel.BlogPublishDateTimeUtc),
                PreviewUrlPath = UrlBuilder.BlogPreviewUrlPath(dbModel.Key),
                MetaDescription = dbModel.MetaDescription
            };

            foreach (var photo in dbModel.Photos.OrderBy(x => x.Rank))
            {
                model.BlogPhotos.Add(new BlogPhotoModel
                {
                    BlogEntryPhotoId = photo.BlogEntryPhotoId,
                    IsDefault = photo.IsDefault,
                    PhotoUrl = photo.PhotoUrl,
                    Title = photo.Title,
                    Description = photo.Description
                });
            }

            foreach (var tagItem in dbModel.BlogEntryTags.OrderBy(x => x.Tag.Name))
            {
                model.BlogTags.Add(tagItem.Tag.Name);
            }
 
            model.Tags = string.Join(", ", model.BlogTags);

            return model;
        }

        public Stream ToAStream(Image image, ImageFormat formaw)
        {
            var stream = new MemoryStream();
            image.Save(stream, formaw);
            stream.Position = 0;
            return stream;
        }


        private void SetBlogTags(BlogManagementEditModel model, BlogEntry dbModel)
        {
            if (model.Tags == null)
                return;

            var currentTags = model.Tags.Split(',');
            var currentTagsFormatted = new ArrayList();
            foreach(var tag in currentTags)
            {
                currentTagsFormatted.Add(tag.Replace("-", " ").Trim());
            }

            var previousTags = new ArrayList();
            foreach (var tag in dbModel.BlogEntryTags)
            {
                previousTags.Add(tag.Tag.Name);
            }

            var tagsToAdd = currentTags.Except(previousTags.ToArray());
            var tagsToRemove = previousTags.ToArray().Except(currentTags);

            foreach (var tag in tagsToAdd)
            {
                var tagName = tag.ToString().Trim();

                if (string.IsNullOrWhiteSpace(tagName))
                    continue;

                if (dbModel.BlogEntryTags.FirstOrDefault(x => x.Tag.Name == tagName) == null)
                {
                    var tagDb = _tagRepository.Get(tagName);

                    if (tagDb == null || tagDb.TagId == 0)
                    {
                        _tagRepository.Create(new Tag
                        {
                            Name = tagName
                        });

                        tagDb = _tagRepository.Get(tagName);
                    }

                    _blogEntryTagRepository.Create(new BlogEntryTag()
                    {
                        BlogEntryId = model.BlogEntryId,
                        TagId = tagDb.TagId,
                    });
                }
            }

            foreach (var tag in tagsToRemove)
            {
                var tagName = tag.ToString().Trim();

                var tagDb = _tagRepository.Get(tagName);

                _blogEntryTagRepository.Delete(tagDb.TagId, model.BlogEntryId);
            }
        }

        private ImageFormat SetImageFormat(string photoUrl)
        {
            var extension = photoUrl.GetFileExtension();

            switch(extension)
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }
    }
}
