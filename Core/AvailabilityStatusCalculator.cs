using Core.Entities;

namespace Core;

public class AvailabilityStatusCalculator
{
  public static List<AvailabilityStatus> Calculate(
    int totalBookCopies,
    List<EmployeeWhoReadsNow> employeeWhoReadsNow,
    long currentEmployeeId
  )
  {
    var availabilityStatuses = new List<AvailabilityStatus>();

    var numberOfReadableCopies = employeeWhoReadsNow
      .Select(x => x.BookCopyId)
      .ToList()
      .Count();

    var hasAvailableCopies = totalBookCopies > numberOfReadableCopies;

    var isCurrentEmployeeReader = employeeWhoReadsNow.Any(x => x.EmployeeId == currentEmployeeId);

    var isSomebodyEmployeeReader = numberOfReadableCopies > 0; 

    if (hasAvailableCopies)
    {
      availabilityStatuses.Add(AvailabilityStatus.InOffice);
    }
    
    if (isCurrentEmployeeReader)
    {
      availabilityStatuses.Add(AvailabilityStatus.OnYou);
    }
    else if (isSomebodyEmployeeReader)
    {
      availabilityStatuses.Add(AvailabilityStatus.OnHand);
    }

    return availabilityStatuses;
  }
}
