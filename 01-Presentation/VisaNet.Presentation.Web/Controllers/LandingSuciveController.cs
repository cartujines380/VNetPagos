using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using VisaNet.Domain.Entities.Enums;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;
using VisaNet.Domain.EntitiesDtos.TableFilters;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Controllers
{
    public class LandingSuciveController : BaseController
    {

        private readonly IWebServiceClientService _webServiceClientService;

        public LandingSuciveController(IWebServiceClientService webServiceClientService)
        {
            _webServiceClientService = webServiceClientService;
        }

        public async Task<ViewResult> Index()
        {

            var services = new List<ServiceDto>()
                           {
                               new ServiceDto() {Id = Guid.Parse("794AC0EA-3466-4FC4-A1E3-3AA140C54255"),Name = "Artigas", },
                               new ServiceDto() {Id = Guid.Parse("148C4ACA-4497-43A7-85B4-DE5A413BCF62"),Name = "Canelones", },
                               new ServiceDto() {Id = Guid.Parse("49756485-1D15-4FD9-9DAB-5859B19CA951"),Name = "Cerro_Largo", },
                               new ServiceDto() {Id = Guid.Parse("393ED46B-2CEB-483D-B5D7-2A056B7B8D92"),Name = "Colonia", },
                               new ServiceDto() {Id = Guid.Parse("E55C9472-3FAE-4CD1-A057-59D45DAC2D53"),Name = "Durazno", },
                               new ServiceDto() {Id = Guid.Parse("2EF55962-D5CA-4D84-A6C5-C10DD4DE0770"),Name = "Flores", },
                               new ServiceDto() {Id = Guid.Parse("3C621552-B804-481C-B7A1-08869F555BB4"),Name = "Florida", },
                               new ServiceDto() {Id = Guid.Parse("DBB63529-F06A-41A5-8041-BF976EEF956E"),Name = "Lavalleja", },
                               new ServiceDto() {Id = Guid.Parse("1757F3AD-C40A-4A8A-AB0F-56ABD09516B9"),Name = "Maldonado", },
                               new ServiceDto() {Id = Guid.Parse("6A8FEA25-418B-4355-B049-BBEA9C67A47C"),Name = "Montevideo", },
                               new ServiceDto() {Id = Guid.Parse("650671D4-B79B-4060-AE6B-51E0EBB382CE"),Name = "Paysandu", },
                               new ServiceDto() {Id = Guid.Parse("6C7C7807-5127-4AC8-B0EB-EF50D285A140"),Name = "Rio_Negro", },
                               new ServiceDto() {Id = Guid.Parse("77CDF3CE-726D-49AD-82E4-AB7856950ED4"),Name = "Rivera", },
                               new ServiceDto() {Id = Guid.Parse("6EA5190A-253D-4944-BF1B-DD5BA93582B6"),Name = "Rocha", },
                               new ServiceDto() {Id = Guid.Parse("7172E5B0-C555-4F8E-92CF-203A1B3569ED"),Name = "San_Jose", },
                               new ServiceDto() {Id = Guid.Parse("3001CF71-0667-4BAA-B460-524A402A3824"),Name = "Soriano", },
                               new ServiceDto() {Id = Guid.Parse("8E95D1BE-741C-41E3-96AA-4AD425312564"),Name = "Tacuarembo", },
                           };

            var serviesModel = services.Select(x => new DeptoModel()
                                                    {
                                                        Name = x.Name,
                                                        Id = x.Id,
                                                        ImgName = x.Name.ToLower() + ".png",
                                                        Active = true,
                                                    }).ToList();

            serviesModel.Add(new DeptoModel()
                             {
                                 Name = "Salto",
                                 ImgName = "Salto" + ".png",
                                 Active = false
                             });
            serviesModel.Add(new DeptoModel()
            {
                Name = "Treinta y Tres",
                ImgName = "Treinta_y_tres" + ".png",
                Active = false
            });

            //tildes
            return View(new LandingModel()
                        {
                            States = serviesModel.OrderBy(x => x.Name).ToList()
                        });
        }
    }
}