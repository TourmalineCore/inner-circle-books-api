Feature: Book copy

Background:
* header Content-Type = 'application/json'

Scenario: Happy Path

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


    # Create a new book with 1 copy
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
        specializations: [1, 2],
        coverUrl: 'http://example.com/artwork.jpg',
        countOfCopies: 1
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
    And assert response.bookCopies.length == 1
    And match response.bookTitle == randomName
    And match response.bookCopies[0].bookCopyId == "#notnull"
    And match response.bookCopies[0].secretKey == "#notnull" 

    * def bookCopyId = response.bookCopies[0].bookCopyId
    * def secretKey = response.bookCopies[0].secretKey

    # Get book copy by bookCopyId
    And path 'api/books/copy', bookCopyId
    And param secretKey = secretKey
    When method GET
    Then status 200
    And match response.title == randomName
    And match response.annotation == 'Test annotation'
    And match response.language == 'en'

    # Cleanup: Delete the book (hard delete)
    And path 'api/books', bookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
