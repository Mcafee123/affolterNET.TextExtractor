describe('Documents Page', () => {
  const mockDocuments = [
    {
      foldername: 'doc-123',
      filename: 'test-document.pdf',
      created: '2024-01-15T10:30:00Z'
    },
    {
      foldername: 'doc-456',
      filename: 'another-document.pdf',
      created: '2024-01-14T09:00:00Z'
    }
  ]

  beforeEach(() => {
    cy.intercept('GET', '/api/listDocuments', {
      statusCode: 200,
      body: mockDocuments
    }).as('listDocuments')
  })

  it('should display the documents list', () => {
    cy.visit('/documents')
    cy.wait('@listDocuments')
    cy.get('article nav').should('have.length', 2)
  })

  it('should display document filenames', () => {
    cy.visit('/documents')
    cy.wait('@listDocuments')
    cy.contains('test-document.pdf').should('be.visible')
    cy.contains('another-document.pdf').should('be.visible')
  })

  it('should display document creation dates', () => {
    cy.visit('/documents')
    cy.wait('@listDocuments')
    cy.contains('added:').should('exist')
  })

  it('should show "no documents found" when list is empty', () => {
    cy.intercept('GET', '/api/listDocuments', {
      statusCode: 200,
      body: []
    }).as('emptyList')

    cy.visit('/documents')
    cy.wait('@emptyList')
    cy.contains('no documents found').should('be.visible')
  })

  it('should navigate to PDF view when clicking a document', () => {
    cy.intercept('GET', '/api/getDocument*', {
      statusCode: 200,
      body: {
        filename: 'test-document.pdf',
        pageNames: ['1'],
        pages: [],
        footnotes: [],
        fontGroups: [],
        fontNames: ''
      }
    }).as('getDocument')

    cy.intercept('GET', '/api/getPage*', {
      statusCode: 200,
      body: {
        nr: 1,
        pageNumberBlockId: null,
        headerBlockIds: [],
        boundingBox: { bottomLeftX: 0, bottomLeftY: 0, topRightX: 612, topRightY: 792 },
        blocks: [],
        imageBlocks: []
      }
    }).as('getPage')

    cy.visit('/documents')
    cy.wait('@listDocuments')
    cy.contains('test-document.pdf').click()
    cy.url().should('include', '/pdf/doc-123/1')
  })

  it('should delete a document when clicking delete button', () => {
    cy.intercept('DELETE', '/api/deleteDocument/*', {
      statusCode: 200,
      body: {}
    }).as('deleteDocument')

    cy.visit('/documents')
    cy.wait('@listDocuments')

    // Find delete button (last button in the nav row) for first document
    cy.get('article nav').first().find('button').last().click()

    cy.wait('@deleteDocument')
    // Document should be removed from the list
    cy.get('article nav').should('have.length', 1)
  })

  it('should sort documents by creation date (newest first)', () => {
    cy.visit('/documents')
    cy.wait('@listDocuments')

    // First document should be the newest one
    cy.get('article nav').first().should('contain.text', 'test-document.pdf')
  })
})
