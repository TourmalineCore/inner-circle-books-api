Feature: Test Flow
# https://github.com/karatelabs/karate/issues/1191
# https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

Background:
* header Content-Type = 'application/json'

Scenario: CRUD operations test flow

    * def jsUtils = read('./js-utils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authLogin = jsUtils().getEnvVariable('AUTH_LOGIN')
    * def authPassword = jsUtils().getEnvVariable('AUTH_PASSWORD')

    # Authentication
    Given url authApiRootUrl
    And path '/auth/login'
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

    Given url apiRootUrl
    And path 'api/books/knowledge-areas'
    When method GET
    Then status 200

    * def  firstKnowledgeAreaId = response.knowledgeAreas[0].id
    * def  firstKnowledgeAreaName = response.knowledgeAreas[0].name
    * def  secondKnowledgeAreaId = response.knowledgeAreas[1].id

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
        knowledgeAreasIds: [#(firstKnowledgeAreaId), #(secondKnowledgeAreaId)],
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

    # Check that the book is created and there are 2 book copies
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And match response.title == randomName
    And assert response.bookCopiesIds.length == 2
    # let's check that name matches only for one knowledge area, we don't need to check for all
    And match response.knowledgeAreas contains { id: #(firstKnowledgeAreaId), name: '#(firstKnowledgeAreaName)' }
    And match response.knowledgeAreas[*].id contains secondKnowledgeAreaId

    # Edit the book's details
    * def editedName = 'Test-edited-book' + Math.random()

    And path 'api/books', newBookId, 'edit'
    And request
    """
    {
        title: '#(editedName)',
        annotation: 'Edited annotation',
        language: 'ru',
        authors: [
            {
                fullName: 'Edited Author'
            }
        ],
        coverUrl: 'http://example.com/edited-artwork.jpg'
    }
    """
    When method POST
    Then status 200

    # Get the edited book by ID and verify the details have changed
    And path 'api/books', newBookId
    When method GET
    Then status 200
    And match response.title == editedName

    # Delete the book (soft delete)
    And path 'api/books', newBookId, 'soft-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Verify the book is no longer in the list
    And path 'api/books'
    When method GET
    Then status 200
    And match response.books !contains { id: '#(newBookId)' }

    # Delete the book (hard delete)
    And path 'api/books', newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
