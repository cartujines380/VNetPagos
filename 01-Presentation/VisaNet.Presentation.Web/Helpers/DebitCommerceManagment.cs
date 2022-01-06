using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using VisaNet.Presentation.Core.WebApi.Interfaces.Interfaces.Web;
using VisaNet.Presentation.Web.Areas.Debit.Mappers;
using VisaNet.Presentation.Web.Constants;
using VisaNet.Presentation.Web.Models;

namespace VisaNet.Presentation.Web.Helpers
{
    public class DebitCommerceManagment
    {
        private const string AppConstant = ApplicationConstants.APPLICATION_SELECTED_COMMERCES;
        private readonly HttpApplicationState _currentApplicationContext = System.Web.HttpContext.Current.Application;
        private readonly DateTime _now = DateTime.Today;
        private readonly IWebDebitClientService _webDebitClientService;

        public DebitCommerceManagment(IWebDebitClientService webDebitClientService)
        {
            _webDebitClientService = webDebitClientService;
        }
        
        public async Task<CommerceModel> GetCommerceFromCatche(Guid commerceId)
        {
            CommerceModel finalCommerce = null;
            var model = _currentApplicationContext[AppConstant] as DebitCommercesModel;

            if (model == null)
            {
                finalCommerce = await InitCatcheCommercesList(commerceId);
            }
            else
            {
                if (model.Date.Date != _now.Date)
                {
                    finalCommerce = await InitCatcheCommercesList(commerceId);
                }
                else
                {
                    finalCommerce = model.SelectedCommerces != null ? model.SelectedCommerces.FirstOrDefault(x => x.Id == commerceId) : null;
                    if (finalCommerce == null)
                    {
                        finalCommerce = await AddCatcheCommercesList(commerceId);
                    }
                }
            }
            return finalCommerce;
        }

        private async Task<IList<CommerceModel>> GetCommercesFromCore()
        {
            var list = await _webDebitClientService.GetCommercesDebit();
            var listModel = list != null ? list.Select(x => x.ToModel()).OrderBy(x => x.Name).ToList() : null;
            return listModel;
        }
        private async Task<CommerceModel> InitCatcheCommercesList(Guid commerceId)
        {
            var core = await GetCommercesFromCore();
            var commerce = core.FirstOrDefault(x => x.Id == commerceId);
                
            if (commerce == null) return null;

            _currentApplicationContext[AppConstant] = new DebitCommercesModel()
            {
                Date = DateTime.Now,
                SelectedCommerces = new List<CommerceModel>() { commerce}
            };
                
            return commerce;
        }
        private async Task<CommerceModel> AddCatcheCommercesList(Guid commerceId)
        {
            var core = await GetCommercesFromCore();
            var commerce = core.FirstOrDefault(x => x.Id == commerceId);

            if (commerce == null) return null;

            var model = _currentApplicationContext[AppConstant] as DebitCommercesModel;
            model.SelectedCommerces.Add(commerce);
            _currentApplicationContext[AppConstant] = model;

            return commerce;
        }
    }
}