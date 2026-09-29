Feature: Test Flow
# https://github.com/karatelabs/karate/issues/1191
# https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

Background:
* header Content-Type = 'application/json'

Scenario: Take and return book flow

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

    * def firstKnowledgeAreaId = response.knowledgeAreas[0].id
    * def firstKnowledgeAreaName = response.knowledgeAreas[0].name

    # Create a new book
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

    * def newBookId = response.newBookId

    # Check the created books using the endpoint to get a list of books
    When method GET
    Then status 200
    And match response.books contains
    """
    {
        "id": "#(newBookId)",
        "title": "#(randomName)",
        "annotation": "Test annotation",
        "language": "en",
        "authors": [{
            "fullName": "Author Name"
        }],
        "knowledgeAreas": [{
            "id": #(firstKnowledgeAreaId),
            "name": #(firstKnowledgeAreaName)
        }],
        "coverUrl": "http://example.com/artwork.jpg",
        "availabilityStatuses": ["InOffice"]
    }
    """

    # Check the created books using the endpoint to get a books by id
    And path newBookId
    When method GET
    Then status 200
    And match response contains
    """
    {
        "id": "#(newBookId)",
        "title": "#(randomName)",
        "annotation": "Test annotation",
        "language": "en",
        "authors": [{
            "fullName": "Author Name"
        }],
        "knowledgeAreas": [{
            "id": #(firstKnowledgeAreaId),
            "name": #(firstKnowledgeAreaName)
        }],
        "coverUrl": "http://example.com/artwork.jpg",
        "availabilityStatuses": ["InOffice"]
    }
    """
    And assert response.bookCopiesIds.length == 1
    And assert response.employeesWhoReadNow.length == 0

    * def bookCopyId = response.bookCopiesIds[0]

    # Take book copy by copy ID
    * def scheduledReturnDate = jsUtils().getDateTwoMonthsLaterThanCurrent()

    And path '/take'
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
    And path newBookId
    When method GET
    Then status 200
    And assert response.employeesWhoReadNow.length == 1
    And assert response.employeesWhoReadNow[0].employeeId == employeeId
    And assert response.employeesWhoReadNow[0].bookCopyId == bookCopyId
    And assert response.availabilityStatuses == "OnHand"

    * def readerFullName = response.employeesWhoReadNow[0].fullName
    * def progressOfReading = 'ReadEntirely'
    * def rating = 5
    * def advantages = "Good book"
    * def disadvantages = "Long book"

    # Return book copy
    And path '/return'
    And request
    """
    {
        "bookCopyId": '#(bookCopyId)',
        "progressOfReading": '#(progressOfReading)',
        "rating": '#(rating)',
        "advantages": '#(advantages)',
        "disadvantages": '#(disadvantages)',
    }
    """
    When method POST
    Then status 200

    # Check that book has zero employeesWhoReadNow
    And path newBookId
    When method GET
    Then status 200
    And assert response.employeesWhoReadNow.length == 0

    # Check that book has feedback
    And path newBookId, '/feedback'
    When method GET
    Then status 200
    And assert response.bookFeedback[0].employeeFullName == readerFullName
    And assert response.bookFeedback[0].progressOfReading == progressOfReading
    And assert response.bookFeedback[0].rating == rating
    And assert response.bookFeedback[0].advantages == advantages
    And assert response.bookFeedback[0].disadvantages == disadvantages

    # Check book history
    And path newBookId, '/history'
    And param page = 1
    And param pageSize = 10
    When method GET
    Then status 200    
    And assert response.totalCount == 1
    And assert response.list.length == 1
    And assert response.list[0].employeeFullName == readerFullName
    And assert response.list[0].bookCopyId == bookCopyId
    
    # Cleanup: Delete the book (hard delete)
    And path newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }