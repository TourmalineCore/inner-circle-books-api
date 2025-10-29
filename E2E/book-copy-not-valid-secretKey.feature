Feature: Book copy

Background:
* header Content-Type = 'application/json'

Scenario: SecretKey is not valid

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def firstUserAuthLogin = jsUtils().getEnvVariable('AUTH_LOGIN')
    * def firstUserAuthPassword = jsUtils().getEnvVariable('AUTH_PASSWORD')

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


    # Create a new book with 2 copies
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

    * def bookId = response.newBookId

    # Get book and copies by bookId
    And path 'api/books/copies', bookId
    When method GET
    Then status 200
    And assert response.bookCopies.length == 2
    And match response.bookTitle == randomName
    And match response.bookCopies[0].bookCopyId == "#notnull"
    And match response.bookCopies[0].secretKey == "#notnull" 

    * def bookCopyId = response.bookCopies[0].bookCopyId

    # Try get book copy with not valid secretKey
    And path 'api/books/copy', bookCopyId
    And param secretKey = 'notValid'
    When method GET
    Then status 404

    # Cleanup: Delete the book (hard delete)
    And path 'api/books', bookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Cleanup: Delete the book copy (hard delete)
    Given path 'api/books/copy', bookCopyId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }