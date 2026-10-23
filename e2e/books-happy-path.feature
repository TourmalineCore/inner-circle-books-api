Feature: Books
# https://github.com/karatelabs/karate/issues/1191
# https://github.com/karatelabs/karate?tab=readme-ov-file#karate-fork

Background:
* header Content-Type = 'application/json'

Scenario: Book flow from creation to deletion

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
    * def secondKnowledgeAreaId = response.knowledgeAreas[1].id

    # Create a new book
    * def randomName = 'Test-book-' + Math.random()

    Given url apiRootUrl
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
        "coverUrl": "http://example.com/artwork.jpg",
        "availabilityStatuses": ["InOffice"]
    }
    """
    And assert response.bookCopiesIds.length == 2
    And assert response.employeesWhoReadNow.length == 0
    # let's check that name matches only for one knowledge area, we don't need to check for all
    And match response.knowledgeAreas contains { id: #(firstKnowledgeAreaId), name: '#(firstKnowledgeAreaName)' }
    And match response.knowledgeAreas[*].id contains secondKnowledgeAreaId

    # Add new book copy
    And path newBookId, '/add-copy'
    When method POST
    Then status 200

    # Get book and copies by bookId
    And path newBookId, '/copies'
    When method GET
    Then status 200
    And assert response.bookCopies.length == 3
    And match response.bookTitle == randomName
    And match response.bookCopies[0].bookCopyId == "#number"
    And match response.bookCopies[0].secretKey == "#notnull"
    And match response.bookCopies[1].bookCopyId == "#number"
    And match response.bookCopies[1].secretKey == "#notnull"

    * def bookCopyId = response.bookCopies[0].bookCopyId
    * def secretKey = response.bookCopies[0].secretKey

    # Use case:
    # I want to leave a feedback for a book that I've already read.
    # I can do this even if I have never taken this book in the service.
    # And my feedback is added to feedback list.
    * def progressOfReading = 'ReadEntirely'
    * def rating = 5
    * def advantages = "Good book"
    * def disadvantages = "Long book"

    # Create book feedback
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

    * def newBookFeedbackId = response.newBookFeedbackId

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

    # Get book by book copy id and check that book has one reader
    And path '/copy', bookCopyId
    And param secretKey = secretKey
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
        "coverUrl": "http://example.com/artwork.jpg",
        "availabilityStatuses": ["InOffice", "OnYou"]
    }
    """
    And match response.knowledgeAreas contains { id: #(firstKnowledgeAreaId), name: '#(firstKnowledgeAreaName)' }
    And match response.knowledgeAreas[*].id contains secondKnowledgeAreaId
    And assert response.bookCopiesIds.length == 3
    And assert response.employeesWhoReadNow.length == 1
    And assert response.employeesWhoReadNow[0].employeeId == employeeId
    And assert response.employeesWhoReadNow[0].bookCopyId == bookCopyId

    * def readerFullName = response.employeesWhoReadNow[0].fullName
    * def returnProgressOfReading = 'ReadEntirely'
    * def returnRating = 5
    * def returnAdvantages = "Good book"
    * def returnDisadvantages = "Long book"

    # Return book copy
    And path '/return'
    And request
    """
    {
        "bookCopyId": '#(bookCopyId)',
        "progressOfReading": '#(returnProgressOfReading)',
        "rating": '#(returnRating)',
        "advantages": '#(returnAdvantages)',
        "disadvantages": '#(returnDisadvantages)',
    }
    """
    When method POST
    Then status 200

    # Check that book has feedback: the one left without taking the book and the one left on return
    And path newBookId, '/feedback'
    When method GET
    Then status 200
    And assert response.bookFeedback.length == 2

    # feedback order is not guaranteed, so we find each feedback by id instead of using an index
    * def feedbackWithoutTakingBook = response.bookFeedback.find(x => x.id == newBookFeedbackId)
    * def feedbackOnReturn = response.bookFeedback.find(x => x.id != newBookFeedbackId)

    And match feedbackWithoutTakingBook.employeeFullName == '#string'
    And assert feedbackWithoutTakingBook.progressOfReading == progressOfReading
    And assert feedbackWithoutTakingBook.rating == rating
    And assert feedbackWithoutTakingBook.advantages == advantages
    And assert feedbackWithoutTakingBook.disadvantages == disadvantages

    And assert feedbackOnReturn.employeeFullName == readerFullName
    And assert feedbackOnReturn.progressOfReading == returnProgressOfReading
    And assert feedbackOnReturn.rating == returnRating
    And assert feedbackOnReturn.advantages == returnAdvantages
    And assert feedbackOnReturn.disadvantages == returnDisadvantages

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

    # Edit the book's details
    * def editedName = 'Test-edited-book' + Math.random()

    And path newBookId, 'edit'
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

    # Check the edited and returned book using the endpoint to get a list of books
    When method GET
    Then status 200

    * def editedBook = response.books.find(x => x.id == newBookId)

    And match editedBook contains
    """
    {
        "id": "#(newBookId)",
        "title": "#(editedName)",
        "annotation": "Edited annotation",
        "language": "ru",
        "authors": [{
            "fullName": "Edited Author"
        }],
        "coverUrl": "http://example.com/edited-artwork.jpg",
        "availabilityStatuses": ["InOffice"]
    }
    """
    And match editedBook.knowledgeAreas contains { id: #(firstKnowledgeAreaId), name: '#(firstKnowledgeAreaName)' }
    And match editedBook.knowledgeAreas[*].id contains secondKnowledgeAreaId

    # Delete the book (soft delete)
    And path newBookId, 'soft-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }

    # Verify the book is no longer in the list
    When method GET
    Then status 200
    And assert response.books.filter(x => x.id == newBookId).length == 0

    # Cleanup: Delete the book (hard delete)
    And path newBookId, 'hard-delete'
    When method DELETE
    Then status 200
    And match response == { isDeleted: true }
