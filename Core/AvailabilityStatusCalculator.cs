using Core.Entities;

namespace Core;

public class AvailabilityStatusCalculator
{
  public static List<AvailabilityStatus> Calculate(
    int totalBookCopies,
    List<EmployeeWhoReadsNow> employeesWhoReadsNow,
    long currentEmployeeId
  )
  {
    var availabilityStatuses = new List<AvailabilityStatus>();

    var booksBeingReadNowCount = employeesWhoReadsNow.Count();

    var hasAvailableCopies = totalBookCopies > booksBeingReadNowCount;

    var isCurrentEmployeeReader = employeesWhoReadsNow.Any(x => x.EmployeeId == currentEmployeeId);

    var isSomebodyEmployeeReader = booksBeingReadNowCount > 0; 

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
