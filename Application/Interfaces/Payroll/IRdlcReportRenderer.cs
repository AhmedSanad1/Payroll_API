namespace PayRollApi.Application.Interfaces.Payroll
{
    public interface IRdlcReportRenderer
    {
        byte[] RenderPdf<T>(string reportName, string dataSetName, string reportTitle, IEnumerable<T> rows);
    }
}
