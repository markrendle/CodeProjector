using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace MarkRendle.CodeProjector
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidCodeProjectorPkgString)]
    public sealed class CodeProjectorPackage : Package
    {
        private ProjectorWindow _projectorWindow;

        private FrameworkElement _mainDocWellGrid;

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public CodeProjectorPackage()
        {
            Trace.WriteLine(string.Format(CultureInfo.CurrentCulture, "Entering constructor for: {0}", this.ToString()));
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overriden Package Implementation
        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Trace.WriteLine (string.Format(CultureInfo.CurrentCulture, "Entering Initialize() of: {0}", this.ToString()));
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if ( null != mcs )
            {
                // Create the command for the menu item.
                CommandID menuCommandID = new CommandID(GuidList.guidCodeProjectorCmdSet, (int)PkgCmdIDList.cmdidProjectCode);
                MenuCommand menuItem = new MenuCommand(MenuItemCallback, menuCommandID );
                mcs.AddCommand( menuItem );
            }
        }
        #endregion

        /// <summary>
        /// This function is the callback used to execute a command when the a menu item is clicked.
        /// See the Initialize method to see how the menu item is associated to this function using
        /// the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            if (this._mainDocWellGrid == null)
            {
                this._mainDocWellGrid = GetMainDocWellGrid();

                this._mainDocWellGrid.MouseMove += (so, ea) =>
                {
                    if (_projectorWindow == null)
                    {
                        return;
                    }

                    var point = ea.GetPosition(this._mainDocWellGrid);

                    var xScale = _projectorWindow.TheGrid.ActualHeight / this._mainDocWellGrid.ActualHeight;
                    var yScale = _projectorWindow.TheGrid.ActualWidth / this._mainDocWellGrid.ActualWidth;

                    _projectorWindow.CursorImage.Margin = new Thickness(Math.Max(point.X * xScale, 0), Math.Max(point.Y * yScale, 0), 0, 0);
                };

                // Mouse button events would work better if they were
                // preview events, or "handled events too", but if I do that
                // then code windows stop rendering for some reason..

                this._mainDocWellGrid.MouseDown += (so, ea) =>
                {
                    if (_projectorWindow == null)
                    {
                        return;
                    }

                    _projectorWindow.CursorImage.RenderTransform = new ScaleTransform(1.5, 1.5, 0.5, 0.5);
                };

                this._mainDocWellGrid.MouseUp += (so, ea) =>
                {
                    if (_projectorWindow == null)
                    {
                        return;
                    }

                    _projectorWindow.CursorImage.RenderTransform = null;
                };

                this._mainDocWellGrid.SizeChanged += (se, ea) =>
                {
                    if (_projectorWindow == null)
                    {
                        return;
                    }

                    _projectorWindow.TheGrid.Width = ea.NewSize.Width;
                    _projectorWindow.TheGrid.Height = ea.NewSize.Height;
                };
            }

            var brush = new VisualBrush(this._mainDocWellGrid);
            _projectorWindow = _projectorWindow ?? new ProjectorWindow(brush);
            _projectorWindow.TheGrid.Width = this._mainDocWellGrid.ActualWidth;
            _projectorWindow.TheGrid.Height = this._mainDocWellGrid.ActualHeight;
            _projectorWindow.Show();
            _projectorWindow.Closed += (o, x) => _projectorWindow = null;
        }

        private FrameworkElement GetMainDocWellGrid()
        {
            FrameworkElement mainDockTarget;
            TryFindChild(Application.Current.MainWindow, "MainDockTarget", out mainDockTarget);
            return mainDockTarget;
        }

        /// <summary>
        /// Finds a Child of a given item in the visual tree. 
        /// </summary>
        /// <param name="parent">A direct parent of the queried item.</param>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="childName">x:Name or Name of child. </param>
        /// <returns>The first parent item that matches the submitted type parameter. 
        /// If not matching item can be found, a null parent is being returned.</returns>
        public static bool TryFindChild(FrameworkElement parent, string childName, out FrameworkElement foundChild)
        {
            foundChild = null;

            // Confirm parent and childName are valid. 
            if (parent == null)
            {
                return false;
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                foundChild = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if (foundChild != null && foundChild.Name == childName)
                {
                    return true;
                }
                if (TryFindChild(foundChild, childName, out foundChild))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
