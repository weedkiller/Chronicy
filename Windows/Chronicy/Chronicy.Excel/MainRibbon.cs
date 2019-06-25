﻿using Chronicy.Excel.App;
using Chronicy.Excel.Information;
using Chronicy.Excel.Tracking;
using Chronicy.Excel.Tracking.Events;
using Chronicy.Excel.UI;
using Chronicy.Excel.UI.Pane;
using Chronicy.Excel.User;
using Chronicy.Excel.Utils;
using Chronicy.Information;
using Chronicy.Tracking;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;

namespace Chronicy.Excel
{
    // TODO: Modularize this more
    public partial class MainRibbon
    {
        private MessageBoxContext informationContext = new MessageBoxContext();

        // UI
        private TaskPane<EditTaskPane> editTaskPane;
        private TaskPane<NotebookTaskPane> notebookTaskPane;

        // Data
        private IExcelExtension extension;
        private ExcelTracker tracker;

        public MainRibbon(IExcelExtension extension) : this()
        {
            this.extension = extension;
        }

        private void SetupActivationCallbacks()
        {
            enableButton.Visible = false;
            selectionGroup.Visible = false;
            trackingGroup.Visible = false;
            toolsGroup.Visible = false;

            extension.ConnectionChanged += (enabled) =>
            {
                enableButton.Visible = enabled;
                selectionGroup.Visible = enabled;
                trackingGroup.Visible = enabled;
                toolsGroup.Visible = enabled;
            };
        }

        private void InitializeTrackingMenus()
        {
            tracker = new ExcelTracker();
            tracker.Register<Workbook>(new WorkbookTrackable());
            tracker.Register<Worksheet>(new WorksheetTrackable());
            tracker.Register<Range>(new RangeTrackable());

            // Upon clicking the checkbox, the StateUpdated event is triggered, which sets the checkbox to its value again.
            // This is not the end of the world, however we're wasting a call by creating this cycle

            workbookEnableCheckBox.Click += (sender, args) => { tracker.Get<Workbook>().Enabled = (sender as RibbonCheckBox).Checked; };
            sheetEnableCheckBox.Click += (sender, args) => { tracker.Get<Worksheet>().Enabled = (sender as RibbonCheckBox).Checked; };
            cellsEnableCheckBox.Click += (sender, args) => { tracker.Get<Range>().Enabled = (sender as RibbonCheckBox).Checked; };

            ITrackable workbook = tracker.Get<Workbook>();
            workbook.StateUpdated += (enabled) => { workbookEnableCheckBox.Checked = enabled; };
            workbook.TrackedValueUpdated += (value) => { workbookCurrentLabel.Label = "Tracked workbook: " + (value as Workbook).Name; };

            ITrackable sheet = tracker.Get<Worksheet>();
            sheet.StateUpdated += (enabled) => { sheetEnableCheckBox.Checked = enabled; };
            sheet.TrackedValueUpdated += (value) => { sheetCurrentLabel.Label = "Tracked sheet: " + (value as Worksheet).Name; };

            ITrackable cells = tracker.Get<Range>();
            cells.StateUpdated += (enabled) => { cellsEnableCheckBox.Checked = enabled; };
            cells.TrackedValueUpdated += (value) => { cellsCurrentLabel.Label = "Tracked range: " + (value as Range).ToAddressString().Replace("$", ""); };
        }

        private void InitializeTaskPanes()
        {
            editTaskPane = new TaskPane<EditTaskPane>("Edit Item", new EditTaskPane());
            notebookTaskPane = new TaskPane<NotebookTaskPane>("Notebook", new NotebookTaskPane());
        }

        private void OnRibbonLoad(object sender, RibbonUIEventArgs e)
        {
            InitializeTrackingMenus();
            SetupActivationCallbacks();
            InitializeTaskPanes();

            extension.StateChanged += (enabled) => { enableButton.Checked = enabled; };
            extension.ConnectionChanged += (connected) => { connectButton.Visible = !connected; };

            extension.OnRibbonLoad();
        }

