﻿using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Analogy;


namespace Philips.Analogy
{
    public partial class LogStatisticsUC : UserControl
    {
        public LogStatistics Statistics { get; set; }
        private static Color[] colors;
        private PieChartUC GlobalPie;
        private PieChartUC SourcePie;
        private PieChartUC ModulePie;

        public LogStatisticsUC()
        {
            InitializeComponent();
            colors = new Color[]
            {
                Color.Aqua, Color.Blue, Color.Green, Color.Red, Color.Goldenrod, Color.Purple, Color.MediumPurple,
                Color.OrangeRed, Color.Fuchsia
            };

        }

        private void CreatePies()
        {
            GlobalPie = new PieChartUC();
            spltCTop.Panel2.Controls.Add(GlobalPie);
            GlobalPie.Dock = DockStyle.Fill;
            GlobalPie.SetDataSources(Statistics.CalculateGlobalStatistics());


        }

        private void LogStatisticsUC_Load(object sender, System.EventArgs e)
        {
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            if (Statistics == null) return;
            CreatePies();
            PopulateGlobalDataGridView();
            PopulateSource();
            PopulateModule();
        }

        private void PopulateModule()
        {
            var modules = Statistics.CalculateModulesStatistics().OrderByDescending(s => s.TotalMessages).ToList();

            ModulePie = new PieChartUC();
            spltcModules.Panel2.Controls.Add(ModulePie);
            ModulePie.Dock = DockStyle.Fill;
            ModulePie.SetDataSources(modules.First());

            dgvModules.SelectionChanged -= dgvModules_SelectionChanged;
            dgvModules.DataSource = modules;
            dgvModules.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvModules.SelectionChanged += dgvModules_SelectionChanged;
        }

        private void PopulateSource()
        {
            var sources = Statistics.CalculateSourcesStatistics().OrderByDescending(s => s.TotalMessages).ToList();

            SourcePie = new PieChartUC();
            spltcSources.Panel2.Controls.Add(SourcePie);
            SourcePie.Dock = DockStyle.Fill;
            SourcePie.SetDataSources(sources.First());

            dgvSource.SelectionChanged -= dgvSource_SelectionChanged;
            dgvSource.DataSource = sources;
            dgvSource.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvSource.SelectionChanged += dgvSource_SelectionChanged;

        }

        public void RefreshStatistics(LogStatistics statistics)
        {
            Statistics = statistics;
            LoadStatistics();
        }

        private void PopulateGlobalDataGridView()
        {
            ItemStatistics item = Statistics.CalculateGlobalStatistics();
            dgvTop.Rows.Add("Total messages:", item.TotalMessages);
            dgvTop.Rows.Add("Events:", item.Events);
            dgvTop.Rows.Add("Errors:", item.Errors);
            dgvTop.Rows.Add("Warnings:", item.Warnings);
            dgvTop.Rows.Add("Criticals:", item.Critical);
            dgvTop.Rows.Add("Debug:", item.Debug);
            dgvTop.Rows.Add("Verbose:", item.Verbose);

        }

        private void dgvSource_SelectionChanged(object sender, System.EventArgs e)
        {
            if (dgvSource.SelectedRows.Count == 0) return;
            if (dgvSource.SelectedRows[0].DataBoundItem is ItemStatistics entry)
            {
                SourcePie.SetDataSources(entry);
            }
        }

        private void dgvSource_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvModules_SelectionChanged(object sender, System.EventArgs e)
        {
            if (dgvModules.SelectedRows.Count == 0) return;
            if (dgvModules.SelectedRows[0].DataBoundItem is ItemStatistics entry)
            {
                ModulePie.SetDataSources(entry);
            }
        }
    }
}