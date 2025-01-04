using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using POS.Models;
using POS.Services.DAO;
using POS.Interfaces;

public class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<RevenueReport> _reports;
    private ObservableCollection<RevenueChartData> _chartData;

    public ObservableCollection<RevenueReport> Reports
    {
        get => _reports;
        set { _reports = value; OnPropertyChanged(); }
    }

    public ObservableCollection<RevenueChartData> ChartData
    {
        get => _chartData;
        set { _chartData = value; OnPropertyChanged(); }
    }

    public MainViewModel()
    {
        Reports = new ObservableCollection<RevenueReport>(GetSampleData());
        ChartData = new ObservableCollection<RevenueChartData>();
    }

    private List<RevenueReport> GetSampleData()
    {
        return new List<RevenueReport>
        {
            
        };
    }

    public void FilterReports(string filterType)
    {
        IEnumerable<RevenueReport> filteredReports = null;

        switch (filterType)
        {
            case "Daily":
                filteredReports = Reports.Where(r => r.ReportDate.Date == DateTime.Today);
                break;

            case "Weekly":
                var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                filteredReports = Reports.Where(r => r.ReportDate.Date >= startOfWeek);
                break;

            case "Monthly":
                filteredReports = Reports.Where(r => r.ReportDate.Month == DateTime.Today.Month && r.ReportDate.Year == DateTime.Today.Year);
                break;
        }

        UpdateChartData(filteredReports);
    }

    private void UpdateChartData(IEnumerable<RevenueReport> reports)
    {
        ChartData.Clear();

        var chartData = reports.GroupBy(r => r.ReportDate.Date)
                                .Select(g => new RevenueChartData { Date = g.Key, TotalRevenue = g.Sum(r => r.Revenue) });

        foreach (var dataPoint in chartData)
        {
            ChartData.Add(dataPoint);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}