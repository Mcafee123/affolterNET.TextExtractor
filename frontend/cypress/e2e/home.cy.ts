describe('Home Page', () => {
  beforeEach(() => {
    cy.intercept('GET', '/api/info', {
      statusCode: 200,
      body: { text: 'TextExtractor API v1.0' }
    }).as('getInfo')

    cy.visit('/')
  })

  it('should display the main heading', () => {
    cy.get('h4').should('contain.text', 'Extract Text from PDF')
  })

  it('should call the info API on load', () => {
    cy.wait('@getInfo')
  })

  it('should display the API response message', () => {
    cy.wait('@getInfo')
    cy.get('h6').should('contain.text', 'TextExtractor API v1.0')
  })

  it('should show loader while fetching data', () => {
    // Delay the API response to see the loader
    cy.intercept('GET', '/api/info', {
      statusCode: 200,
      body: { text: 'Delayed response' },
      delay: 500
    }).as('getInfoDelayed')

    cy.visit('/')
    // The loader component should be present in the DOM
    cy.get('an-loader').should('exist')
  })

  it('should handle API errors gracefully', () => {
    cy.intercept('GET', '/api/info', {
      statusCode: 500,
      body: { error: 'Internal Server Error' }
    }).as('getInfoError')

    cy.visit('/')
    // Page should still render even if API fails
    cy.get('h4').should('contain.text', 'Extract Text from PDF')
  })
})
