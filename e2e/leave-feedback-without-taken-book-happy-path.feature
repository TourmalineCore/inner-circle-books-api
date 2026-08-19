Feature: Feedback

Background:
* header Content-Type = 'application/json'

# Use case:
# I want to leave a feedback for a book that I've already read.
# I can do this even if I have never taken this book in the service.
# And my feedback is added to feedback list.
Scenario: Leave feedback without taken the book

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

    * def newBookId = response.newBookId
    * def progressOfReading = 'ReadEntirely'
    * def rating = 5
    * def advantages = "Good book"
    * def disadvantages = "Long book"

    # Leave feedback
    And path newBookId, 'feedback'
    And request
    """
     {
        "progressOfReading": '#(progressOfReading)',
        "rating": '#(rating)',
        "advantages": '#(advantages)',
        "disadvantages": '#(disadvantages)'
    }
    """
    When method POST
    Then status 200
    
    * def newFeedbackId = response.newFeedbackId

    # Check that book has feedback
    And path '/feedback', newBookId
    When method GET
    Then status 200
    And assert response.bookFeedback[0].id == newFeedbackId
    # And assert response.bookFeedback[0].employeeFullName == readerFullName
    And assert response.bookFeedback[0].progressOfReading == progressOfReading
    And assert response.bookFeedback[0].rating == rating
    And assert response.bookFeedback[0].advantages == advantages
    And assert response.bookFeedback[0].disadvantages == disadvantages

    # Cleanup: Delete the book with feedback (hard delete)
    And path newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