        private void OnRibbonClose(object sender, EventArgs e)
        {
            extension.OnRibbonUnload();
        }

        private void OnConnectClicked(object sender, RibbonControlEventArgs e)
        {
            try
            {
                extension.Connect();
            }
            catch (EndpointConnectionException)
            {
                InformationDispatcher.Default.Dispatch("Could not connect to the Chronicy service!\n" +
                                                       "Make sure that the service is installed and running.",
                                                        informationContext, 
                                                        InformationKind.Error);
            }
            catch (Exception ex)
            {
                InformationDispatcher.Default.Dispatch("An unknown error occurred while trying to connect to the service: " + ex.Message,
                                                        informationContext);
            }
        }

        private void OnEnableToggled(object sender, RibbonControlEventArgs e)
        {
            extension.Enabled = enableButton.Checked;
        }

        private void OnNewNotebookClicked(object sender, RibbonControlEventArgs e)
        {
            editTaskPane.Visible = true;
        }

        private void OnNewStackClicked(object sender, RibbonControlEventArgs e)
        {
            editTaskPane.Visible = true;
        }

        private void OnViewAllClicked(object sender, RibbonControlEventArgs e)
        {
            notebookTaskPane.Visible = true;
        }

        private void OnSyncClicked(object sender, RibbonControlEventArgs e)
        {
            try
            {
                extension.Sync();
            }
            catch (EndpointConnectionException ex)
            {
                InformationDispatcher.Default.Dispatch(ex.Message, informationContext, InformationKind.Error);
            }
        }

        private void OnTrackWorkbook(object sender, RibbonControlEventArgs e)
        {
            // 1. Submit the current workbook to ExcelTracker
            ITrackable trackable = tracker.Get<Workbook>();
            trackable.ValueUpdated += (value) => extension.Tracking.Post<Workbook>(new TrackingEvent((Workbook)value));
            trackable.TrackedValue = Globals.ThisAddIn.Application.ActiveWorkbook;
            trackable.Enabled = true;
        }

        private void OnTrackSheet(object sender, RibbonControlEventArgs e)
        {
            // 1. Ask the user to pick a sheet
            SelectSheetAction action = new SelectSheetAction();
            action.ActionCompleted += (sheet) => 
            {
                /* 2. Submit the sheet to ExcelTracker */
                ITrackable trackable = tracker.Get<Worksheet>();
                trackable.ValueUpdated += (value) => extension.Tracking.Post<Worksheet>(new TrackingEvent((Worksheet)value));
                trackable.TrackedValue = sheet;
                trackable.Enabled = true;
            };
            action.Run();
        }

        private void OnTrackCellRange(object sender, RibbonControlEventArgs e)
        {
            // 1. Ask the user to pick a range
            SelectCellRangeAction action = new SelectCellRangeAction();
            action.ActionCompleted += (range) => 
            {
                /* 2. Submit the range to ExcelTracker */
                ITrackable trackable = tracker.Get<Range>();
                trackable.ValueUpdated += (value) => extension.Tracking.Post<Range>(new TrackingEvent((Range)value));
                trackable.TrackedValue = range;
                trackable.Enabled = true;
            };
            action.Run();
        }

        private void OnHelpClicked(object sender, RibbonControlEventArgs e)
        {
            ExternalLink link = new ExternalLink(Properties.Resources.LINK_HELP);
            link.Open();
        }

        private void OnReportBugClicked(object sender, RibbonControlEventArgs e)
        {
            ExternalLink link = new ExternalLink(Properties.Resources.LINK_SUBMIT_BUG);
            link.Open();
        }

        private void OnViewGitHubClicked(object sender, RibbonControlEventArgs e)
        {
            ExternalLink link = new ExternalLink(Properties.Resources.LINK_PROJECT_PAGE);
            link.Open();
        }
    }
}
