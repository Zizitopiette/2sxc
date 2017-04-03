﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.FileSystem;
using ToSic.SexyContent.Adam;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ImportExport
{
    public class XmlExporter: EavXmlExporter
    {
        // private IFolderManager DnnFolders = DotNetNuke.Services.FileSystem.FolderManager.Instance;
        private readonly IFileManager _dnnFiles = DotNetNuke.Services.FileSystem.FileManager.Instance;
        internal AdamManager AdamManager;

        public XmlExporter(int zoneId, int appId, bool appExport, string[] attrSetIds, string[] entityIds):base()
        {
            // do things first

            _app = new App(zoneId, appId, PortalSettings.Current);
            EavAppContext = new EavBridge(_app).FullController;
            AdamManager = new AdamManager(PortalSettings.Current.PortalId, _app);
            Constructor(appExport, attrSetIds, entityIds);

            // do following things
            // this must happen very early, to ensure that the file-lists etc. are correct for exporting when used externally
            InitExportXDocument(PortalSettings.Current.DefaultLanguage);

        }

        private void AddAdamFilesToExportQueue()
        {
            var adamIds = AdamManager.Export.AppFiles;
            adamIds.ForEach(AddFileAndFolderToQueue);

            // also add folders in adam - because empty folders may also have metadata assigned
            var adamFolders = AdamManager.Export.AppFolders;
            adamFolders.ForEach(AddFolderToQueue);
        }

        public override void AddFilesToExportQueue()
        {
            AddAdamFilesToExportQueue();
        }

        internal override void AddFileAndFolderToQueue(int fileNum)
        {
            try
            {
                ReferencedFileIds.Add(fileNum);

                // also try to remember the folder
                try
                {
                    var file = _dnnFiles.GetFile(fileNum);
                    AddFolderToQueue(file.FolderId);
                }
                catch
                {
                    // don't do anything, because if the file doesn't exist, its FOLDER should also not land in the queue
                }
            }
            catch
            {
                // don't do anything, because if the file doesn't exist, it should also not land in the queue
            }
        }

        private void AddFolderToQueue(int folderId)
        {
            ReferencedFolderIds.Add(folderId);
        }

        internal override string ResolveFolderId(int folderId)
        {
            var folderController = FolderManager.Instance;
            var folder = folderController.GetFolder(folderId);
            return folder?.FolderPath;
        }

        internal ImpExpFileItem ResolveFile(int fileId)
        {
            var fileController = DotNetNuke.Services.FileSystem.FileManager.Instance;
            var file = fileController.GetFile(fileId);
            return new ImpExpFileItem
            {
                Id = fileId,
                RelativePath = file?.RelativePath.Replace('/', '\\'),
                Path = file?.PhysicalPath
            };
        }

    }
}