Feature: Test Flow
# https://github.com/karatelabs/karate/issues/1191
# https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

Background:
* url 'http://localhost:1080/mockServer/verify'
* header Content-Type = 'application/json'

Scenario: Take and return book flow

    * def jsUtils = read('../jsUtils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def firstUserAuthLogin = jsUtils().getEnvVariable('AUTH_LOGIN')
    * def firstUserAuthPassword = jsUtils().getEnvVariable('AUTH_PASSWORD')
    * def secondUserAuthLogin = jsUtils().getEnvVariable('SECOND_USER_AUTH_LOGIN')
    * def secondUserAuthPassword = jsUtils().getEnvVariable('SECOND_USER_AUTH_PASSWORD')

    # First user authentication 
    Given url authApiRootUrl
    And path '/api/auth/login'
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
        countOfCopies: 2
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
    And assert response.bookCopiesIds.length == 2
    And assert response.readers.length == 0

    * def firstBookCopyId = response.bookCopiesIds[0]

    # Take book copy by copy ID
    * def scheduledReturnDate = jsUtils().getDateTwoMonthsLaterThanCurrent()

    And path 'api/books/take'
    And request
    """
    {
        bookCopyId: '#(firstBookCopyId)',
        scheduledReturnDate: '#(scheduledReturnDate)'
    }
    """
    When method POST
    Then status 200

    # Check that book has one reader
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And assert response.readers.length == 1
    And assert response.readers[0].employeeId == employeeId

    # First user logout
    Given url authApiRootUrl
    And path '/auth/logout'
    And method GET
    Then status 200

    # Second user authentication
    And path '/api/auth/login'
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

    # Try to take the same book copy by copy ID
    Given url apiRootUrl
    And path 'api/books/take'
    And request
    """
    {
        bookCopyId: '#(firstBookCopyId)',
        scheduledReturnDate: '#(scheduledReturnDate)'
    }
    """
    When method POST
    Then status 500

    # Second user logout
    Given url authApiRootUrl
    And path '/auth/logout'
    And method GET
    Then status 200

    # First user authentication
    And path '/api/auth/login'
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
    And assert response.readers.length == 1
    And assert response.readers[0].employeeId == employeeId

    # Return book copy
    And path 'api/books/return'
    And request
    """
    {
        "bookCopyId": '#(firstBookCopyId)',
        "progressOfReading": 'ReadEntirely'
    }
    """
    When method POST
    Then status 200

    # Check that book has zero readers
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And assert response.readers.length == 0

    # Delete the book (hard delete)
    And path 'api/books', newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }