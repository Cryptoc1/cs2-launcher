import defaultTheme from 'tailwindcss/defaultTheme'
import path from 'node:path'

const ProjectDirectory = process.env.MSBUILD_PROJECT_DIR
const SupportedFileExtensions = ['razor']

export default {
  content: SupportedFileExtensions.map(ext => path.join(ProjectDirectory, `./**/*.${ext}`)),
  plugins: [
    require('@tailwindcss/forms'),
  ],
  safelist: ['invalid', 'modified', 'validation-message'],
  theme: {
    extend: {
      fontFamily: {
        mono: ['IBM Plex Mono', ...defaultTheme.fontFamily.mono],
        sans: ['Nunito', ...defaultTheme.fontFamily.sans]
      },
    }
  },
}