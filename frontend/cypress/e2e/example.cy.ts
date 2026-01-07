// Smoke test to verify the app loads
describe('App Smoke Test', () => {
  beforeEach(() => {
    // Mock API to avoid backend dependency
    cy.intercept('GET', '/api/info', {
      statusCode: 200,
      body: { text: 'API is running' }
    }).as('getInfo')
  })

  it('should load the application', () => {
    cy.visit('/')
    cy.get('header').should('be.visible')
    cy.get('main').should('be.visible')
  })

  it('should display the page title', () => {
    cy.visit('/')
    cy.get('h4').should('contain.text', 'Extract Text from PDF')
  })

  it('should have working navigation structure', () => {
    cy.visit('/')
    cy.get('header nav').should('be.visible')
    cy.get('header nav a').should('have.length.at.least', 3)
  })
})
