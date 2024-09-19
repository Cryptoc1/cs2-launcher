import defaultTheme from 'tailwindcss/defaultTheme'
import path from 'node:path'

import FormsPlugin from '@tailwindcss/forms'

const ProjectDirectory = process.env.MSBUILD_PROJECT_DIR
const SupportedFileExtensions = ['razor', 'razor.cs']

export default {
  content: SupportedFileExtensions.map(ext => path.join(ProjectDirectory, `./**/*.${ext}`)),
  plugins: [
    FormsPlugin()
  ],
  safelist: ['active', 'invalid', 'modified', 'validation-message'],
  theme: {
    extend: {
      fontFamily: {
        mono: ['IBM Plex Mono', ...defaultTheme.fontFamily.mono],
        sans: ['Nunito Sans', 'Noto Sans', 'Roboto', ...defaultTheme.fontFamily.sans]
      },
    }
  },
}