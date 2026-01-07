describe('PDF View Page', () => {
  const mockDocument = {
    filename: 'test-document.pdf',
    pageNames: ['1', '2', '3'],
    pages: [],
    footnotes: [],
    fontGroups: ['12', '14', '16'],
    fontNames: 'Arial, Times',
    textContent: 'Sample extracted text'
  }

  const mockPage = {
    nr: 1,
    pageNumberBlockId: null,
    headerBlockIds: [],
    boundingBox: {
      bottomLeftX: 0,
      bottomLeftY: 0,
      topRightX: 612,
      topRightY: 792
    },
    blocks: [
      {
        id: 1,
        boundingBox: { bottomLeftX: 50, bottomLeftY: 700, topRightX: 500, topRightY: 750 },
        words: [
          {
            id: 1,
            text: 'Hello',
            fontName: 'Arial',
            boundingBox: { bottomLeftX: 50, bottomLeftY: 700, topRightX: 100, topRightY: 720 },
            baseLineY: 705,
            orientation: 0,
            letters: []
          }
        ],
        lines: [
          {
            boundingBox: { bottomLeftX: 50, bottomLeftY: 700, topRightX: 500, topRightY: 720 },
            baseLineY: 705,
            topDistance: 0,
            fontSizeAvg: 12,
            words: []
          }
        ]
      }
    ],
    imageBlocks: []
  }

  beforeEach(() => {
    cy.intercept('GET', '/api/getDocument*', {
      statusCode: 200,
      body: mockDocument
    }).as('getDocument')

    cy.intercept('GET', '/api/getPage*', {
      statusCode: 200,
      body: mockPage
    }).as('getPage')
  })

  it('should load the PDF view page', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')
    cy.get('.pdfview').should('exist')
  })

  it('should display the filename', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')
    cy.contains('test-document.pdf').should('be.visible')
  })

  it('should display page navigation for multi-page documents', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')
    // Page number input should exist
    cy.get('input[type="number"]').should('exist')
  })

  it('should display font information', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')
    cy.contains('Font-Sizes').should('be.visible')
    cy.contains('Fonts').should('be.visible')
  })

  it('should have download buttons', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')
    cy.contains('button', 'JSON').should('be.visible')
    cy.contains('button', 'TXT').should('be.visible')
  })

  it('should have view settings section', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')
    cy.contains('Anzeigen').should('be.visible')
  })

  it('should have canvas elements for rendering', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')
    cy.get('canvas').should('have.length.at.least', 1)
  })

  it('should navigate between pages using arrows', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')

    // Right arrow should be visible for multi-page docs
    cy.get('.nextcol').should('exist')
  })

  it('should handle document not found', () => {
    cy.intercept('GET', '/api/getDocument*', {
      statusCode: 404,
      body: { error: 'Not found' }
    }).as('getDocumentNotFound')

    cy.visit('/pdf/nonexistent/1')
    // Should show error toast
    cy.get('an-toasts').should('exist')
  })

  it('should refresh view when clicking reload button', () => {
    cy.visit('/pdf/test-folder/1')
    cy.wait('@getDocument')

    // Click reload button
    cy.get('button[title="reload"]').click()

    // Should fetch document again
    cy.wait('@getDocument')
  })
})
