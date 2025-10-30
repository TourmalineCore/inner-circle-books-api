Feature: Test Flow
# https://github.com/karatelabs/karate/issues/1191
# https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

Background:
* header Content-Type = 'application/json'

Scenario: Take and return book flow

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def firstUserAuthLogin = jsUtils().getEnvVariable('AUTH_LOGIN')
    * def firstUserAuthPassword = jsUtils().getEnvVariable('AUTH_PASSWORD')
    * def secondUserAuthLogin = jsUtils().getEnvVariable('SECOND_USER_AUTH_LOGIN')
    * def secondUserAuthPassword = jsUtils().getEnvVariable('SECOND_USER_AUTH_PASSWORD')

    # First user authentication 
    Given url authApiRootUrl
    And path '/auth/login'
    And request
    """
    {
        "login": "#(firstUserAuthLogin)",
        "password": "#(firstUserAuthPassword)"
    }
    """
    And method POST
    Then status 200

    * def accessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(accessToken)

    * def employeeId = jsUtils().getEmployeeIdFromToken(accessToken)

    # Create a new book
    * def randomName = 'Test-book-' + Math.random()

    Given url apiRootUrl
    And path 'api/books'
    And request
    """
    {
        title: '#(randomName)',
        annotation: 'Test annotation',
        language: 'en',
        authors: [
            {
                fullName: 'Author Name'
            }
        ],
        coverUrl: 'http://example.com/artwork.jpg',
        countOfCopies: 1
    }
    """
    When method POST
    Then status 200
    And match response.newBookId == '#number'

    * def newBookId = response.newBookId

    # Get book data to get book copy ID
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And match response.title == randomName
    And assert response.bookCopiesIds.length == 1
    And assert response.employeesWhoReadNow.length == 0

    * def bookCopyId = response.bookCopiesIds[0]

    # Take book copy by copy ID
    * def scheduledReturnDate = jsUtils().getDateTwoMonthsLaterThanCurrent()

    And path 'api/books/take'
    And request
    """
    {
        bookCopyId: '#(bookCopyId)',
        scheduledReturnDate: '#(scheduledReturnDate)'
    }
    """
    When method POST
    Then status 200

    # Check that book has one reader
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And assert response.employeesWhoReadNow.length == 1
    And assert response.employeesWhoReadNow[0].employeeId == employeeId
    And assert response.employeesWhoReadNow[0].bookCopyId == bookCopyId

    # First user authentication
    Given url authApiRootUrl
    And path '/auth/login'
    And request
    """
    {
        "login": '#(firstUserAuthLogin)',
        "password": '#(firstUserAuthPassword)'
    }
    """
    And method POST
    Then status 200

    * def accessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(accessToken)

    # Check that book still has the same reader
    Given url apiRootUrl
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And assert response.employeesWhoReadNow.length == 1
    And assert response.employeesWhoReadNow[0].employeeId == employeeId

    * def readerFullName = response.employeesWhoReadNow[0].fullName

    # Return book copy
    And path 'api/books/return'
    And request
    """
    {
        "bookCopyId": '#(bookCopyId)',
        "progressOfReading": 'ReadEntirely'
    }
    """
    When method POST
    Then status 200

    # Check that book has zero employeesWhoReadNow
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And assert response.employeesWhoReadNow.length == 0

    # Check book history
    And path 'api/books/history', newBookId
    And param page = 1
    And param pageSize = 10
    When method GET
    Then status 200    
    And assert response.totalCount == 1
    And assert response.list.length == 1
    And assert response.list[0].employeeFullName == readerFullName
    And assert response.list[0].bookCopyId == 1

    * def bookCopyReadingHistoryId = response.list[0].id
    
    # Cleanup: Delete the book copy reading history (hard delete)
    And path 'api/books/history', bookCopyReadingHistoryId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup: Delete the book copy (hard delete)
    Given path 'api/books/copy', bookCopyId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup: Delete the book (hard delete)
    And path 'api/books', newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }