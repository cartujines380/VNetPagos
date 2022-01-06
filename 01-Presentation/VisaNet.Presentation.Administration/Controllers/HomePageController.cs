using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Filters.Web;
using VisaNet.Presentation.Administration.Mappers;
using VisaNet.Presentation.Administration.Models;
using VisaNet.Presentation.Core;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Administration;
using VisaNet.Utilities.Notifications;

namespace VisaNet.Presentation.Administration.Controllers
{
    public class HomePageController : BaseController
    {
        private readonly IHomePageClientService _homePageClientService;
        private readonly IHomePageItemClientService _homePageItemClientService;

        public HomePageController(IHomePageClientService homePageClientService, IHomePageItemClientService homePageItemClientService)
        {
            _homePageClientService = homePageClientService;
            _homePageItemClientService = homePageItemClientService;
        }

        [CustomAuthentication(Actions.HomePageDetails)]
        public async Task<ActionResult> Index()
        {
            var homePages = await _homePageClientService.FindAll();

            var homePage = homePages.First();

            return View(homePage.ToModel());
        }

        [CustomAuthentication(Actions.HomePageDetails)]
        public async Task<ActionResult> Details(Guid id)
        {
            var homePageItem = await _homePageItemClientService.Find(id);

            return View(homePageItem.ToModel());
        }

        [CustomAuthentication(Actions.HomePageEdit)]
        public async Task<ActionResult> Edit(Guid id)
        {
            try
            {
                var homePageItem = await _homePageItemClientService.Find(id);

                return View(homePageItem.ToModel());
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [CustomAuthentication(Actions.HomePageEdit)]
        public async Task<ActionResult> Edit(HomePageItemModel homePageItem)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(homePageItem);
                }

                bool imagechanged = false;
                bool filechanged = false;

                //si quiso borrar la imagen
                if (homePageItem.DeleteImage)
                {
                    homePageItem.Image = null;
                    imagechanged = true;
                }

                if (homePageItem.DeleteFile)
                {
                    homePageItem.File = null;
                    filechanged = true;
                }

                //si hay imagenes nuevas
                var image = Request.Files["Image"];
                if (image != null && image.ContentLength != 0)
                {
                    if (!IsImage(image))
                    {
                        ShowNotification(PresentationAdminStrings.Image_wrong_type, NotificationType.Error);
                        return View(homePageItem);
                    }
                    if (!IsSizeCorrect(image))
                    {
                        ShowNotification(PresentationAdminStrings.Image_wrong_size, NotificationType.Error);
                        return View(homePageItem);
                    }

                    var imageP = Guid.NewGuid() + "_";
                    var imageFilename = Path.GetFileName(imageP + image.FileName);

                    homePageItem.Image_originalname = image.FileName;
                    homePageItem.Image_internalname = imageP + image.FileName;
                    homePageItem.Image = imageFilename;
                    imagechanged = true;
                    image.SaveAs(Path.Combine(ConfigurationManager.AppSettings["SharedImagesFolder"], imageFilename));
                }

                var file = Request.Files["File"];
                if (file != null && file.ContentLength != 0)
                {

                    if (!IsFile(file))
                    {
                        ShowNotification(PresentationAdminStrings.File_wrong_type, NotificationType.Error);
                        return View(homePageItem);
                    }

                    if (!IsSizeCorrect(file))
                    {
                        ShowNotification(PresentationAdminStrings.File_wrong_size, NotificationType.Error);
                        return View(homePageItem);
                    }

                    var fileP = Guid.NewGuid() + "_";
                    var fileFilename = Path.GetFileName(fileP + file.FileName);

                    homePageItem.File_originalname = file.FileName;
                    homePageItem.File_internalname = fileP + file.FileName;
                    homePageItem.File = fileFilename;
                    filechanged = true;
                    file.SaveAs(Path.Combine(ConfigurationManager.AppSettings["SharedImagesFolder"], fileFilename));
                }

                await _homePageItemClientService.Edit(homePageItem.ToDto());
                ShowNotification(PresentationCoreMessages.NotificationSuccess, NotificationType.Success);

                //si se elimino o modifico imagenes viejas, borrar fisicamente
                if (imagechanged && !String.IsNullOrEmpty(homePageItem.ImagenBD))
                {
                    var path = Path.Combine(ConfigurationManager.AppSettings["SharedImagesFolder"], homePageItem.ImagenBD);
                    var fileInfo = new FileInfo(path);
                    if (fileInfo.Exists) fileInfo.Delete();
                }

                if (filechanged && !String.IsNullOrEmpty(homePageItem.FileBD))
                {
                    var path = Path.Combine(ConfigurationManager.AppSettings["SharedImagesFolder"], homePageItem.FileBD);
                    var fileInfo = new FileInfo(path);
                    if (fileInfo.Exists) fileInfo.Delete();
                }

                return RedirectToAction("Index");
            }
            catch (WebApiClientBusinessException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }
            catch (WebApiClientFatalException e)
            {
                ShowNotification(e.Message, NotificationType.Error);
            }

            return View(homePageItem);
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            var formats = new[] { ".jpg", ".png", ".gif", ".jpeg" };
            return file.ContentType.Contains("image") && formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsFile(HttpPostedFileBase file)
        {
            var formats = new [] { ".pdf" };
            return file.ContentType.Contains("pdf") && formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsSizeCorrect(HttpPostedFileBase file)
        {
            int byteCount = file.ContentLength;
            return byteCount < 5242880; //5MB
        }
    }
}
