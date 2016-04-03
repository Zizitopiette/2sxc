﻿using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Services.Exceptions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    public partial class View : SexyViewContentOrApp, IActionable
    {
        /// <summary>
        /// Page Load event - preload template chooser if necessary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected new void Page_Load(object sender, EventArgs e)
        {
            // always do this, part of the guarantee that everything will work
            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();

            #region check installer message
            pnlError.Visible = false;

            // check if installer is running or should be...
            var notReady = Installer.CheckUpgradeMessage(UserInfo.IsSuperUser);
            if(!string.IsNullOrEmpty(notReady))
                ShowError(notReady, pnlError, notReady, false);
            #endregion

            if (!UserMayEditThisModule) return;

            #region If logged in, inject Edit JavaScript, and delete / add items
            // register scripts and css
            try
            {
                var renderHelp = new RenderingHelpers(_sxcInstance);
                renderHelp.RegisterClientDependencies(Page, string.IsNullOrEmpty(Request.QueryString["debug"]));
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
            #endregion
        }

        /// <summary>
        /// Process View if a Template has been set
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!Installer.UpgradeComplete) return;

            try
            {
                var isSharedModule = ModuleConfiguration.PortalID != ModuleConfiguration.OwnerPortalID;

                // check things if it's a module of this portal (ensure everything is ok, etc.)
                if (!isSharedModule && !ContentBlock.ContentGroupExists)
                    new DnnStuffToRefactor().EnsurePortalIsConfigured(_sxcInstance, Server, ControlPath);
                

                //if (ContentBlock.ContentGroupExists) // AppId.HasValue)
                //{
                    //if (_sxcInstance.ContentGroup.Content.Any() && Template != null)
                        ProcessView(phOutput, pnlError);
                    //else if (!_sxcInstance.IsContentApp && UserMayEditThisModule)
                    //// Select first available template automatically if it's not set yet - then refresh page
                    //{
                    //    var templates = _sxcInstance.AppTemplates.GetAvailableTemplatesForSelector(ModuleConfiguration.ModuleID, _sxcInstance.AppContentGroups).ToList();
                    //    if (templates.Any())
                    //    {
                    //        _sxcInstance.AppContentGroups.SetPreviewTemplateId(ModuleConfiguration.ModuleID, templates.First().TemplateId);
                    //        Response.Redirect(Request.RawUrl);
                    //    }
                    //}
                //}
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

    }
}