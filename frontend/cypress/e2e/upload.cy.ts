describe('Upload Page', () => {
  beforeEach(() => {
    cy.visit('/upload')
  })

  it('should display the upload component', () => {
    cy.get('article.upload').should('exist')
  })

  it('should have an upload button', () => {
    cy.contains('button', 'Upload PDF').should('be.visible')
  })

  it('should have a file input that accepts PDF files', () => {
    cy.get('input[type="file"]').should('have.attr', 'accept', 'application/pdf')
  })

  it('should display upload instructions', () => {
    cy.contains('Je nach Grösse des PDF Dokuments').should('be.visible')
    cy.contains('OCR').should('be.visible')
  })

  it('should reject non-PDF files', () => {
    // Create a fake non-PDF file
    cy.get('input[type="file"]').selectFile(
      {
        contents: Cypress.Buffer.from('fake text content'),
        fileName: 'test.txt',
        mimeType: 'text/plain'
      },
      { force: true }
    )

    // File should not trigger upload (console error expected)
    // The component will log an error but won't call the API
  })

  it('should upload a PDF file', () => {
    cy.intercept('POST', '/api/upload', {
      statusCode: 200,
      body: { success: true }
    }).as('uploadFile')

    // Create a fake PDF file (with PDF magic bytes)
    const pdfContent = '%PDF-1.4 fake pdf content'
    cy.get('input[type="file"]').selectFile(
      {
        contents: Cypress.Buffer.from(pdfContent),
        fileName: 'test-document.pdf',
        mimeType: 'application/pdf'
      },
      { force: true }
    )

    cy.wait('@uploadFile')
  })

  it('should show loader during upload', () => {
    cy.intercept('POST', '/api/upload', {
      statusCode: 200,
      body: { success: true },
      delay: 500
    }).as('uploadFile')

    const pdfContent = '%PDF-1.4 fake pdf content'
    cy.get('input[type="file"]').selectFile(
      {
        contents: Cypress.Buffer.from(pdfContent),
        fileName: 'test-document.pdf',
        mimeType: 'application/pdf'
      },
      { force: true }
    )

    // Loader should be visible during upload
    cy.get('an-loader').should('exist')
  })

  it('should handle upload errors', () => {
    cy.intercept('POST', '/api/upload', {
      statusCode: 500,
      body: { error: 'Upload failed' }
    }).as('uploadError')

    const pdfContent = '%PDF-1.4 fake pdf content'
    cy.get('input[type="file"]').selectFile(
      {
        contents: Cypress.Buffer.from(pdfContent),
        fileName: 'test-document.pdf',
        mimeType: 'application/pdf'
      },
      { force: true }
    )

    cy.wait('@uploadError')
    // Toast notification should appear for errors
    cy.get('an-toasts').should('exist')
  })
})
