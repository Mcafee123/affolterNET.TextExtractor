describe('Navigation', () => {
  beforeEach(() => {
    // Mock API calls to avoid dependency on backend
    cy.intercept('GET', '/api/info', {
      statusCode: 200,
      body: { text: 'API is running' }
    }).as('getInfo')
  })

  it('should display the header navigation', () => {
    cy.visit('/')
    cy.get('header nav').should('be.visible')
  })

  it('should have Home link in navigation', () => {
    cy.visit('/')
    cy.get('header nav a[href="/"]').should('exist')
    cy.get('header nav a[href="/"]').should('contain.text', 'Home')
  })

  it('should have Documents link in navigation', () => {
    cy.visit('/')
    cy.get('header nav a[href="/documents"]').should('exist')
    cy.get('header nav a[href="/documents"]').should('contain.text', 'PDF')
  })

  it('should have Upload link in navigation', () => {
    cy.visit('/')
    cy.get('header nav a[href="/upload"]').should('exist')
    cy.get('header nav a[href="/upload"]').should('contain.text', 'Upload')
  })

  it('should navigate to Documents page', () => {
    cy.intercept('GET', '/api/listDocuments', {
      statusCode: 200,
      body: []
    }).as('listDocuments')

    cy.visit('/')
    cy.get('header nav a[href="/documents"]').click()
    cy.url().should('include', '/documents')
  })

  it('should navigate to Upload page', () => {
    cy.visit('/')
    cy.get('header nav a[href="/upload"]').click()
    cy.url().should('include', '/upload')
  })

  it('should navigate back to Home from Documents', () => {
    cy.intercept('GET', '/api/listDocuments', {
      statusCode: 200,
      body: []
    }).as('listDocuments')

    cy.visit('/documents')
    cy.get('header nav a[href="/"]').click()
    cy.url().should('eq', Cypress.config().baseUrl + '/')
  })
})
