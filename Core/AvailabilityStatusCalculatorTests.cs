using Core.Entities;
using Xunit;

namespace Core;

public class AvailabilityStatusCalculatorTests
{
  public static TheoryData<int, int, List<EmployeeWhoReadsNow>, List<AvailabilityStatus>> CalculateTestData()
  {
    return new TheoryData<int, int, List<EmployeeWhoReadsNow>, List<AvailabilityStatus>>
    {
        // 1. Should return list with InOffice when nobody is reading book now
        {
          1, // currentEmployeeId
          1, // totalBookCopies
          new List<EmployeeWhoReadsNow>(), // employeeWhoReadsNow
          new List<AvailabilityStatus>
          {
            AvailabilityStatus.InOffice  // expected
          }
        },

        // 2. Should return list with OnHand when somebody is reading book now and book has no available copies
        {
          1,
          1,
          new List<EmployeeWhoReadsNow>
          {
            new EmployeeWhoReadsNow
            {
              EmployeeId = 3,
              FullName = "Test",
              BookCopyId = 1
            }
          },
          new List<AvailabilityStatus>
          {
            AvailabilityStatus.OnHand
          }
        },

        // 3. Should return list with InOffice and OnHand when somebody is reading book now and book has available copies
        {
            1,
            2,
            new List<EmployeeWhoReadsNow>
            {
              new EmployeeWhoReadsNow
              {
                EmployeeId = 3,
                FullName = "Test",
                BookCopyId = 1
              }
            },
            new List<AvailabilityStatus>
            {
              AvailabilityStatus.InOffice,
              AvailabilityStatus.OnHand
            }
        },

        // 4. Should return list with OnYou when you are reading book now and book has no available copies
        {
          1,
          1,
          new List<EmployeeWhoReadsNow>
          {
            new EmployeeWhoReadsNow
            {
              EmployeeId = 1,
              FullName = "Test",
              BookCopyId = 1
            }
          },
          new List<AvailabilityStatus>
          {
            AvailabilityStatus.OnYou
          }
        },

        // 5. Should return list with InOffice and OnYou when you are reading book now and book has available copies
        {
          1,
          2,
          new List<EmployeeWhoReadsNow>
          {
            new EmployeeWhoReadsNow
            { 
              EmployeeId = 1,
              FullName = "Test",
              BookCopyId = 1
            }
          },
          new List<AvailabilityStatus>
          {
            AvailabilityStatus.InOffice,
            AvailabilityStatus.OnYou
          }
        }
    };
  }

  [Theory]
  [MemberData(nameof(CalculateTestData))]
  public void Calculate_ShouldReturnExpectedStatuses(
    int currentEmployeeId,
    int totalBookCopies,
    List<EmployeeWhoReadsNow> employeeWhoReadsNow,
    List<AvailabilityStatus> expected
  )
  {
    var result = AvailabilityStatusCalculator.Calculate(
      totalBookCopies,
      employeeWhoReadsNow,
      currentEmployeeId
    );

    Assert.Equal(expected, result);
  }
}
