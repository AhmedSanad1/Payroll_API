using FluentAssertions;
using PayRollApi.Application.DTOs.Payroll;
using PayRollApi.Application.Interfaces;
using PayRollApi.Application.Validators.Payroll;

public class EmployeeDtoValidatorTests
{
    private static readonly EmployeeDtoValidator Validator = new(new StubLocalizer());

    [Fact]
    public void Valid_employee_passes()
    {
        var dto = MakeDto(hireDate: new DateOnly(2023, 1, 1), birthDate: new DateOnly(2000, 1, 1));

        Validator.Validate(dto).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Rejects_hire_date_in_the_future()
    {
        var dto = MakeDto(hireDate: DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1), birthDate: new DateOnly(2000, 1, 1));

        Validator.Validate(dto).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Rejects_employee_one_day_short_of_18_at_hire_date()
    {
        var hireDate = new DateOnly(2020, 1, 1);
        var birthDate = hireDate.AddYears(-18).AddDays(1);

        Validator.Validate(MakeDto(hireDate, birthDate)).IsValid.Should().BeFalse();
    }

    [Fact]
    public void Accepts_employee_exactly_18_at_hire_date()
    {
        var hireDate = new DateOnly(2020, 1, 1);
        var birthDate = hireDate.AddYears(-18);

        Validator.Validate(MakeDto(hireDate, birthDate)).IsValid.Should().BeTrue();
    }

    [Fact]
    public void Rejects_hire_date_before_birth_date()
    {
        var dto = MakeDto(hireDate: new DateOnly(1990, 1, 1), birthDate: new DateOnly(2000, 1, 1));

        Validator.Validate(dto).IsValid.Should().BeFalse();
    }

    private static EmployeeDto MakeDto(DateOnly hireDate, DateOnly birthDate) => new()
    {
        FullName = "Test Employee",
        Address = "123 Street",
        Phone = "0100000000",
        Email = "test@example.com",
        JobGradeId = 1,
        DepartmentId = 1,
        HireDate = hireDate,
        BirthDate = birthDate
    };

    private class StubLocalizer : ILocalizer
    {
        public string Get(string key) => key;
    }
}
