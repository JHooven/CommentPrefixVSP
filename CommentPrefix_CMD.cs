//------------------------------------------------------------------------------
// <copyright file="CommentPrefix_CMD.cs" company="John E. Hooven">
//     Copyright (c) John E. Hooven  No rights reserved.
//     Feel free to use this source code in any way you wish.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;
using EnvDTE80;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CommentPrefixVSP
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CommentPrefix_CMD
    {
        /// <summary>
        /// The DTE application object
        /// </summary>
        private DTE2 _applicationObject;

        /// <summary>
        /// Our settings
        /// </summary>
        private Settings mSettings;

        /// <summary>
        /// Command ID for inserting comment prefix.
        /// </summary>
        public const int CommentPrefix_CommandId = 0x0100;

        /// <summary>
        /// Command ID for showing settings dialog
        /// </summary>
        public const int ShowSettings_CommandId = 0x0101;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("a8d81e36-2042-488f-ad2b-99ecee0c175b");

        /// <summary>
        /// VS Package that provides these commands.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentPrefix_CMD"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CommentPrefix_CMD(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            // JEH 17-Dec-2016 Load our settings
            LoadSettings();


            _applicationObject = (DTE2)ServiceProvider.GetService(typeof(DTE));

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommentPrefix_CommandId);
                var menuItem = new MenuCommand(this.InsertComment_MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);

                var menuSettingsCommandID = new CommandID(CommandSet, ShowSettings_CommandId);
                var menuSettingsItem = new MenuCommand(this.ShowSettings_MenuItemCallback, menuSettingsCommandID);
                commandService.AddCommand(menuSettingsItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CommentPrefix_CMD Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CommentPrefix_CMD(package);
        }

        private void LoadSettings()
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CommentPrefix", "Settings.xml");

            if (File.Exists(fileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(Settings));

                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    mSettings = ((Settings)xs.Deserialize(fs));
                }
            }
            else
            {
                mSettings = new Settings();
                mSettings.Save();
            }
        }

        private string GetCommentToken(string fileType)
        {
            string str = "//";

            str = mSettings.CommentTokens.Find(t => t.FileType == fileType).Token;

            if (null == str)
            {
                str = mSettings.CommentTokens.Find(t => t.FileType == "<default>").Token;
            }

            return str;
        }
        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void InsertComment_MenuItemCallback(object sender, EventArgs e)
        {
            string fileName = _applicationObject.ActiveDocument.Name;
            int lastIndex = fileName.LastIndexOf(".");
            string fileType = fileName.Substring(lastIndex, fileName.Length - lastIndex);
            string commentToken = GetCommentToken(fileType);
            TextSelection ts = (TextSelection)_applicationObject.ActiveDocument.Selection;

            VirtualPoint currentCaret = ((TextSelection)_applicationObject.ActiveDocument.Selection).ActivePoint;

            string str = String.Format("{0}", commentToken);
            ts.Insert(str);

            // JEH 17-Dec-2016 If the comment token has a space in it, such as '<!-- -->'
            // JEH 17-Dec-2016 move the caret just before the space.
            int spaceIndex = commentToken.IndexOf(" ");

            if (spaceIndex >= 0)
            {
                int charsToMove = (str.Length - (str.Length - spaceIndex));
                ((TextSelection)_applicationObject.ActiveDocument.Selection).CharLeft(false, charsToMove);
            }

            str = string.Format("{0}{1:" + mSettings.DateFormat + "}", mSettings.Prefix, DateTime.Now);
            ts.Insert(str);
        }
        private void ShowSettings_MenuItemCallback(object sender, EventArgs e)
        {
            SettingsDialog sd = new SettingsDialog(mSettings);

            if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                mSettings = sd.Settings;
                mSettings.Save();
            }
        }
    }
}
