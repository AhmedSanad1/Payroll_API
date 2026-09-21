using System.Net;
using FluentAssertions;
using PayRollApi.Application.DTOs;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Helpers;
using PayRollApi.Domain.Enums;

namespace PayRollApi.IntegrationTests;

// One long walk through the whole system, in order, so each step builds on the last.
public class FullPayrollLifecycleTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task FullPayrollLifecycle_WorksEndToEnd()
    {
        // Turn off auto cookies — otherwise the client would re-send the refresh token and hide the reuse tests.
        var client = factory.CreateClient(new() { BaseAddress = new Uri("https://localhost"), HandleCookies = false });

        // 1. Protected endpoints reject anonymous callers.
        (await client.GetAsync("/api/departments")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await client.GetAsync("/api/job-grades")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        (await client.GetAsync("/api/absences/month?year=2026&month=1")).StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 2. Wrong credentials are rejected, correct credentials succeed.
        var badLogin = await client.PostJsonAsync<LoggedUser>("/api/auth/login",
            new { Username = CustomWebApplicationFactory.AdminUsername, Password = "wrong-password" });
        badLogin.Status.Should().Be(HttpStatusCode.Unauthorized);

        var loginResponse = await client.PostAsJsonAsyncTracked<LoggedUser>("/api/auth/login",
            new { Username = CustomWebApplicationFactory.AdminUsername, Password = CustomWebApplicationFactory.AdminPassword });
        loginResponse.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        loginResponse.Body!.Object!.Username.Should().Be(CustomWebApplicationFactory.AdminUsername);
        loginResponse.Body.Object.AccessToken.Should().NotBeNullOrWhiteSpace();

        var accessToken = loginResponse.Body.Object.AccessToken;
        var refreshToken = loginResponse.Response.ExtractCookieValue("refreshToken");
        client.SetBearerToken(accessToken);
        client.SetRefreshCookie(refreshToken);

        // 3. Me reflects the authenticated admin.
        var me = await client.GetJsonAsync<CurrentUserDto>("/api/auth/me");
        me.Status.Should().Be(HttpStatusCode.OK);
        me.Body!.Object!.Username.Should().Be(CustomWebApplicationFactory.AdminUsername);

        // 4. Reference data seeded via HasData is present.
        var jobGrades = await client.GetJsonAsync<ICollection<JobGradeDto>>("/api/job-grades");
        jobGrades.Body!.Object.Should().HaveCount(3);
        var firstGrade = jobGrades.Body.Object!.Single(g => g.Id == 1);
        firstGrade.BaseSalary.Should().Be(12000m);

        var tiers = await client.GetJsonAsync<ICollection<ServiceIncentiveTierDto>>("/api/service-incentive-tiers");
        tiers.Body!.Object.Should().HaveCount(2);

        var rules = await client.GetJsonAsync<ICollection<AttendanceRuleDto>>("/api/attendance-rules");
        rules.Body!.Object.Should().HaveCount(5);

        // 5. Business-rule conflicts: overlapping attendance range, duplicate service-tier years.
        var overlapping = await client.PostJsonAsync<AttendanceRuleDto>("/api/attendance-rules",
            new { FromDays = 4, ToDays = 8, AdjustmentType = (int)AttendanceAdjustmentType.Deduction, Percent = 1m });
        overlapping.Status.Should().Be(HttpStatusCode.Conflict);

        var duplicateTier = await client.PostJsonAsync<ServiceIncentiveTierDto>("/api/service-incentive-tiers",
            new { MinYearsExceeded = 5, Percent = 1m });
        duplicateTier.Status.Should().Be(HttpStatusCode.Conflict);

        // 6. Payroll settings default to Additive.
        var settings = await client.GetJsonAsync<PayrollSettingsDto>("/api/payroll-settings");
        settings.Body!.Object!.CalculationMode.Should().Be(PayrollCalculationMode.Additive);

        // 7. Department CRUD, including validation failure.
        var invalidDept = await client.PostJsonAsync<DepartmentDto>("/api/departments", new { Name = "", IncentivePercent = 10m });
        invalidDept.Status.Should().Be(HttpStatusCode.BadRequest);
        invalidDept.Body!.Errors.Should().ContainKey("Name");

        var deptCreate = await client.PostJsonAsync<DepartmentDto>("/api/departments", new { Name = "Engineering", IncentivePercent = 10m });
        deptCreate.Status.Should().Be(HttpStatusCode.OK);
        var departmentId = deptCreate.Body!.Object!.Id;

        var lookup = await client.GetJsonAsync<ICollection<DepartmentLookupDto>>("/api/departments/lookup");
        lookup.Body!.Object.Should().Contain(d => d.Id == departmentId);

        // 8. Employee creation, calibrated so the generated payroll has exact, predictable figures:
        //    6 completed years of service (5% tier) and 4 absences in the period (5% deduction tier).
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var periodMonth = today.AddMonths(-1);
        var periodStart = new DateOnly(periodMonth.Year, periodMonth.Month, 1);
        var periodEnd = periodStart.AddMonths(1).AddDays(-1);
        var hireDate = periodEnd.AddYears(-6);
        var birthDate = hireDate.AddYears(-25);

        var futureHire = await client.PostJsonAsync<EmployeeDto>("/api/employees", new
        {
            FullName = "Future Hire",
            BirthDate = birthDate,
            Address = "1 Test Street",
            Phone = "01000000000",
            Email = "future.hire@payroll.local",
            JobGradeId = 1,
            DepartmentId = departmentId,
            HireDate = today.AddDays(1)
        });
        futureHire.Status.Should().Be(HttpStatusCode.BadRequest);

        var underage = await client.PostJsonAsync<EmployeeDto>("/api/employees", new
        {
            FullName = "Too Young",
            BirthDate = hireDate.AddYears(-10),
            Address = "1 Test Street",
            Phone = "01000000001",
            Email = "too.young@payroll.local",
            JobGradeId = 1,
            DepartmentId = departmentId,
            HireDate = hireDate
        });
        underage.Status.Should().Be(HttpStatusCode.BadRequest);

        var employeeCreate = await client.PostJsonAsync<EmployeeDto>("/api/employees", new
        {
            FullName = "Jane Payroll",
            BirthDate = birthDate,
            Address = "1 Test Street",
            Phone = "01000000002",
            Email = "jane.payroll@payroll.local",
            JobGradeId = 1,
            DepartmentId = departmentId,
            HireDate = hireDate
        });
        employeeCreate.Status.Should().Be(HttpStatusCode.OK);
        var employeeId = employeeCreate.Body!.Object!.Id;

        var duplicateEmail = await client.PostJsonAsync<EmployeeDto>("/api/employees", new
        {
            FullName = "Duplicate Email",
            BirthDate = birthDate,
            Address = "1 Test Street",
            Phone = "01000000003",
            Email = "jane.payroll@payroll.local",
            JobGradeId = 1,
            DepartmentId = departmentId,
            HireDate = hireDate
        });
        duplicateEmail.Status.Should().Be(HttpStatusCode.Conflict);

        // 9. Record exactly 4 absence days inside the target period.
        var absenceDates = Enumerable.Range(0, 4).Select(periodStart.AddDays).ToList();
        var batch = await client.PostJsonAsync<bool>("/api/absences/batch", new
        {
            Add = absenceDates.Select(d => new { EmployeeId = employeeId, Date = d }).ToList(),
            Remove = Array.Empty<object>()
        });
        batch.Status.Should().Be(HttpStatusCode.OK);

        var grid = await client.GetJsonAsync<AbsenceMonthGridDto>($"/api/absences/month?year={periodStart.Year}&month={periodStart.Month}");
        grid.Body!.Object!.IsLocked.Should().BeFalse();
        grid.Body.Object.Employees.Single(e => e.EmployeeId == employeeId).AbsenceDates.Should().HaveCount(4);

        // 10. Generate payroll and verify the calculation engine's output exactly.
        var generate = await client.PostJsonAsync<PayrollRunDetailDto>("/api/payroll-runs",
            new { Year = periodStart.Year, Month = periodStart.Month });
        generate.Status.Should().Be(HttpStatusCode.OK);
        var run = generate.Body!.Object!;
        run.EmployeeCount.Should().Be(1);
        run.Status.Should().Be(PayrollRunStatus.Draft);
        run.TotalBaseSalary.Should().Be(12000m);
        run.TotalDeptIncentive.Should().Be(1200m);
        run.TotalServiceIncentive.Should().Be(600m);
        run.TotalAttendanceAdjustment.Should().Be(-600m);
        run.TotalNetSalary.Should().Be(13200m);
        var runId = run.Id;

        var items = await client.GetJsonAsync<PageList<PayrollItemDto>>($"/api/payroll-runs/{runId}/items");
        items.Body!.Object!.Items.Should().HaveCount(1);
        var item = items.Body.Object.Items[0];
        item.BaseSalary.Should().Be(12000m);
        item.DeptIncentivePercent.Should().Be(10m);
        item.DeptIncentiveAmount.Should().Be(1200m);
        item.ServiceYears.Should().Be(6);
        item.ServiceIncentivePercent.Should().Be(5m);
        item.ServiceIncentiveAmount.Should().Be(600m);
        item.AbsentDays.Should().Be(4);
        item.AttendanceAdjustmentType.Should().Be(AttendanceAdjustmentType.Deduction);
        item.AttendancePercent.Should().Be(5m);
        item.AttendanceAmount.Should().Be(-600m);
        item.NetSalary.Should().Be(13200m);

        var payslip = await client.GetJsonAsync<PayrollItemDto>($"/api/employees/{employeeId}/payslip?runId={runId}");
        payslip.Body!.Object!.NetSalary.Should().Be(13200m);

        // 11. Regenerating a Draft run replaces its items and keeps the same totals.
        var regenerate = await client.PostJsonAsync<PayrollRunDetailDto>("/api/payroll-runs",
            new { Year = periodStart.Year, Month = periodStart.Month });
        regenerate.Status.Should().Be(HttpStatusCode.OK);
        regenerate.Body!.Object!.Id.Should().Be(runId);
        regenerate.Body.Object.TotalNetSalary.Should().Be(13200m);

        // 12. Approve locks the run and the absence month; re-approving or regenerating is rejected.
        var approve = await client.PostJsonAsync<PayrollRunDetailDto>($"/api/payroll-runs/{runId}/approve", new { });
        approve.Status.Should().Be(HttpStatusCode.OK);
        approve.Body!.Object!.Status.Should().Be(PayrollRunStatus.Approved);

        (await client.PostJsonAsync<PayrollRunDetailDto>($"/api/payroll-runs/{runId}/approve", new { })).Status
            .Should().Be(HttpStatusCode.Conflict);

        (await client.PostJsonAsync<PayrollRunDetailDto>("/api/payroll-runs",
            new { Year = periodStart.Year, Month = periodStart.Month })).Status.Should().Be(HttpStatusCode.Conflict);

        var lockedBatch = await client.PostJsonAsync<bool>("/api/absences/batch", new
        {
            Add = new[] { new { EmployeeId = employeeId, Date = periodStart.AddDays(5) } },
            Remove = Array.Empty<object>()
        });
        lockedBatch.Status.Should().Be(HttpStatusCode.Conflict);

        var lockedGrid = await client.GetJsonAsync<AbsenceMonthGridDto>($"/api/absences/month?year={periodStart.Year}&month={periodStart.Month}");
        lockedGrid.Body!.Object!.IsLocked.Should().BeTrue();

        // 13. Reports render without error for the generated data.
        (await client.GetAsync($"/api/reports/employees?departmentId={departmentId}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.GetAsync($"/api/reports/attendance?year={periodStart.Year}&month={periodStart.Month}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.GetAsync($"/api/reports/salaries?runId={runId}")).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.GetAsync($"/api/reports/incentives-deductions?runId={runId}")).StatusCode.Should().Be(HttpStatusCode.OK);

        // 14. Refresh rotates the token; reusing the now-revoked old token is detected and
        //     kills the whole chain, so even the token issued by that rotation stops working.
        var refresh1 = await client.PostAsJsonAsyncTracked<LoggedUser>("/api/auth/refresh", new { });
        refresh1.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        refresh1.Body!.Object!.AccessToken.Should().NotBe(accessToken);
        var rotatedRefreshToken = refresh1.Response.ExtractCookieValue("refreshToken");
        rotatedRefreshToken.Should().NotBe(refreshToken);

        client.SetRefreshCookie(refreshToken); // the old, already-rotated token
        var reuse = await client.PostAsync("/api/auth/refresh", null);
        reuse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        client.SetRefreshCookie(rotatedRefreshToken); // even the freshly-issued token is now dead
        var afterReuse = await client.PostAsync("/api/auth/refresh", null);
        afterReuse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        // 15. Logout revokes the current session's refresh token.
        var relogin = await client.PostAsJsonAsyncTracked<LoggedUser>("/api/auth/login",
            new { Username = CustomWebApplicationFactory.AdminUsername, Password = CustomWebApplicationFactory.AdminPassword });
        client.SetBearerToken(relogin.Body!.Object!.AccessToken);
        client.SetRefreshCookie(relogin.Response.ExtractCookieValue("refreshToken"));

        (await client.PostAsync("/api/auth/logout", null)).StatusCode.Should().Be(HttpStatusCode.OK);
        (await client.PostAsync("/api/auth/refresh", null)).StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
