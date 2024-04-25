const path = require('node:path')

const ProjectDirectory = process.env.MSBUILD_PROJECT_DIR
const SupportedFileExtensions = [
  'razor',
  'razor.cs',
  'razor.js'
]

module.exports = {
  content: SupportedFileExtensions.map(ext => path.join(ProjectDirectory, `./**/*.${ext}`)),
  safelist: ['invalid', 'modified', 'validation-message'],
  plugins: [
    require('@tailwindcss/forms'),
  ],
}