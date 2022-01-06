using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VisaNet.Domain.EntitiesDtos;
using VisaNet.Domain.EntitiesDtos.Enums;

namespace VisaNet.Presentation.VisaNetOn.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class BaseController : Controller
    {
        protected string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        protected IDictionary<string, string> GenerateDictionary(NameValueCollection form)
        {
            var data = form.AllKeys.ToDictionary(key => key, key => form[key]);
            return data;
        }

        protected ServiceGatewayDto GetBestGateway(IEnumerable<ServiceGatewayDto> list)
        {
            var sublist = list.Where(g => g.Active).ToList();
            if (!sublist.Any()) return null;
            if (sublist.Count() == 1) return sublist.FirstOrDefault();

            return sublist.Any(g =>
                g.Gateway.Enum == (int)GatewayEnumDto.Banred) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Banred)
                : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sistarbanc)
                    : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Sucive)
                        : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Geocom) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Geocom)
                            : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Carretera) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Carretera)
                                : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.Apps) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.Apps)
                                    : sublist.Any(g => g.Gateway.Enum == (int)GatewayEnumDto.PagoLink) ? sublist.FirstOrDefault(g => g.Gateway.Enum == (int)GatewayEnumDto.PagoLink)
                                        : null;
        }

        protected void FillNameAndSurname(string modelName, string modelSurname, string nameTh, out string name, out string surname)
        {
            //Si el comercio no paso el nombre y apellido, se obtienen de nombre de la tarjeta
            name = !string.IsNullOrEmpty(modelName) ? modelName : nameTh.Split(' ')[0];
            surname = modelSurname;
            if (string.IsNullOrEmpty(surname))
            {
                if (name.Equals(nameTh.Split(' ')[0], StringComparison.InvariantCultureIgnoreCase) && nameTh.Length > name.Length)
                {
                    surname = nameTh.Substring(name.Length + 1);
                }
                else if (nameTh.Split(' ').Length > 1 && nameTh.Split(' ')[1] != string.Empty)
                {
                    surname = nameTh.Substring(nameTh.Split(' ')[0].Length + 1);
                }
                else
                {
                    surname = nameTh;
                }
            }
        }

    }
}