Feature: Test Flow
# https://github.com/karatelabs/karate/issues/1191
# https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

Background:
* url 'http://localhost:1080/mockServer/verify'
* header Content-Type = 'application/json'

Scenario: CRUD operations test flow

    * def jsUtils = read('../jsUtils.js')
    * def authApiRootUrl = jsUtils().getEnvVariable('AUTH_API_ROOT_URL')
    * def apiRootUrl = jsUtils().getEnvVariable('API_ROOT_URL')
    * def authLogin = jsUtils().getEnvVariable('AUTH_LOGIN')
    * def authPassword = jsUtils().getEnvVariable('AUTH_PASSWORD')

    # Authentication
    Given url authApiRootUrl
    And path '/auth/login'
    And request {"login": #(authLogin), "password": #(authPassword)}
    And method POST
    Then status 200

    * def accessToken = karate.toMap(response.accessToken.value)

    * configure headers = jsUtils().getAuthHeaders(accessToken)

    # Step 1: Create a new book
    * def randomName = 'Test-book-' + Math.random()
    Given url apiRootUrl
    Given path 'api/books'
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

    # Step 2: Check that the book is created and there are 2 book copies
    Given path 'api/books', newBookId
    When method GET
    Then status 200
    And match response.title == randomName
    And assert response.bookCopiesIds.length == 2
    * def firstBookCopyId = response.bookCopiesIds[0]

    # Step 3: Take book copy by ID
    Given path 'api/books/take'
    And request
    """
    {
        bookCopyId: '#(firstBookCopyId)',
        sсheduledReturnDate: '2025-10-22'
    }
    """
    When method POST
    Then status 200
    And match response.newBookCopyReadingHistoryId == '#number'

    # Step 4: Edit the book's details
    * def editedName = 'Test-edited-book' + Math.random()
    Given path 'api/books', newBookId, 'edit'
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

    # Step 5: Get the edited book by ID and verify the details have changed
    Given path 'api/books', newBookId
    When method GET
    Then status 200
    And match response.title == editedName

    # Step 6: Delete the book (soft delete)
    Given path 'api/books', newBookId, 'soft-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Step 7: Verify the book is no longer in the list
    Given url apiRootUrl
    Given path 'api/books'
    When method GET
    Then status 200
    And match response.books !contains { id: '#(newBookId)' }

    # Step 8: Delete the book (hard delete)
    Given path 'api/books', newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
