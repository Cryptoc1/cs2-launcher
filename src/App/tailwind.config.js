import path from 'node:path'

const ProjectDirectory = process.env.MSBUILD_PROJECT_DIR
const SupportedFileExtensions = ['razor', 'razor.cs']

export default {
  content: SupportedFileExtensions.map(ext => path.join(ProjectDirectory, `./**/*.${ext}`)),
  safelist: ['active', 'invalid', 'modified', 'validation-message'],
}