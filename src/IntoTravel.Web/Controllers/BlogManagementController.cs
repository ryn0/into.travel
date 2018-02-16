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
using IntoTravel.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

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
        private readonly IImageUploaderService _imageUploaderService;
        private readonly IMemoryCache _memoryCache;


        public BlogManagementController(
            IBlogEntryPhotoRepository blogEntryPhotoRepository,
            IBlogEntryTagRepository blogEntryTagRepository,
            ITagRepository tagRepository,
            IBlogEntryRepository blogEntryRepository, 
            ISiteFilesRepository siteFilesRepository,
            IImageUploaderService imageUploaderService,
            IMemoryCache memoryCache)
        {
            _blogEntryPhotoRepository = blogEntryPhotoRepository;
            _blogEntryTagRepository = blogEntryTagRepository;
            _tagRepository = tagRepository;
            _blogEntryRepository = blogEntryRepository;
            _siteFilesRepository = siteFilesRepository;
            _imageUploaderService = imageUploaderService;
            _memoryCache = memoryCache;
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

            //if (pageNumber == 0)
            //{
            //        ResizeAll();
            //}

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
            await DeleteBlogPhoto(blogEntryPhotoId);

            var allBlogPhotos = _blogEntryPhotoRepository.GetBlogPhotos(entry.BlogEntryId)
                                                        .Where(x => x.BlogEntryPhotoId != blogEntryPhotoId)
                                                        .OrderBy(x => x.Rank);

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
                        var fullsizePhotoUrl = await _siteFilesRepository.UploadAsync(file, folderPath);
                        var thumbnailPhotoUrl = await _imageUploaderService.UploadReducedQualityImage(folderPath, fullsizePhotoUrl, 300, 200, "_thumb");
                        var fullScreenPhotoUrl = await _imageUploaderService.UploadReducedQualityImage(folderPath, fullsizePhotoUrl, 1600, 1200, "_fullscreen");
                        var previewPhotoUrl = await  _imageUploaderService.UploadReducedQualityImage(folderPath, fullsizePhotoUrl, 800, 600, "_preview");
                       
                        var existingPhoto = allBlogPhotos.FirstOrDefault(x => x.PhotoUrl == fullsizePhotoUrl.ToString());

                        if (existingPhoto == null)
                        {
                            _blogEntryPhotoRepository.Create(new BlogEntryPhoto()
                            {
                                BlogEntryId = blogEntryId,
                                PhotoUrl = fullsizePhotoUrl.ToString(),
                                PhotoThumbUrl = thumbnailPhotoUrl.ToString(),
                                PhotoFullScreenUrl = fullScreenPhotoUrl.ToString(),
                                PhotoPreviewUrl = previewPhotoUrl.ToString(),
                                Rank = currentRank + 1
                            });

                            currentRank++;
                        }
                        else
                        {
                            existingPhoto.PhotoUrl = fullsizePhotoUrl.ToString();
                            existingPhoto.PhotoThumbUrl = thumbnailPhotoUrl.ToString();
                            existingPhoto.PhotoFullScreenUrl = fullScreenPhotoUrl.ToString();
                            existingPhoto.PhotoPreviewUrl = previewPhotoUrl.ToString();
                            _blogEntryPhotoRepository.Update(existingPhoto);
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
            await RotateImage(entry.BlogEntryId, entry.PhotoUrl);
            await RotateImage(entry.BlogEntryId, entry.PhotoPreviewUrl);
            await RotateImage(entry.BlogEntryId, entry.PhotoThumbUrl);
            await RotateImage(entry.BlogEntryId, entry.PhotoFullScreenUrl);

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

                _memoryCache.Remove($"{dbModel.BlogPublishDateTimeUtc.Year}/{dbModel.BlogPublishDateTimeUtc.Month}/{dbModel.BlogPublishDateTimeUtc.Day}/{dbModel.Key}");

                return RedirectToAction("Index");
            }

            return View(model);
        }


        [Route("blogmanagement/preview/{key}")]
        [HttpGet]
        public IActionResult Preview(string key)
        {
            var model = _blogEntryRepository.Get(key);

            return View("DisplayBlog", ModelConverter.ConvertToBlogDisplayModel(model, null, null));
        }


        private async Task<BlogEntryPhoto> DeleteBlogPhoto(int blogEntryPhotoId)
        {
            var entry = _blogEntryPhotoRepository.Get(blogEntryPhotoId);

            await _siteFilesRepository.DeleteFileAsync(entry.PhotoUrl);
            await _siteFilesRepository.DeleteFileAsync(entry.PhotoThumbUrl);
            await _siteFilesRepository.DeleteFileAsync(entry.PhotoFullScreenUrl);
            await _siteFilesRepository.DeleteFileAsync(entry.PhotoPreviewUrl);

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
                    PhotoThumbUrl = photo.PhotoThumbUrl,
                    Title = photo.Title,
                    Description = photo.Description
                });
            }

            foreach (var tagItem in dbModel.BlogEntryTags.OrderBy(x => x.Tag.Name))
            {
                model.BlogTags.Add(tagItem.Tag.Name);
            }

            model.BlogTags = model.BlogTags.OrderBy(x => x).ToList();

            model.Tags = string.Join(", ", model.BlogTags);

            return model;
        }

        private void SetBlogTags(BlogManagementEditModel model, BlogEntry dbModel)
        {
            if (model.Tags == null)
                return;

            var currentTags = model.Tags.Split(',');
            var currentTagsFormatted = new ArrayList();
            foreach (var tag in currentTags)
            {
                currentTagsFormatted.Add(tag.UrlKey());
            }

            var currentTagsFormattedArray = currentTagsFormatted.ToArray();

            var previousTags = new ArrayList();
            foreach (var tag in dbModel.BlogEntryTags)
            {
                previousTags.Add(tag.Tag.Key);
            }

            var tagsToRemove = previousTags.ToArray().Except(currentTagsFormatted.ToArray());

            AddNewTags(model, dbModel, currentTags);

            RemoveDeletedTags(model, tagsToRemove);
        }

        private void RemoveDeletedTags(BlogManagementEditModel model, IEnumerable<object> tagsToRemove)
        {
            foreach (var tag in tagsToRemove)
            {
                var tagKey = tag.ToString().UrlKey();

                var tagDb = _tagRepository.Get(tagKey);

                _blogEntryTagRepository.Delete(tagDb.TagId, model.BlogEntryId);
            }
        }


        private string GetBlogPhotoFolder(int blogEntryId)
        {
            return string.Format("/blogphotos/{0}/", blogEntryId);
        }

        private async Task RotateImage(int blogEntryId, string photoUrl)
        {
            var folderPath = GetBlogPhotoFolder(blogEntryId);
            var stream = await _imageUploaderService.ToStreamAsync(photoUrl);
            var imageHelper = new ImageUtilities();
            const float angle = 90;
            var rotatedBitmap = imageHelper.RotateImage(Image.FromStream(stream), angle);

            Image fullPhoto = rotatedBitmap;

            var streamRotated = _imageUploaderService.ToAStream(fullPhoto, _imageUploaderService.SetImageFormat(photoUrl));

            await _siteFilesRepository.UploadAsync(
                                        streamRotated,
                                        photoUrl.GetFileNameFromUrl(),
                                        folderPath);

            fullPhoto.Dispose();
            streamRotated.Dispose();
            rotatedBitmap.Dispose();
        }

        private void ResizeAll()
        {
            int total;
            
                var all = _blogEntryRepository.GetPage(1, int.MaxValue, out total);

            foreach (var blog in all)
            {
                var folderPath = GetBlogPhotoFolder(blog.BlogEntryId);
                var allBlogPhotos = _blogEntryPhotoRepository.GetBlogPhotos(blog.BlogEntryId);

                foreach (var blogPhoto in allBlogPhotos)
                {
                    try
                    {
                        var thumbnailPhotoUrl = Task.Run(() =>
                           _imageUploaderService.UploadReducedQualityImage(folderPath, new Uri(blogPhoto.PhotoUrl), 300, 200, "_thumb")).Result;
                        blogPhoto.PhotoThumbUrl = thumbnailPhotoUrl.ToString();

                        var previewlPhotoUrl = Task.Run(() =>
                            _imageUploaderService.UploadReducedQualityImage(folderPath, new Uri(blogPhoto.PhotoUrl), 800, 600, "_preview")).Result;
                        blogPhoto.PhotoPreviewUrl = previewlPhotoUrl.ToString();

                        _blogEntryPhotoRepository.Update(blogPhoto);
                    }
                    catch (Exception ex)
                    {
                        var x = ex.ToString();
                    }
                }
            }
            
        }


        private void AddNewTags(BlogManagementEditModel model, BlogEntry dbModel, string[] currentTags)
        {
            foreach (var tagName in currentTags)
            {
                var tagKey = tagName.UrlKey();

                if (string.IsNullOrWhiteSpace(tagKey))
                    continue;

                if (dbModel.BlogEntryTags.FirstOrDefault(x => x.Tag.Key == tagKey) == null)
                {
                    var tagDb = _tagRepository.Get(tagKey);

                    if (tagDb == null || tagDb.TagId == 0)
                    {
                        _tagRepository.Create(new Tag
                        {
                            Name = tagName.Trim(),
                            Key = tagKey
                        });

                        tagDb = _tagRepository.Get(tagKey);
                    }

                    _blogEntryTagRepository.Create(new BlogEntryTag()
                    {
                        BlogEntryId = model.BlogEntryId,
                        TagId = tagDb.TagId,
                    });
                }
            }
        }

    }
}
