using Core.Entities;

namespace Core;

public class BookAvailabilityStatusCalculator
{
  public static List<BookAvailabilityStatus> Calculate(
    int totalBookCopies,
    List<EmployeeWhoReadsNow> employeesWhoReadNow,
    long currentEmployeeId
  )
  {
    var availabilityStatuses = new List<BookAvailabilityStatus>();

    var readersCount = employeesWhoReadNow.Count();

    var hasAvailableCopies = totalBookCopies > readersCount;

    var isCurrentEmployeeReader = employeesWhoReadNow.Any(x => x.EmployeeId == currentEmployeeId);

    var isSomebodyEmployeeReader = readersCount > 0; 

    if (hasAvailableCopies)
    {
      availabilityStatuses.Add(BookAvailabilityStatus.InOffice);
    }
    
    if (isCurrentEmployeeReader)
    {
      availabilityStatuses.Add(BookAvailabilityStatus.OnYou);
    }
    else if (isSomebodyEmployeeReader)
    {
      availabilityStatuses.Add(BookAvailabilityStatus.OnHand);
    }

    return availabilityStatuses;
  }
}
