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
    var bookAvailabilityStatuses = new List<BookAvailabilityStatus>();

    var readersCount = employeesWhoReadNow.Count;

    var hasAvailableCopies = totalBookCopies > readersCount;

    var isReadByCurrentEmployee = employeesWhoReadNow.Any(x => x.EmployeeId == currentEmployeeId);

    var isReadByAnyone = readersCount > 0; 

    if (hasAvailableCopies)
    {
      bookAvailabilityStatuses.Add(BookAvailabilityStatus.InOffice);
    }
    
    if (isReadByCurrentEmployee)
    {
      bookAvailabilityStatuses.Add(BookAvailabilityStatus.OnYou);
    }
    else if (isReadByAnyone)
    {
      bookAvailabilityStatuses.Add(BookAvailabilityStatus.OnHand);
    }

    return bookAvailabilityStatuses;
  }
}
