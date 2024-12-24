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
    * def randomName = 'Book-' + Math.random()
    Given url apiRootUrl
    Given path 'api/books'
    And request { title: '#(randomName)', annotation: 'Test annotation', language: 'en', authors: [{fullName: 'Author Name'}], artworkUrl: 'http://example.com/artwork.jpg' }
    When method POST
    Then status 200
    And match response.newBookId == '#number'
    * def newBookId = response.newBookId

    # Step 2: Check that the book is created
    Given path 'api/books', newBookId
    When method GET
    Then status 200
    And match response.title == randomName

    # Step 3: Update the book's details
    * def updatedName = 'Updated-' + Math.random()
    Given path 'api/books', newBookId, 'edit'
    And request { title: '#(updatedName)', annotation: 'Updated annotation', language: 'ru', authors: [{fullName:'Updated Author'}], artworkUrl: 'http://example.com/updated-artwork.jpg' }
    When method POST
    Then status 200

    # Step 4: Get the updated book by ID and verify the details have changed
    Given path 'api/books', newBookId
    When method GET
    Then status 200
    And match response.title == updatedName
    And match response.annotation == 'Updated annotation'
    And match response.language == 'ru'
    And match response.authors == [{"fullName":"Updated Author"}]
    And match response.artworkUrl == 'http://example.com/updated-artwork.jpg'

    # Step 5: Delete the book (hard delete)
    Given path 'api/books', newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Step 6: Verify the book is no longer in the list
    Given url apiRootUrl
    Given path 'api/books'
    When method GET
    Then status 200
    And match response.books !contains { id: '#(newBookId)' }
