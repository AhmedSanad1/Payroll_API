using System.Data;
using System.Reflection;
using Microsoft.Reporting.NETCore;
using PayRollApi.Application.Interfaces.Payroll;

namespace PayRollApi.Infrastructure.Services
{
    public class RdlcReportRenderer : IRdlcReportRenderer
    {
        private static readonly string ReportsDirectory = Path.Combine(AppContext.BaseDirectory, "Reports");

        public byte[] RenderPdf<T>(string reportName, string dataSetName, string reportTitle, IEnumerable<T> rows)
        {
            var reportPath = Path.Combine(ReportsDirectory, $"{reportName}.rdlc");
            if (!File.Exists(reportPath))
                throw new FileNotFoundException($"Report definition not found: {reportPath}", reportPath);

            var report = new LocalReport { ReportPath = reportPath };
            report.DataSources.Add(new ReportDataSource(dataSetName, ToDataTable(rows)));
            report.SetParameters(new ReportParameter("ReportTitle", reportTitle));

            return report.Render("PDF");
        }

        // RDLC chokes on DateOnly/TimeOnly ("#Error"), so widen them here instead of touching the DTOs.
        private static DataTable ToDataTable<T>(IEnumerable<T> rows)
        {
            var properties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.GetIndexParameters().Length == 0)
                .ToArray();

            var table = new DataTable();
            foreach (var property in properties)
                table.Columns.Add(property.Name, ToReportType(property.PropertyType));

            foreach (var row in rows)
            {
                var values = new object[properties.Length];
                for (var i = 0; i < properties.Length; i++)
                    values[i] = ToReportValue(properties[i].GetValue(row));
                table.Rows.Add(values);
            }

            return table;
        }

        private static Type ToReportType(Type type)
        {
            var underlying = Nullable.GetUnderlyingType(type) ?? type;
            if (underlying == typeof(DateOnly)) return typeof(DateTime);
            if (underlying == typeof(TimeOnly)) return typeof(TimeSpan);
            return underlying;
        }

        private static object ToReportValue(object? value) => value switch
        {
            null => DBNull.Value,
            DateOnly date => date.ToDateTime(TimeOnly.MinValue),
            TimeOnly time => time.ToTimeSpan(),
            _ => value
        };
    }
}
