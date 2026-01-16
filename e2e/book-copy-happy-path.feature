Feature: Book copy

Background:
* header Content-Type = 'application/json'

Scenario: Happy Path

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authLogin = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_LOGIN_WITH_ALL_PERMISSIONS')
    * def authPassword = jsUtils().getEnvVariable('AUTH_FIRST_TENANT_PASSWORD_WITH_ALL_PERMISSIONS')

    # Authentication
    Given url authApiRootUrl
    And path '/login'
    And request
    """
    {
        "login": "#(authLogin)",
        "password": "#(authPassword)"
    }
    """
    And method POST
    Then status 200

    * def accessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(accessToken)

    * def employeeId = jsUtils().getEmployeeIdFromToken(accessToken)

    Given url apiRootUrl
    And path '/knowledge-areas'
    When method GET
    Then status 200

    * def  firstKnowledgeAreaId = response.knowledgeAreas[0].id

    # Create a new book with 1 copy
    * def randomName = 'Test-book-' + Math.random()

    Given url apiRootUrl
    And request
    """
    {
        title: '#(randomName)',
        annotation: 'Test annotation',
        language: 'en',
        knowledgeAreasIds: [#(firstKnowledgeAreaId)],
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

    * def bookId = response.newBookId

    # Get book and copies by bookId
    And path '/copies', bookId
    When method GET
    Then status 200
    And assert response.bookCopies.length == 1
    And match response.bookTitle == randomName
    And match response.bookCopies[0].bookCopyId == "#notnull"
    And match response.bookCopies[0].secretKey == "#notnull" 

    * def bookCopyId = response.bookCopies[0].bookCopyId
    * def secretKey = response.bookCopies[0].secretKey

    # Get book copy by bookCopyId
    And path '/copy', bookCopyId
    And param secretKey = secretKey
    When method GET
    Then status 200
    And match response.title == randomName
    And match response.annotation == 'Test annotation'
    And match response.language == 'en'

    # Cleanup: Delete the book (hard delete)
    And path bookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
