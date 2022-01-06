using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using VisaNet.Common.Resource.Presentation;
using VisaNet.Common.Security.Entities;
using VisaNet.Common.Security.Entities.Enums;
using VisaNet.Common.Security.Entities.Resources;
using VisaNet.Presentation.Administration.Constants;
using Action = VisaNet.Common.Security.Entities.Action;


namespace VisaNet.Presentation.Administration.MvcHtmlHelpers
{
    public static class HtmlHelperExtensions
    {
        #region FunctionalityGroupActionLink
        public static MvcHtmlString FunctionalityGroupActionLink(this HtmlHelper helper, FunctionalitiesGroups functionalityGroup)
        {
            return FunctionalityGroupActionLink(helper, functionalityGroup, null, null, null, null, null);
        }

        public static MvcHtmlString FunctionalityGroupActionLink(this HtmlHelper helper,
                                                        FunctionalitiesGroups functionalityGroup,
                                                        string linkText)
        {
            return FunctionalityGroupActionLink(helper, functionalityGroup, linkText, null, null, null, null);
        }

        public static MvcHtmlString FunctionalityGroupActionLink(this HtmlHelper helper,
                                                        FunctionalitiesGroups functionalityGroup,
                                                        string linkText,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return FunctionalityGroupActionLink(helper, functionalityGroup, linkText, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString FunctionalityGroupActionLink(this HtmlHelper helper,
                                                        FunctionalitiesGroups functionalityGroup,
                                                        string linkText,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return FunctionalityGroupActionLink(helper, functionalityGroup, linkText, null, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString FunctionalityGroupActionLink(this HtmlHelper helper,
                                                        FunctionalitiesGroups functionalityGroup,
                                                        string linkText,
                                                        string actionName,
                                                        string controllerName,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            var functionalityGroups = (IList<FunctionalityGroup>)HttpContext.Current.Session[SessionConstants.CURRENT_USER_FUNCTIONALITY_GROUP];
            if (functionalityGroups != null)
            {
                var functionalityGroupSelected = functionalityGroups.FirstOrDefault(a => a.Id == (int)functionalityGroup);
                var defaultLinkText = ActionsStrings.ResourceManager.GetString(Enum.GetName(typeof(FunctionalitiesGroups), functionalityGroup));

                if (functionalityGroupSelected != null)
                {
                    defaultLinkText = functionalityGroupSelected.Name;
                    var defaultAction = "FunctionalityGroup";
                    var defaultController = "Account";

                    if (!string.IsNullOrEmpty(linkText) && !string.IsNullOrWhiteSpace(linkText))
                        defaultLinkText = linkText;

                    if (!string.IsNullOrEmpty(actionName) && !string.IsNullOrWhiteSpace(actionName))
                        defaultAction = actionName;

                    if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrWhiteSpace(controllerName))
                        defaultController = controllerName;

                    if (routeValues == null)
                    {
                        routeValues = new RouteValueDictionary { { "id", functionalityGroupSelected.Id } };
                    }

                    var actualFunctionalityGroup =
                        HttpContext.Current.Session[SessionConstants.FUNCTIONALITY_GROUP_SELECTED];

                    if (htmlAttributes == null)
                        htmlAttributes = new Dictionary<string, object>();

                    if (actualFunctionalityGroup != null && (int)actualFunctionalityGroup == functionalityGroupSelected.Id)
                    {
                        if (htmlAttributes.ContainsKey("Class"))
                            htmlAttributes["Class"] += " active";
                        else
                            htmlAttributes.Add("Class", "active");
                    }

                    var i = new TagBuilder("i");
                    i.MergeAttribute("class", functionalityGroupSelected.IconClass);

                    var repId = Guid.NewGuid().ToString();
                    var enabledLink = helper.ActionLink(repId, defaultAction, defaultController, routeValues, htmlAttributes)
                        .ToHtmlString()
                        .Replace(repId, i + defaultLinkText);

                    return MvcHtmlString.Create(enabledLink);
                }
                else
                    return helper.Label(defaultLinkText);
            }
            return null;
        }
        #endregion

        #region FunctionalityActionLink
        public static MvcHtmlString FunctionalityActionLink(this HtmlHelper helper, Functionalities functionality)
        {
            return FunctionalityActionLink(helper, functionality, null, null, null, null, null);
        }

        public static MvcHtmlString FunctionalityActionLink(this HtmlHelper helper,
                                                        Functionalities functionality,
                                                        string linkText)
        {
            return FunctionalityActionLink(helper, functionality, linkText, null, null, null, null);
        }

        public static MvcHtmlString FunctionalityActionLink(this HtmlHelper helper,
                                                        Functionalities functionality,
                                                        string linkText,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return FunctionalityActionLink(helper, functionality, linkText, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString FunctionalityActionLink(this HtmlHelper helper,
                                                        Functionalities functionality,
                                                        string linkText,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return FunctionalityActionLink(helper, functionality, linkText, null, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString FunctionalityActionLink(this HtmlHelper helper,
                                                        Functionalities functionality,
                                                        string linkText,
                                                        string actionName,
                                                        string controllerName,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            var functionalities = (IList<Functionality>)HttpContext.Current.Session[SessionConstants.CURRENT_USER_FUNCTIONALITIES_GROUP_ACTUAL];
            if (functionalities != null)
            {
                var functionalitySelected = functionalities.FirstOrDefault(a => a.Id == (int)functionality);
                var funcionalitStringResx = Enum.GetName(typeof (Functionalities), functionality);
                var defaultLinkText = string.IsNullOrWhiteSpace(funcionalitStringResx) ? string.Empty : FunctionalityStrings.ResourceManager.GetString(funcionalitStringResx);
                if (functionalitySelected != null)
                {
                    var defaultAction = "Functionality";
                    var defaultController = "Account";

                    if (!string.IsNullOrEmpty(linkText) && !string.IsNullOrWhiteSpace(linkText))
                        defaultLinkText = linkText;

                    if (!string.IsNullOrEmpty(actionName) && !string.IsNullOrWhiteSpace(actionName))
                        defaultAction = actionName;

                    if (!string.IsNullOrEmpty(controllerName) && !string.IsNullOrWhiteSpace(controllerName))
                        defaultController = controllerName;

                    if (routeValues == null)
                    {
                        routeValues = new RouteValueDictionary { { "id", functionalitySelected.Id } };
                    }

                    var functionalityActual = HttpContext.Current.Session[SessionConstants.FUNCTIONALITY_SELECTED];

                    if (htmlAttributes == null)
                        htmlAttributes = new Dictionary<string, object>();

                    if (functionalityActual != null && (int)functionalityActual == functionalitySelected.Id)
                    {
                        if (htmlAttributes.ContainsKey("Class"))
                            htmlAttributes["Class"] += " active";
                        else
                            htmlAttributes.Add("Class", "active");
                    }

                    var i = new TagBuilder("i");
                    i.MergeAttribute("class", functionalitySelected.IconClass);

                    var repId = Guid.NewGuid().ToString();
                    var enabledLink = helper.ActionLink(repId, defaultAction, defaultController, routeValues, htmlAttributes)
                        .ToHtmlString()
                        .Replace(repId, i + defaultLinkText);

                    return MvcHtmlString.Create(enabledLink);
                }
            }
            return null;
        }
        #endregion

        #region ActionActionLink

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper, Actions action)
        {
            return ActionActionLink(helper, action, null, null, null, null, null);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper, Actions action, Guid id)
        {
            var route = new RouteValueDictionary { { "id", id } };
            return ActionActionLink(helper, action, route);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText)
        {
            return ActionActionLink(helper, action, linkText, null, null, null, null);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return ActionActionLink(helper, action, null, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        RouteValueDictionary routeValueDictionary)
        {
            return ActionActionLink(helper, action, null, null, null, routeValueDictionary, null);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        RouteValueDictionary routeValueDictionary,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return ActionActionLink(helper, action, null, null, null, routeValueDictionary, htmlAttributes);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return ActionActionLink(helper, action, linkText, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return ActionActionLink(helper, action, linkText, null, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText,
                                                        string actionName,
                                                        string controllerName,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            var selectedAction = ((IList<Action>)HttpContext.Current.Application[ApplicationConstants.SYSTEM_ACTIONS]).First(a => a.Id == (int)action);
            var enabledActions = (IList<Action>)HttpContext.Current.Session[SessionConstants.CURRENT_USER_ENABLED_ACTIONS];
            var defaultLinkText = (string.IsNullOrWhiteSpace(linkText)) ? ActionsStrings.ResourceManager.GetString(Enum.GetName(typeof(Actions), action)) : linkText;

            var defaultAction = (string.IsNullOrWhiteSpace(actionName)) ? selectedAction.MvcAction : actionName;
            var defaultController = (string.IsNullOrWhiteSpace(controllerName)) ? selectedAction.MvcController : controllerName;

            var clase = string.Empty;
            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            string repId;
            string enabledLink;
            if (enabledActions.Any(a => a.Id == selectedAction.Id))
            {
                #region El usuario tiene permisos para realizar esta acción
                switch (selectedAction.ActionType)
                {
                    case (int)ActionType.Details:
                        defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                        clase = "detail";
                        break;
                    case (int)ActionType.Edit:
                        defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                        clase = "edit";
                        break;
                    case (int)ActionType.Create:
                        clase = "tableButton";
                        break;
                }

                if (htmlAttributes.ContainsKey("Class"))
                    htmlAttributes["Class"] += string.Format(" {0}", clase);
                else
                    htmlAttributes.Add("Class", clase);

                repId = Guid.NewGuid().ToString();
                enabledLink = helper.ActionLink(repId, defaultAction, defaultController, routeValues, htmlAttributes)
                    .ToHtmlString()
                    .Replace(repId, defaultLinkText);

                return MvcHtmlString.Create(enabledLink);
                #endregion
            }

            #region El usuario NO tiene permisos para realizar esta acción
            switch (selectedAction.ActionType)
            {
                case (int)ActionType.Details:
                    defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                    clase = "detail";
                    break;
                case (int)ActionType.Edit:
                    defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                    clase = "edit";
                    break;
                case (int)ActionType.Create:
                    clase = "tableButton";
                    break;
            }

            if (htmlAttributes.ContainsKey("Class"))
                htmlAttributes["Class"] += string.Format(" {0} disabled", clase);
            else
                htmlAttributes.Add("Class", string.Format(" {0} disabled", clase));

            if (!htmlAttributes.ContainsKey("disabled"))
                htmlAttributes.Add("disabled", "disabled");
            else
                htmlAttributes["disabled"] = "disabled";

            if (!htmlAttributes.ContainsKey("title"))
                htmlAttributes.Add("title", PresentationCoreMessages.Common_NotAuthorizedUser);
            else
                htmlAttributes["title"] = PresentationCoreMessages.Common_NotAuthorizedUser;

            repId = Guid.NewGuid().ToString();
            var disabledLink = helper.ActionLink(repId, "NotAllowed", "Account", null, htmlAttributes)
                .ToHtmlString()
                .Replace(repId, defaultLinkText);

            return MvcHtmlString.Create(disabledLink);

            #endregion
        }
        #endregion

        #region ActionGridActionLink

        public static MvcHtmlString ActionGridActionLink(this HtmlHelper helper, Actions action)
        {
            return ActionGridActionLink(helper, action, null, null, null, null, null);
        }

        public static MvcHtmlString ActionGridActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText)
        {
            return ActionGridActionLink(helper, action, linkText, null, null, null, null);
        }

        public static MvcHtmlString ActionGridActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return ActionGridActionLink(helper, action, null, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString ActionGridActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        RouteValueDictionary routeValueDictionary)
        {
            return ActionGridActionLink(helper, action, null, null, null, routeValueDictionary, null);
        }

        public static MvcHtmlString ActionGridActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return ActionGridActionLink(helper, action, linkText, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString ActionGridActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            return ActionGridActionLink(helper, action, linkText, null, null, routeValues, htmlAttributes);
        }

        public static MvcHtmlString ActionGridActionLink(this HtmlHelper helper,
                                                        Actions action,
                                                        string linkText,
                                                        string actionName,
                                                        string controllerName,
                                                        RouteValueDictionary routeValues,
                                                        IDictionary<string, object> htmlAttributes)
        {
            const string oObjDataId = "oObjDataId";

            var selectedAction = ((IList<Action>)HttpContext.Current.Application[ApplicationConstants.SYSTEM_ACTIONS]).First(a => a.Id == (int)action);
            var enabledActions = (IList<Action>)HttpContext.Current.Session[SessionConstants.CURRENT_USER_ENABLED_ACTIONS];
            var defaultLinkText = (string.IsNullOrWhiteSpace(linkText)) ? ActionsStrings.ResourceManager.GetString(Enum.GetName(typeof(Actions), action)) : linkText;

            var defaultAction = (string.IsNullOrWhiteSpace(actionName)) ? selectedAction.MvcAction : actionName;
            var defaultController = (string.IsNullOrWhiteSpace(controllerName)) ? selectedAction.MvcController : controllerName;

            var clase = string.Empty;
            if (htmlAttributes == null)
                htmlAttributes = new Dictionary<string, object>();

            string repId;
            if (enabledActions.Any(a => a.Id == selectedAction.Id))
            {
                #region El usuario tiene permisos para realizar esta acción
                switch (selectedAction.ActionType)
                {
                    case (int)ActionType.Details:
                        defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                        clase = "detail";
                        break;
                    case (int)ActionType.Edit:
                        defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                        clase = "edit";
                        break;
                    case (int)ActionType.Create:
                        clase = "tableButton";
                        break;
                }
                if (htmlAttributes.ContainsKey("Class"))
                    htmlAttributes["Class"] += string.Format(" {0}", clase);
                else
                    htmlAttributes.Add("Class", clase);

                repId = Guid.NewGuid().ToString();
                var enabledLink = helper.ActionLink(repId, defaultAction, defaultController, new RouteValueDictionary { { "id", oObjDataId } }, htmlAttributes)
                    .ToHtmlString()
                    .Replace(oObjDataId, "'+ oObj.aData.Id + '")
                    .Replace(repId, defaultLinkText);

                return MvcHtmlString.Create(enabledLink);

                #endregion
            }

            #region El usuario NO tiene permisos para realizar esta acción
            switch (selectedAction.ActionType)
            {
                case (int)ActionType.Details:
                    defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                    clase = "detail";
                    break;
                case (int)ActionType.Edit:
                    defaultLinkText = (!string.IsNullOrEmpty(defaultLinkText)) ? defaultLinkText : " ";
                    clase = "edit";
                    break;
                case (int)ActionType.Create:
                    clase = "tableButton";
                    break;
            }
            if (htmlAttributes.ContainsKey("Class"))
                htmlAttributes["Class"] += string.Format(" {0} disabled", clase);
            else
                htmlAttributes.Add("Class", string.Format(" {0} disabled", clase));

            if (!htmlAttributes.ContainsKey("disabled"))
                htmlAttributes.Add("disabled", "disabled");
            else
                htmlAttributes["disabled"] = "disabled";

            if (!htmlAttributes.ContainsKey("title"))
                htmlAttributes.Add("title", PresentationCoreMessages.Common_NotAuthorizedUser);
            else
                htmlAttributes["title"] = PresentationCoreMessages.Common_NotAuthorizedUser;

            htmlAttributes.Add("onclick", "javascript:return false;");

            repId = Guid.NewGuid().ToString();
            var disabledLink = helper.ActionLink(repId, "NotAllowed", "Account", null, htmlAttributes)
                .ToHtmlString()
                .Replace(repId, defaultLinkText);

            return MvcHtmlString.Create(disabledLink);
            #endregion
        }
        #endregion

        #region DropDownList
        public static MvcHtmlString DropDownListSelectable(this HtmlHelper h, string name, IEnumerable<SelectListItem> items, string selected, string optionLabel, object htmlAttributes)
        {
            var select = new TagBuilder("select");
            select.MergeAttribute("name", name);
            select.MergeAttribute("id", name.Replace('.', '_'));
            object opcionSeleccionada = null;

            select.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            var options = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(optionLabel))
            {
                options.Add(new SelectListItem { Text = optionLabel, Value = string.Empty });
            }

            options = options.Concat(items).ToList();

            foreach (var item in options)
            {
                var option = new TagBuilder("option");
                option.MergeAttribute("value", item.Value);
                option.InnerHtml = item.Text;
                if (!String.IsNullOrWhiteSpace(selected))
                {
                    if (item.Value.ToLower() == selected.ToLower())
                    {
                        option.MergeAttribute("selected", "selected");
                        opcionSeleccionada = item;
                    }
                }
                select.InnerHtml += option.ToString();
            }

            if (opcionSeleccionada == null)
            {
                var otherSelect = new TagBuilder("select");
                otherSelect.MergeAttribute("name", name);
                otherSelect.MergeAttribute("id", name);
                otherSelect.MergeAttributes(new RouteValueDictionary(htmlAttributes));

                foreach (var item in options)
                {
                    var option = new TagBuilder("option");

                    if (item.Value == "")
                    {
                        option.MergeAttribute("selected", "selected");
                        option.MergeAttribute("value", "");
                    }
                    else
                    {
                        option.MergeAttribute("value", item.Value);
                    }
                    option.InnerHtml = item.Text;
                    otherSelect.InnerHtml += option.ToString();
                }
                return MvcHtmlString.Create(otherSelect.ToString());

            }
            return MvcHtmlString.Create(select.ToString());
        }

        //public static MvcHtmlString DropDownListSelectable(this HtmlHelper h, string name, IEnumerable<SelectListItem> items, string selected, object htmlAttributes)
        //{
        //    var select = new TagBuilder("select");
        //    select.MergeAttribute("name", name);
        //    select.MergeAttribute("id", name.Replace('.', '_'));
        //    object opcionSeleccionada = null;

        //    select.MergeAttributes(new RouteValueDictionary(htmlAttributes));

        //    foreach (var item in items)
        //    {
        //        var option = new TagBuilder("option");
        //        option.MergeAttribute("value", item.Value);
        //        option.InnerHtml = item.Text;
        //        if (!String.IsNullOrWhiteSpace(selected))
        //        {
        //            if (item.Value.ToLower() == selected.ToLower())
        //            {
        //                option.MergeAttribute("selected", "selected");
        //                opcionSeleccionada = item;
        //            }
        //        }
        //        select.InnerHtml += option.ToString();
        //    }

        //    if (opcionSeleccionada == null)
        //    {
        //        var otherSelect = new TagBuilder("select");
        //        otherSelect.MergeAttribute("name", name);
        //        otherSelect.MergeAttribute("id", name);
        //        otherSelect.MergeAttributes(new RouteValueDictionary(htmlAttributes));

        //        foreach (var item in items)
        //        {
        //            var option = new TagBuilder("option");

        //            if (item.Value == "")
        //            {
        //                option.MergeAttribute("selected", "selected");
        //                option.MergeAttribute("value", "");
        //            }
        //            else
        //            {
        //                option.MergeAttribute("value", item.Value);
        //            }
        //            option.InnerHtml = item.Text;
        //            otherSelect.InnerHtml += option.ToString();
        //        }
        //        return MvcHtmlString.Create(otherSelect.ToString());

        //    }
        //    return MvcHtmlString.Create(select.ToString());
        //}

        public static MvcHtmlString DropDownListSelectable(this HtmlHelper h, string name, IEnumerable<SelectListItem> items, string selected, string optionLabel)
        {
            return DropDownListSelectable(h, name, items, selected, optionLabel, null);
        }

        public static MvcHtmlString DropDownListSelectable(this HtmlHelper h, string name, IEnumerable<SelectListItem> items, Guid selectedId, string optionLabel)
        {
            string selectedValue = "";
            if (selectedId != default(Guid))
                selectedValue = selectedId.ToString();
            return DropDownListSelectable(h, name, items, selectedValue, optionLabel, null);
        }

        public static MvcHtmlString DropDownListSelectable(this HtmlHelper h, string name, IEnumerable<SelectListItem> items, Guid selectedId, string optionLabel, object htmlAttributes)
        {
            string selectedValue = "";
            if (selectedId != default(Guid))
                selectedValue = selectedId.ToString();
            return DropDownListSelectable(h, name, items, selectedValue, optionLabel, htmlAttributes);
        }

        public static MvcHtmlString DropDownListSelectable(this HtmlHelper h, string name, IEnumerable<SelectListItem> items, Guid? selectedId, string optionLabel)
        {
            string selectedValue = "";
            if (selectedId.HasValue)
            {
                if (selectedId != default(Guid)) selectedValue = selectedId.ToString();
                return DropDownListSelectable(h, name, items, selectedValue, optionLabel, null);
            }

            return DropDownListSelectable(h, name, items, selectedValue, optionLabel, null);
        }

        /// <summary>
        /// Returns a Select element with groups (keys of the dictionary)
        /// </summary>
        /// <param name="h"></param>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="selected">Value of the items wich is selected</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListGrouped(this HtmlHelper h, string name, Dictionary<string, IEnumerable<SelectListItem>> items, string selected, object htmlAttributes)
        {
            var select = new TagBuilder("select");
            select.MergeAttribute("name", name);
            select.MergeAttribute("id", name.Replace('.', '_'));

            select.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            foreach (var item in items)
            {
                var group = new TagBuilder("optgroup");
                group.MergeAttribute("label", item.Key);

                foreach (var optionItem in item.Value)
                {
                    var option = new TagBuilder("option");
                    option.MergeAttribute("value", optionItem.Value);
                    option.InnerHtml = optionItem.Text;

                    group.InnerHtml += option.ToString();
                }
                select.InnerHtml += group.ToString();
            }

            return MvcHtmlString.Create(select.ToString());
        }

        /// <summary>
        /// Returns a Select element with groups (keys of the dictionary)
        /// </summary>
        /// <param name="h"></param>
        /// <param name="name"></param>
        /// <param name="items"></param>
        /// <param name="selected">Value of the items wich is selected</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListGrouped(this HtmlHelper h, string name, Dictionary<string, IEnumerable<SelectListItem>> items, string selected, IDictionary<string, object> htmlAttributes)
        {
            var select = new TagBuilder("select");
            select.MergeAttribute("name", name);
            select.MergeAttribute("id", name.Replace('.', '_'));

            select.MergeAttributes(new RouteValueDictionary(htmlAttributes));

            foreach (var item in items)
            {
                var group = new TagBuilder("optgroup");
                group.MergeAttribute("label", item.Key);

                foreach (var optionItem in item.Value)
                {
                    var option = new TagBuilder("option");
                    option.MergeAttribute("value", optionItem.Value);
                    if (optionItem.Selected)
                    {
                        option.MergeAttribute("selected", optionItem.Selected.ToString());
                    }
                    option.InnerHtml = optionItem.Text;

                    group.InnerHtml += option.ToString();
                }
                select.InnerHtml += group.ToString();
            }

            return MvcHtmlString.Create(select.ToString());
        }

        #endregion
    }
}