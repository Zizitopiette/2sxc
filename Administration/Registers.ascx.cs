﻿using DotNetNuke.Entities.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ToSic.SexyContent.Administration
{
    public partial class Registers : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Correct Local Resource File
            var ResourceFile = DotNetNuke.Common.Globals.ResolveUrl("~/DesktopModules/ToSIC_SexyContent/Administration/App_LocalResources/Registers.ascx.resx");   

            var ParentModule = (PortalModuleBase)Parent;

            var Registers = new List<string>();
            Registers.Add(SexyContent.ControlKeys.GettingStarted);

            // Add Buttons if ZoneID is set
            if (SexyContent.GetZoneID(ParentModule.PortalId).HasValue)
            {
                Registers.Add(SexyContent.ControlKeys.Import);
                Registers.Add(SexyContent.ControlKeys.ManageTemplates);
                Registers.Add(SexyContent.ControlKeys.EavManagement);
            }

            Registers.Add(SexyContent.ControlKeys.PortalConfiguration);

            if(String.IsNullOrEmpty(Request.QueryString[SexyContent.AppIDString]))
            {
                throw new ArgumentNullException(SexyContent.AppIDString, "AppId cannot be blank.");
            }

            rptRegisters.DataSource = from c in Registers
                                      select new {
                                        Name = DotNetNuke.Services.Localization.Localization.GetString(c + ".Text",  ResourceFile),
                                        Key = c,
                                        Url =  ParentModule.EditUrl(ParentModule.TabId, c, true, "mid=" + ParentModule.ModuleId + "&" + SexyContent.AppIDString + "=" + Request.QueryString[SexyContent.AppIDString]),
                                        Active = Request.QueryString["ctl"].ToLower() == c.ToLower()
                                      };
            rptRegisters.DataBind();
        }
    }
}