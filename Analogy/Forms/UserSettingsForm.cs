﻿using Analogy.DataTypes;
using Analogy.Interfaces;
using Analogy.Managers;
using Analogy.Properties;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Analogy.Forms
{

    public partial class UserSettingsForm : XtraForm
    {



        private UserSettingsManager Settings { get; } = UserSettingsManager.UserSettings;
        private int InitialSelection = -1;

        public UserSettingsForm()
        {
            InitializeComponent();

        }

        public UserSettingsForm(int tabIndex) : this()
        {
            InitialSelection = tabIndex;
        }

        private void UserSettingsForm_Load(object sender, EventArgs e)
        {
            ShowIcon = true;

            Icon = UserSettingsManager.UserSettings.GetIcon();
            LoadSettings();
            SetupEventsHandlers();

            if (InitialSelection >= 0)
            {
                tabControlMain.SelectedTabPageIndex = InitialSelection;
            }
        }

        private void SetupEventsHandlers()
        {
        }



        private void LoadSettings()
        {



            cbUpdates.Properties.Items.AddRange(typeof(UpdateMode).GetDisplayValues().Values);
            cbUpdates.SelectedItem = UpdateManager.Instance.UpdateMode.GetDisplay();
            if (AnalogyNonPersistSettings.Instance.DisableUpdatesByDataProvidersOverrides)
            {
                lblDisableUpdates.Visible = true;
                cbUpdates.Enabled = false;
            }


            tsEnableFirstChanceException.IsOn = Settings.EnableFirstChanceException;

        }

        private void SaveSetting()
        {
           



            var options = typeof(UpdateMode).GetDisplayValues();
            UpdateManager.Instance.UpdateMode = (UpdateMode)Enum.Parse(typeof(UpdateMode),
                options.Single(k => k.Value == cbUpdates.SelectedItem.ToString()).Key, true);
            Settings.EnableFirstChanceException = tsEnableFirstChanceException.IsOn;

            Settings.Save();
        }


        private void UserSettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSetting();
        }

        private void btnDataProviderCustomSettings_Click(object sender, EventArgs e)
        {
       
        }

        //private void sBtnMoveUp_Click(object sender, EventArgs e)
        //{
        //    if (chkLstDataProviderStatus.SelectedIndex <= 0)
        //    {
        //        return;
        //    }

        //    var selectedIndex = chkLstDataProviderStatus.SelectedIndex;
        //    var currentValue = chkLstDataProviderStatus.Items[selectedIndex];
        //    chkLstDataProviderStatus.Items[selectedIndex] = chkLstDataProviderStatus.Items[selectedIndex - 1];
        //    chkLstDataProviderStatus.Items[selectedIndex - 1] = currentValue;
        //    chkLstDataProviderStatus.SelectedIndex = chkLstDataProviderStatus.SelectedIndex - 1;
        //}

        //private void sBtnMoveDown_Click(object sender, EventArgs e)
        //{
        //    if (chkLstDataProviderStatus.SelectedIndex == chkLstDataProviderStatus.Items.Count - 1)
        //    {
        //        return;
        //    }

        //    var selectedIndex = chkLstDataProviderStatus.SelectedIndex;
        //    var currentValue = chkLstDataProviderStatus.Items[selectedIndex + 1];
        //    chkLstDataProviderStatus.Items[selectedIndex + 1] = chkLstDataProviderStatus.Items[selectedIndex];
        //    chkLstDataProviderStatus.Items[selectedIndex] = currentValue;
        //    chkLstDataProviderStatus.SelectedIndex = chkLstDataProviderStatus.SelectedIndex + 1;
        //}
      

    }
}

